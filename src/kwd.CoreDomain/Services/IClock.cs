using System;

namespace kwd.CoreDomain.Services;

/// <summary>
/// A simple interface to access current date-time.
/// </summary>
/// <remarks>
/// This is a stop-gap to avoid using:
///
/// 1. The Microsoft.AspNetCore.Authentication.ISystemClock from AspNetCore
///   since that implies a web app, which is not always true.
///
/// 2. The Microsoft.Extensions.Internal.ISystemClock
///   since that implies use of an internal interface.
/// </remarks>
#if NET8_0_OR_GREATER
[Obsolete("Consider replacing with System.TimeProvider")]
#endif
public interface  IClock
{
    /// <summary>
    /// The DateTime Now (UTC).
    /// </summary>
    public DateTime UtcNow { get; }
}