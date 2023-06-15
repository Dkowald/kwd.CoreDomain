using System.Text.Json;
using kwd.CoreDomain.EntityCreation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.CoreDomain.Samples;

[TestClass]
public class Samples
{
    [TestMethod("Basic usage")]
    public async Task BasicUsage()
    {
        //setup container
        await using var cont = new ServiceCollection()
            //other services
            .AddLogging()
            //add entity provider
            .AddEntityProvider(cfg =>
            {
                //add only particular entities.
                cfg.AddEntityTypes<Address>();
                //add all from assembly with entities
                cfg.AddAssembly<Address>();

                //change factory scope (if desired)
                cfg.EntityLifetime(ServiceLifetime.Scoped);
            })
             .BuildServiceProvider();

        //create entity

        var provider = cont.GetRequiredService<IEntityProvider>();

        var entity = await provider.Create<Address>(InternalStateEmpty.Value);
        
        Assert.AreEqual("", entity.Name);
    }

    [TestMethod("An entity with no state, but wants services")]
    public async Task Address_()
    {
        await using var cont = new ServiceCollection()
            .AddEntityProvider(cfg => cfg.AddEntityTypes<Address>())
            .AddLogging()
            .BuildServiceProvider();

        var provider = cont.GetRequiredService<IEntityProvider>();

        var entity = await provider.Create<Address>(InternalStateEmpty.Value);
        
        Assert.AreEqual("", entity.Name);
    }

    [TestMethod("Entity with state and a service")]
    public async Task Staff_()
    {
        await using var cont = new ServiceCollection()
            .AddLogging()
            .AddEntityProvider(cfg => cfg.AddEntityTypes<Staff>())
            .BuildServiceProvider();

        var provider = cont.GetRequiredService<IEntityProvider>();

        var entity = await provider.Create<Staff>(new Staff.State("bob"));
        Assert.AreEqual("bob", entity.Name);
    }

    [TestMethod("entity with ctor services and implicit static factory.")]
    public async Task ClassRoom_()
    {
        await using var cont = new ServiceCollection()
            .AddEntityProvider(cfg => cfg.AddEntityTypes<ClassRoom>())
            .AddLogging()
            .AddSingleton<IRepository, Repository>()
            .BuildServiceProvider();

        var provider = cont.GetRequiredService<IEntityProvider>();

        //Use entity provider to create entities.
        var room1A = await provider.Create<ClassRoom>(ClassRoom.State.New("rm1", 10, 210));

        Assert.AreEqual(10, room1A.Desks);

    }
    
    [TestMethod("An entity with state, and explicit factory.")]
    public async Task Student_()
    {
        await using var cont = new ServiceCollection()
            .AddEntityProvider(cfg => cfg.AddEntityTypes<Student>())
            .AddLogging()
            .BuildServiceProvider();

        var provider = cont.GetRequiredService<IEntityProvider>();

        var entity = await provider.Create<Student>(new Student.State("bob"));
        
        Assert.AreEqual("bob", entity.Name);

        Assert.IsFalse(entity.CreatedFromCtor);
    }

    [TestMethod("Entity with both ctor-factory and static-factory")]
    public async Task Desk_()
    {
        await using var cont = new ServiceCollection()
            .AddEntityProvider(cfg => cfg.AddEntityTypes<Desk>())
            .BuildServiceProvider();

        var provider = cont.GetRequiredService<IEntityProvider>();

        var desk1 = await provider.Create<Desk>(new Desk.State("", "Broken"));

        Assert.IsTrue(desk1.IsBroken);
    }

    [TestMethod("Complex example with 2-stage create, with internal state")]
    public async Task School_()
    {
        await using var cont = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IRepository, Repository>()
            .AddEntityProvider(cfg => cfg.AddAssembly<School>())
            .BuildServiceProvider();

        var provider = cont.GetRequiredService<IEntityProvider>();

        var sue = await provider.Create<Staff>(new Staff.State("sue"));

        var repo = cont.GetRequiredService<IRepository>();
        repo.Store("bob", await provider.Create<Staff>(new Staff.State("bob")));
        repo.Store("sue", sue);
        repo.Store("sam", await provider.Create<Staff>(new Staff.State("sam")));

        var school = await provider.Create<School>(new School.State("S1", new[] { "bob", "sam" }));

        Assert.AreEqual(2, school.Staff.Count);

        school.AddStaff(sue);

        var updatedState = school.CurrentState();
        Assert.IsTrue(updatedState.Staff.Contains("sue"));
    }

    [TestMethod("Use my custom entity factory")]
    public async Task AlternateFactory()
    {
        await using var cont = new ServiceCollection()
            //Add my factory
            .AddScoped(_ => new StudentEntityFactory{ExplicitlyAddedToContainer = true})
            //register as the entity factory.
            .AddScoped<IEntityFactory<Student, Student.State>>(ctx => ctx.GetRequiredService<StudentEntityFactory>())

            .AddEntityProvider(cfg => cfg.AddEntityTypes<Student>())
            .BuildServiceProvider();

        var provider = cont.GetRequiredService<IEntityProvider>();

        var _ = await provider.Create<Student>(new Student.State("bob"));

        var customFactory = cont.GetRequiredService<StudentEntityFactory>();
        Assert.IsTrue(customFactory.ExplicitlyAddedToContainer);
        Assert.IsTrue(customFactory.CreatedAnEntity);
    }

    [TestMethod("Load and store an entity")]
    public async Task StoreAndLoadAnEntity()
    {
        await using var cont = new ServiceCollection()
            .AddEntityProvider(cfg => cfg.AddEntityTypes<ClassRoom>())
            .AddLogging()
            .AddSingleton<IRepository, Repository>()
            .BuildServiceProvider();
        
        var provider = cont.GetRequiredService<IEntityProvider>();

        //use provider to create an entity from state.
        
        var entity = await provider.Create<ClassRoom, ClassRoom.State>(
            ClassRoom.State.New("rm1", 10, 100));
        
        //store the entity state with favored serialization.
        var entityState = ((IInternalState)entity).GetCurrentState();
        var storedJson = JsonSerializer.Serialize(entityState);
        
        //now re-load:
        var stateType = provider.TryGetStateType<ClassRoom>() ?? typeof(ClassRoom);
        var loadedState = JsonSerializer.Deserialize(storedJson, stateType)
                          ?? throw new Exception("Deserialize failed");

        var loadedEntity = await provider.Create<ClassRoom>(loadedState);
        
        Assert.AreEqual(entity.Name, loadedEntity.Name);
    }
}