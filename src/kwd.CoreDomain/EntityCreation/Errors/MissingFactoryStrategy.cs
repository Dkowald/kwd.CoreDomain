using System;
using kwd.CoreDomain.EntityCreation.impl;

namespace kwd.CoreDomain.EntityCreation.Errors;

/// <summary>
/// Raised if no crate strategy can be found for a <see cref="EntityType"/>.
/// </summary>
public class MissingFactoryStrategy : Exception
{
    /// <inheritdoc/>
    public MissingFactoryStrategy(Type methodFactory)
    {
        MethodFactoryType = methodFactory;
        EntityType = methodFactory.GenericTypeArguments[0];
        StateType = methodFactory.GenericTypeArguments[1];
    }

    /// <inheritdoc />
    public MissingFactoryStrategy(Type entityType, Type stateType)
    {
        EntityType = entityType;
        StateType = stateType;
        MethodFactoryType = EntityMethodFactory.MethodFactoryType(entityType, stateType);
    }

    /// <summary>The type of factory not found.</summary>
    public Type MethodFactoryType { get; }

    /// <summary>The entity </summary>
    public Type EntityType { get; }

    /// <summary>The entities internal-state type</summary>
    public Type StateType { get; }

    /// <inheritdoc />
    public override string Message =>
        $"Method factory strategy for entity {EntityType.Name}, with state {StateType.Name} not found";
}

