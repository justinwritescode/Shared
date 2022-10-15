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
using JustinWritesCode.IO.Extensions;

public class RestoredDirectory : RestoredFile
{
    [JsonConstructor]
    public RestoredDirectory() : this(null, null, null) { }

    public RestoredDirectory(DirectoryInfo ProjectDirectoryInfo = null, DirectoryInfo SourceRoot = null, DirectoryInfo Source = null, DirectoryInfo Destination = null)
        : base(ProjectDirectoryInfo, SourceRoot, Source, Destination)
    {

    }

    // [JsonConverter(typeof(DirectoryInfoJsonConverter))]
    // public DirectoryInfo Source => new DirectoryInfo(base.Source.FullName);
    // [JsonConverter(typeof(DirectoryInfoJsonConverter))]
    // public DirectoryInfo Destination => new DirectoryInfo(base.Destination.FullName);
    // public string ProjectDirectory { get; init; } = ProjectDirectoryInfo.FullName;
    // public string SourceRootDirectory { get; init; } = SourceRoot.FullName;
    // public string SourceDirectory { get; init; } = Source.FullName;
    // public string DestinationDirectory { get; init; } = Destination.FullName;
    public DateTime Timestamp { get; init; } = System.DateTime.UtcNow;

    // public RestoredDirectory() : this(new DirectoryInfo(Directory.GetCurrentDirectory()), new DirectoryInfo(Directory.GetCurrentDirectory()), new DirectoryInfo(Directory.GetCurrentDirectory())) { }
    // public RestoredDirectory(string ProjectDirectory, string SourceDirectory, string DestinationDirectory, string DirectoryHash, DateTime Timestamp) :
    //     this(new DirectoryInfo(ProjectDirectory), new DirectoryInfo(SourceDirectory), new DirectoryInfo(DestinationDirectory))
    // {
    //     this.DirectoryHash = DirectoryHash;
    //     this.Timestamp = Timestamp;
    // }

    protected override string GetFileHash(FileSystemInfo directory)
        => directory.Exists ? new DirectoryInfo(directory.FullName).GetDirectoryHash() : null;

    public override ITaskItem ToProjectItem(ProjectItemGroupElement itemGroup)
    {
        var item = new TaskItem(this.Destination.FullName);
        item.SetMetadata("SourceDirectory", this.Source.FullName);
        item.SetMetadata("SourceRoot", this.SourceRoot.FullName);
        item.SetMetadata("ProjectDirectory", this.ProjectDirectoryInfo.FullName);
        item.SetMetadata("DirectoryHash", this.FileHash);
        item.SetMetadata("Timestamp", this.Timestamp.ToString("o"));
        item.SetMetadata("Status", this.Status.ToString());
        return item;
    }
}
//     }
//     {
//         public RestoredDirectory() : base(new DirectoryInfo(Directory.GetCurrentDirectory()), new DirectoryInfo(Directory.GetCurrentDirectory()), new DirectoryInfo(Directory.GetCurrentDirectory())) { }
//         public RestoredDirectory(string ProjectDirectory, string SourceDirectory, string DestinationDirectory, string FileHash, DateTime Timestamp) :
//             base(new DirectoryInfo(ProjectDirectory), new FileInfo(SourceDirectory), new FileInfo(DestinationDirectory))
//         {

//         }
//         public RestoredDirectory(DirectoryInfo projectDirectory, DirectoryInfo directory) : base(projectDirectory, directory, directory)
//         {
//         }

//         protected override string GetFileHash(FileSystemInfo file)
//         {
//             return Directory.EnumerateFiles(file.FullName, "*", SearchOption.AllDirectories).OrderBy(f => f).Aggregate("", (acc, f) => acc + base.GetFileHash(new FileInfo(f)));
//         }

//         public virtual Microsoft.Build.Construction.ProjectItemElement ToProjectItem(ProjectItemGroupElement itemGroup)
//         {
//             var item = itemGroup.AddItem("RestoredDirectory", this.DestinationFile);
//             item.AddMetadata("SourceFile", this.SourceFile);
//             item.AddMetadata("FileHash", this.FileHash);
//             item.AddMetadata("Timestamp", this.Timestamp.ToString("o"));
//             return item;
//         }
//     }
// }
