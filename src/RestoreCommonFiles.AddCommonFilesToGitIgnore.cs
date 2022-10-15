using Microsoft.Build.Construction;

namespace JustinWritesCode.Common;

public partial class RestoreCommonFiles
{
    const string GitIgnoreComment = "### Common Files to Ignore (because they're restored when the RestoreCommonFiles.restoreproj is built) ###";

    protected FileInfo GitIgnoreFile => new FileInfo(Path.Combine(ProjectDirectory, ".gitignore"));
    public void AddCommonItemsToGitignore(RestoredFiles restoredFiles)
    {
        if (GitIgnoreFile.Exists)
        {
            var newGitIgnoreLines = restoredFiles.Files.Where(f => f.Status == RestoredFileStatus.RestoredNew).Select(f => f.Destination.FullName).Where(line => !line.Contains(".github"));
            var gitIgnore = File.ReadAllLines(GitIgnoreFile.FullName).ToList();
            var gitIgnoreLine = gitIgnore.IndexOf(GitIgnoreComment);
            if (gitIgnoreLine >= 0)
            {
                gitIgnore.InsertRange(gitIgnoreLine + 1, newGitIgnoreLines);
            }
            else
            {
                gitIgnore.Add(GitIgnoreComment);
                gitIgnore.AddRange(newGitIgnoreLines);
            }
            File.WriteAllLines(GitIgnoreFile.FullName, gitIgnore);
        }
    }
}
