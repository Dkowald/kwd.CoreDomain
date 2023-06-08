using kwd.CoreDomain.EntityCreation;

namespace kwd.CoreDomain.Samples;

public class User : IEntityState<User.State>
{
    private readonly IEntityProvider _objProvider;
    private readonly State _init;

    public record State(string Name, string? Message = null, string StateOnlyData = nameof(State.StateOnlyData));

    private User(IEntityProvider objProvider, State state)
    {
        _objProvider = objProvider;
        _init = state;
        Message = state.Message;
    }

    public User(IEntityProvider objProvider, string name)
        : this(objProvider, new State(name)) { }

    public string Name => _init.Name;

    public string? Message { get; set; }

    public State CurrentState() => _init with
    {
        Message = Message
    };
}
