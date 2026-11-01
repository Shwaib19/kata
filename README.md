# Gestion de bibliothèque

Application de gestion d'une petite bibliothèque : catalogue de livres, lecteurs, et
réservations avec quota, expiration et cycle de vie.

- **Backend** : .NET 10, C# 14, PostgreSQL 18, Entity Framework Core 10
- **Frontend** : Nuxt 4 (Vue 3), Tailwind CSS 4
- **Tests** : xUnit, 84 tests unitaires de domaine, 2 tests d'intégration sur base réelle

---

## Lancer l'application

Un seul prérequis : Docker.

```bash
docker compose up -d
```

|                        |                                                                   |
| ---------------------- | ----------------------------------------------------------------- |
| Interface              | <http://localhost:3000>                                           |
| API                    | <http://localhost:5199>                                           |
| Documentation de l'API | <http://localhost:5199/scalar/v1>                                 |
| PostgreSQL             | `localhost:5432`, base `kata`, utilisateur et mot de passe `kata` |

Pour arrêter en conservant les données : `docker compose down`.

## Développer en local

La base reste dans Docker, le reste tourne sur la machine.

```bash
docker compose up -d database

# API sur http://localhost:5199, documentation sur /scalar/v1
cd Kata-api/src/Kata.Api
dotnet run

# Front sur http://localhost:3000
cd Kata-client
bun install
bun run dev
```

## Lancer les tests

```bash
cd Kata-api
dotnet test Kata.sln
```

Les tests d'intégration ont besoin d'un PostgreSQL joignable. Ils utilisent une base
**séparée**, `kata_tests`, qu'ils créent au premier lancement et dont ils vident les tables
entre les cas, jamais la base `kata` de l'application.

```bash
docker compose up -d database   # suffit pour les tests
```

La chaîne de connexion est surchargeable par la variable `KATA_TEST_DATABASE`, ce qu'utilise
l'intégration continue. Celle-ci lance la suite complète à chaque _push_ et chaque _pull
request_ touchant `Kata-api/`, avec son propre service PostgreSQL
(`.github/workflows/backend-tests.yml`).

Le front a son propre contrôle : `cd Kata-client && bun run lint`.

---

## Le bug corrigé

L'analyse du problème, la stratégie de correction et les tests qui la prouvent sont dans
[BUG.md](BUG.md).

---

## Choix de la stack

**.NET 10** parce que c'est l'environnement que je maîtrise le mieux, et que le typage strict
et les _records_ rendent un domaine riche expressif sans cérémonie. **PostgreSQL** parce que la
correction du bug repose sur un index unique **partiel**, que tous les moteurs ne proposent pas.
**Nuxt 4** pour le rendu serveur, qui donne des pages complètes sans écran de chargement et
rend l'interface lisible sans JavaScript côté client pour le premier affichage. **Tailwind**
pour tenir une interface cohérente sans écrire de CSS, le brief n'attendant aucun design.

## Architecture

```
Kata-api/src/
  Kata.Domain/          entités et règles métier, aucune dépendance technique
  Kata.Application/     cas d'usage (commandes et requêtes), contrats de dépôts
  Kata.infrastructure/  EF Core, PostgreSQL, implémentation des dépôts
  Kata.Api/             endpoints HTTP, traduction des erreurs en codes
  SharedKernel/         Result, Error, abstractions de messagerie
  Kata.test/            tests unitaires du domaine
  Kata.IntegrationTests/ tests sur base réelle
Kata-client/app/        pages, composables, composants
```

Les règles métier vivent dans `Kata.Domain` et nulle part ailleurs. Un cas d'usage orchestre :
il charge, délègue la décision au domaine, persiste. Les endpoints ne font que traduire.

Les échecs attendus ne sont pas des exceptions mais un `Result` portant une `Error` typée
(`ErrorType.NotFound`, `Conflict`, `Validation`…). Un seul endroit, `CustomResults`, traduit ce
type en code HTTP, ce qui garantit qu'une même erreur métier produit toujours le même statut.

---

## Décisions et arbitrages

