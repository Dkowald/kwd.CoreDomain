# Overview

A set of patterns / tooling to help create 
richer domain-centric code in .NET

## EntityProvider
A pattern to describe an entity with

1. Internal state seperated from entity state.
2. Inject services to an entity.
3. Allow a 2-phase create, so additional services can be used


### Quick start

_Describe the entity_
```cs

//declare the entity with its internal-state type.
public ClassRoom : IEntityState<ClassRoom.State>{
  private int ReservedSpaceForTeacher = 50;
  private const int DeskSize = 9;

  //Don't need to expose a internal field for serializing.
  private int _floorSpace;

  //It's core internal state
  public record State(
     int NumberOfDesks, 
     int FloorSpace, 
     string[] Students);
  
  //create state from entity
  IEntityState<ClassRoom.State>.CurrentState()
    => new State(NumberOfDesks, _floorSpace, );

  //create entity from state; 
  //include entity-service's
  //include init-service's
  public static Task<ClassRoom> New(
     ClassRoom.State state, 
     ILogger<ClassRoom> log,
     IRepository repo)
  { ... }

  //Dont need public setter.
  public int NumberOfDesks {get; private set;}
  
  //calculated; not part of internal state.
  public int MaxDesks => 
   FloorSpace 
   - ReservedSpaceForTeacher
   - Desks * DeskSize;
}
```

Use the provider
```cs
using var cont = 
  new ServiceCollection()
   .BuildServiceProvider();

var entityState = ((IEntityState)entity).GetCurrentState();
```

