namespace kwd.CoreDomain.EntityCreation;


/// <summary>
/// Represents a entities internal state.. when it has none.
/// </summary>
public record NoInternalState
{
    /// <summary>
    /// The single no internal state instance.
    /// </summary>
    public static readonly NoInternalState Value = new ();
}