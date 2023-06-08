using System;

namespace kwd.CoreDomain.EntityCreation.Errors;

/// <summary>
/// Raised when the selected entity creation strategy has multiple possibilities
/// </summary>
public class EntityFactoryDuplicates : Exception
{
    /// <summary>
    /// The reason a <see cref="EntityFactoryDuplicates"/> exception was raised.
    /// </summary>
    public enum Reasons
    {
        /// <summary>Found multiple <see cref="IEntityFactory{TEntity,TState}" /> candidates</summary>
        MultipleExplicitFactories,

        /// <summary>Found multiple <see cref="EntityType"/>(<see cref="StateType"/> state, ...) candidates</summary>
        MultipleConstructors,

        /// <summary>Found multiple candidate static methods</summary>
        MultipleStaticMethods
    }

    /// <summary>
    /// Raised if multiple explicit entity factories found.
    /// </summary>
    public EntityFactoryDuplicates(Reasons reason, Type entityType, Type stateType, Exception? inner = null)
        : base($"Multiple factory types for entity {entityType.Name} using state {stateType.Name} cannot automatically add", inner)
    {
        Reason = reason;

        EntityType = entityType;
        StateType = stateType;
    }

    /// <summary>
    /// The reason the error was raised. <see cref="Reasons"/>
    /// </summary>
    public Reasons Reason { get; }
    
    /// <summary>The type of entity</summary>
    public Type EntityType { get; }

    /// <summary>The entities internal-state type</summary>
    public Type StateType { get; }

    /// <inheritdoc />
    public override string Message {
        get
        {
            if (Reason == Reasons.MultipleExplicitFactories)
            {
                return $"Multiple explicit entity factories found for {EntityType.Name}";
            }

            if (Reason == Reasons.MultipleConstructors)
            {
                return $"Multiple candidate constructors found: {EntityType.Name}({StateType.Name} state, ...)";
            }

            return $"Multiple candidate static factory methods found for {EntityType.Name}";
        }
    }
}