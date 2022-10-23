namespace JustinWritesCode.JustInTimeVersioning;

public class WriteVersion : Microsoft.Build.Utilities.Task
{
    [Microsoft.Build.Framework.Required]
    public string Version { get; set; } = string.Empty;
    [Microsoft.Build.Framework.Required]
    public string OutputFile { get; set; } = string.Empty;
    public override bool Execute()
    {
        System.IO.File.WriteAllText(OutputFile, Version);
        return true;
    }
}
