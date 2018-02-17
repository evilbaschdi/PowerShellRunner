using EvilBaschdi.Core;

namespace PowerShellRunner.Gui.Internal
{
    public interface ITaskbarIconConfiguration : IRun
    {
        void StartMinimized();
    }
}