#pragma warning disable
namespace JustinWritesCode.Common;

using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Execution;
using System.Linq;
using Microsoft.Build.Construction;

public partial class CleanCommonFiles : MSBTask
{
    [Required]
    public string? ProjectPath { get; set; }
    public string ProjectDirectory => ProjectDirectoryInfo.FullName;
    public DirectoryInfo ProjectDirectoryInfo => Directory.GetParent(ProjectPath);
    [Output]
    public ITaskItem[] CleansedFiles { get; set; }
    [Output]
    public ITaskItem[] CleansedDirectories { get; set; }
    public ProjectRootElement Project => ProjectRootElement.Open(ProjectPath);

    private string[] DeleteTheseDirectoriesAlways = new string[] { "bin", "obj", "artifacts" };
    private DirectoryInfo[] DeleteTheseDirectoryInfosAlways => DeleteTheseDirectoriesAlways.Select(d => new DirectoryInfo(Path.Combine(ProjectDirectory, d))).ToArray();

    public override bool Execute()
    {
        var restoredFilesRecords =
            File.Exists(Constants.RestoredFilesRecordsFileName(ProjectDirectory)) ?
            JsonSerializer.Deserialize<RestoredFiles>(File.ReadAllText(Constants.RestoredFilesRecordsFileName(ProjectDirectory)), Constants.JsonSerializerOptions) :
        new RestoredFiles { ProjectDirectoryInfo = ProjectDirectoryInfo };

        if (restoredFilesRecords.Files.Count == 0)
        {
            Log.LogMessage("Nothing to clean.");
        }

        var cleansedFiles = new List<CleansedFile>();
        var cleansedDirectories = new List<CleansedDirectory>();

        foreach (var file in restoredFilesRecords.Files)
        {
            if (file.Destination.Exists)
            {
                file.Destination.Delete();
                cleansedFiles.Add(new CleansedFile(ProjectDirectoryInfo, file.Destination));
                Log.LogTelemetry("CleanedFile", new Dictionary<string, string> { { "File", file.Destination.FullName } });
                Log.LogMessage($"Cleaned file '{file.Destination.FullName}'.");
            }
            else
            {
                cleansedFiles.Add(new CleansedFile(ProjectDirectoryInfo, file.Destination, CleansedFileStatus.SkippedDidNotExist));
                Log.LogTelemetry("SkippedCleaningFile", new Dictionary<string, string> { { "File", file.Destination.FullName } });
                Log.LogMessage($"Skipped cleaning file '{file.Destination.FullName}' because it did not exist.");
            }
        }

        foreach (var directory in restoredFilesRecords.Directories)
        {
            if (directory.Destination.Exists && !(new DirectoryInfo(directory.Destination.FullName).EnumerateFileSystemInfos().Any()))
            {
                directory.Destination.Delete();
                cleansedDirectories.Add(new CleansedDirectory(ProjectDirectoryInfo, directory.Destination));
                Log.LogTelemetry("CleanedDirectory", new Dictionary<string, string> { { "Directory", directory.Destination.FullName } });
                Log.LogMessage($"Cleaned directory '{directory.Destination.FullName}'.");
            }
            else if (!directory.Destination.Exists)
            {
                cleansedDirectories.Add(new CleansedDirectory(ProjectDirectoryInfo, directory.Destination, CleansedFileStatus.SkippedDidNotExist));
                Log.LogTelemetry("SkippedCleaningDirectory_Exists", new Dictionary<string, string> { { "Directory", directory.Destination.FullName } });
                Log.LogMessage($"Skipped cleaning directory '{directory.Destination.FullName}' because it did not exist.");
            }
            else if (new DirectoryInfo(directory.Destination.FullName).GetFileSystemInfos().Length > 0)
            {
                cleansedDirectories.Add(new CleansedDirectory(ProjectDirectoryInfo, directory.Destination, CleansedFileStatus.SkippedNotEmpty));
                Log.LogTelemetry("SkippedCleaningDirectory_NotEmpty", new Dictionary<string, string> { { "Directory", directory.Destination.FullName } });
                Log.LogMessage($"Skipped cleaning directory '{directory.Destination.FullName}' because it was not empty.");
            }
        }

        foreach (var deleteAlways in DeleteTheseDirectoryInfosAlways)
        {
            if (deleteAlways.Exists)
            {
                deleteAlways.Delete(true);
                cleansedDirectories.Add(new CleansedDirectory(ProjectDirectoryInfo, deleteAlways));
                Log.LogTelemetry("CleanedDirectory", new Dictionary<string, string> { { "Directory", deleteAlways.FullName } });
                Log.LogMessage($"Cleaned always-delete directory '{deleteAlways.FullName}'.");
            }
        }

        CleansedFiles = cleansedFiles.Select(f => f.ToProjectItem()).ToArray();
        CleansedDirectories = cleansedDirectories.Select(d => d.ToProjectItem()).ToArray();

        return true;
    }
}
