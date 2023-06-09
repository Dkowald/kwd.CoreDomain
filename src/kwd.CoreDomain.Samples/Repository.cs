using System.Text.Json;
using kwd.CoreDomain.EntityCreation;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// A simple in-memory JSON store for entities, <see cref="IEntityState{TState}"/>.
/// </summary>
public interface IRepository
{
    public IRepository Store<TEntity>(string id, TEntity entity)
        where TEntity : IEntityState;

    public Task<TEntity?> Load<TEntity>(string id)
        where TEntity : IEntityState;
}

/// <summary>
/// A Simple in-memory entity repository
/// </summary>
public class Repository : IRepository
{
    private readonly IEntityProvider _provider;

    private record Item(string Json);

    private readonly Dictionary<string, Item> _data = new();

    public Repository(IEntityProvider provider)
    {
        _provider = provider;
    }

    public IRepository Store<TEntity>(string id, TEntity? entity)
        where TEntity : IEntityState
    {
        var state = entity?.GetCurrentState();

        _data[id] = new(JsonSerializer.Serialize(state));

        return this;
    }

    public async Task<TEntity?> Load<TEntity>(string id)
        where TEntity : IEntityState
    {
        if (!_data.TryGetValue(id, out var item))
            return default;

        var stateType = _provider.TryGetStateType<TEntity>() ??
             throw new Exception("Not an entity Type");

        var state = JsonSerializer.Deserialize(item.Json, stateType)
            ?? throw new Exception("Reading entity state failed.");

        return await _provider.Create<TEntity>(state);
    }
}
