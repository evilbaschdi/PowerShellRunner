using Avalonia.Controls;

namespace PowerShellRunner.Gui;

/// <inheritdoc />
public class TrayIconMenuUpdater : ITrayIconMenuUpdater
{
    private const string CloseApplicationHeader = "⏻ Close application";
    private const string DynamicSeparatorHeader = "DynamicSeparator";
    private const string LoadingHeader = "⏳ Loading...";

    /// <inheritdoc />
    public void EnsureStaticCloseMenuEntry(App app, EventHandler onClose)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(onClose);

        var trayMenu = TrayIcon.GetIcons(app)?.FirstOrDefault()?.Menu;
        if (trayMenu == null)
        {
            return;
        }

        var hasCloseEntry = trayMenu.Items.Any(item => item is NativeMenuItem menuItem
                                                        && string.Equals(menuItem.Header, CloseApplicationHeader, StringComparison.Ordinal));
        if (hasCloseEntry)
        {
            return;
        }

        var closeMenuItem = new NativeMenuItem(CloseApplicationHeader);
        closeMenuItem.Click += onClose;
        trayMenu.Items.Add(closeMenuItem);
    }

    /// <inheritdoc />
    public void EnsureLoadingMenuEntry(App app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var trayMenu = TrayIcon.GetIcons(app)?.FirstOrDefault()?.Menu;
        if (trayMenu == null)
        {
            return;
        }

        var hasLoadingEntry = trayMenu.Items.Any(item => item is NativeMenuItem menuItem
                                                          && string.Equals(menuItem.Header, LoadingHeader, StringComparison.Ordinal));
        if (hasLoadingEntry)
        {
            return;
        }

        var closeItemIndex = CloseItemIndexFor(trayMenu);
        var loadingMenuItem = new NativeMenuItem(LoadingHeader) { IsEnabled = false };
        trayMenu.Items.Insert(closeItemIndex, loadingMenuItem);
    }

    /// <inheritdoc />
    public void ApplyRootNodes(App app, IReadOnlyCollection<ScriptDirectoryNode> rootNodes, Action<FileInfo> onScriptSelected)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(rootNodes);
        ArgumentNullException.ThrowIfNull(onScriptSelected);

        var trayMenu = TrayIcon.GetIcons(app)?.FirstOrDefault()?.Menu;
        if (trayMenu == null)
        {
            return;
        }

        RemoveLoadingMenuEntry(trayMenu);
        RemoveDynamicMenuEntries(trayMenu);

        var closeItemIndex = CloseItemIndexFor(trayMenu);
        foreach (var rootNode in rootNodes)
        {
            var menuItem = new NativeMenuItem($"📂 {rootNode.Name}") { Menu = CreateNativeMenu(rootNode, onScriptSelected) };
            trayMenu.Items.Insert(closeItemIndex, menuItem);
            closeItemIndex++;
        }

        if (rootNodes.Count == 0)
        {
            return;
        }

        var separator = new NativeMenuItemSeparator { Header = DynamicSeparatorHeader };
        trayMenu.Items.Insert(closeItemIndex, separator);
    }

    /// <inheritdoc />
    public NativeMenu CreateNativeMenu(ScriptDirectoryNode node, Action<FileInfo> onScriptSelected)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(onScriptSelected);

        var menu = new NativeMenu();

        foreach (var childDirectory in node.ChildDirectories)
        {
            var childDirectoryMenuItem = new NativeMenuItem($"📁 {childDirectory.Name}")
                                         {
                                             Menu = CreateNativeMenu(childDirectory, onScriptSelected)
                                         };
            menu.Items.Add(childDirectoryMenuItem);
        }

        foreach (var scriptFile in node.ScriptFiles)
        {
            var fileInfo = new FileInfo(scriptFile);
            var fileMenuItem = new NativeMenuItem($"📄 {fileInfo.Name}");
            fileMenuItem.Click += (_, _) => onScriptSelected(fileInfo);
            menu.Items.Add(fileMenuItem);
        }

        return menu;
    }

    /// <inheritdoc />
    public void RemoveDynamicMenuEntries(NativeMenu menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        for (var index = menu.Items.Count - 1; index >= 0; index--)
        {
            if (menu.Items[index] is NativeMenuItem menuItem && string.Equals(menuItem.Header, CloseApplicationHeader, StringComparison.Ordinal))
            {
                continue;
            }

            if (menu.Items[index] is NativeMenuItemSeparator separator && string.Equals(separator.Header, DynamicSeparatorHeader, StringComparison.Ordinal))
            {
                menu.Items.RemoveAt(index);
                continue;
            }

            if (menu.Items[index] is NativeMenuItem)
            {
                menu.Items.RemoveAt(index);
            }
        }
    }

    /// <inheritdoc />
    public void RemoveLoadingMenuEntry(NativeMenu menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        for (var index = menu.Items.Count - 1; index >= 0; index--)
        {
            if (menu.Items[index] is NativeMenuItem menuItem && string.Equals(menuItem.Header, LoadingHeader, StringComparison.Ordinal))
            {
                menu.Items.RemoveAt(index);
            }
        }
    }

    /// <inheritdoc />
    public int CloseItemIndexFor(NativeMenu menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return menu.Items
                   .Select((item, index) => new { item, index })
                   .FirstOrDefault(pair => pair.item is NativeMenuItem menuItem
                                           && string.Equals(menuItem.Header, CloseApplicationHeader, StringComparison.Ordinal))
                   ?.index ?? menu.Items.Count;
    }
}
