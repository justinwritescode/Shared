namespace JustinWritesCode.Common;
using System.Text.Json.Serialization;

public class FileSystemInfoJsonConverter<T> : JsonConverter<T> where T : FileSystemInfo?
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableTo(typeof(FileSystemInfo)) || typeToConvert.IsAssignableTo(typeof(DirectoryInfo)) || typeToConvert.IsAssignableTo(typeof(FileInfo));
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var json = reader.GetString();
        if (json is null)
        {
            return null;
        }
        if (json.StartsWith("file://"))
        {
            return new FileInfo(json.Replace("file://", "")) as T;
        }
        else if (json.StartsWith("dir://"))
        {
            return new DirectoryInfo(json.Replace("dir://", "")) as T;
        }
        else
        {
            throw new JsonException($"Could not convert {json} to a FileSystemInfo");
        }
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else if (value is FileInfo)
        {
            writer.WriteStringValue($"file://{value.FullName}");
        }
        else if (value is DirectoryInfo)
        {
            writer.WriteStringValue($"dir://{value.FullName}");
        }
        else
        {
            throw new JsonException($"Could not convert {value} to a FileSystemInfo");
        }
    }
}

public class DirectoryInfoJsonConverter : FileSystemInfoJsonConverter<DirectoryInfo?>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableTo(typeof(DirectoryInfo));
    }

    public override DirectoryInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return base.Read(ref reader, typeToConvert, options) as DirectoryInfo;
    }

    public override void Write(Utf8JsonWriter writer, DirectoryInfo? value, JsonSerializerOptions options)
    {
        base.Write(writer, value, options);
    }
}
public class FileInfoJsonConverter : FileSystemInfoJsonConverter<FileInfo?>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableTo(typeof(FileInfo));
    }

    public override FileInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return base.Read(ref reader, typeToConvert, options) as FileInfo;
    }

    public override void Write(Utf8JsonWriter writer, FileInfo? value, JsonSerializerOptions options)
    {
        base.Write(writer, value, options);
    }
}

// public class FileInfoListJsonConverter<TItem> : JsonConverter<List<TItem>> where TItem : RestoredFile
// {
//     public override bool CanConvert(Type typeToConvert)
//     {
//         return typeToConvert.IsAssignableTo(typeof(List<FileSystemInfo>));
//     }

//     public override List<TItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//     {
//         var json = reader.GetString();
//         if (json is null)
//         {
//             return new List<TItem>();
//         }
//         var fileSystemInfos = new List<TItem>();
//         foreach (var item in json.Split(","))
//         {
//             if (item.StartsWith("file://"))
//             {
//                 fileSystemInfos.Add(new FileInfo(item.Replace("file://", "")));
//             }
//             else if (item.StartsWith("dir://"))
//             {
//                 fileSystemInfos.Add(new DirectoryInfo(item.Replace("dir://", "")));
//             }
//             else
//             {
//                 throw new JsonException($"Could not convert {item} to a FileSystemInfo");
//             }
//         }
//         return fileSystemInfos;
//     }

//     public override void Write(Utf8JsonWriter writer, List<FileSystemInfo> value, JsonSerializerOptions options)
//     {
//         if (value is null)
//         {
//             writer.WriteNullValue();
//         }
//         else
//         {
//             writer.WriteStringValue(string.Join(",", value.Select(x => x.FullName)));
//         }
//     }
// }
