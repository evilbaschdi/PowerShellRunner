using System;
using System.Collections.Generic;
using EvilBaschdi.Core.Internal;
using EvilBaschdi.Core.Model;

namespace PowerShellRunner.Core
{
    public class PowerShellScriptsRaw : IPowerShellScriptsRaw
    {
        private readonly IFileListFromPath _fileListFromPath;
        private readonly IScriptPaths _scriptPaths;

        public PowerShellScriptsRaw(IFileListFromPath fileListFromPath, IScriptPaths scriptPaths)
        {
            _fileListFromPath = fileListFromPath ?? throw new ArgumentNullException(nameof(fileListFromPath));
            _scriptPaths = scriptPaths ?? throw new ArgumentNullException(nameof(scriptPaths));
        }

        public List<string> Value
        {
            get
            {
                var fileList = new List<string>();
                foreach (var scriptPath in _scriptPaths.Value)
                {
                    var filePathDirectoryLists = new FileListFromPathFilter
                                                 {
                                                     FilterExtensionsToEqual = new List<string> { "ps1" },
                                                     FilterFilePathsNotToEqual = new List<string>
                                                                                 {
                                                                                     @"modules\pswindowsupdate",
                                                                                     "packages"
                                                                                 }
                                                 };
                    fileList.AddRange(_fileListFromPath.ValueFor(scriptPath, filePathDirectoryLists));
                }

                return fileList;
            }
        }
    }
}