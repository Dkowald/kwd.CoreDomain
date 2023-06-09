using kwd.CoreDomain.EntityCreation;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// Provide an explicit entity factory;
/// Provider will prefer to use this.
/// </summary>
public class StudentEntityFactory : IEntityFactory<Student, Student.State>
{
    public ValueTask<Student> Create(Student.State state)
    {
        var result = new Student(state)
        {
            CreatedFromCtor = false
        };
        CreatedAnEntity = true;
        return ValueTask.FromResult(result);
    }

    public bool CreatedAnEntity { get; private set; }

    public bool ExplicitlyAddedToContainer { get; set; }
}