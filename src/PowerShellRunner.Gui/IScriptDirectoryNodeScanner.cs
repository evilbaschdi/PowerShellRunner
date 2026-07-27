namespace PowerShellRunner.Gui;

/// <summary>
///     Builds script directory nodes for tray rendering.
/// </summary>
public interface IScriptDirectoryNodeScanner
{
    /// <summary />
    IReadOnlyCollection<ScriptDirectoryNode> ValueFor(IReadOnlyCollection<string> scriptPaths);

    /// <summary />
    ScriptDirectoryNode ValueFor(DirectoryInfo directoryInfo);
}
