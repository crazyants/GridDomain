using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Console
{
    class ConsoleAkkaConfiguretion : AkkaConfiguration
    {
        public ConsoleAkkaConfiguretion(string serverHostName) : 
            base(new AkkaNetworkAddress("GridConsole", serverHostName, 0),
                new ConsoleDbConfig())
        {
        }
    }
}