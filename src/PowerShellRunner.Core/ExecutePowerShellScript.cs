using System.ComponentModel;

namespace PowerShellRunner.Core;

/// <inheritdoc />
public class ExecutePowerShellScript(
    IPowerShellExecutableCandidates powerShellExecutableCandidates,
    IPowerShellProcessStarter powerShellProcessStarter) : IExecutePowerShellScript
{
    private readonly IPowerShellExecutableCandidates _powerShellExecutableCandidates =
        powerShellExecutableCandidates ?? throw new ArgumentNullException(nameof(powerShellExecutableCandidates));

    private readonly IPowerShellProcessStarter _powerShellProcessStarter =
        powerShellProcessStarter ?? throw new ArgumentNullException(nameof(powerShellProcessStarter));

    /// <inheritdoc />
    public void RunFor(FileInfo script) => RunForAsync(script).GetAwaiter().GetResult();

    private async Task RunForAsync(FileInfo script)
    {
        ArgumentNullException.ThrowIfNull(script);
        if (!script.Exists)
        {
            throw new FileNotFoundException("PowerShell script not found.", script.FullName);
        }

        Win32Exception lastException = null;
        foreach (var executable in _powerShellExecutableCandidates.Value)
        {
            var executableNotFoundException = await TryStartWithExecutableAsync(executable, script).ConfigureAwait(false);
            if (executableNotFoundException == null)
            {
                return;
            }

            lastException = executableNotFoundException;
        }

        throw new InvalidOperationException("No usable PowerShell executable was found.", lastException);
    }

    private async Task<Win32Exception> TryStartWithExecutableAsync(string executable, FileInfo script)
    {
        try
        {
            using var process = _powerShellProcessStarter.ValueFor(executable, script);
            await process.WaitForExitAsync().ConfigureAwait(false);
            return null;
        }
        catch (Win32Exception exception) when (exception.NativeErrorCode == 2)
        {
            return exception;
        }
    }
}