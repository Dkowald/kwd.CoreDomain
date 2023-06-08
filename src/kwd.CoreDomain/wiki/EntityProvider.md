# Overview
EntityProvider implements an enhanced object creation pattern.

Supporting:
1. Creating an entity from a seperate EntityState record.
2. Injecting services into an entity during creation.
3. Allow use of async while creating an entity.

## Describe an entity

To partisipate the entity MUST implement __IEntityState&lt;TState&gt;__,
this describes the entities internal-state, allong with a method to
retrieve it from the entity.

Next; the entity needs to include a factory method. 
The factory MUST consume a __TState__. 
There are 3 approaches for describing the factory.

1. Explicit define a __IEntityFactory&lt;TEntity, TState&gt;__
2. Include a static method that comsumes a __TState__ and returns Task&lt;TEntity&gt;
3. Include a constructor that consumes a __TState__

## Setup container

The __IServiceCollection.AddEntityProvider__ extension wraps adding the 
IEntityProvider, allong with registering IEntityFactory's

The provider can be configured to 

|Description|Sample|
|-----------|------|
|

## Using the provider


### A note on container lifetime scoping.

The EntityProvider, and implicit method factories are
registered with the container using the configured lifetime-scope.

Care must be taken with selecting this; it is reasonably likely that
users will create entities within a container scoped lifetime (e.g. web-request).

As such, the provider (and factories) should allso be in the same scoped,
so created entites recieve service instance from the corect scope.