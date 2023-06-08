# An Entity creation pattern

One of the first hurdles to using richer domain objects in
.NET, is to break from the common DTO + Services approach
for software.

In the past I've used a variaty of approaches to try model 
software with mutable entites rather than DTO + Services.
This EntityProvider pattern allows me to create classes that:

1. Creating an entity from a seperate EntityState record.

   So I can use read-only or derived properties on my entity, 
    and not spend follow-up code trying to serialize.

2. Injecting services into an entity during creation.

   So I can use services in Entity's methods, such as __ILogger__.   

3. Allow use of async while creating an entity.

   So I can use a 2-stage create allowing an entity to
    call additional services during creation / initialization.
   With this I can verify check other systems as part of initalization.


# EntityProvider

This provides an approach to more clearly code a rich; mutable __entity__, and allow 
for service injection; async construct; and clear seperation of entity-state.

## Quick start
For this story, lets start at the end. 
I've an implementation of EntityProvider in XXX???XXX.
Below is a quick-start for using the library:

```cs
//An entity: 
public ClassRoom : IEntityState<ClassRoom.State>{
  private int ReservedSpaceForTeacher = 50;
  private const int DeskSize = 9;

  //Don't need to expose a internal field for serializing.
  private int _floorSpace;

  //It's core internal state
  public record State(int NumberOfDesks, int FloorSpace);
  
  //create state from entity
  IEntityState<ClassRoom.State>.CurrentState()
    => new State(NumberOfDesks, _floorSpace);

  //create entity from state.
  public static Task<ClassRoom> New(ClassRoom.State state, ILogger<ClassRoom> log)
  { ... }

  //Dont need to have the proerty mutable.  
  public int NumberOfDesks {get; private set;}
  
  public int MaxDesks => 
   FloorSpace 
   - ReservedSpaceForTeacher
   - Desks * DeskSize;
}
```

_using the provider_
```cs
 await using var cont = 
   new ServiceCollection()
   .AddEntityProvider(
      cfg => cfg.AddEntityTypes<ClassRoom>())
   .BuildServiceProvider();

 var provider = 
   cont.GetRequiredService<IEntityProvider>();

 var initState = new ClassRoom.State(10, 100);

 var entity = 
   provider.Create<ClassRoom>(initState);

 entity.AddDesk()
```

Lets now break down the different parts of the pattern.

### entity state
Firstly each entity implements ***IEntityState&lt;TState&gt;*** this identifies an entity, and 
the (POCO) __entityState__ object to be used for state.

This seperation means my Entity is not constrained by a need to serialize.
The simple POCO representation of the Entity is captured as a TState object.

There is strong coupling between an Entity and its State, thats why I prefere
to use a _nested record for entity state_

Because the entityState is seperate, it only contains what's needed, 
and should not need special attributes. 
The entityState is tightly coupled with the entity.

Because of this tight-coupling, I prefer to use a _nested record_ to represent __TState__

### entity factory

We also need the inverse; create an entity from the entityState

Lets define a ***IEntityFactory&lt;TEntity,TState&gt;*** to capture this.

```cs
//Create entity from entityState
public interface IEntityFactory<TEntity, TState>
{
   ValueTask<TEntity> Create(TState entityState) 
}
```

These factories can be explicitly defined; or infered from the entity itself.
There are 2 strategies to infer the factory:

1. Constructor based. 

    Look for a constructor on  the **entity** that takes an **entityState** and 
zero or more services.

2. Static based.

    Look for a static method that takes a **entityState** and zero or more services.
And returns a **Task&lt;TEntity&gt;**

In this simple sample, we only need a basic constructor.

```cs 
public class ClassRoom{
  //create a ClassRoom from its state.
  public ClassRoom(State state) {
     NumberOfDesks = state.NumberOfDesks;
     FloorSpace = state.FloorSpace;
  }
}
```

### Putting it together.
```cs
var cont = new ServiceCollection()
.Build();
```

-------



This is the essential core of the approach.
I now need only register a set of ***IEntityFactory&lt;TEntity,TState&gt;***
in a container, and use them to implement an **EntityProvider**

```cs
public interface IEntityProvider
{
  ValueTask<TEntity> Create<TEntity>(object state);
}
```

By using a factory, aditional services can be injected to pass to the entity, and/or complete
construction.

These ***IEntityFactory&lt;TEntity,TState&gt;*** services must be available in the container, 
normally registered as singletons, since they should only require services as internal state.

Coding an *IEntityFactory* is a little redundent in many situations, particularly 
where an entity has no need for 2nd stage creation.
So the additional ***EntityMethodFactory*** can be used to leverage methods on the entity itself
by resolving the approach at run-time (via reflection). 

The **EntityMethodFactory** requires either
1. a static factory method that 
    + returns a  __ValueTask&lt;TEntity&gt;__
    + at-least takes a __TState__ object parameter
2. a constructor that at-least take a __TState__ object,
    it may also take additional services

_Note_: If both methods are available, the static factory method is used.
