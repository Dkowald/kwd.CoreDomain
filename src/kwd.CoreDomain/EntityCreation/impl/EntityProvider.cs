using System;
using System.Threading.Tasks;
using kwd.CoreDomain.EntityCreation.Errors;

namespace kwd.CoreDomain.EntityCreation.impl;

/// <summary>
/// Implement a <see cref="IEntityProvider"/>
/// </summary>
public class EntityProvider : IEntityProvider
{
    private readonly IServiceProvider _container;

    /// <summary>
    /// The <see cref="IEntityFactory{TEntity,TState}"/> that will be used to create a <paramref name="entityType"/>.
    /// </summary>
    public static Type EntityFactoryType(Type entityType, Type stateType)
        => typeof(IEntityFactory<,>).MakeGenericType(entityType, stateType);

    /// <summary>Get the entities state type</summary>
    public static Type EntityStateType(Type entityType) =>
        TryGetEntityStateType(entityType) ?? throw new EntityMissingState(entityType);

    /// <summary> Try get the entity state type, returns null if it doesn't have a stateType</summary>
    public static Type? TryGetEntityStateType(Type entityType) =>
        entityType.GetInterface(typeof(IInternalState<>).Name)?.GenericTypeArguments[0];

    /// <summary>
    /// Create a <see cref="EntityProvider"/> using the
    /// given <see cref="IServiceProvider"/> to resolve additional services.
    /// </summary>
    public EntityProvider(IServiceProvider container)
    {
        _container = container;
    }

    /// <inheritdoc cref="IEntityProvider.Create{TEntity}"/>
    public async ValueTask<T> Create<T>(object state)
    {
        var stateType = state.GetType();
        var entityType = typeof(T);

        if (stateType.IsAssignableTo(entityType)) return (T)state;

        var factoryType = EntityFactoryType(entityType, stateType);

        var factory = _container.GetService(factoryType) as IEntityFactory ??
                      throw new EntityFactoryNotRegistered(entityType, stateType);

        var objResult = await factory.Create(state);
        return (T)objResult;
    }
}
