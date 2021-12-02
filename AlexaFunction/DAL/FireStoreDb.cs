using Google.Cloud.Firestore;

namespace AlexaFunction.DAL;

public class FirestoreUserData
{
    private readonly FirestoreDb _firestoreDb;
    public FirestoreUserData()
    {
        _firestoreDb = FirestoreDb.Create("alexaskill-97042");
    }

    public async Task<UserStationData> GetData(string id)
    {
        var collectionReference = _firestoreDb.Collection("UserStationData");
        var snapshot = await collectionReference.GetSnapshotAsync();

        var documentSnapshots = snapshot.First(x =>
                x.ConvertTo<UserStationData>().AmazonUserId == id)
            .ConvertTo<UserStationData>();

        return documentSnapshots;
    }
}