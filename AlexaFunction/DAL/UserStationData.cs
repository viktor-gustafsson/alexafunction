using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlexaFunction.DAL;

public class UserStationData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string AmazonUserId { get; set; }
    public string FromStation { get; set; }
    public string ToStation { get; set; }
    public int DepartureBuffer { get; set; }
}