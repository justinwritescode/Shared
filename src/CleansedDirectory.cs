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


public class CleansedDirectory : CleansedFile
{
    public CleansedDirectory(DirectoryInfo projectDirectory, FileSystemInfo cleanedDirectory, CleansedFileStatus status = CleansedFileStatus.Cleansed)
        : base(projectDirectory, new FileInfo(cleanedDirectory.FullName), status) { }

    public override ITaskItem ToProjectItem()
    {
        var item = new TaskItem(this.CleansedDestination.FullName);
        item.SetMetadata("Status", this.Status.ToString());
        item.SetMetadata("ProjectDirectory", this.ProjectDirectory.FullName);
        return item;
    }
}
