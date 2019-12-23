using System;
using MongoDB.Driver;

namespace AlexaFunction.DAL
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly string _connectionString =  Environment.GetEnvironmentVariable("mongoConnectionString");

        public MongoDbContext()
        {
            var client = GetMongoClient();
            if (client != null) _database = client.GetDatabase("alexa");
        }

        private MongoClient GetMongoClient() =>
            new MongoClient(MongoClientSettings.FromConnectionString(_connectionString));

        public IMongoCollection<UserStationData> UserStationData =>
            _database.GetCollection<UserStationData>("userStationData");
    }
}