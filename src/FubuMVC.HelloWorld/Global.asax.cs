using System.Web;
using Bottles;
using FubuCore.Configuration;
using FubuMVC.Core;
using FubuMVC.HelloWorld.Services;
using StructureMap;
using FubuMVC.StructureMap;

namespace FubuMVC.HelloWorld
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            FubuApplication
                .For<HelloWorldFubuRegistry>()
                .StructureMap(() => new Container(SetupContainer))
                .Bootstrap();

            // If there is an error during bootstrapping, it will not automatically be considered
            // fatal and there will be no YSOD.  This is to help during initial debugging and 
            // troubleshooting package loading. Normally, however, you want a YSOD if there is
            // a bootstrapping failure or a package-loading failure. This next line ensures that.
            BottlesRegistry.AssertNoFailures(); 
        }

        private static void SetupContainer(ConfigurationExpression x)
        {
            x.For<IHttpSession>().Use<CurrentHttpContextSession>();
            x.Scan(i =>
            {
                i.TheCallingAssembly();
                i.Convention<SettingsScanner>();
            });
            x.For<ISettingsProvider>().Use<AppSettingsProvider>();
            x.SetAllProperties(s => s.Matching(p => p.Name.EndsWith("Settings")));
        }
    }
}
