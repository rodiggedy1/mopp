using Domain.Entities.Base;
using Domain.Entities.User;
using Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace MediatR;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEvents(this IMediator mediator, List<BaseEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }

    public static List<BaseEvent> GetDomainEvents(DbContext context)
    {
        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        var userEntities = context.ChangeTracker
            .Entries<ApplicationUser>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var userDomainEvents = userEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        userEntities.ToList().ForEach(e => e.ClearDomainEvents());

        return domainEvents.Concat(userDomainEvents)
                           .ToList();
    }
}