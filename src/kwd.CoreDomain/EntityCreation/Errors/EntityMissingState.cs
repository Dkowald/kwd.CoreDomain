using System;

namespace kwd.CoreDomain.EntityCreation.Errors;

/// <summary>
/// Entity does not implement <see cref="IInternalState{TState}"/>.
/// </summary>
/// <remarks>
/// Raised if entity uses the base <see cref="IInternalState"/>.
/// </remarks>
public class EntityMissingState : Exception
{
    /// <inheritdoc cref="EntityMissingState"/>
    public EntityMissingState(Type entityType, Exception? inner = null)
        : base($"Entity type {entityType.Name} must implement {nameof(IInternalState)}{{TEntity}},{{TState}}")
    {
        EntityType = entityType;
    }

    /// <summary>
    /// The attempted entity type.
    /// </summary>
    public Type EntityType { get; }
}
