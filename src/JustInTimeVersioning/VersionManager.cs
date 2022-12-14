/*
 * VersionManager.cs
 *
 *   Created: 2022-11-10-09:07:36
 *   Modified: 2022-11-19-04:05:35
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace JustInTimeVersioning;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;

public class VersionManager : IDisposable
{
	private Mutex _mutex = new Mutex();
	private const int MutexTimeout = 10000;
	public VersionManager(MSBLog log)
	{
		Log = log;
		_mutex.WaitOne(MutexTimeout);
	}

	private MSBLog Log { get; }

    public virtual void SaveVersions()
    {
        var sortedVersions = Versions.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
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

    public DirectoryInfo CurrentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
    public virtual string Configuration { get; set; } = "Local";
    public virtual string VersionsPropsFileName { get; set; } = "Packages/Versions.{0}.props";
    public virtual string VersionsJsonFileName { get; set; } = "Packages/Versions.{0}.json";
    public virtual string VersionsJsonFilePath => string.Format(Path.Combine(GetDirectoryNameOfFileAbove(CurrentDirectory.FullName, string.Format(VersionsJsonFileName, Configuration)), VersionsJsonFileName), Configuration);
    public virtual string VersionsPropsFilePath => string.Format(Path.Combine(GetDirectoryNameOfFileAbove(CurrentDirectory.FullName, string.Format(VersionsPropsFileName, Configuration)), VersionsPropsFileName), Configuration);

    public virtual IDictionary<string, string> Versions => _versions ??= InitializeVersionsDictionary();

	private IDictionary<string, string>? _versions = null;
	private IDictionary<string, string> InitializeVersionsDictionary()
	{
		if (!File.Exists(VersionsJsonFilePath))
		{
			File.WriteAllText(VersionsJsonFilePath, "{}");
		}
		if (!File.Exists(VersionsPropsFilePath))
		{
			File.WriteAllText(VersionsPropsFilePath, "<Project />");
		}
		return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(
			System.IO.File.ReadAllText(VersionsJsonFilePath))!;
	}

    public virtual string GetPathOfFileAbove(string fileName)
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

    public virtual string GetDirectoryNameOfFileAbove(string startingDirectory, string fileName)
    {
        var currentDirectory = startingDirectory;
        var directoryInfo = new DirectoryInfo(currentDirectory);
        var lookingForFile = new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
        var lookingForDirectory = new DirectoryInfo(Path.Combine(directoryInfo.FullName, fileName));
        while (directoryInfo is not null && !lookingForFile.Exists && !lookingForDirectory.Exists)
        {
			Log.LogMessage($"Looking for {fileName} in {directoryInfo}...");
            directoryInfo = directoryInfo.Parent;
            lookingForFile = new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
            lookingForDirectory = new DirectoryInfo(Path.Combine(directoryInfo.FullName, fileName));
        }

        return directoryInfo.FullName;
    }

    public virtual string GetPathOfRootDirectory(DirectoryInfo currentDirectory, string lookingForDirectoryName, DirectoryInfo? lastDirectoryWhereTheThingWasFound = default)
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
	private bool disposedValue;

	public virtual string? GetVersion(string packageName)
        => Versions.ContainsKey(packageName) ? Versions[packageName] : null;

    public virtual void SaveVersion(string packageName, string version)
    {
        lock (Lock)
        {
            Versions[packageName] = version;
            SaveVersions();
        }
    }

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_mutex.ReleaseMutex();
			}

			_versions = null;
			disposedValue = true;
		}
	}

	// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	// ~VersionManager()
	// {
	//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
