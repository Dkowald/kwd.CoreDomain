using kwd.CoreDomain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace kwd.CoreDomain.Tests.Services;

[TestClass]
public class ClockTests
{
    [TestMethod]
    public void FixedTime()
    {
        var now = new DateTime(2000, 10, 10, 2, 10, 00, DateTimeKind.Utc);

        var cont = new ServiceCollection()
            //default clock.
            .AddClock()
            //fixed-time clock for testing.
            .AddSingleton<IClock>(new Clock(now))
            .BuildServiceProvider();

        var when = cont.GetRequiredService<IClock>().UtcNow;

        Assert.AreEqual(now, when);
    }
}