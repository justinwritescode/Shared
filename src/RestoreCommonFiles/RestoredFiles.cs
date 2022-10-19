#pragma warning disable
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace JustinWritesCode.Common;

public class RestoredFiles
{
    public static RestoredFiles LoadFrom(string restoredFilesJsonPath)
         => LoadFrom(new FileInfo(restoredFilesJsonPath));
    public static RestoredFiles LoadFrom(FileInfo restoredFilesJsonPath)
    {
        if (restoredFilesJsonPath.Exists)
        {
            var restoredFilesRecords = JsonSerializer.Deserialize<RestoredFiles>(File.ReadAllText(restoredFilesJsonPath.FullName), Constants.JsonSerializerOptions);
            restoredFilesRecords.SourceFileInfo = restoredFilesJsonPath;
            // restoredFilesRecords.SourceFile = restoredFilesJsonPath.FullName;
            restoredFilesRecords.ProjectDirectoryInfo = Directory.GetParent(restoredFilesJsonPath.FullName);
            // restoredFilesRecords.ProjectDirectory = Directory.GetParent(restoredFilesJsonPath.FullName).FullName;
            return restoredFilesRecords;
        }
        throw new FileNotFoundException($"Could not find {restoredFilesJsonPath.FullName}");
    }

    public virtual bool ContainsSource(FileInfo file)
        => Files.Any(f => f.Source.FullName == file.FullName);
    public virtual bool ContainsDestination(FileInfo file)
        => ContainsDestination(file.FullName);
    public virtual bool ContainsDestination(string file)
        => Files.Any(f => f.Destination.FullName == file);

    [JsonIgnore]
    public virtual IDictionary<string, RestoredFile> SourceFiles => Files.ToDictionary(f => f.Source.FullName);
    [JsonIgnore]
    public virtual IDictionary<string, RestoredFile> DestinationFiles => Files.ToDictionary(f => f.Destination.FullName);

    public RestoredFiles()
    {
        this.ProjectDirectoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        this.SourceFileInfo = new FileInfo(Constants.RestoredFilesRecordsFileName(this.ProjectDirectory));
        this.Files = new List<RestoredFile>();
        this.Directories = new List<RestoredDirectory>();
    }
    [JsonConverter(typeof(FileInfoJsonConverter))]
    public FileInfo SourceFileInfo { get; set; }
    public string SourceFile => SourceFileInfo.FullName;
    [JsonConverter(typeof(DirectoryInfoJsonConverter))]
    public DirectoryInfo ProjectDirectoryInfo { get; set; }
    public string ProjectDirectory => ProjectDirectoryInfo.FullName;

    public List<RestoredFile> Files { get; set; } = new List<RestoredFile>();
    public List<RestoredDirectory> Directories { get; set; } = new List<RestoredDirectory>();
}
