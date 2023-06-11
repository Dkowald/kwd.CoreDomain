using kwd.CoreDomain.EntityCreation.Errors;
using kwd.CoreDomain.EntityCreation.impl;
using kwd.CoreDomain.Samples;

using Microsoft.Extensions.DependencyInjection;

namespace kwd.CoreDomain.Tests.EntityCreation.impl;

[TestClass]
public class EntityProviderTests
{
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
}
