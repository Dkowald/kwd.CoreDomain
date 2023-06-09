using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using kwd.CoreDomain.EntityCreation.Errors;
using kwd.CoreDomain.EntityCreation.impl;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace kwd.CoreDomain.EntityCreation;

/// <summary>
/// Collects the <see cref="EntityProvider"/> configuration,
/// and applies it to a container.
/// </summary>
public class EntityProviderConfig
{
    private readonly List<Type> _entityTypes = new();
    private readonly List<Assembly> _entityAssemblies = new();

    private ServiceLifetime _entityScope = ServiceLifetime.Scoped;

    /// <summary>
    /// Include the specified entities.
    /// </summary>
    public EntityProviderConfig AddEntityTypes<TEntity>()
        => AddEntityTypes(typeof(TEntity));

    /// <inheritdoc cref="AddEntityTypes"/>
    public EntityProviderConfig AddEntityTypes(params Type[] entityTypes)
    {
        _entityTypes.AddRange(entityTypes);
        return this;
    }

    /// <summary>
    /// Scan the assembly(s) and include any 
    /// </summary>
    public EntityProviderConfig AddAssembly(params Assembly[] assemblies)
    {
        _entityAssemblies.AddRange(assemblies);
        return this;
    }

    /// <summary>
    /// Scan the <typeparamref name="TEntity"/> assembly for 
    /// </summary>
    public EntityProviderConfig AddAssembly<TEntity>() =>
        AddAssembly(typeof(TEntity).Assembly);

    /// <summary>
    /// Select scope for entity factory's;
    /// default to <see cref="ServiceLifetime.Scoped"/>
    /// </summary>
    public EntityProviderConfig EntityLifetime(ServiceLifetime scope)
    {
        _entityScope = scope;
        return this;
    }

    /// <summary>
    /// Add the <see cref="IEntityProvider"/>, <see cref="IEntityFactory{TEntity,TState}"/>
    /// and <see cref="EntityMethodStrategies"/> services.
    /// </summary>
    internal IServiceCollection RegisterTypes(IServiceCollection services)
    {
        //strategies are always singleton.
        var strategies = new EntityMethodStrategies();
        services.AddSingleton<IEntityMethodStrategies>(strategies);

        //provider is scoped.
        AddWithLifetime(services, typeof(IEntityProvider), typeof(EntityProvider));

        var statefulEntities = _entityAssemblies.SelectMany(x => x.GetTypes())
            .Where(x => x.IsAssignableTo(typeof(IEntityState)) && x.IsClass &&
                        !x.IsAbstract &&
                        (!x.IsGenericType || x.IsConstructedGenericType));

        var allEntities = _entityTypes.Union(statefulEntities)
            .Distinct().ToArray();

        //resolve factories.
        foreach (var entityType in allEntities)
        {
            var stateType = EntityProvider.EntityStateType(entityType);
            var factoryType = EntityProvider.EntityFactoryType(entityType, stateType);

            //only factory defined in the same assembly as the entity.
            var explicitFactory = entityType.Assembly.GetTypes()
                .Where(x => x.IsAssignableTo(factoryType)).ToArray();

            if (explicitFactory.Length > 1)
                throw new EntityFactoryDuplicates(EntityFactoryDuplicates.Reasons.MultipleExplicitFactories, entityType, stateType);
            
            if (explicitFactory.Length == 1)
            {
                TryAddWithLifetime(services, factoryType, explicitFactory[0]);

                continue;
            }

            var implicitFactory = EntityMethodFactory.MethodFactoryType(entityType, stateType);

            strategies
                .AddStrategy(implicitFactory,
                EntityMethodFactory.MethodFactoryStrategy(entityType, stateType));
            
            //factories live in same lifetime as entity.
            TryAddWithLifetime(services, factoryType, implicitFactory);
        }

        return services;
    }

    private void TryAddWithLifetime(IServiceCollection services, Type interfaceType, Type implementationType)
    {
        switch (_entityScope)
        {
            case ServiceLifetime.Singleton:
                services.TryAddSingleton(interfaceType, implementationType);
                break;
            case ServiceLifetime.Scoped:
                services.TryAddScoped(interfaceType, implementationType);
                break;
            default:
                services.TryAddTransient(interfaceType, implementationType);
                break;
        }
    }

    private void AddWithLifetime(IServiceCollection services, Type interfaceType, Type implementationType)
    {
        switch (_entityScope)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(interfaceType, implementationType);
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped(interfaceType, implementationType);
                break;
            default:
                services.AddTransient(interfaceType, implementationType);
                break;
        }
    }

    private void AddWithLifetime<T>(IServiceCollection service, Func<IServiceProvider, T> factory)
        where T : class
    {
        switch (_entityScope)
        {
            case ServiceLifetime.Singleton:
                service.AddSingleton(factory);
                break;
            case ServiceLifetime.Scoped:
                service.AddScoped(factory);
                break;
            default:
                service.AddTransient(factory);
                break;
        }
    }
}
