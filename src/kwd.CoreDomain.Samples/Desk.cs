using kwd.CoreDomain.EntityCreation;
using kwld.CoreUtil.Strings;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// Entity with both ctor-factory and static-factory.
/// Internal state NOT the same as presented state.
/// </summary>
public class Desk : IInternalState<Desk.State>
{
    private readonly string _room;

    public record State(string CurrentRoomId, string Status)
    {
        public static State New(string roomId)
            => new State(roomId, "new");
    }

    public Desk(State state)
    {
        Status = state.Status;
        _room = state.CurrentRoomId;
    }

    public static Task<Desk> Create(State state)
    {
        var result = new Desk(state)
        {
            IsInitalized = true
        };

        return Task.FromResult(result);
    }

    State IInternalState<State>.CurrentState() =>
        new(_room, Status);

    public string Status { get; private set; }

    public bool IsBroken
    {
        get { return Status.Same("broken"); }
        set
        {
            if (value) { Status = "working"; }
            else Status = "broken";
        }
    }

    //True if entity created from the static method.
    public bool IsInitalized { get; private set; }

}