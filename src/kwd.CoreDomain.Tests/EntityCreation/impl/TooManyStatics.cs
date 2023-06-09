using kwd.CoreDomain.EntityCreation;

namespace kwd.CoreDomain.Tests.EntityCreation.impl;

public class TooManyStatics : IEntityStateNull
{
    public static Task<TooManyStatics> New(NoInternalState _)
        => Task.FromResult(new TooManyStatics());

    public static ValueTask<TooManyStatics> Create(NoInternalState _)
        => ValueTask.FromResult(new TooManyStatics());
}