using System;
using System.Windows.Threading;

namespace TimeLogger.UserInterface.ViewModels
{
    public interface IComposer
    {
        bool Ending { get; set; }
        void LogWork(DateTime targetTime);
        ComposerState GetState();
    }
}