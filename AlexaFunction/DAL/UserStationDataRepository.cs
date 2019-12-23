using System.Threading.Tasks;
using MongoDB.Driver;

namespace AlexaFunction.DAL
{
    public class UserStationDataRepository
    {
        private readonly IMongoCollection<UserStationData> _collection;

        public UserStationDataRepository()
        {
            _collection = new MongoDbContext().UserStationData;
            _collection.Indexes.CreateOne(
                new CreateIndexModel<UserStationData>(
                    Builders<UserStationData>.IndexKeys.Ascending(data => data.AmazonUserId)));
        }

        public async Task<UserStationData> InsertUserIfNotExists(string amazonUserId)
        {
            var filterDefinition = Builders<UserStationData>.Filter.Eq(data => data.AmazonUserId, amazonUserId);
            var userStationData = await _collection.Find(filterDefinition).FirstOrDefaultAsync();

            if (userStationData != null) return userStationData;
            
            var newUserStationData = new UserStationData
            {
                AmazonUserId = amazonUserId
            };
            await _collection.InsertOneAsync(newUserStationData);
            userStationData = newUserStationData;

            return userStationData;
        }
    }
}