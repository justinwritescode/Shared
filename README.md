<!--
 README.md
 
   Created: 2022-10-29-06:17:38
   Modified: 2022-11-01-10:19:22
 
   Author: Justin Chase <justin@justinwritescode.com>
   
   Copyright © 2022-2023 Justin Chase, All Rights Reserved
      License: MIT (https://opensource.org/licenses/MIT)
-->

span

# Shared

## RestoreProjSdk

This repository contains files that are common to all of my projects.
To restore the files, add a ``.restoreproj`` file to the root of your git repo with at least the following contents:

```xml
<Project Sdk="RestoreProjSdk" />
```

Alternatively, stick this line in any ``*proj`` file at the top level of your repo:

```xml
<Sdk Name="RestoreProjSdk" />
```

And add a ``global.json`` file with at least the following contents:

```json
{
    "msbuild-sdks": {
        "RestoreProjSdk": "the-latest-package-version"
    }
}
```

Then run the following command and voila! The files will be restored!

```bash
dotnet build *.restoreproj -t:RestoreSharedFiles
```

If you are restoring a root repo without anything above it, you'll need to restore all of the central files too. Use the following command for that:

```bash
dotnet build *.restoreproj -t:RestoreCentralFiles
```

## RestoreSharedFilesTargets

If you don't want to use the SDK, you can add a reference to the ``RestoreSharedFilesTargets`` package in one of your ``*proj`` files:

```xml
<PackageReference Include="RestoreSharedFilesTargets" Version="the-latest-package-version" />
```

This will add the ``.targets`` file to your project, which will restore the files when you build your project using either of the two restore commands above (but on your ``*proj`` file instead of the ``.restoreproj`` file).

## JustInTimeVersioning

This project keeps a centralized log of all current package versions by configuration.  The current versions are stored in ``Packages/Versions.{Configuration}.json`` and ``Packages/Versions.{Configuration}.props.``. Whenever a package is built, this program runs a target called ``SaveVersionNumberCentrally,`` which stores the package version to these files.  With this repo, you don't have to worry about updating your package versions because it's done for you.

!!! note
   You'll need to run the ``RestoreCentralFiles`` target to the repo you're using or a repo above it in the directory tree for this to work properly.
