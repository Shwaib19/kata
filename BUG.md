# Description du bug

Analyse du défaut présent dans le pseudo-code fourni, stratégie retenue pour le corriger,
et tests qui l'établissent.

## Le problème

Le pseudo-code fourni contient deux défauts distincts.

**1. Une fenêtre entre la vérification et l'écriture.** La séquence est : lire le livre, tester
`livre.disponible`, créer la réservation, écrire. Entre le test et l'écriture, rien n'empêche
une seconde requête de faire exactement la même chose. Les deux lisent « disponible », les deux
écrivent, et le livre est réservé deux fois. C'est une latence entre le moment de la verification et le moment de la modification. la décision est prise sur une information qui n'est plus garantie au moment d'agir.

**2. La disponibilité est stockée.** C'est la question que posait
l'indice. `livre.disponible` est une donnée **dérivée** : un livre est disponible si aucune
réservation ne le détient à cet instant. La stocker crée une seconde source de vérité, qui peut
contredire la première alors qu'aucune réservation n'existe, parce qu'une
annulation a échoué à mi-chemin. Et elle rend le bug possible. Si l'information
n'etait pas recopiée sur le livre on aurait pas besoin dela relire puis la réécrire.

## La correction

**La disponibilité n'est jamais stockée.** `Book` n'a pas de champ `disponible`. Un livre est
disponible si aucune réservation ne le détient, ce qui est recalculé à chaque lecture
(`SearchBooksQueryHandler`). Il n'y a donc plus rien à maintenir en cohérence.

**Un index unique partiel arbitre les écritures.** Dans `ReservationConfiguration` :

```csharp
builder.HasIndex(reservation => reservation.BookId)
    .IsUnique()
    .HasFilter($"\"Status\" IN ({(int)ReservationStatus.Reserved}, {(int)ReservationStatus.Borrowed})");
```

Un livre ne peut apparaître qu'une fois parmi les réservations actives. Le filtre est
ce qui en fait une table d'historique : les réservations annulées, retournées et expirées sont
ignorées, donc le même livre peut être réservé indéfiniment au fil du temps.

Le code applicatif ne contient **aucune** vérification « ce livre est-il déjà réservé ». C'est
délibéré : une vérification applicative serait une seconde autorité, qui peut se tromper. La
seule autorité est l'index. Le dépôt traduit son refus en erreur métier :

```csharp
catch (DbUpdateException exception)
    when (exception.InnerException is PostgresException
          { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: ... })
{
    return Result.Failure(ReservationErrors.BookAlreadyReserved);
}
```

L'API répond alors **409** avec un message exploitable, jamais une erreur serveur.

**Pourquoi cette stratégie plutôt qu'un verrou ou une transaction.** Poser la contrainte dans
la base, par l'index, est la façon la plus fiable de la garantir : la règle est vérifiée par la base elle-même. une vérification écrite dans le code doit être appelée pour servir, et peut donc être oubliée ;
l'index s'applique à toute écriture, qu'elle vienne de l'application, d'une migration ou d'un
script lancé à la main.

**Un effet de bord qu'il a fallu traiter.** L'expiration est dérivée du temps écoulé, alors que
l'index porte sur le statut _stocké_. Une réservation abandonnée restait donc `Reserved` en base
et bloquait son livre pour toujours le livre se lisait « disponible » mais toute réservation
était refusée. `ReserveBook` solde donc les réservations périmées **du livre concerné** avant
d'insérer. C'est le seul endroit qui écrit `Expired` ; les lectures continuent de le dériver.

## Les tests

`Kata-api/src/Kata.IntegrationTests/DoubleReservationTest.cs`, sur un PostgreSQL réel la
garantie testée étant un index, aucun fournisseur en mémoire ne peut la vérifier.

**`The_Database_Refuses_A_Second_Reservation_Holding_The_Same_Book`** Deux `DbContext`, on écrit une réservation, puis une seconde sur le même livre, et on vérifie que PostgreSQL lève un `23505`. Il contourne volontairement le cas d'usage : ce qu'il établit, c'est la garantie du stockage et rien d'autre.

**`Two_Simultaneous_Requests_Leave_One_Reservation_And_One_Explicit_Refusal`** le scénario de
concurrence. Deux appels au cas d'usage lancés sans attendre le premier, puis `Task.WhenAll`.
Trois assertions : exactement un succès, exactement un échec portant
`ReservationErrors.BookAlreadyReserved`, et **une seule** réservation active en base.
