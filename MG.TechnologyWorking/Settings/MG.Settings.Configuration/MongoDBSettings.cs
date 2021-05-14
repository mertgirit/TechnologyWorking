namespace MG.Settings.Configuration
{
    using MG.Settings.Configuration.Interface;

    public class MongoDBSettings : IMongoDBSettings
    {
        public string MongoDBUserName { get; set; }
        public string MongoDBPassword { get; set; }
        public string MongoDBName { get; set; }
        public string MongoDBAuthMechanism { get; set; }
        public string MongoDBHost { get; set; }
        public int MongoDBPort { get; set; }
    }
}