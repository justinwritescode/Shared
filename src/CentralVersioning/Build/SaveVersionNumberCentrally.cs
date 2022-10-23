namespace JustinWritesCode.Versioning;

public class SaveVersionNumberCentrally : MSBTask
{
    [Required]
    public string PackageName { get; set; } = string.Empty;

    [Required]
    public string Version { get; set; } = string.Empty;
    [Required]
    public string Configuration { get; set; } = "Local";
    public string CentralVersionsFolderPath { get; set; } = string.Empty;
    public string VersionsJsonFilePath { get; set; } = "Versions.{0}.json";
    public string VersionsPropsFilePath { get; set; } = "Versions.{0}.pkgs";

    public override bool Execute()
    {
        new VersionManager(Configuration, VersionsJsonFilePath, VersionsPropsFilePath).SetVersion(PackageName, Version);
        System.Console.WriteLine($"Saved version {Version} for package {PackageName}.");
        return true;
    }
}
