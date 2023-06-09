using kwd.CoreDomain.EntityCreation;
using kwd.CoreDomain.EntityCreation.impl;
using kwd.CoreDomain.Samples;

using Microsoft.Extensions.DependencyInjection;

namespace kwd.CoreDomain.Tests.EntityCreation;

[TestClass]
public class EntityProviderConfigTests
{
    [TestMethod]
    public void EntityLifetime_()
    {
        var services = new ServiceCollection();
        
        var target = new EntityProviderConfig();
        target.EntityLifetime(ServiceLifetime.Transient);

        target.AddEntityTypes<Student>()
            .AddEntityTypes<ClassRoom>()
            .AddEntityTypes<Desk>();

        target.RegisterTypes(services);
        
        var provider = services.SingleOrDefault(x => x.ServiceType == typeof(IEntityProvider));
        Assert.AreEqual(ServiceLifetime.Transient, provider?.Lifetime, "Provider lifetime matches configured");

        var factories = services.Where(x => x.ImplementationType?.IsAssignableTo(typeof(IEntityFactory)) == true)
            .ToArray();

        Assert.IsTrue(factories.All(x => x.Lifetime == ServiceLifetime.Transient), "All registered with configured lifetime");
    }

    [TestMethod]
    public void ExplicitFactoryRegistered()
    {
        var services = new ServiceCollection();

        //Explicit registration.
        services.AddSingleton<IEntityFactory<Student, Student.State>, StudentEntityFactoryCustom>();
        
        var target = new EntityProviderConfig();
        target.EntityLifetime(ServiceLifetime.Scoped);
        target.AddEntityTypes<Student>();

        target.RegisterTypes(services);

        var factory = 
            services.SingleOrDefault(x => x.ImplementationType?.IsAssignableTo(typeof(IEntityFactory)) == true);
        
        Assert.IsNotNull(factory);
        Assert.AreEqual(ServiceLifetime.Singleton, factory.Lifetime, "Already added, leave it be.");
    }

    [TestMethod]
    public void RegisterFactoryWithClosedGeneric()
    {
        var target = new EntityProviderConfig();

        target.AddEntityTypes<Desk>();

        var service = new ServiceCollection();
        
        target.RegisterTypes(service);

        var factoryType = EntityProvider.EntityFactoryType(typeof(Desk), typeof(Desk.State));

        var methodFactoryType = EntityMethodFactory.MethodFactoryType(typeof(Desk), typeof(Desk.State));

        var typeRegistration = service.SingleOrDefault(x => x.ServiceType == factoryType);
        
        Assert.IsNotNull(typeRegistration, "factory was registered");
        Assert.AreEqual(methodFactoryType, typeRegistration.ImplementationType, "factory is a method factory");
    }

    [TestMethod]
    public void MethodFactoryMatchesEntityFactory()
    {
        var methodFactory = EntityMethodFactory.MethodFactoryType(typeof(ClassRoom), typeof(ClassRoom.State));

        var entityFactory = EntityProvider.EntityFactoryType(typeof(ClassRoom), typeof(ClassRoom.State));

        Assert.IsTrue(methodFactory.IsAssignableTo(entityFactory), "The EntityMethodFactory must be usable by EntityProvider");
    }

    [TestMethod]
    public void Register_ScanAssembly()
    {
        var services = new ServiceCollection();

        var target = new EntityProviderConfig();
        target.AddAssembly<Desk>();
        
        target.RegisterTypes(services);

        var factoryType = EntityProvider.EntityFactoryType(typeof(Student),
            EntityProvider.EntityStateType(typeof(Student)));

        var factoryRegistration = services.SingleOrDefault(x => x.ServiceType == factoryType);

        Assert.IsNotNull(factoryRegistration, "Scans assembly for entities");
    }
}
