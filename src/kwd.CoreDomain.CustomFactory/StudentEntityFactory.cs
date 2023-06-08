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

        return ValueTask.FromResult(result);
    }
}