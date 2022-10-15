namespace JustinWritesCode.Versioning;

public class WriteVersion : Microsoft.Build.Utilities.Task
{
    [Microsoft.Build.Framework.Required]
    public string Version { get; set; }
    [Microsoft.Build.Framework.Required]
    public string OutputFile { get; set; }
    public override bool Execute()
    {
        System.IO.File.WriteAllText(OutputFile, Version);
        return true;
    }
}
