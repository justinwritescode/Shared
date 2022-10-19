using System.Reflection.Emit;
using System.IO;
using System.Resources;

#pragma warning disable
namespace JustinWritesCode.Common;

using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Execution;
using System.Linq;
using Microsoft.Build.Utilities;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Construction;
using global::JustinWritesCode.IO.Extensions;

public partial class RestoreCommonFiles : MSBTask
{
    [Required]
    public ITaskItem[] Include { get; set; }
    public string Exclude { get; set; } = string.Empty;
    [Required]
    public string IncludeRootPath { get; set; } = string.Empty;
    public string? ProjectDirectory => Directory.GetParent(ProjectPath).FullName;
    [Required]
    public string? ProjectPath { get; set; }
    public DirectoryInfo ProjectDirectoryInfo => new DirectoryInfo(ProjectDirectory);
    [Output]
    public ITaskItem[] RestoredFiles { get; set; }
    [Output]
    public ITaskItem[] RestoredDirectories { get; set; }
    public ProjectRootElement Project => ProjectRootElement.Open(ProjectPath);

    public override bool Execute()
    {
        var restoredFilesRecords =
            File.Exists(Constants.RestoredFilesRecordsFileName(ProjectDirectory)) ?
            Common.RestoredFiles.LoadFrom(Constants.RestoredFilesRecordsFileName(ProjectDirectory)) :
            new RestoredFiles { ProjectDirectoryInfo = ProjectDirectoryInfo };

        var commonFiles = string.Join(", ", Include.Select(i => new FileInfo(i.ItemSpec)).ToList());
        Log.LogMessage($"Restoring common files: {commonFiles} to {ProjectDirectory}...");

        if (IncludeRootPath is null || ProjectDirectory is null)
        {
            Log.LogError("Include and OutputDirectory must be set.");
            return false;
        }

        var _include = new DirectoryInfo(IncludeRootPath);
        var projectDirectory = new DirectoryInfo(ProjectDirectory);

        if (!_include.Exists)
        {
            Log.LogError($"Include '{IncludeRootPath}' does not exist.");
            return false;
        }

        if (!projectDirectory.Exists)
        {
            Log.LogError($"OutputDirectory '{ProjectDirectory}' does not exist.");
            return false;
        }

        var excludedFiles = !string.IsNullOrEmpty(Exclude) ? _include.GetFiles(Exclude, SearchOption.AllDirectories) : new FileInfo[0];
        var files = Include.Select(i => new FileInfo(i.ItemSpec)).Except(excludedFiles).ToArray();
        foreach (var file in files)
        {
            var relativePath = file.FullName.Substring(_include.FullName.Length);
            var destination = Path.Combine(projectDirectory.FullName, relativePath);
            var destinationDirectory = Path.GetDirectoryName(destination);
            if (destinationDirectory is null)
            {
                Log.LogError($"Could not get directory name from '{destination}'.");
                return false;
            }

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
                restoredFilesRecords.Directories.Add(new RestoredDirectory(ProjectDirectoryInfo, _include, new DirectoryInfo(file.DirectoryName), new DirectoryInfo(destinationDirectory)));
                Log.LogTelemetry("RestoredDirectory", new Dictionary<string, string> { { "Destination", destinationDirectory } });
            }

            if (File.Exists(destination))
            {
                if (!restoredFilesRecords.ContainsDestination(destination))
                {
                    Log.LogMessage($"File '{destination}' already exists and is being overwritten with newer version");
                    Log.LogTelemetry("OverwrittenFile", new Dictionary<string, string> { { "Destination", destination }, { "Source", file.FullName } });
                    File.Copy(file.FullName, destination, true);
                    restoredFilesRecords.Files.Add(new RestoredFile(ProjectDirectoryInfo, _include, file, new FileInfo(destination)));
                }
                else
                {
                    Log.LogMessage($"File '{destination}' already exists and is being overwritten with newer version");
                    Log.LogTelemetry("OverwrittenFile", new Dictionary<string, string> { { "Destination", destination }, { "Source", file.FullName } });
                    File.Copy(file.FullName, destination, true);
                    restoredFilesRecords.DestinationFiles[destination].Status = RestoredFileStatus.Overwritten;
                }
            }
            else if (!File.Exists(destination))
            {
                File.Copy(file.FullName, destination, true);
                if (!restoredFilesRecords.DestinationFiles.ContainsKey(destination))
                {
                    restoredFilesRecords.Files.Add(new RestoredFile(ProjectDirectoryInfo, _include, file, new FileInfo(destination)));
                    restoredFilesRecords.DestinationFiles[destination].Status = RestoredFileStatus.RestoredNew;
                }
                else if (restoredFilesRecords.DestinationFiles.ContainsKey(destination))
                {
                    restoredFilesRecords.DestinationFiles[destination].Status = RestoredFileStatus.RestoredMissing;
                }
                Log.LogMessage("Restored file '{0}' to '{1}'", file.FullName, destination);
                Log.LogTelemetry("RestoredFile", new Dictionary<string, string> { { "Destination", destination }, { "Source", file.FullName } });
            }
            else if (restoredFilesRecords.DestinationFiles.ContainsKey(destination))
            {
                restoredFilesRecords.DestinationFiles[new FileInfo(destination).FullName].Status = RestoredFileStatus.SkippedAlreadyExists;
                Log.LogMessage($"File '{destination}' already exists. Skipping.");
                Log.LogTelemetry("SkippedFile", new Dictionary<string, string> { { "Destination", destination }, { "Source", file.FullName } });
            }
        }

        var restoredFilesFileInfo = new FileInfo(Constants.RestoredFilesRecordsFileName(ProjectDirectory));
        if (!restoredFilesRecords.ContainsDestination(restoredFilesFileInfo))
        {
            restoredFilesRecords.Files.Add(new RestoredFile(ProjectDirectoryInfo, _include, restoredFilesFileInfo, restoredFilesFileInfo));
            restoredFilesRecords.DestinationFiles[restoredFilesFileInfo.FullName].Status = RestoredFileStatus.RestoredNew;
        }
        else
        {
            restoredFilesRecords.DestinationFiles[restoredFilesFileInfo.FullName].Status = RestoredFileStatus.Overwritten;
        }

        //RestoreDirectoryBuild(restoredFilesRecords);
        File.WriteAllText(Constants.RestoredFilesRecordsFileName(ProjectDirectory), JsonSerializer.Serialize(restoredFilesRecords, Constants.JsonSerializerOptions));

        AddCommonItemsToGitignore(restoredFilesRecords);

        var restoredFilesItemGroup = Project.AddItemGroup();
        restoredFilesItemGroup.Label = "Restored Files";
        RestoredFiles = restoredFilesRecords.Files.Select(rf => rf.ToProjectItem(restoredFilesItemGroup)).ToArray();
        var restoredDirectoriesItemGroup = Project.AddItemGroup();
        restoredDirectoriesItemGroup.Label = "Restored Directories";
        RestoredDirectories = restoredFilesRecords.Directories.Select(rd => rd.ToProjectItem(restoredDirectoriesItemGroup)).ToArray();

        return true;
    }
}
