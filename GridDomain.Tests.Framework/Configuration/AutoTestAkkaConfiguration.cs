using System.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    public class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public AutoTestAkkaConfiguration(LogVerbosity verbosity = LogVerbosity.Trace)
            : base(new AutoTestAkkaNetworkAddress(),
                   GetConfiguration(), 
                   verbosity)
        {
        }

        private static IAkkaDbConfiguration GetConfiguration()
        {
            if(ConnectionStringPresentedInConfiguration()) 
                return new ConfigAkkaDbConfiguration();

            return new AutoTestAkkaDbConfiguration();
        }

        private static bool ConnectionStringPresentedInConfiguration()
        {
            return !string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[ConfigAkkaDbConfiguration.WriteDatabaseConnectionStringName]?.ConnectionString);
        }
    }
}