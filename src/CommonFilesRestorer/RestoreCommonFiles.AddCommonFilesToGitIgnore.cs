using Microsoft.Build.Construction;

namespace JustinWritesCode.Common;

public partial class RestoreCommonFiles
{
    const string GitIgnoreStartComment = "### ⬇️ Start Common Files to Ignore (because they're restored when the RestoreCommonFiles.restoreproj is built) ⬇️ ###";
    const string GitIgnoreEndComment = "### ⬆️ End Common Files to Ignore (because they're restored when the RestoreCommonFiles.restoreproj is built) ⬆️ ###";

    protected FileInfo GitIgnoreFile => new FileInfo(Path.Combine(ProjectDirectory, ".gitignore"));
    public void AddCommonItemsToGitignore(RestoredFiles restoredFiles)
    {
        // if (GitIgnoreFile.Exists)
        // {
        //     var newGitIgnoreLines = restoredFiles.Files/*.Where(f => f.Status == RestoredFileStatus.RestoredNew)*/.Select(f => f.Destination.FullName).Where(line => !line.Contains(".github")).ToList();
        //     newGitIgnoreLines.Insert(0, GitIgnoreStartComment);
        //     newGitIgnoreLines.Add(GitIgnoreEndComment);

        //     var gitIgnoreLines = File.ReadAllLines(GitIgnoreFile.FullName).ToList();
        //     var gitIgnoreStartCommentLine = gitIgnoreLines.IndexOf(GitIgnoreStartComment);
        //     var gitIgnoreEndCommentLine = gitIgnoreLines.IndexOf(GitIgnoreEndComment);
        //     if (gitIgnoreStartCommentLine >= 0 && gitIgnoreEndCommentLine >= 0)
        //     {
        //         gitIgnoreLines.RemoveRange(gitIgnoreStartCommentLine, gitIgnoreEndCommentLine - gitIgnoreStartCommentLine + 1);
        //         gitIgnoreLines.InsertRange(gitIgnoreStartCommentLine, newGitIgnoreLines);
        //     }
        //     else
        //     {
        //         gitIgnoreLines.AddRange(newGitIgnoreLines);
        //     }
        //     File.WriteAllLines(GitIgnoreFile.FullName, gitIgnoreLines);
        // }
    }
}
