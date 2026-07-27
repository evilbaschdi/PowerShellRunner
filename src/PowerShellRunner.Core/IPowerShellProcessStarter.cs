using System.Diagnostics;

namespace PowerShellRunner.Core;

/// <summary>
///     Represents a starter for PowerShell processes.
/// </summary>
public interface IPowerShellProcessStarter
{
    /// <summary>
    ///     Creates and starts a PowerShell process for the specified executable and script.
    /// </summary>
    /// <param name="executable"></param>
    /// <param name="script"></param>
    /// <returns></returns>
    Process ValueFor(string executable, FileInfo script);
}