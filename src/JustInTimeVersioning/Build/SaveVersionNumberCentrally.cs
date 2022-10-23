namespace JustinWritesCode.JustInTimeVersioning;

public class SaveVersionNumberCentrally : MSBTask
{
    [Required]
    public string PackageName { get; set; } = string.Empty;

    [Required]
    public string Version { get; set; } = string.Empty;
    [Required]
    public string Configuration { get; set; } = "Local";
    public string VersionsJsonFileName { get => VersionManager.VersionsJsonFileName; set => VersionManager.VersionsJsonFileName = value; }
    public string VersionsPropsFileName { get => VersionManager.VersionsPropsFileName; set => VersionManager.VersionsPropsFileName = value; }

    public override bool Execute()
    {
        VersionManager.Configuration = Configuration;
        VersionManager.SaveVersion(PackageName, Version);
        System.Console.WriteLine($"Saved version {Version} for package {PackageName}.");
        return true;
    }
}
