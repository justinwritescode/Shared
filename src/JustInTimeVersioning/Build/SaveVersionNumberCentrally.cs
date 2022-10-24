//    
//    SaveVersionNumberCentrally.cs
//    
//    Author: Justin Chase <justin@justinwritescode.com>
//    
//    MIT License
//    
//    Copyright (c) 2022 Justin Chase <justin@justinwritescode.com>
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.
//    
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
