using System;
using System.Linq;
using Cassandra;
using Cassandra.Mapping;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MG.DAL.Cassandra
{
    using MG.Shared.Configurations;
    using MG.Models.CassandraModels;
    using MG.DAL.Cassandra.Interface;
    using MG.Shared.ExtensionMethods;
    using MG.Models.CassandraModels.Interface;

    public class CassandraRepository<T> : ICassandraRepository<T> where T : IDocumentModel, new()
    {
        ISession Session = null;
        IMapper Mapper = null;
        private CassandraConfiguration CassandraConfiguration;
        public CassandraRepository(IConfiguration configuration)
        {
            var CassandraConfiguration = configuration.GetSection("CassandraConfiguration").Get<CassandraConfiguration>();
            this.CassandraConfiguration = CassandraConfiguration;
            Initialize(CassandraConfiguration);
        }

        private void Initialize(CassandraConfiguration configuration)
        {
            try
            {
                Cluster cluster = Cluster.Builder().AddContactPoint(configuration.Host).Build();
                Session = cluster.Connect(configuration.KeystoreName);

                MapCollections();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void MapCollections()
        {
            try
            {
                Mapper = new Mapper(Session);

                var isFileDocumentDefined = MappingConfiguration.Global.Get<FileDocumentModel>();

                if (isFileDocumentDefined == null)
                {
                    MappingConfiguration.Global.Define(
                        new Map<FileDocumentModel>()
                        .TableName("filedocument")
                        .PartitionKey(p => p.Id)
                    );
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> FindByIdAsync(Guid id)
        {
            try
            {
                string tableName = typeof(T).GetAttributeValue((TableNameAttribute tna) => tna.Value);
                var result = await Mapper.FetchAsync<T>($"select * from {tableName} where id = {id}");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public async Task<T> InsertAsync(T model)
        {
            try
            {
                await Mapper.InsertAsync(model);
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Insert(T model)
        {
            Mapper.Insert(model);
        }
    }
}