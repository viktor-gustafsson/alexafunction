using Google.Cloud.Firestore;

namespace AlexaFunction.DAL;

public class FireStore
{
    private readonly FirestoreDb _firestoreDb;
    public FireStore()
    {
        _firestoreDb = FirestoreDb.Create("alexaskill-97042");
    }

    public async Task<UserStationData> GetUserStationData(string id)
    {
        var collectionReference = _firestoreDb.Collection("UserStationData");
        var snapshot = await collectionReference.GetSnapshotAsync();

        var documentSnapshots = snapshot.First(x =>
                x.ConvertTo<UserStationData>().AmazonUserId == id)
            .ConvertTo<UserStationData>();

        return documentSnapshots;
    }
    
    public async Task<UserStationData> GetFirst()
    {
        var collectionReference = _firestoreDb.Collection("UserStationData");
        var snapshot = await collectionReference.GetSnapshotAsync();

        var documentSnapshots = snapshot.First().ConvertTo<UserStationData>();

        return documentSnapshots;
    }

    public async Task<DeviationData> GetLastDeviationData()
    {
        var collectionReference = _firestoreDb.Collection("DeviationData");
        var snapshot = await collectionReference.Document("aHAFyd1kbKfHT4Tl14Mo").GetSnapshotAsync();

        return snapshot.ConvertTo<DeviationData>();
    }

    public async Task WriteDeviationData(string deviationData)
    {
        var collectionReference = _firestoreDb.Collection("DeviationData");
        await collectionReference.Document("aHAFyd1kbKfHT4Tl14Mo")
            .SetAsync(new DeviationData
        {
            Deviation = deviationData,
            Time = DateTime.UtcNow
        });
    }
}