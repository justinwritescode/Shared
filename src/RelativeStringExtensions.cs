#pragma warning disable
namespace JustinWritesCode.Common;

using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Execution;
using System.Linq;
using Microsoft.Build.Utilities;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Construction;

public static class RelativeStringExtensions
{
    public static string GetRelativePathTo(this string fromPath, string toPath)
        => toPath.GetRelativePathFrom(fromPath);

    public static string GetRelativePathFrom(this string path, string relativeTo)
    {
        if (path.StartsWith(relativeTo))
        {
            return path.Substring(relativeTo.Length).TrimStart('\\');
        }
        else
        {
            var pathUri = new Uri($"file:///{path}");
            // Folders must end in a slash
            if (!relativeTo.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                relativeTo += Path.DirectorySeparatorChar;
            }
            try
            {
                var folderUri = new Uri($"file:///{relativeTo}");
                var relativeUri = Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
                var relativePath = new Uri(relativeUri).LocalPath;
                if (relativePath.StartsWith(Path.DirectorySeparatorChar + ""))
                {
                    relativePath = relativePath.Substring(1);
                }
                //         Console.WriteLine(
                // @$"path: {path}, 
                // relativeTo: {relativeTo} 
                // is {relativePath}");
                return relativePath;
            }
            catch (UriFormatException urifex)
            {
                Console.WriteLine($"Couldn't find a relative path for {path} relative to {relativeTo}");
                return null;
            }
        }
    }
}
