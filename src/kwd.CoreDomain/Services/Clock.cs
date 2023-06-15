using System;

namespace kwd.CoreDomain.Services;

/// <summary>
/// Implement the <see cref="IClock"/>
/// </summary>
public class Clock : IClock
{
    private readonly DateTime? _fixed;

    /// <summary>
    /// Create a <see cref="IClock"/> that
    /// uses <see cref="DateTime.UtcNow"/>.
    /// </summary>
    public Clock()
    { _fixed = null; }

    /// <summary>
    /// Create a <see cref="IClock"/> with
    /// a fixed <see cref="DateTime"/> of <paramref name="now"/>
    /// </summary>
    public Clock(DateTime now)
    { _fixed = now; }

    /// <inheritdoc />
    public DateTime UtcNow => _fixed ?? DateTime.UtcNow;
}