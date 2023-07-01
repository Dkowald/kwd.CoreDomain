# Overview

A set of patterns / tooling to help create 
richer domain-centric code in .NET

## Entity provider
An approach to enhance entites to support:
1. Seperate internal from presented state.
2. Allow service-injection during create.
3. Allow async 2-step create / initalization.

 _Quick start_
```cs
public class Staff : IEntityState<Staff.State>
{
    //internal entity state
    public record State(string Name);

    //Get current internal state.
    State IEntityState<State>.CurrentState() => new(Name);

    //Create from state, with service injected
    public Staff(State state, ILogger<Staff> log)
    {
        Name = state.Name;
        log.LogDebug("Staff loaded");
    }

    public string Name { get; }
}

await using var cont = new ServiceCollection()
            .AddLogging()
            .AddEntityProvider(cfg => cfg.AddEntityTypes<Staff>())
            .BuildServiceProvider();

var provider = cont.GetRequiredService<IEntityProvider>();

var entity = await provider.Create<Staff>(new Staff.State("bob"));
Assert.AreEqual("bob", entity.Name);
```

See [wiki](https://github.com/Dkowald/kwd.CoreDomain/wiki/) for details

---
^ [source](https://github.com/Dkowald/kwd.CoreDomain) | [nuget](https://www.nuget.org/packages/kwd.CoreDomain/)

^ Icon by <a href="https://www.svgrepo.com" target="_blank">SVG Repo</a>