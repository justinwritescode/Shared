namespace JustinWritesCode.Versioning;

public class SaveVersionNumberCentrally : MSBTask
{
    [Required]
    public string PackageName { get; set; } = string.Empty;

    [Required]
    public string Version { get; set; } = string.Empty;
    [Required]
    public string Configuration { get; set; } = "Local";
    public string VersionsJsonFileName { get; set; } = "Versions.{0}.json";
    public string VersionsPropsFileName { get; set; } = "Versions.{0}.props";

    public override bool Execute()
    {
        new VersionManager(Configuration, VersionsJsonFileName, VersionsPropsFileName).SetVersion(PackageName, Version);
        System.Console.WriteLine($"Saved version {Version} for package {PackageName}.");
        return true;
    }
}
