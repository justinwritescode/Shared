// 
// SaveVersionNumberCentrally.cs
// 
//   Created: 2022-10-24-04:48:49
//   Modified: 2022-10-29-12:13:24
// 
//   Author: Justin Chase <justin@justinwritescode.com>
//   
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 

namespace JustInTimeVersioning;

public class SaveVersionNumberCentrally : MSBTask
{
    [MSBF.Required]
    public string PackageName { get; set; } = string.Empty;

    [MSBF.Required]
    public string Version { get; set; } = string.Empty;
    [MSBF.Required]
    public string Configuration { get; set; } = "Local";
    public string VersionsJsonFileName { get => VersionManager.VersionsJsonFileName; set => VersionManager.VersionsJsonFileName = value; }
    public string VersionsPropsFileName { get => VersionManager.VersionsPropsFileName; set => VersionManager.VersionsPropsFileName = value; }

    public override bool Execute()
    {
        VersionManager.Configuration = Configuration;
        VersionManager.SaveVersion(PackageName, Version);
        System.Console.WriteLine($"Saved version {Version} for package {PackageName} to {VersionManager.VersionsPropsFilePath}.");
        return true;
    }
}
