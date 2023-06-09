using System;

namespace kwd.CoreDomain.EntityCreation.Errors;

/// <summary>
/// Entity does not implement <see cref="IEntityState{TState}"/>.
/// </summary>
/// <remarks>
/// Raised if entity uses the base <see cref="IEntityState"/>.
/// </remarks>
public class EntityMissingState : Exception
{
    /// <inheritdoc cref="EntityMissingState"/>
    public EntityMissingState(Type entityType, Exception? inner = null)
        : base($"Entity type {entityType.Name} must implement {nameof(IEntityState)}{{TEntity}},{{TState}}")
    {
        EntityType = entityType;
    }

    /// <summary>
    /// The attempted entity type.
    /// </summary>
    public Type EntityType { get; }
}
