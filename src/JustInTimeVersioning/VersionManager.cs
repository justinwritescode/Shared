// 
// VersionManager.cs
// 
//   Created: 2022-10-24-04:48:49
//   Modified: 2022-10-30-03:09:40
// 
//   Author: Justin Chase <justin@justinwritescode.com>
//   
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 

namespace JustInTimeVersioning;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;

public static class VersionManager
{
    // public VersionManager(string configuration, string versionsJsonFileName = "Packages/JustinWritesCode.Versions.{0}.json", string versionsPropsFileName = "Packages/JustinWritesCode.Versions.{0}.props")
    // {
    //     Configuration = configuration;
    //     VersionsJsonFileName = versionsJsonFileName;
    //     VersionsPropsFileName = versionsPropsFileName;
    //     if (!File.Exists(VersionsJsonFilePath))
    //     {
    //         File.WriteAllText(VersionsJsonFilePath, "{}");
    //     }
    //     if (!File.Exists(VersionsPropsFilePath))
    //     {
    //         File.WriteAllText(VersionsPropsFilePath, "<Project />");
    //     }
    //     Versions = System.Text.Json.JsonSerializer.Deserialize<IDictionary<string, string>>(
    //     System.IO.File.ReadAllText(VersionsJsonFilePath));
    // }

    private static void SaveVersions()
    {
        var sortedVersions = Versions.Value.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        System.IO.File.WriteAllText(VersionsJsonFilePath, System.Text.Json.JsonSerializer.Serialize(sortedVersions));
        var versionsProps = new XDocument(
            new XComment("<auto-generated />"),
            new XComment("This file is automatically generated by JustinWritesCode.Versioning. Do not edit."),
            new XElement("Project", 
                new XElement("PropertyGroup",
                    new XElement("JustInTimeVersioningVersion", sortedVersions["JustInTimeVersioning"])),
                new XElement("ItemGroup",
                    sortedVersions.Select(kvp =>
                        new XElement("PackageReference",
                            new XAttribute("Update", kvp.Key),
                            new XAttribute("Version", $"[{kvp.Value}, )"),
                            new XAttribute("Condition", $"'$(PackageId)' != '{kvp.Key}'"))))));
        versionsProps.Save(VersionsPropsFilePath);
    }

    public static DirectoryInfo CurrentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
    public static string Configuration { get; set; } = "Local";
    public static string VersionsPropsFileName { get; set; } = "Packages/Versions.{0}.props";
    public static string VersionsJsonFileName { get; set; } = "Packages/Versions.{0}.json";
    public static string VersionsJsonFilePath => string.Format(Path.Combine(GetDirectoryNameOfFileAbove(CurrentDirectory.FullName, string.Format(VersionsJsonFileName, Configuration)), VersionsJsonFileName), Configuration);
    public static string VersionsPropsFilePath => string.Format(Path.Combine(GetDirectoryNameOfFileAbove(CurrentDirectory.FullName, string.Format(VersionsPropsFileName, Configuration)), VersionsPropsFileName), Configuration);

    public static Lazy<IDictionary<string, string>> Versions { get; } = new Lazy<IDictionary<string, string>>(() =>
    {
        if (!File.Exists(VersionsJsonFilePath))
        {
            File.WriteAllText(VersionsJsonFilePath, "{}");
        }
        if (!File.Exists(VersionsPropsFilePath))
        {
            File.WriteAllText(VersionsPropsFilePath, "<Project />");
        }
        return System.Text.Json.JsonSerializer.Deserialize<IDictionary<string, string>>(
            System.IO.File.ReadAllText(VersionsJsonFilePath));
    });

    public static string GetPathOfFileAbove(string fileName)
    {
        Console.WriteLine($"Looking for {fileName} in {Directory.GetCurrentDirectory()}");
        var currentDirectory = Directory.GetCurrentDirectory();
        var directoryInfo = new DirectoryInfo(currentDirectory);
        var lookingForFile = new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
        var lookingForDirectory = new DirectoryInfo(Path.Combine(directoryInfo.FullName, fileName));
        while (directoryInfo != null && !lookingForFile.Exists && !lookingForDirectory.Exists)
        {
            directoryInfo = directoryInfo.Parent;
            lookingForFile = new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
            lookingForDirectory = new DirectoryInfo(Path.Combine(directoryInfo.FullName, fileName));
        }

        return Path.Combine(directoryInfo.FullName, fileName);
    }

    public static string GetDirectoryNameOfFileAbove(string startingDirectory, string fileName)
    {
        var currentDirectory = startingDirectory;
        var directoryInfo = new DirectoryInfo(currentDirectory);
        var lookingForFile = new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
        var lookingForDirectory = new DirectoryInfo(Path.Combine(directoryInfo.FullName, fileName));
        while (directoryInfo is not null && !lookingForFile.Exists && !lookingForDirectory.Exists)
        {
            Console.WriteLine($"Looking for {fileName} in {directoryInfo}...");
            directoryInfo = directoryInfo.Parent;
            lookingForFile = new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
            lookingForDirectory = new DirectoryInfo(Path.Combine(directoryInfo.FullName, fileName));
        }

        return directoryInfo.FullName;
    }

    public static string GetPathOfRootDirectory(DirectoryInfo currentDirectory, string lookingForDirectoryName, DirectoryInfo? lastDirectoryWhereTheThingWasFound = default)
    {
        Console.WriteLine($"Looking for {lookingForDirectoryName} in {currentDirectory.FullName}");
        if (currentDirectory == null)
        {
            return lastDirectoryWhereTheThingWasFound.FullName;
        }
        else if (currentDirectory.Name == lookingForDirectoryName)
        {
            return currentDirectory.FullName;
        }
        else if (currentDirectory.GetFileSystemInfos(lookingForDirectoryName).Any())
        {
            lastDirectoryWhereTheThingWasFound = new DirectoryInfo(currentDirectory.GetFileSystemInfos(lookingForDirectoryName).First().FullName);
        }
        return GetPathOfRootDirectory(currentDirectory.Parent, lookingForDirectoryName, currentDirectory);
    }

    private static object Lock = "Lock";

    public static string GetVersion(string packageName)
        => Versions.Value[packageName];

    public static void SaveVersion(string packageName, string version)
    {
        lock (Lock)
        {
            Versions.Value[packageName] = version;
            SaveVersions();
        }
    }
}
