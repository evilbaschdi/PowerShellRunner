using System.Diagnostics;

namespace PowerShellRunner.Core;

/// <inheritdoc />
public class PowerShellProcessStarter : IPowerShellProcessStarter
{
    /// <inheritdoc />
    public Process ValueFor(string executable, FileInfo script)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executable);
        ArgumentNullException.ThrowIfNull(script);

        var process = new Process
                      {
                          StartInfo = new ProcessStartInfo
                                      {
                                          FileName = executable,
                                          Arguments = $"-ExecutionPolicy Unrestricted -File \"{script.FullName}\"",
                                          UseShellExecute = false
                                      }
                      };

        if (!process.Start())
        {
            process.Dispose();
            throw new InvalidOperationException($"Could not start PowerShell process: {executable}");
        }

        return process;
    }
}