using System.Text.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Outbox;

namespace SharedKernel.Interceptors;

public sealed class OutboxInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context!;

        var entries = dbContext.ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList();

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            foreach (var @event in entity.DomainEvents)
            {
                dbContext.Set<OutboxMessage>().Add(OutboxMessage.Create(
                    @event.GetType().FullName!,
                    JsonSerializer.Serialize(@event, @event.GetType())));
            }

            entity.ClearDomainEvents();
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