**Une ligne du catalogue est un exemplaire, pas un titre.** Deux saisies identiques créent deux
exemplaires, chacun réservable séparément, ce qui correspond à une bibliothèque réelle.
Le revers assumé : un double envoi du formulaire crée un exemplaire fantôme, sans avertissement.

**L'expiration est calculée, pas planifiée.** Aucune tâche de fond. `StatusAt(now)` compare la
date de réservation à l'instant courant, donc une réservation abandonnée se lit `Expired` sans
que rien n'ait eu à l'écrire. Le statut stocké n'est matérialisé que sur le chemin d'écriture,
pour la raison expliquée dans BUG.md .

``**Un effet de bord qu'il a fallu traiter.** L'expiration est dérivée du temps écoulé, alors que
l'index porte sur le statut _stocké_. Une réservation abandonnée restait donc `Reserved` en base
et bloquait son livre pour toujours le livre se lisait « disponible » mais toute réservation
était refusée. `ReserveBook` solde donc les réservations périmées **du livre concerné** avant
d'insérer. C'est le seul endroit qui écrit `Expired` ; les lectures continuent de le dériver.``

**Supprimer, c'est retirer de la circulation.** `Book` et `Reader` implémentent
`ISoftDeletable` : la ligne est conservée et masquée par un filtre global de requête. Sans
cela, les clés étrangères en `Restrict` rendraient tout livre déjà emprunté indéfiniment
indestructible. L'historique des réservations continue donc de nommer un livre retiré du
catalogue, via `IgnoreQueryFilters()` sur la jointure. Conséquence visible : un lien vers un
livre retiré mène à une page « Ce livre n'existe pas, ou plus ».

**Le quota se compte en mémoire, pas en SQL.** Les réservations ouvertes d'un lecteur sont
chargées puis filtrées avec `IsActive(now)`. Le `WHERE` complet aurait dupliqué en SQL la règle
des 48 h : deux endroits à modifier le jour où elle change. Le coût est nul, un lecteur a au
pire quelques lignes.

**Les listes évitent le N+1 sans jointure par ligne.** Une liste de livres charge en une seule
requête les réservations de tous les livres trouvés, puis décide de la disponibilité en
mémoire. Idem pour le compteur par lecteur. Deux requêtes quel que soit le nombre de résultats.

**`ErrorType.Validation` est traduit en 400, pas en 422.** Le brief cite 422 parmi les codes
attendus ; j'ai retenu 400 pour rester homogène avec la convention de mon code existant. Un
corps de requête rejeté par le domaine et un corps illisible partagent donc le même statut, ce
qui est moins précis.

**Le front recalcule les chiffres du tableau de bord** à partir des deux mêmes endpoints que
les pages Livres et Lecteurs, sans endpoint d'agrégats. Les compteurs ne peuvent donc pas
contredire les listes ce qui ne tiendrait pas à grande échelle.

**Le filtre « disponible / indisponible » est appliqué côté client**, contrairement à la
recherche par titre et auteur qui part à l'API. La disponibilité étant dérivée, le serveur
devrait calculer tout le catalogue avant de pouvoir filtrer. Pour utiliser la pagination, il faudrait
remonter ce filtre dans l'API.

**Les migrations s'appliquent au démarrage de l'API**, pour que `docker compose up` donne une
application qui fonctionne.

**La documentation de l'API est exposée dans tous les environnements**, alors que le défaut du
modèle la réserve au développement, sans quoi elle serait invisible dans le conteneur

---

## Ce qui n'a pas été fait

**Aucun test sur les cas d'usage.** Le domaine est couvert par 84 tests unitaires et la
concurrence par 2 tests d'intégration, mais le quota, les 404 et les transitions refusées n'ont
été vérifiés qu'à la main, pas automatiquement.

**Pas d'authentification ni de notion d'utilisateur** : l'authentificaiton etait hors périmètre demandé.

**Pas de pagination ni de filtre par année de publication**, ni de vue mobile dédiée : ce sont des
bonus non traités.

**Pas de page par réservation** : les actions du cycle de vie sont en ligne dans la liste, pour éviter d'avoir un écran pour deux boutons.
