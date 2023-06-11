using kwd.CoreDomain.EntityCreation;
using Microsoft.Extensions.Logging;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// An entity with internal-state; services, init-services, and async-create with implicit static factory.
/// </summary>
public class ClassRoom : IInternalState<ClassRoom.State>
{
    private readonly ILogger<ClassRoom> _log;
    private const int ReservedSpaceForTeacher = 50;
    private const int DeskSize = 9;

    //don't need to have it as a property for serialization
    private readonly int _floorSpace;

    private readonly List<Student> _students = new();

    private readonly State _state;

    public record State(string Name, int NumberOfDesks, int FloorSpace, string[] Students)
    {
        //short-hand helper for new entity internal state.
        public static State New(string name, int desks = 0, int floorSpace = 0)
            => new(name, desks, floorSpace, Array.Empty<string>());
    }

    private ClassRoom(State state, ILogger<ClassRoom> log)
    {
        _log = log;
        _state = state;

        Name = state.Name;
        Desks = state.NumberOfDesks;
        _floorSpace = state.FloorSpace;
    }

    //can take init-services, and class-services
    public static async ValueTask<ClassRoom> Create(State state, ILogger<ClassRoom> log, IRepository repo)
    {
        log.LogDebug("create class room from state with static-method factory");

        var result = new ClassRoom(state, log);

        log.LogDebug("Loading student details.");

        foreach (string studentId in state.Students)
        {
            var student = await repo.Load<Student>(studentId);

            if (student is null)
            {
                log.LogWarning("Student {id} no longer exists, removing from class room", studentId);
                continue;
            }

            result._students.Add(student);
        }

        return result;
    }

    State IInternalState<State>.CurrentState()
        => new(Name, Desks, _floorSpace, Students.Select(x => x.Name).ToArray());

    public string Name { get; }

    public int Desks { get; private set; }

    //can have calculated properties, since I store the state, not the entity.
    public int MaxDesks => _floorSpace - ReservedSpaceForTeacher - Desks * DeskSize;

    //can resolve other objects during init.
    public Student[] Students => _students.ToArray();

    public ClassRoom AddDesks(int howMany)
    {
        _log.LogDebug("Adding desks to class room; check have space.");
        var deskSpace = _state.FloorSpace - ReservedSpaceForTeacher;
        var remainingSpace = deskSpace - _state.NumberOfDesks * DeskSize;

        if (remainingSpace <= DeskSize * howMany)
        {
            _log.LogError($"Add desks failed, no room (use {nameof(MaxDesks)} to check).");
            throw new Exception($"Not enough room for {howMany} more desks");
        }

        return this;
    }

    public ClassRoom AddStudents(params Student[] who)
    {
        foreach (Student student in who)
        {
            var found = _students.FirstOrDefault(x => x.Name == student.Name);
            if (found is not null)
            {
                _log.LogInformation("Student {name} already added, skipping", found.Name);
                continue;
            }

            _students.Add(student);
        }

        return this;
    }
}
