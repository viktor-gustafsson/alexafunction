using Google.Cloud.Firestore;

namespace AlexaFunction.DAL
{
    [FirestoreData]
    public class UserStationData
    {
        [FirestoreProperty] public string AmazonUserId { get; set; }
        [FirestoreProperty] public string FromStation { get; set; }
        [FirestoreProperty] public string ToStation { get; set; }
        [FirestoreProperty] public int DepartureBuffer { get; set; }
    }
}