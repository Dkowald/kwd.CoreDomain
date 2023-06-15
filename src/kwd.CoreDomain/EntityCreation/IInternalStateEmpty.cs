namespace kwd.CoreDomain.EntityCreation;

/// <summary>
/// Identifies an entity that has no internal-state.
/// </summary>
public interface IInternalStateEmpty : IInternalState<InternalStateEmpty>
{
    /// <summary>
    /// Current state for an entity with no internal state is
    /// the readonly <see cref="InternalStateEmpty.Value"/>
    /// </summary>
    InternalStateEmpty IInternalState<InternalStateEmpty>.CurrentState() => InternalStateEmpty.Value;
}