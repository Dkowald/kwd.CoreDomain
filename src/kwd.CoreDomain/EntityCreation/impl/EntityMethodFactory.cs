using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using kwd.CoreDomain.EntityCreation.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace kwd.CoreDomain.EntityCreation.impl;

/// <summary>
/// Base for <see cref="EntityMethodFactory{TEntity,TState}"/>
/// </summary>
public abstract class EntityMethodFactory
{
    /// <summary>
    /// Determines the strategy to use for a <see cref="EntityMethodFactory{TEntity,TState}"/>.
    /// If no strategy can be found; throws a <see cref="MissingFactoryStrategy"/>.
    /// </summary>
    /// <exception cref="MissingFactoryStrategy">Raised if no method factory strategy found</exception>
    public static Strategy MethodFactoryStrategy(Type entityType, Type stateType)
    {
        var staticFactoryMethod = FindStaticFactoryMethod(entityType, stateType);

        if (staticFactoryMethod is not null)
        {
            return new Strategy(staticFactoryMethod, null);
        }

        var constructor = FindConstructorMethod(entityType, stateType);

        if (constructor is not null)
        {
            return new Strategy(null, constructor);
        }

        throw new MissingFactoryStrategy(entityType, stateType);
    }

    /// <summary>
    /// The concrete <see cref="EntityMethodFactory{TEntity,TState}"/> type.
    /// </summary>
    public static Type MethodFactoryType(Type entity, Type stateType)
        => typeof(EntityMethodFactory<,>).MakeGenericType(entity, stateType);

    /// <summary>
    /// The static method that returns a Task{TEntity}
    /// </summary>
    internal protected static MethodInfo? FindStaticFactoryMethod(Type entityType, Type stateType)
    {
        var returnTypes = new[]
        {
            typeof(Task<>).MakeGenericType(entityType),
            typeof(ValueTask<>).MakeGenericType(entityType)
        };

        var staticFactoryMethods = entityType
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Where(x => returnTypes.Contains(x.ReturnType) &&
                        x.GetParameters().SingleOrDefault(p => p.ParameterType == stateType) is not null)
            .ToArray();
        
        if (staticFactoryMethods.Length > 1)
            throw new EntityFactoryDuplicates(EntityFactoryDuplicates.Reasons.MultipleStaticMethods,
                entityType, stateType);

        return staticFactoryMethods.SingleOrDefault();
    }

    /// <summary>
    /// The constructor that takes at-least a TState
    /// </summary>
    internal protected static ConstructorInfo? FindConstructorMethod(Type entityType, Type stateType)
    {
        var ctors = entityType
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(x => x.GetParameters().Any(p => p.ParameterType == stateType))
            .ToArray();
        
        if (ctors.Length > 1)
        {
            throw new EntityFactoryDuplicates(EntityFactoryDuplicates.Reasons.MultipleConstructors,
                entityType, stateType);
        }

        return ctors.SingleOrDefault();
    }
}

/// <summary>
/// Implements a <see cref="IEntityFactory{TEntity,TState}"/> using
/// a method on the entity for creation.
/// Uses (in preference order)
/// <list type="bullet">
/// <item>static-create:
///   Task&lt;<typeparamref name="TEntity"/>&gt; XX(<typeparamref name="TState"/> state, ...)
/// </item>
/// <item>state-ctor: <typeparamref name="TEntity"/>(<typeparamref name="TState"/> state, ...)</item>
/// </list> 
/// </summary>
public class EntityMethodFactory<TEntity, TState> : EntityMethodFactory, IEntityFactory<TEntity, TState>
    where TEntity : notnull
{
    private readonly IServiceProvider _container;

    private readonly Strategy _factoryStrategy;

    /// <inheritdoc cref="EntityMethodFactory"/>
    public EntityMethodFactory(IServiceProvider container, IEntityMethodStrategies strategies)
    {
        _container = container;

        _factoryStrategy = strategies[GetType()];
    }

    /// <inheritdoc />
    public async ValueTask<TEntity> Create(TState state)
    {
        var stateType = typeof(TState);
        
        if (_factoryStrategy.Static is not null)
        {
            var args = _factoryStrategy.Static.GetParameters().Select(p =>
                    p.ParameterType == stateType ? state : GetInjectedService(p.ParameterType))
                .ToArray();
            var objResult = _factoryStrategy.Static.Invoke(null, args) ??
                            throw new Exception("Static factory method returned null");

            return objResult switch
            {
                ValueTask<TEntity> valueTask => await valueTask,
                Task<TEntity> task => await task,
                _ => throw new Exception("Factory static strategy unknown factory method return.")
            };
        }

        if (_factoryStrategy.Constructor is not null)
        {
            var args = _factoryStrategy.Constructor.GetParameters().Select(p =>
                    p.ParameterType == stateType ? state : GetInjectedService(p.ParameterType))
                .ToArray();

            var objResult = _factoryStrategy.Constructor.Invoke(args) ??
                            throw new Exception("Entity constructor returned null");

            return (TEntity)objResult;
        }

        throw new Exception($"No factory strategy found for {GetType().Name}");
    }

    private object GetInjectedService(Type serviceType)
    {
        try
        {
            return _container.GetRequiredService(serviceType);
        }
        catch (InvalidOperationException ex)
        {
            throw new MissingEntityServices(typeof(TEntity), serviceType, ex);
        }
    }
}
