using kwd.CoreDomain.EntityCreation;
using kwd.CoreDomain.EntityCreation.Errors;
using kwd.CoreDomain.EntityCreation.impl;
using kwd.CoreDomain.Samples;
using Microsoft.Extensions.DependencyInjection;

namespace kwd.CoreDomain.Tests.EntityCreation.impl;
[TestClass]
public class EntityMethodFactoryTests
{
    [TestMethod]
    public void MethodFactoryStrategy_PreferStatic()
    {
        var strategy = EntityMethodFactory.MethodFactoryStrategy(
            typeof(EntityWithBothStaticAndCtor), typeof(NoInternalState));

        Assert.IsNotNull(strategy.Static);
    }

    [TestMethod]
    public void FindStaticFactoryMethod_ValueTaskOrTask()
    {
        var op1 = EntityMethodFactory.FindStaticFactoryMethod(typeof(EntityWithBothStaticAndCtor), typeof(NoInternalState));
        
        Assert.IsNotNull(op1);
        
        var op2 = EntityMethodFactory.MethodFactoryStrategy(typeof(EntityWithTaskStatic), typeof(NoInternalState));

        Assert.IsNotNull(op2);
    }

    [TestMethod]
    public void FindStaticFactoryMethod_MultipleValidStatics()
    {
        try
        {
            var _ = EntityMethodFactory.FindStaticFactoryMethod(typeof(TooManyStatics), typeof(NoInternalState));
            Assert.Fail("Entity is broken");
        }
        catch (EntityFactoryDuplicates ex)
        {
            Assert.AreEqual(EntityFactoryDuplicates.Reasons.MultipleStaticMethods, ex.Reason);
        }
    }

    [TestMethod]
    public void FindConstructorMethod_MultipleOptions()
    {
        try
        {
            var _ = EntityMethodFactory.FindConstructorMethod(typeof(TooManyCtors), typeof(NoInternalState));
            Assert.Fail("Entity is broken");
        }
        catch (EntityFactoryDuplicates ex)
        {
            Assert.AreEqual(EntityFactoryDuplicates.Reasons.MultipleConstructors, ex.Reason);
        }
    }
}

public class EntityWithBothStaticAndCtor : IEntityStateNull
{
    public EntityWithBothStaticAndCtor(NoInternalState _){}

    public static ValueTask<EntityWithBothStaticAndCtor> New(NoInternalState _)
    {
        throw new NotImplementedException();
    }
}

public class EntityWithTaskStatic : IEntityStateNull
{
    public static Task<EntityWithTaskStatic> X(NoInternalState _)
    { throw new NotImplementedException(); }
}