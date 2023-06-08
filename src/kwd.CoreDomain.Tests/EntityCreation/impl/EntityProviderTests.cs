using System.Security.Cryptography;
using System.Xml.Xsl;
using kwd.CoreDomain.EntityCreation;
using kwd.CoreDomain.EntityCreation.Errors;
using kwd.CoreDomain.EntityCreation.impl;
using kwd.CoreDomain.Samples;
using Microsoft.Extensions.DependencyInjection;

namespace kwd.CoreDomain.Tests.EntityCreation.impl;

internal class SingleService{}
internal class ScopedService{}
internal class TransientService{}

internal class MyEntity : IEntityStateNull
{
    public MyEntity(NoInternalState _, SingleService singleService, ScopedService scopedService, TransientService tempService)
    {
        SingleService = singleService;
        ScopedService = scopedService;
        TempService = tempService;
    }

    public SingleService SingleService { get; }
    public ScopedService ScopedService { get; }
    public TransientService TempService { get; }
}

[TestClass]
public class EntityProviderTests
{

    private class Services : IDisposable
    {
        private readonly ServiceProvider _container;
        public Services(Action<IServiceCollection>? cfg = null)
        {
            var services = new ServiceCollection()
                .AddScoped<EntityProvider>()
                .AddScoped<IEntityProvider>(ctx => ctx.GetRequiredService<EntityProvider>());

            cfg?.Invoke(services);

            _container = services.BuildServiceProvider();
        }

        public IServiceProvider Container => _container;

        public EntityProvider GetTarget() =>
            _container.GetRequiredService<EntityProvider>();

        public void Dispose()
        {
            _container.Dispose();
        }
    }

    [TestMethod]
    public async Task Create_MissingFactory()
    {
        await using var services = new ServiceCollection()
            .BuildServiceProvider();

        var target = new EntityProvider(services);
        try
        {
            var _ = await target.Create<Desk>(Desk.State.New("a.1"));
            Assert.Fail("Entity type factory not registered.");
        }
        catch (EntityFactoryNotRegistered ex)
        {
            Assert.AreEqual(typeof(Desk), ex.EntityType);
        }
    }

    //[TestMethod]
    //public async Task Create_InScopedLifetime()
    //{
    //    await using var cont = new ServiceCollection()
    //        .AddSingleton<SingleService>()
    //        .AddScoped<ScopedService>()
    //        .AddTransient<TransientService>()
    //        .AddEntityProvider(cfg => cfg
    //            .EntityLifetime(ServiceLifetime.Scoped)
    //            .AddEntityTypes<MyEntity>())
    //        .BuildServiceProvider();

    //    IServiceProvider svc = cont;

    //    //tho global / un-scoped provider.
    //    var provider = svc.GetRequiredService<IEntityProvider>();

    //    var a = await provider.Create<MyEntity>(NoInternalState.Value);

    //    using (var scope = cont.CreateScope())
    //    {
    //        var scopedProvider = scope.ServiceProvider.GetRequiredService<IEntityProvider>();
            
    //        var b = await scopedProvider.Create<MyEntity>(NoInternalState.Value);

    //        Assert.IsFalse(ReferenceEquals(a.ScopedService, b.ScopedService), "Uses scoped service");

    //        Assert.IsFalse(ReferenceEquals(a.TempService, b.TempService), "Transient service used");

    //        Assert.IsTrue(ReferenceEquals(a.SingleService, b.SingleService), "Singleton used");
    //    }

    //    var c = await provider.Create<MyEntity>(NoInternalState.Value);

    //    Assert.IsFalse(ReferenceEquals(a.TempService, c.TempService), "");
    //}

    //[TestMethod]
    //public async Task Create_MissingEntityServices()
    //{
    //    var service = new ServiceCollection()
    //        .AddEntityProvider(cfg => cfg.AddEntityTypes<School>())
    //        .AddLogging()
    //        .BuildServiceProvider();

    //    var target = (EntityProvider)service.GetRequiredService<IEntityProvider>();

    //    try
    //    {
    //        await target.Create<School>(School.State.New("S"));
    //        Assert.Fail("needs missing service");
    //    }
    //    catch (MissingEntityServices ex)
    //    {
    //        Assert.AreEqual(typeof(IRepository), ex.ServiceType);
    //        Assert.AreEqual(typeof(School), ex.EntityType);
    //    }
    //}

    //[TestMethod]
    //public async Task Create_WithExplicitFactory()
    //{
    //    var svc = new Services(cfg =>
    //        cfg.AddSingleton<IEntityFactory<Student, Student.State>, StudentEntityFactory>());

    //    var target = svc.GetTarget();

    //    var result = await target.Create<Student>(new Student.State("test"));

    //    Assert.IsFalse(result.CreatedFromCtor);
    //}

    //[TestMethod]
    //public async Task Create_AsyncInitEntity()
    //{
    //    using var svc = new Services(cfg =>
    //    {
    //        cfg.AddEntityProvider(x => x.AddEntityTypes<Desk>());
    //    });

    //    var target = svc.GetTarget();

    //    var data = await target.Create<Desk>(new Desk.State("1A", "new"));

    //    Assert.IsTrue(data.IsInitalized, "Entity init should be called");
    //}

    //[TestMethod]
    //public async Task ReadWrite_EntityWithStateAndServices()
    //{
    //    using var svc = new Services(cfg =>
    //        cfg.AddEntityProvider(x => x.AddEntityTypes<User>()));

    //    var target = svc.GetTarget();

    //    var entity = await target.Create<User>(new User.State("Upton"));

    //    Assert.AreEqual("Upton", entity.Name);
    //}
    
}
