using System;
using Microsoft.Win32;
using TimeLogger.Utils.Core;

namespace TimeLogger.Utils.Domain
{
    public class WindowsOsTracker : IOsTracker, IDisposable
    {
        private bool disposed = false;

        public WindowsOsTracker()
        {
            SystemEvents.SessionSwitch += HandleSessionSwitching;
            SystemEvents.PowerModeChanged += HandlePowerChanging;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                SystemEvents.SessionSwitch -= HandleSessionSwitching;
                SystemEvents.PowerModeChanged -= HandlePowerChanging;
            }
        }

        public event ComputerEventHandler UserLeft;
        public event ComputerEventHandler UserReturned;

        private void HandlePowerChanging(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    Resume();
                    break;
                case PowerModes.Suspend:
                    Pause();
                    break;
                default:
                    return;
            }
        }

        private void HandleSessionSwitching(object sender, SessionSwitchEventArgs sessionSwitchEventArgs)
        {
            switch (sessionSwitchEventArgs.Reason)
            {
                case SessionSwitchReason.RemoteConnect:
                case SessionSwitchReason.SessionLogon:
                case SessionSwitchReason.SessionUnlock:
                    Resume();
                    break;
                case SessionSwitchReason.SessionLogoff:
                case SessionSwitchReason.RemoteDisconnect:
                case SessionSwitchReason.SessionLock:
                    Pause();
                    break;
                default:
                    return;
            }
        }

        private void Pause()
        {
            if (UserLeft != null)
            {
                UserLeft.Invoke(this);
            }
        }

        private void Resume()
        {
            if (UserReturned != null)
            {
                UserReturned.Invoke(this);
            }
        }
    }
}