using kwd.CoreDomain.EntityCreation;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// An entity with state, and explicit factory.
/// </summary>
public class Student : IEntityState<Student.State>
{
    public record State(string Name);

    //Get current internal state.
    State IEntityState<State>.CurrentState() => new (Name);

    //This would normally be used as the entity factory
    public Student(State state)
    {
        Name = state.Name;
        CreatedFromCtor = true;
    }

    public string Name { get; }

    public bool CreatedFromCtor { get; set; }
}
