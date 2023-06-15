using System;

namespace kwd.CoreDomain.EntityCreation;

/// <summary>
/// Describe the internal state of an entity.
/// </summary>
public interface IInternalState
{
    /// <summary>
    /// The entities runtime-type
    /// </summary>
    Type EntityType { get; }

    /// <summary>
    /// The entities internal state type.
    /// </summary>
    Type StateType { get; }

    /// <summary>
    /// Create a object to represent
    /// the entities current internal state.
    /// </summary>
    object GetCurrentState();
}

/// <summary>
/// <inheritdoc cref="IInternalState"/>
/// This entity can create its own <typeparamref name="TState"/> state object.
/// The entity MUST provide a factory:
/// <list type="bullet">
/// <item>An explicit <see cref="IEntityFactory{TEntity,TState}"/> registered in the container</item>
/// <item>A static Task&lt;TEntity&gt; XX(TState state, ...) or ValueTask&lt;TEntity&gt; XX(TState state, ...) method</item>
/// <item>A constructor : TEntity(TSTate state, ...) </item>
/// </list>
/// Note: first in list is used as factory; Error thrown if multiple of selected strategy found.
/// </summary>
public interface IInternalState<out TState> : IInternalState
    where TState : class
{
    /// <inheritdoc />
    Type IInternalState.EntityType => GetType();

    /// <inheritdoc />
    Type IInternalState.StateType => typeof(TState);

    /// <inheritdoc />
    object IInternalState.GetCurrentState() => CurrentState();

    /// <summary>
    /// Create a <typeparamref name="TState"/> to represent
    /// the entities current internal state.
    /// </summary>
    TState CurrentState();
}