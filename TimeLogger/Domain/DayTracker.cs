using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Core;
using TimeLogger.Models;
using TimeLogger.Services;

namespace TimeLogger.Domain
{
    /// <summary>
    /// Keeps track of a working day.
    /// 
    /// </summary>
    public class LogTracker : ILogTracker
    {
        private readonly ILogRepository _logRepository;
        private DateTime StartTime { get; set; }
        private DateTime TodaysDate { get; set; }

        public LogTracker(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public void SetDayStartTime(DateTime start)
        {
            TodaysDate = start.Date;
            StartTime = start;
        }

        public int GetMinutesToLog(DateTime targetTime)
        {
            var todaysLogs = _logRepository
                .GetAllLogs()
                .Where(l => l.Date == TodaysDate);

            var logged = todaysLogs
                .Where(l => l.TicketCodes != null && l.TicketCodes.Any())
                .Sum(l => l.Minutes);
            
            var skipped = todaysLogs
                .Where(l => l.TicketCodes == null || !l.TicketCodes.Any())
                .Sum(l => l.Minutes);
            
            skipped = Math.Min(60, skipped);

            return (int) Math.Ceiling((targetTime
                   - StartTime
                   - TimeSpan.FromMinutes(logged)
                   - TimeSpan.FromMinutes(skipped)).TotalMinutes);
        }
    }
}
