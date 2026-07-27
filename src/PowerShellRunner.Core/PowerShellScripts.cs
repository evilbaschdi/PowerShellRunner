using System.Collections.Concurrent;
using EvilBaschdi.Core.Extensions;

namespace PowerShellRunner.Core;

/// <inheritdoc />
public class PowerShellScripts : IPowerShellScripts
{
    private readonly IPowerShellScriptsRaw _powerShellScriptsRaw;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="powerShellScriptsRaw"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public PowerShellScripts(IPowerShellScriptsRaw powerShellScriptsRaw)
    {
        _powerShellScriptsRaw =
            powerShellScriptsRaw ?? throw new ArgumentNullException(nameof(powerShellScriptsRaw));
    }

    /// <inheritdoc />
    public List<FileInfo> Value
    {
        get
        {
            var powerShellScripts = new ConcurrentBag<FileInfo>();

            Parallel.ForEach(_powerShellScriptsRaw.Value,
                path => { powerShellScripts.Add(path.FileInfo()); });
            return [.. powerShellScripts];
        }
    }
}