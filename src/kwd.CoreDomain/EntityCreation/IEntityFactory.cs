using System;
using System.Threading.Tasks;

namespace kwd.CoreDomain.EntityCreation;

/// <summary>
/// Generic base interface;
/// implementations should use <see cref="IEntityFactory{TEntity,TState}"/>
/// </summary>
public interface IEntityFactory
{
    /// <summary>
    /// The type of entity to create
    /// </summary>
    Type EntityType { get; }

    /// <summary>
    /// The type of state object to use for creation
    /// </summary>
    Type StateType { get; }

    /// <summary>
    /// Create the entity within a particular state.
    /// </summary>
    ValueTask<object> Create(object state);
}

/// <summary>
/// Specify an alternate type used
/// to create an entity. This provides a path to
/// create via async method.
/// </summary>
public interface IEntityFactory<TEntity, in TState> : IEntityFactory
    where TEntity : notnull
{
    Type IEntityFactory.EntityType => typeof(TEntity);
    Type IEntityFactory.StateType => typeof(TState);
    async ValueTask<object> IEntityFactory.Create(object state) => await Create((TState)state);

    /// <summary>
    /// Create a <typeparamref name="TEntity"/> from its internal state <typeparamref name="TState"/>.
    /// </summary>
    public ValueTask<TEntity> Create(TState state);
}
