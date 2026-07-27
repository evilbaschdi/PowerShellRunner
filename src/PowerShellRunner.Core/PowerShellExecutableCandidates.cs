namespace PowerShellRunner.Core;

/// <inheritdoc />
public class PowerShellExecutableCandidates : IPowerShellExecutableCandidates
{
    private static readonly string WindowsPowerShellExecutable =
        Path.Combine(Environment.SystemDirectory, @"WindowsPowerShell\v1.0\powershell.exe");

    /// <inheritdoc />
    public IReadOnlyCollection<string> Value => BuildCandidates();

    private static IReadOnlyCollection<string> BuildCandidates()
    {
        var candidates = new List<string>();
        AddFromPath(candidates);
        AddInstalledPowerShellCore(candidates);
        AddCandidate(candidates, WindowsPowerShellExecutable);
        return candidates;
    }

    private static void AddFromPath(List<string> candidates)
    {
        foreach (var pathSegment in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            var executable = Path.Combine(pathSegment.Trim(), "pwsh.exe");
            if (File.Exists(executable))
            {
                AddCandidate(candidates, executable);
            }
        }
    }

    private static void AddInstalledPowerShellCore(List<string> candidates)
    {
        var powerShellDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PowerShell");
        if (!Directory.Exists(powerShellDirectory))
        {
            return;
        }

        var installedExecutables = Directory.EnumerateDirectories(powerShellDirectory)
                                            .Select(directory => new
                                                                 {
                                                                     Executable = Path.Combine(directory, "pwsh.exe"),
                                                                     DirectoryName = Path.GetFileName(directory)
                                                                 })
                                            .Where(candidate => File.Exists(candidate.Executable))
                                            .Select(candidate => new
                                                                 {
                                                                     candidate.Executable,
                                                                     Version = ParseMajorVersion(candidate.DirectoryName)
                                                                 })
                                            .OrderByDescending(candidate => candidate.Version)
                                            .ThenByDescending(candidate => candidate.Executable, StringComparer.OrdinalIgnoreCase)
                                            .Select(candidate => candidate.Executable);

        foreach (var executable in installedExecutables)
        {
            AddCandidate(candidates, executable);
        }
    }

    private static void AddCandidate(List<string> candidates, string executable)
    {
        if (string.IsNullOrWhiteSpace(executable) || candidates.Any(candidate => string.Equals(candidate, executable, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        candidates.Add(executable);
    }

    private static int ParseMajorVersion(string directoryName)
    {
        if (string.IsNullOrWhiteSpace(directoryName))
        {
            return -1;
        }

        var majorPart = directoryName.Split(['.', '-'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return int.TryParse(majorPart, out var majorVersion)
            ? majorVersion
            : -1;
    }
}