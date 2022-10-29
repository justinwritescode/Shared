# Shared

## RestoreProjSdk

This repository contains files that are common to all of my projects. 
To restore the files, add a ```.restoreproj``` file to the root of your git repo with at least the following contents:

```xml
<Project Sdk="JustinWritesCode.RestoreProjSdk" />
```

Alternatively, stick this line in any ```*proj``` file at the top level of your repo:

```xml
<Sdk Name="JustinWritesCode.RestoreProjSdk" />
```

And add a ```global.json``` file with at least the following contents:

```json
{
    "msbuild-sdks": {
        "JustinWritesCode.RestoreProjSdk": "the-latest-package-version"
    }
}
```

Then run the following command and voila! The files will be restored!

```dotnet build *.restoreproj -t:RestoreSharedFiles```

If you are restoring a root repo without anything above it, you'll need to restore all of the central files too. Use the following command for that:

```dotnet build *.restoreproj -t:RestoreCentralFiles```
