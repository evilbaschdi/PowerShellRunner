namespace PowerShellRunner.Core;

/// <summary>
///     Represents a collection of PowerShell executable candidates.
/// </summary>
public interface IPowerShellExecutableCandidates
{
    /// <summary>
    ///     Gets the collection of PowerShell executable candidates.
    /// </summary>
    IReadOnlyCollection<string> Value { get; }
}