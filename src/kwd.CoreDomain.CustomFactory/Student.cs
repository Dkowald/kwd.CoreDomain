using kwd.CoreDomain.EntityCreation;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// An entity with state, and explicit factory.
/// </summary>
public class Student : IEntityState<Student.State>
{
    private readonly State _state;

    public record State(string Name);

    State IEntityState<State>.CurrentState() =>
        new State(Name);

    //This would normally be used as the entity factory
    public Student(State state)
    {
        _state = state;
        CreatedFromCtor = true;
    }

    public string Name => _state.Name;

    public bool CreatedFromCtor { get; set; }
}
