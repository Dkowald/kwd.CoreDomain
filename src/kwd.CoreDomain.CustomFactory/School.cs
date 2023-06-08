using kwd.CoreDomain.EntityCreation;
using Microsoft.Extensions.Logging;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// TODO: any value?
/// </summary>
public class School : IEntityState<School.State>
{
    private readonly IRepository _repo;
    private readonly List<ClassRoom> _rooms = new();

    public record State(string Name, string[] Rooms)
    {
        public static State New(string name)
            => new(name, Array.Empty<string>());
    }

    public School(string name, IRepository repo)
    {
        Name = name;
        _repo = repo;
    }

    public static async Task<School> Create(State state,
        IRepository repo, //a entity-service
        ILogger<School> log //a init-service
        )
    {
        var result = new School(state.Name, repo);

        log.LogDebug("Recovering rooms");
        foreach (string roomId in state.Rooms)
        {
            await result.AddRoom(roomId);
        }

        return result;
    }

    State IEntityState<State>.CurrentState()
        => new State(Name, _rooms.Select(x => x.Name).ToArray());

    public string Name { get; }

    public IReadOnlyCollection<ClassRoom> Rooms => _rooms;

    public async Task<School> AddRoom(string roomId)
    {
        var room = await _repo.Load<ClassRoom>(roomId);

        if (room is null)
            throw new Exception("Room not found.");

        var found = _rooms.FirstOrDefault(x => x.Name == roomId);
        if (found is null)
            _rooms.Add(room);

        return this;
    }
}
