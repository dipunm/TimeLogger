using System;
using TimeLogger.Domain.OfficeManager;

namespace TimeLogger.Domain.UI
{
    public interface IUserInterface
    {
        Timings GetTimings();
        DateTime GetStartTime();

        void LogTime(IOfficeManager officeManager);
    }

    public class UserInterface : IUserInterface
    {
        private Timings _timings;

        public Timings GetTimings()
        {
            if (_timings == null)
            {
                _timings = new Timings()
                    {
                        SleepAmount = TimeSpan.FromMinutes(30),
                        SnoozeAmount = TimeSpan.FromMinutes(5),
                        SnoozeLimit = TimeSpan.FromMinutes(20)
                    };
            }
            return _timings;
        }

        public DateTime GetStartTime()
        {
            throw new NotImplementedException();
        }

        public void LogTime(IOfficeManager officeManager)
        {
            throw new NotImplementedException();
        }
    }
}