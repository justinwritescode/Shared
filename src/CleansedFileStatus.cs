#pragma warning disable
#pragma warning disable
namespace JustinWritesCode.Common;

using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Execution;
using System.Linq;

public enum CleansedFileStatus
{
    Cleansed,
    SkippedDidNotExist,
    SkippedNotEmpty
}
