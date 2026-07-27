namespace PowerShellRunner.Gui;

/// <summary>
///     Represents a directory node for tray menu rendering.
/// </summary>
public sealed class ScriptDirectoryNode
{
    /// <summary />
    public ScriptDirectoryNode(string name, IReadOnlyCollection<ScriptDirectoryNode> childDirectories, IReadOnlyCollection<string> scriptFiles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(childDirectories);
        ArgumentNullException.ThrowIfNull(scriptFiles);

        Name = name;
        ChildDirectories = childDirectories;
        ScriptFiles = scriptFiles;
    }

    /// <summary />
    public string Name { get; }

    /// <summary />
    public IReadOnlyCollection<ScriptDirectoryNode> ChildDirectories { get; }

    /// <summary />
    public IReadOnlyCollection<string> ScriptFiles { get; }

    /// <summary />
    public bool HasContent => ChildDirectories.Count > 0 || ScriptFiles.Count > 0;
}
