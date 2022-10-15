#pragma warning disable
#pragma warning disable
namespace JustinWritesCode.Common;

using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Execution;
using System.Linq;
using Microsoft.Build.Construction;
using System.Text.Json.Serialization;
using Microsoft.Build.Utilities;

public class CleansedFile
{
    public CleansedFile(DirectoryInfo projectDirectory, FileSystemInfo destination, CleansedFileStatus status = CleansedFileStatus.Deleted)
    {
        ProjectDirectory = projectDirectory;
        CleansedDestination = destination;
        RelativeCleansedPath = CleansedDestination.FullName.Replace(projectDirectory.FullName, string.Empty).TrimStart('\\');
        Status = status;
    }

    [JsonConverter(typeof(DirectoryInfoJsonConverter))]
    public DirectoryInfo ProjectDirectory { get; init; }
    [JsonConverter(typeof(FileSystemInfoJsonConverter<FileSystemInfo>))]
    public FileSystemInfo CleansedDestination { get; set; }
    public string RelativeCleansedPath { get; set; }
    public CleansedFileStatus Status { get; set; }

    public virtual ITaskItem ToProjectItem()
    {
        var item = new TaskItem(this.CleansedDestination.FullName);
        item.SetMetadata("Status", this.Status.ToString());
        return item;
    }
}
