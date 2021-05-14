namespace MG.Settings.Configuration
{
    public class CassandraConfiguration
    {
        /// <summary>
        /// Cassandra Sunucu Bilgisi
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Cassandrada kullanılan ilgili DB Adı
        /// </summary>
        public string KeystoreName { get; set; }
    }
}