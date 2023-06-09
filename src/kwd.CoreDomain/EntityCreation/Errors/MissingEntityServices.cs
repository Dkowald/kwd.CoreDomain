using System;

namespace kwd.CoreDomain.EntityCreation.Errors;

/// <summary>
/// Cannot resolve all services required to create a <see cref="EntityType"/>
/// </summary>
public class MissingEntityServices : Exception
{
    /// <inheritdoc />
    public MissingEntityServices(Type entityType, Type serviceType, Exception inner)
        : base($"Failed create {entityType.Name}; service {serviceType.Name} not registered", inner)
    {
        EntityType = entityType;
        ServiceType = serviceType;
    }

    /// <summary>Type of entity being created</summary>
    public Type EntityType { get; }

    /// <summary>Missing service type</summary>
    public Type ServiceType { get; }
}
