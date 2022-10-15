using System.Security.Cryptography;

namespace JustinWritesCode.IO.Extensions;

public static class FileSystemInfoExtensions
{
    private static readonly SHA1 HashAlgorithm = SHA1.Create();

    #region file hashes
    public static string GetFileHash(this FileInfo file)
    {
        using var stream = file.OpenRead();
        var hash = HashAlgorithm.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public static string GetDirectoryHash(this DirectoryInfo directory)
    {
        var files = directory.GetFiles("*", SearchOption.AllDirectories).OrderBy(f => f.FullName);
        var hashes = files.Select(file => file.GetFileHash());
        var hash = string.Join("-", hashes);
        return hash;
    }
    #endregion

    #region file attributes
    public static bool IsDirectory(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.Directory);
    }

    public static bool IsFile(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.Normal);
    }

    public static bool IsSymbolicLink(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
    }

    public static bool IsHidden(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.Hidden);
    }

    public static bool IsReadOnly(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.ReadOnly);
    }

    public static bool IsSystem(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.System);
    }

    public static bool IsArchive(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.Archive);
    }

    public static bool IsTemporary(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.Temporary);
    }

    public static bool IsCompressed(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.Compressed);
    }

    public static bool IsEncrypted(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.Encrypted);
    }

    public static bool IsSparseFile(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.SparseFile);
    }

    public static bool IsReparsePoint(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
    }

    public static bool IsIntegrityStream(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.IntegrityStream);
    }

    public static bool IsNoScrubData(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.NoScrubData);
    }

    public static bool IsOffline(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.Offline);
    }

    public static bool IsNotContentIndexed(this FileSystemInfo fileSystemInfo)
    {
        return fileSystemInfo.Attributes.HasFlag(FileAttributes.NotContentIndexed);
    }
    #endregion
}
