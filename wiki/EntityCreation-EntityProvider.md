# Overview
EntityProvider implements an enhanced object creation pattern.

Supporting:
1. Creating an entity from a seperate InternalState record.
2. Injecting services into an entity during creation.
3. Allow use of async while creating an entity.

There are a number of samples included in the repository.

# Usage

## Entity internal state

An entity MUST implement __IInternalState&lt;TState&gt;__,
this describes the entities internal-state, allong with a method to
retrieve it from the entity.

Internal state represents the data to create an entity,
 it can be different from what the entity presents on its interface.

Alternatly use __IInternalStateEmpty__ where no internal-state is needed.

## Entity factory

The entity needs to include a factory method. 

The factory MUST consume a __TState__.
If using __IInternalStateEmpty__, the internal state is a __InternalStateEmpty__.

There are 3 approaches for describing the factory:

1. Explicit define a __IEntityFactory&lt;TEntity, TState&gt;__
    
    > Generally not required. 
    > One of the other methods is preferrable.

2. Include a static method that comsumes a __TState__ and returns Task&lt;TEntity&gt; or ValueTask&lt;TEntity&gt;

    > Use this if require 2-step creation.
    > Includes injecting services.
    
3. Include a constructor that consumes a __TState__

    > Construct with injected services.

The provider selects the first factory, based on the order above.
The selected factory must have only one implementation.

e.g

 1. No explicit _EntityFactory_ found look for static method.
 2. If found multiple static methods that could be used,
    > throw EntityFactoryDuplicates

## Setup container

The __IServiceCollection.AddEntityProvider__ extension wraps adding the 
IEntityProvider, allong with discovering and registering IEntityFactory's

This will

1. Add the **IEntityProvider** service, used to create an entity.

2. Discover entity factories, and register corresponding __IEntityFactory&lt;TEntity, TState&gt;__ services.

By default these are registered as Scoped services.

^ Also adds a supporting IEntityMethodStrategies singleton

### A note on container lifetime scoping.

The EntityProvider, and implicit method factories are
registered with the container using the configured lifetime-scope.

Care must be taken with selecting this; it is reasonably likely that
users will create entities within a container scoped lifetime (e.g. web-request).

As such, the provider (and factories) should allso be in the same scope,
so created entites recieve service instance from the corect scope.

## Samples
See the Samples project in this repository for more details on usage.
