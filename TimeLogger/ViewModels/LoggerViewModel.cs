using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeLogger.Core.Utils;
using TimeLogger.Domain;
using TimeLogger.MVVM;
using TimeLogger.Models;
using TimeLogger.Services;

namespace TimeLogger.ViewModels
{
    public class LoggerViewModel : ObservableObject
    {
        private readonly Mediator _mediator;
        private readonly ILogRepository _logRepository;
        private readonly IClock _clock;
        private DateTime _startTime;
        private string _title;

        public LoggerViewModel(Mediator mediator, ILogRepository logRepository, IClock clock)
        {
            _logRepository = logRepository;
            _clock = clock;
            _mediator = mediator;
            Entry = new WorkEntryViewModel();
            Stats = new WorkLogStatsViewModel();
            Entry.WorklogSubmitted += HandleWorkLog;
        }
        
        public void HandleWorkLog(WorkLog workLog)
        {
            _logRepository.AddLog(workLog);
            Stats.NumberOfMinutesLogged += workLog.Minutes;
            if (Stats.PercentageComplete >= 1)
            {
                var timeSpent = _clock.Now() - _startTime;
                _logRepository.AddLog(new WorkLog()
                    {
                        Comment="Logging Work",
                        Date = _startTime.Date,
                        Minutes = (int)Math.Ceiling(timeSpent.TotalMinutes),
                        TicketCodes = _mediator.GetTimeTickets()
                    });
                _mediator.Sleep();
            }
        }

        public WorkEntryViewModel Entry { get; private set; }
        public WorkLogStatsViewModel Stats { get; private set; }

        public string Title
        {
            get { return _title; }
            private set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public void Reset(int minutesToLog, DateTime startTime)
        {
            _startTime = startTime;
            Entry.ResetProperties(startTime);
            Stats.ResetStats(minutesToLog);
            Title = String.Format("Logging time for {0}", _startTime.ToShortDateString());
        }
    }
}
