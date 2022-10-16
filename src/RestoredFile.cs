#pragma warning disable
namespace JustinWritesCode.Common;

using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Execution;
using System.Linq;
using Microsoft.Build.Utilities;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Construction;
using System.Text.Json.Serialization;
using global::JustinWritesCode.IO.Extensions;

public class RestoredFile
{
    [JsonConstructor]
    public RestoredFile() : this(null, null, null, null) { }

    public RestoredFile(DirectoryInfo ProjectDirectoryInfo, DirectoryInfo SourceRoot, FileSystemInfo Source, FileSystemInfo Destination)
    {
        this.ProjectDirectoryInfo = ProjectDirectoryInfo;
        this.SourceRoot = SourceRoot;
        this.Source = Source;
        this.Destination = Destination;
        this.Timestamp = System.DateTime.UtcNow;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RestoredFileStatus Status { get; set; } = RestoredFileStatus.None;

    [JsonConverter(typeof(DirectoryInfoJsonConverter))]
    public DirectoryInfo ProjectDirectoryInfo { get; init; }
    [JsonConverter(typeof(DirectoryInfoJsonConverter))]
    public DirectoryInfo SourceRoot { get; init; }
    [JsonConverter(typeof(FileSystemInfoJsonConverter<FileSystemInfo>))]
    public FileSystemInfo Source { get; init; }
    [JsonConverter(typeof(FileSystemInfoJsonConverter<FileSystemInfo>))]
    public FileSystemInfo Destination { get; init; }
    // public string ProjectDirectory { get; init; } = ProjectDirectoryInfo.FullName;
    // public string SourceRootDirectory { get; init; } = SourceRoot.FullName;
    // public string SourceFileRelativePath { get; init; } = SourceRoot.FullName.GetRelativePathTo(Source.FullName);
    // public string DestinationFileRelativePath { get; init; } = ProjectDirectoryInfo.FullName.GetRelativePathTo(Destination.FullName);
    private string _fileHash = null;
    public string FileHash
    {
        get => _fileHash ?? GetFileHash(Destination);
        set => _fileHash = value;
    }

    public DateTime Timestamp { get; init; } = System.DateTime.UtcNow;

    // public RestoredFile() : this(new DirectoryInfo(Directory.GetCurrentDirectory()), new DirectoryInfo(Directory.GetCurrentDirectory()), new FileInfo(Directory.GetCurrentDirectory())) { }
    // public RestoredFile(string ProjectDirectory, string SourceFile, string DestinationFile, string FileHash, DateTime Timestamp) :
    //     this(new DirectoryInfo(ProjectDirectory), new FileInfo(SourceFile), new FileInfo(DestinationFile))
    // {
    //     this.FileHash = FileHash;
    //     this.Timestamp = Timestamp;
    // }
    protected virtual string GetFileHash(FileSystemInfo file)
        => file.Exists ? new FileInfo(file.FullName).GetFileHash() : null;

    public virtual ITaskItem ToProjectItem(ProjectItemGroupElement itemGroup)
    {
        var item = new TaskItem(this.Destination.FullName);
        item.SetMetadata("SourceFile", this.Source?.FullName);
        item.SetMetadata("SourceRoot", this.SourceRoot?.FullName);
        item.SetMetadata("ProjectDirectory", this.ProjectDirectoryInfo.FullName);
        item.SetMetadata("FileHash", this.FileHash);
        item.SetMetadata("Timestamp", this.Timestamp.ToString("o"));
        item.SetMetadata("Status", this.Status.ToString());
        return item;
    }
}

// public class RestoredFile
// {
//     public string ProjectDirectory { get; set; }
//     public string SourceFile { get; set; }
//     public string DestinationFile { get; set; }
//     public string FileHash { get; set; }
//     public DateTime Timestamp { get; set; }

//     public RestoredFile() : this(new DirectoryInfo(Directory.GetCurrentDirectory()), new FileInfo(Directory.GetCurrentDirectory()), new FileInfo(Directory.GetCurrentDirectory())) { }
//     public RestoredFile(string ProjectDirectory, string SourceFile, string DestinationFile, string FileHash, DateTime Timestamp) :
//         this(new DirectoryInfo(ProjectDirectory), new FileInfo(SourceFile), new FileInfo(DestinationFile))
//     {
//         this.FileHash = FileHash;
//         this.Timestamp = Timestamp;
//     }
//     public RestoredFile(DirectoryInfo projectDirectory, FileSystemInfo source = null, FileSystemInfo destination = null) : base()
//     {
//         this.ProjectDirectory = projectDirectory.FullName;
//         this.SourceFile = source.FullName;//.GetRelativePath(projectDirectory.FullName);
//         this.DestinationFile = destination.FullName.GetRelativePathFrom(projectDirectory.FullName);
//         this.Timestamp = System.DateTime.UtcNow;
//         this.FileHash = GetFileHash(destination);
//     }

//     protected virtual string GetFileHash(FileSystemInfo file)
//     {
//         return BitConverter.ToString(Constants.HashAlgorithm.ComputeHash(File.ReadAllBytes(file.FullName))).Replace("-", string.Empty);
//     }

//     public virtual Microsoft.Build.Construction.ProjectItemElement ToProjectItem(ProjectItemGroupElement itemGroup)
//     {
//         var item = itemGroup.AddItem("RestoredFile", this.DestinationFile);
//         item.AddMetadata("SourceFile", this.SourceFile);
//         item.AddMetadata("FileHash", this.FileHash);
//         item.AddMetadata("Timestamp", this.Timestamp.ToString("o"));
//         return item;
//     }
// }
