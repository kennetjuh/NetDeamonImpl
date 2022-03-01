using NetDaemon.AppModel;
using NetDaemonImpl.apps;
using System.Linq;
using System.Reflection;
using Xunit;

namespace NetDaemonTest
{
    public class NoFocusAttributeTest
    {
        [Fact]
        public void TestNoFocusAttribute()
        {
            var appsWithFocus = typeof(TestApp).Assembly.GetTypes().Where(t => t.GetCustomAttribute<FocusAttribute>() != null);
            Assert.Empty(appsWithFocus);
        }
    }
}
