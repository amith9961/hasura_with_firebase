using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Api;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string pathToCredentials = "D:\\CamundaSetup\\LastTry\\FirebaseIntegration\\firebase_admin.json";

// Initialize Firebase Admin SDK
FirestoreDb db = FirestoreDb.Create("fir-firebase-89304", new FirestoreClientBuilder
{
    CredentialsPath = pathToCredentials
}.Build());
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(pathToCredentials),
});
builder.Services.AddSingleton(FirebaseAuth.DefaultInstance);
string documentId = "test_document";
string collectionName = "test_collection";

try
{
    DocumentReference docRef = db.Collection(collectionName).Document(documentId);
    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

    if (snapshot.Exists)
    {
        Console.WriteLine("Document data:");
        Console.WriteLine(snapshot.ToDictionary());
    }
    else
    {
        Console.WriteLine("Document not found.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("Error reading from Firestore: " + ex.Message);
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
