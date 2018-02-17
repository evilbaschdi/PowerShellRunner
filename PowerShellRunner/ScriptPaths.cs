using System.Collections.Generic;

namespace PowerShellRunner.Core
{
    public class ScriptPaths : IScriptPaths
    {
        public List<string> Value
        {
            get
            {
                //todo: setting
                return new List<string> { @"C:\Git" };
            }
        }
    }
}