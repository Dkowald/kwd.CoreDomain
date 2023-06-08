using System;

namespace kwd.CoreDomain.EntityCreation.Errors;

/// <summary>
/// Raised when the required <see cref="IEntityFactory{TEntity,TState}"/> cannot be found
/// in the repository.
/// </summary>
public class EntityFactoryNotRegistered : Exception
{
    /// <inheritdoc />
    public EntityFactoryNotRegistered(Type entityType, Type stateType)
        : base($"No factory found for entity '{entityType.Name}' using state '{stateType.Name}'")
    {
        EntityType = entityType;
        StateType = stateType;
    }

    /// <summary>The attempted type of Entity </summary>
    public Type EntityType { get; }

    /// <summary>The type of the Entity internal-state</summary>
    public Type? StateType { get; }
}