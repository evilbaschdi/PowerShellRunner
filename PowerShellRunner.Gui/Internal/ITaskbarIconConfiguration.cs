using EvilBaschdi.Core;

namespace PowerShellRunner.Gui.Internal
{
    /// <inheritdoc />
    public interface ITaskBarIconConfiguration : IRun
    {
        /// <summary>
        /// </summary>
        void StartMinimized();
    }
}