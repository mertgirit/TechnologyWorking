namespace MG.Shared.Configurations
{
    public class MongoDBSettings
    {
        public string MongoDBUserName { get; set; }
        public string MongoDBPassword { get; set; }
        public string MongoDBName { get; set; }
        public string MongoDBAuthMechanism { get; set; }
        public string MongoDBHost { get; set; }
        public int MongoDBPort { get; set; }
    }
}