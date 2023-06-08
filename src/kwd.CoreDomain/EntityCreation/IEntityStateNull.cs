namespace kwd.CoreDomain.EntityCreation;

/// <summary>
/// Identifies an entity that has no internal-state.
/// </summary>
public interface IEntityStateNull : IEntityState<NoInternalState>
{
    /// <summary>
    /// Current state for an entity with no internal state is
    /// the readonly <see cref="NoInternalState.Value"/>
    /// </summary>
    NoInternalState IEntityState<NoInternalState>.CurrentState() => NoInternalState.Value;
}