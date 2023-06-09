using kwd.CoreDomain.EntityCreation;
using kwd.CoreDomain.Samples;

namespace kwd.CoreDomain.Tests.EntityCreation;

public class StudentEntityFactoryCustom : IEntityFactory<Student, Student.State>
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
}