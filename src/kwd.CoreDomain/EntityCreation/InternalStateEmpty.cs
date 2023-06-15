namespace kwd.CoreDomain.EntityCreation;


/// <summary>
/// Represents a entities internal state.. when it has none.
/// </summary>
public record InternalStateEmpty
{
    /// <summary>
    /// The single no internal state instance.
    /// </summary>
    public static readonly InternalStateEmpty Value = new ();
}