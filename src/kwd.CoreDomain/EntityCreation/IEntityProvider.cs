using System.Threading.Tasks;

namespace kwd.CoreDomain.EntityCreation;

/// <summary>
/// Combines the use of a serializer with a IoC container to store / retrieve objects.
/// Adds a 2-stage create allowing for async init.
/// </summary>
/// <remarks>
/// Used as a drop-in replacement for serialization calls.
/// </remarks>
public interface IEntityProvider
{
    /// <summary>
    /// Create a <typeparamref name="TEntity"/> from its state <paramref name="state"/>.
    /// (If <paramref name="state"/> is already a <typeparamref name="TEntity"/> this just returns (TEntity)state)
    /// </summary>
    ValueTask<TEntity> Create<TEntity>(object state);

    /// <inheritdoc cref="Create{TEntity}"/>
    ValueTask<TEntity> Create<TEntity, TState>(TState state)
        where TState : class
        where TEntity : IInternalState<TState>
        => Create<TEntity>(state);
}