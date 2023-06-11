using kwd.CoreDomain.EntityCreation;
using Microsoft.Extensions.Logging;

namespace kwd.CoreDomain.Samples;

/// <summary>
/// An entity with no internal state, but can still use the factory's
/// </summary>
/// <remarks>
/// It MUST still have a valid factory,
/// but the <see cref="InternalStateEmpty.Value"/> is used as its internal state.
/// </remarks>
public class Address : IInternalStateEmpty
{
    private readonly ILogger<Address> _log;
    public Address(InternalStateEmpty _, ILogger<Address> log)
    {
        _log = log;
    }

    public int Number { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Address Update(int number, string name)
    {
        _log.LogDebug("Street address updated.");
        Name = name;
        Number = number;

        return this;
    }
}
