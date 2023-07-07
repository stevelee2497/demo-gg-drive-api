using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;

Console.WriteLine("Hello, World!");

string PathToServiceAccountKeyFile = "C:\\Tempt\\turn-it-blue.json";
string UploadFileName = "C:\\Tempt\\pexels-nout-gons-378570.jpg";
string DirectoryId = "1p2qXn-Bl1a2E0n6XVp2rCAryQsJVC79v";

var credential = GoogleCredential.FromFile(PathToServiceAccountKeyFile).CreateScoped(DriveService.ScopeConstants.Drive);
var service = new DriveService(new BaseClientService.Initializer()
{
    HttpClientInitializer = credential,
    ApplicationName = "Turn it blue"
});

// Create folder 
var folderMetadata = new Google.Apis.Drive.v3.Data.File()
{
    Name = "user a",
    MimeType = "application/vnd.google-apps.folder",
    Parents = new List<string>() { DirectoryId }
};
var request = service.Files.Create(folderMetadata);
request.Fields = "id";
var folder = request.Execute();
Console.WriteLine("Folder ID: " + folder.Id);


// Upload file
var fileMetadata = new Google.Apis.Drive.v3.Data.File()
{
    Name = "pexels-nout-gons.jpg",
    Parents = new List<string>() { folder.Id }
};
await using var fsSource = new FileStream(UploadFileName, FileMode.Open, FileAccess.Read);
var request2 = service.Files.Create(fileMetadata, fsSource, "image/jpeg");
request2.Fields = "*";
var results = await request2.UploadAsync(CancellationToken.None);
if (results.Status == UploadStatus.Failed)
{
    Console.WriteLine($"Error uploading file: {results.Exception.Message}");
}

// the file id of the new file we created
var uploadedFileId = request2.ResponseBody?.Id;
Console.WriteLine("File ID: " + uploadedFileId);


// Define parameters of request.
FilesResource.ListRequest listRequest = service.Files.List();
listRequest.PageSize = 10;
listRequest.Fields = "nextPageToken, files(id, name)";
listRequest.Q = $"'{DirectoryId}' in parents";

// List files.
IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
Console.WriteLine("Files:");
if (files == null || files.Count == 0)
{
    Console.WriteLine("No files found.");
    return;
}
foreach (var file in files)
{
    Console.WriteLine("{0} ({1})", file.Name, file.Id);
}