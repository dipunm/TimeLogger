using System;
using Microsoft.Win32;
using TimeLogger.Core;
using TimeLogger.Services;

namespace TimeLogger.Domain
{
    public class ActivityTracker : IActivityTracker
    {
        private readonly IClock _clock;

        public ActivityTracker(IClock clock)
        {
            _clock = clock;
            SystemEvents.SessionSwitch += HandleSessionSwitching;
            SystemEvents.PowerModeChanged += HandlePowerChanging;
        }

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
            LastEnded = _clock.Now();
            if (UserLeft != null)
            {
                UserLeft.Invoke(this);
            }
        }

        private void Resume()
        {
            LastBegun = _clock.Now();
            if (UserReturned != null)
            {
                UserReturned.Invoke(this);
            }
        }

        public DateTime? LastEnded { get; private set; }

        public DateTime LastBegun { get; private set; }

        public event ActivityTrackerEventHandler UserLeft;
        public event ActivityTrackerEventHandler UserReturned;
    }
}