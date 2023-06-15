using kwd.CoreDomain.EntityCreation;
using kwd.CoreDomain.EntityCreation.Errors;
using kwd.CoreDomain.EntityCreation.impl;

namespace kwd.CoreDomain.Tests.EntityCreation.impl;
[TestClass]
public class EntityMethodFactoryTests
{
    [TestMethod]
    public void MethodFactoryStrategy_PreferStatic()
    {
        var strategy = EntityMethodFactory.MethodFactoryStrategy(
            typeof(EntityWithBothStaticAndCtor), typeof(InternalStateEmpty));
        
        Assert.IsNotNull(strategy.Static);
    }

    [TestMethod]
    public void TryFindStaticFactoryMethod_ValueTaskOrTask()
    {
        var op1 = EntityMethodFactory.TryFindStaticFactoryMethod(typeof(EntityWithBothStaticAndCtor), typeof(InternalStateEmpty));
        
        Assert.IsNotNull(op1);
        
        var op2 = EntityMethodFactory.MethodFactoryStrategy(typeof(EntityWithTaskStatic), typeof(InternalStateEmpty));

        Assert.IsNotNull(op2);
    }

    [TestMethod]
    public void TryFindStaticFactoryMethod_MultipleValidStatics()
    {
        try
        {
            var _ = EntityMethodFactory.TryFindStaticFactoryMethod(typeof(TooManyStatics), typeof(InternalStateEmpty));
            Assert.Fail("Entity is broken");
        }
        catch (EntityFactoryDuplicates ex)
        {
            Assert.AreEqual(EntityFactoryDuplicates.Reasons.MultipleStaticMethods, ex.Reason);
        }
    }

    [TestMethod]
    public void TryFindConstructorMethod_MultipleOptions()
    {
        try
        {
            var _ = EntityMethodFactory.TryFindConstructorMethod(typeof(TooManyCtors), typeof(InternalStateEmpty));
            Assert.Fail("Entity is broken");
        }
        catch (EntityFactoryDuplicates ex)
        {
            Assert.AreEqual(EntityFactoryDuplicates.Reasons.MultipleConstructors, ex.Reason);
        }
    }
}

public class EntityWithBothStaticAndCtor : IInternalStateEmpty
{
    public EntityWithBothStaticAndCtor(InternalStateEmpty _){}

    public static ValueTask<EntityWithBothStaticAndCtor> New(InternalStateEmpty _)
    {
        throw new NotImplementedException();
    }
}

public class EntityWithTaskStatic : IInternalStateEmpty
{
    public static Task<EntityWithTaskStatic> X(InternalStateEmpty _)
    { throw new NotImplementedException(); }
}