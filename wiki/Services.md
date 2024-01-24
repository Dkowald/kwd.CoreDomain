## Overview

General use service(s)

### Basic clock interface
A simple __IClock__ to leverage an injectable interface for 
__DateTime__ (testable).

I prefere this over 

[Microsoft.AspNetCore.Authentication.ISystemClock](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.isystemclock?view=aspnetcore-7.0)
, which requires AspNet, which I may not be interested in taking a dependancy on.

[Microsoft.Extensions.Internal.ISystemClock](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.internal.isystemclock?view=dotnet-plat-ext-7.0)
, which adds dependency on Caching.Abstractions (not too bad). 
But the _.Internal__ namespace part is off-putting.

Note: as of .NET 8 the new [System.TimeProvider](https://learn.microsoft.com/en-us/dotnet/api/system.timeprovider?view=net-8.0)
  provides an injectable clock.