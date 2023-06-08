using System.Text.Json;
using kwd.CoreDomain.EntityCreation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.CoreDomain.Samples;

[TestClass]
public class Samples
{
    [TestMethod("An entity with no state, but wants services")]
    public async Task Address_()
    {
        await using var cont = new ServiceCollection()
            .AddEntityProvider(cfg => cfg.AddEntityTypes<Address>())
            .AddLogging()
            .BuildServiceProvider();

        var provider = cont.GetRequiredService<IEntityProvider>();

        var entity = await provider.Create<Address>(NoInternalState.Value);
        
        Assert.AreEqual("", entity.Name);
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
            .AddEntityProvider(cfg => cfg.AddEntityTypes<ClassRoom>())
            .AddLogging()
            .BuildServiceProvider();

        var entity = cont.GetRequiredService<IEntityProvider>();

        //Note:
        // This also demonstrated replacing the factory for a pre-existing entity
        // with a better one.
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
        var entityState = ((IEntityState)entity).GetCurrentState();
        var storedJson = JsonSerializer.Serialize(entityState);

        
        //now re-load:
        var jsonCfg = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        
        var stateType = provider.TryGetStateType<ClassRoom>() ?? typeof(ClassRoom);
        var loadedState = JsonSerializer.Deserialize(storedJson, stateType)
                          ?? throw new Exception("Deserialize failed");

        var loadedEntity = await provider.Create<ClassRoom>(loadedState);
        
        Assert.AreEqual(entity.Name, loadedEntity.Name);
    }
    
    //Entity with both ctor-factory and static-factory
    //assert.warn
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
}