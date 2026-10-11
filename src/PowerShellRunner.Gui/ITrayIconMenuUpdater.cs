using Avalonia.Controls;

namespace PowerShellRunner.Gui;

/// <summary>
///     Updates tray icon menu entries.
/// </summary>
public interface ITrayIconMenuUpdater
{
    /// <summary />
    void EnsureStaticCloseMenuEntry(App app, EventHandler onClose);

    /// <summary />
    void EnsureLoadingMenuEntry(App app);

    /// <summary />
    void ApplyRootNodes(App app, IReadOnlyCollection<ScriptDirectoryNode> rootNodes, Action<FileInfo> onScriptSelected);

    /// <summary />
    NativeMenu CreateNativeMenu(ScriptDirectoryNode node, Action<FileInfo> onScriptSelected);

    /// <summary />
    void RemoveDynamicMenuEntries(NativeMenu menu);

    /// <summary />
    void RemoveLoadingMenuEntry(NativeMenu menu);

    /// <summary />
    int CloseItemIndexFor(NativeMenu menu);
}