using System;
using TimeLogger.UserInterface.ViewModels;

namespace TimeLogger.Wpf.ViewModels
{
    public interface IApplication
    {
        void Shutdown();
        IComposer CreateComposer();
        DateTime GetTime();
        DateTime GetEndTime();
    }
}