using kwd.CoreDomain.EntityCreation;
using Microsoft.Extensions.Logging;

namespace kwd.CoreDomain.Tests.EntityCreation.impl;

public class TooManyCtors : IEntityStateNull
{
    private TooManyCtors(NoInternalState _){}
    public TooManyCtors(NoInternalState _, ILogger<TooManyCtors> __){}
}