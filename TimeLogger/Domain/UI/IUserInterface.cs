using System;
using TimeLogger.Domain.OfficeManager;
using TimeLogger.Services;

namespace TimeLogger.Domain.UI
{
    public interface IUserInterface
    {
        Timings GetTimings();
        DateTime GetStartTime();

        void LogTime(IOfficeManager officeManager, TimeSpan timeToLog, TimeSpan restTime);
    }

    public class UserInterface : IUserInterface
    {
        private readonly IClock _clock;
        private Timings _timings;
        private DateTime _startTime;

        public UserInterface(IClock clock)
        {
            _clock = clock;
        }

        public Timings GetTimings()
        {
            if (_timings == null)
            {
                //LoadWelcomeWindow.
            }
            return _timings;
        }

        public DateTime GetStartTime()
        {
            if (_startTime.Date != _clock.Now().Date)
            {
                //LoadWelcomeScreen
            }
            return _startTime;
        }

        public void LogTime(IOfficeManager officeManager, TimeSpan timeToLog, TimeSpan restTime)
        {
            //popup
            // onSnooze{officeManager.RemindMeInABit()}
            // disableSnooze:restTime
            //timeWindow
            // onComplete{officeManager.SubmitWork(allWork)}
        }
    }
}