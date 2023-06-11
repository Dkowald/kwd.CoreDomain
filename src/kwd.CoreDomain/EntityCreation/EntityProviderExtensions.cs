using System;

using kwd.CoreDomain.EntityCreation.impl;

using Microsoft.Extensions.DependencyInjection;

namespace kwd.CoreDomain.EntityCreation;

/// <summary>
/// Extensions to help use the <see cref="IEntityProvider"/>.
/// </summary>
public static class EntityProviderExtensions
{
    /// <summary>
    /// Registers a <see cref="IEntityProvider"/> and
    /// all associated parts found in the <see cref="EntityProviderConfig"/>.
    /// </summary>
    public static IServiceCollection AddEntityProvider(this IServiceCollection services,
        Action<EntityProviderConfig> cfg)
    {
        var options = new EntityProviderConfig();
        cfg(options);
        
        return options.RegisterTypes(services);
    }
    
    /// <summary>
    /// Try get the state object type for a <typeparamref name="TEntity"/>
    /// Using <see cref="IInternalState{TState}"/>.
    /// </summary>
    public static Type? TryGetStateType<TEntity>(this IEntityProvider _) 
        => EntityProvider.TryGetEntityStateType(typeof(TEntity));

    /// <summary>
    /// Try get the state object type for a <paramref name="entityType"/>
    /// Using <see cref="IInternalState{TState}"/>.
    /// </summary>
    public static Type? TryGetStateType(this IEntityProvider _, Type entityType)
        => EntityProvider.TryGetEntityStateType(entityType);

    /// <summary>
    /// Returns true if the given type <typeparamref name="T"/> is a Entity Type.
    /// </summary>
    public static bool IsEntity<T>() 
        => EntityProvider.TryGetEntityStateType(typeof(T)) is not null;

    /// <summary>
    /// Returns true if the given type <paramref name="t"/> is a Entity Type.
    /// </summary>
    public static bool IsEntity(Type t)
        => EntityProvider.TryGetEntityStateType(t) is not null;
}
