 # Overview
 A set of patterns / tooling to help create 
   richer domain-centric code in .NET

 ## Entity provider
 A service to create entities, supporting

 1. Seperate internal-state from entity interface.
 2. Support injecting services into an entity
 3. Support 2-step creation, with async initalization.
 
See [[EntityProvider|EntityCreation/EntityProvider]] for details.

## Domain Services

Basic services useful in a domain

1. A simple __IClock__ for injectable time.

See [[Services|Services]] for details.
