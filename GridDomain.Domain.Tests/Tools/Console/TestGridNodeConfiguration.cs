using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework.Configuration;

namespace GridDomain.Tests.Tools.Console
{
    class TestGridNodeConfiguration : AkkaConfiguration
    {
        public TestGridNodeConfiguration() : base(
            new AkkaNetworkAddress("RemoteSystem", "40.118.71.240", 9000),
            new AutoTestAkkaDbConfiguration())
        {
        }
    }
}