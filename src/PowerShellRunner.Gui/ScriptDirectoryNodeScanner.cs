namespace PowerShellRunner.Gui;

/// <inheritdoc />
public class ScriptDirectoryNodeScanner : IScriptDirectoryNodeScanner
{
    /// <inheritdoc />
    public IReadOnlyCollection<ScriptDirectoryNode> ValueFor(IReadOnlyCollection<string> scriptPaths)
    {
        ArgumentNullException.ThrowIfNull(scriptPaths);

        var rootNodes = new List<ScriptDirectoryNode>();
        foreach (var scriptPath in scriptPaths)
        {
            var directoryInfo = new DirectoryInfo(scriptPath);
            if (!directoryInfo.Exists)
            {
                continue;
            }

            var node = ValueFor(directoryInfo);
            if (node.HasContent)
            {
                rootNodes.Add(node);
            }
        }

        return rootNodes;
    }

    /// <inheritdoc />
    public ScriptDirectoryNode ValueFor(DirectoryInfo directoryInfo)
    {
        ArgumentNullException.ThrowIfNull(directoryInfo);

        var childDirectories = directoryInfo.GetDirectories()
                                            .Where(dir => !dir.Name.Equals("packages", StringComparison.OrdinalIgnoreCase)
                                                          && !dir.Name.Equals("Modules", StringComparison.OrdinalIgnoreCase))
                                            .OrderBy(dir => dir.Name, StringComparer.OrdinalIgnoreCase)
                                            .Select(ValueFor)
                                            .Where(node => node.HasContent)
                                            .ToList();

        var scriptFiles = directoryInfo.GetFiles("*.ps1")
                                       .OrderBy(file => file.Name, StringComparer.OrdinalIgnoreCase)
                                       .Select(file => file.FullName)
                                       .ToList();

        return new ScriptDirectoryNode(directoryInfo.Name, childDirectories, scriptFiles);
    }
}
