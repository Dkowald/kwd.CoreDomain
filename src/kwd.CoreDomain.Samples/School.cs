using kwd.CoreDomain.EntityCreation;
using Microsoft.Extensions.Logging;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// 2-stage create;
/// internal vs public state;
/// services and init-services
/// </summary>
public class School : IEntityState<School.State>
{
    private readonly ILogger<School> _log;
    private readonly List<Staff> _staff = new();

    public record State(string Name, string[] Staff);

    //normal ctor
    private School(string name, ILogger<School> log)
    {
        _log = log;
        Name = name;
    }

    //factory
    public static async Task<School> Create(
        State state,
        ILogger<School> log,
        IRepository repo //a init-service
        )
    {
        var result = new School(state.Name, log);

        log.LogDebug("Loading staff");
        foreach (var id in state.Staff)
        {
            var item = await repo.Load<Staff>(id) ?? 
                       throw new Exception("Staff not found");

            result._staff.Add(item);
        }

        return result;
    }

    public State CurrentState()
        => new (Name, _staff.Select(x => x.Name).ToArray());

    public string Name { get; }

    public IReadOnlyCollection<Staff> Staff => _staff;

    public School AddStaff(Staff who)
    {
        var found = _staff.FirstOrDefault(x => x.Name == who.Name);
        if (found is not null)
        {
            _log.LogWarning("Staff already added");
        }
        else {_staff.Add(who);}

        return this;
    }
}
