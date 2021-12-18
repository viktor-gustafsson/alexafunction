using Google.Cloud.Firestore;

namespace AlexaFunction.DAL;

[FirestoreData]
public class DeviationData
{
    [FirestoreProperty] public string Deviation { get; set; }
    [FirestoreProperty] public DateTime Time { get; set; }
}