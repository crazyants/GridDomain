namespace GridDomain.Node.Configuration.Persistence
{
    public class LocalDbConfiguration : IDbConfiguration
    {
        public string ReadModelConnectionString
            => @"Server=tcp:soloinfra.cloudapp.net,5099;Database=stagereadmodel;User ID=solomoto;Password=s0l0moto;Connection Timeout=30;MultipleActiveResultSets=True";

        public string LogsConnectionString
            => @"Server=tcp:soloinfra.cloudapp.net,5099;Database=stagelogs;User ID=solomoto;Password=s0l0moto;Connection Timeout=30;MultipleActiveResultSets=True";
    }
}