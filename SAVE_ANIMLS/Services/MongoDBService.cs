using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SAVE_ANIMLS.Models;

namespace SAVE_ANIMLS.Services
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;

        public MongoDBService(IOptions<MongoDBSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _database = client.GetDatabase(options.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public List<T> GetAll<T>(string collectionName)
        {
            var collection = _database.GetCollection<T>(collectionName);
            return collection.Find(FilterDefinition<T>.Empty).ToList();
        }

        public void Insert<T>(string collectionName, T document)
        {
            var collection = _database.GetCollection<T>(collectionName);
            collection.InsertOne(document);
        }
    }
}