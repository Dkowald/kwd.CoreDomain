using kwd.CoreDomain.EntityCreation;

namespace kwd.CoreDomain.Tests.EntityCreation.impl;

public class TooManyStatics : IInternalStateEmpty
{
    public static Task<TooManyStatics> New(InternalStateEmpty _)
        => Task.FromResult(new TooManyStatics());

    public static ValueTask<TooManyStatics> Create(InternalStateEmpty _)
        => ValueTask.FromResult(new TooManyStatics());
}