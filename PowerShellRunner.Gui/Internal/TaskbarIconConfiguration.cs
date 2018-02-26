using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using EvilBaschdi.Core.Extensions;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro.IconPacks;
using PowerShellRunner.Core;

namespace PowerShellRunner.Gui.Internal
{
    public class TaskbarIconConfiguration : ITaskbarIconConfiguration
    {
        private readonly MainWindow _mainWindow;

        private readonly TaskbarIcon _taskbarIcon;
        private readonly IPowerShellScripts _powerShellScripts;
        private readonly IExecutePowerShellScript _executePowerShellScript;
        private readonly IScriptPaths _scriptPaths;

        /// <summary>
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="taskbarIcon"></param>
        /// <param name="powerShellScripts"></param>
        /// <param name="executePowerShellScript"></param>
        /// <param name="scriptPaths"></param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="mainWindow" /> is <see langword="null" />.
        ///     <paramref name="taskbarIcon" /> is <see langword="null" />.
        /// </exception>
        public TaskbarIconConfiguration(MainWindow mainWindow, TaskbarIcon taskbarIcon, IPowerShellScripts powerShellScripts, IExecutePowerShellScript executePowerShellScript,
                                        IScriptPaths scriptPaths)
        {
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            _taskbarIcon = taskbarIcon ?? throw new ArgumentNullException(nameof(taskbarIcon));
            _powerShellScripts = powerShellScripts ?? throw new ArgumentNullException(nameof(powerShellScripts));
            _executePowerShellScript = executePowerShellScript ?? throw new ArgumentNullException(nameof(executePowerShellScript));
            _scriptPaths = scriptPaths ?? throw new ArgumentNullException(nameof(scriptPaths));
        }


        public void Run()
        {
            var filePath = Assembly.GetEntryAssembly().Location;
            _taskbarIcon.Icon = Icon.ExtractAssociatedIcon(filePath);
            _taskbarIcon.ContextMenu = TaskbarIconContextMenu();
            _taskbarIcon.TrayMouseDoubleClick += TaskbarIconDoubleClick;
        }

        /// <summary>
        /// </summary>
        public void StartMinimized()
        {
            _taskbarIcon.Visibility = Visibility.Visible;
            _mainWindow.Hide();
        }

        // ReSharper disable UseObjectOrCollectionInitializer
        private ContextMenu TaskbarIconContextMenu()
        {
            var contextMenu = new ContextMenu();

            foreach (var scriptPath in _scriptPaths.Value)
            {
                DirectoryInfo info = new DirectoryInfo(scriptPath);
                if (info.Exists)
                {
                    var rootNode = new MenuItem
                    {
                        Header = info.Name,
                        Icon = new PackIconMaterial
                        {
                            Kind = PackIconMaterialKind.Folder
                        },
                        Tag = info
                    };
                    GetFiles(info, rootNode);
                    GetDirectories(info.GetDirectories(), rootNode);
                    contextMenu.Items.Add(rootNode);
                }
            }

            var restoreApplication = new MenuItem
            {
                Header = "Restore application",
                Icon = new PackIconMaterial
                {
                    Kind = PackIconMaterialKind.WindowRestore
                }
            };
            restoreApplication.Click += ContextMenuItemRestoreClick;

            var closeApplication = new MenuItem
            {
                Header = "Close application",
                Icon = new PackIconMaterial
                {
                    Kind = PackIconMaterialKind.Power
                }
            };
            closeApplication.Click += ContextMenuItemCloseClick;

            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(restoreApplication);
            contextMenu.Items.Add(closeApplication);

            return contextMenu;
        }

        private void GetDirectories(IEnumerable<DirectoryInfo> subDirs, ItemsControl nodeToAddTo)
        {
            foreach (var subDir in subDirs.Where(dir => !dir.Name.In("packages", "Modules")))
            {
                var aNode = new MenuItem
                {
                    Header = subDir.Name,
                    Icon = new PackIconMaterial
                    {
                        Kind = PackIconMaterialKind.Folder
                    },
                    Tag = subDir
                };

                var subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }

                GetFiles(subDir, aNode);
                if (aNode.HasItems)
                {
                    nodeToAddTo.Items.Add(aNode);
                }
            }
        }

        private void GetFiles(DirectoryInfo directoryInfo, ItemsControl nodeToAddTo)
        {
            foreach (var fileInfo in directoryInfo.GetFiles("*.ps1"))
            {
                var fNode = new MenuItem
                {
                    Header = fileInfo.Name,
                    Icon = new PackIconMaterial
                    {
                        Kind = PackIconMaterialKind.Script
                    },
                    Tag = fileInfo
                };
                fNode.Click += (sender, args) => _executePowerShellScript.RunFor(fileInfo);
                nodeToAddTo.Items.Add(fNode);
            }
        }

        private void ContextMenuItemCloseClick(object sender, EventArgs e)
        {
            _taskbarIcon.Dispose();
            _mainWindow.Close();
        }

        private void ContextMenuItemRestoreClick(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }

        private void TaskbarIconDoubleClick(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }
    }
}