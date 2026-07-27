using EvilBaschdi.Core.Internal;
using EvilBaschdi.Core.Model;

namespace PowerShellRunner.Core;

/// <inheritdoc />
public class PowerShellScriptsRaw : IPowerShellScriptsRaw
{
    private readonly IFileListFromPath _fileListFromPath;
    private readonly IScriptPaths _scriptPaths;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="fileListFromPath"></param>
    /// <param name="scriptPaths"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public PowerShellScriptsRaw(IFileListFromPath fileListFromPath, IScriptPaths scriptPaths)
    {
        _fileListFromPath = fileListFromPath ?? throw new ArgumentNullException(nameof(fileListFromPath));
        _scriptPaths = scriptPaths ?? throw new ArgumentNullException(nameof(scriptPaths));
    }

    /// <inheritdoc />
    public List<string> Value
    {
        get
        {
            var fileList = new List<string>();
            foreach (var scriptPath in _scriptPaths.Value)
            {
                var filePathDirectoryLists = new FileListFromPathFilter
                                             {
                                                 FilterExtensionsToEqual = ["ps1"],
                                                 FilterFilePathsNotToEqual =
                                                 [
                                                     @"modules\pswindowsupdate",
                                                     "packages"
                                                 ]
                                             };
                fileList.AddRange(_fileListFromPath.ValueFor(scriptPath, filePathDirectoryLists));
            }

            return fileList;
        }
    }
}