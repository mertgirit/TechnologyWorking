namespace MG.Settings.Configuration.Interface
{
    public interface IMongoDBSettings
    {
        string MongoDBUserName { get; set; }
        string MongoDBPassword { get; set; }
        string MongoDBName { get; set; }
        string MongoDBAuthMechanism { get; set; }
        string MongoDBHost { get; set; }
        int MongoDBPort { get; set; }
    }
}