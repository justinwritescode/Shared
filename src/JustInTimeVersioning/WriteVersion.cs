// 
// WriteVersion.cs
// 
//   Created: 2022-10-24-04:48:49
//   Modified: 2022-10-29-12:09:25
// 
//   Author: Justin Chase <justin@justinwritescode.com>
//   
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 

namespace JustInTimeVersioning;

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
