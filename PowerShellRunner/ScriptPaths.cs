using System.Collections.Generic;

namespace PowerShellRunner.Core
{
    public class ScriptPaths : IScriptPaths
    {
        public List<string> Value => new()
                                     { @"C:\Git" };
    }
}