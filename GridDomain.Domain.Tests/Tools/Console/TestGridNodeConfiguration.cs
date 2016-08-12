using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework.Configuration;

namespace GridDomain.Tests.Tools.Console
{
    class RemotetGridNodeConfiguration : AkkaConfiguration
    {
        public RemotetGridNodeConfiguration() : base(
            new AkkaNetworkAddress("RemoteSystem","localhost",9000),
            new AutoTestAkkaDbConfiguration())
        {
        }
    }
}