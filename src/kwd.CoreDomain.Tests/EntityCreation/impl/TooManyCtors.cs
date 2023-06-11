using kwd.CoreDomain.EntityCreation;
using Microsoft.Extensions.Logging;

namespace kwd.CoreDomain.Tests.EntityCreation.impl;

public class TooManyCtors : IInternalStateEmpty
{
    private TooManyCtors(InternalStateEmpty _){}
    public TooManyCtors(InternalStateEmpty _, ILogger<TooManyCtors> __){}
}