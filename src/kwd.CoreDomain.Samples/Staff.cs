using kwd.CoreDomain.EntityCreation;
using Microsoft.Extensions.Logging;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// Entity with state and a service.
/// </summary>
public class Staff : IEntityState<Staff.State>
{
    //internal entity state
    public record State(string Name);

    //Get current internal state.
    State IEntityState<State>.CurrentState() => new(Name);

    //Create from state, with service injected
    public Staff(State state, ILogger<Staff> log)
    {
        Name = state.Name;
        log.LogDebug("Staff loaded");
    }

    public string Name { get; }
}