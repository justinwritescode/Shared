#pragma warning disable
namespace JustinWritesCode.Common;
using System.Text.Json.Serialization;

public static class Constants
{
    public static string RestoredFilesRecordsJsonFileName(string relativeTo) => "./.restored-files.json".GetRelativePathFrom(relativeTo);
    public static string RestoredFilesRecordsFileName(string relativeTo) => "./.restored-files".GetRelativePathFrom(relativeTo);
    public static readonly System.Security.Cryptography.SHA1 HashAlgorithm = System.Security.Cryptography.SHA1.Create();

    public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        IgnoreNullValues = true,
        IgnoreReadOnlyProperties = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            new DirectoryInfoJsonConverter(),
            new FileInfoJsonConverter(),
            new FileSystemInfoJsonConverter<FileSystemInfo>()
        }
    };
}
