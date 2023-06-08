using System;
using System.Collections.Generic;
using kwd.CoreDomain.EntityCreation.Errors;

namespace kwd.CoreDomain.EntityCreation.impl;

/// <summary>
/// Maintains the set of selected method strategies to use for each registered
/// <see cref="IEntityFactory{TEntity,TState}"/>.
/// </summary>
/// <remarks>
/// This is added as a singleton;
/// capturing the results from reflection.
/// Each <see cref="EntityMethodFactory{TEntity,TState}"/> then
/// requests its strategy details as-needs.
/// </remarks>
public class EntityMethodStrategies : IEntityMethodStrategies
{
    private readonly Dictionary<Type, Strategy> _strategies = new();
    
    internal EntityMethodStrategies AddStrategy(Type methodFactoryType, Strategy strategy)
    {
        _strategies[methodFactoryType] = strategy;
        return this;
    }

    Strategy IEntityMethodStrategies.this[Type methodFactoryType]
    {
        get
        {
            if (_strategies.TryGetValue(methodFactoryType, out var strategy))
                return strategy;

            throw new MissingFactoryStrategy(methodFactoryType);
        }
    }
}