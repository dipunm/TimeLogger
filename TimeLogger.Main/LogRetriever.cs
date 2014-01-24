using System;
using System.Collections.Generic;
using System.Threading;
using TimeLogger.Cache.Core;
using TimeLogger.Utils.Core;
using TimeLogger.Wpf.Domain.Controllers;
using TimeLogger.Wpf.ViewModels;

namespace TimeLogger.Main
{
    public class LogRetriever : ILogRetriever
    {
        private enum InternalState
        {
            Idle = 0,
            Uncertain,
            Prompting,
            Logging
        };

        private InternalState _state;
        private readonly DialogController<PromptViewModel> _promptController;
        private readonly DialogController<LoggerViewModel> _loggerController;
        private readonly ITimeFactory _timeFactory;

        public LogRetriever(DialogController<PromptViewModel> promptController,
                            DialogController<LoggerViewModel> loggerController,
                            ITimeFactory timeFactory)
        {
            _promptController = promptController;
            _loggerController = loggerController;
            _timeFactory = timeFactory;
            _state = InternalState.Idle;
            LoggingTicketCodes = new List<string>() {"AD-22"};
        }

        public LoggingResponse GetWorkLogs(TimeSpan durationToLog, bool skipPleasantries)
        {
            _state = InternalState.Uncertain;
            bool? result;
            if (!skipPleasantries)
            {
                //Time response time and add to targetTime
                var timer = _timeFactory.CreateTimeTracker();
                timer.Start();
                _state = InternalState.Prompting;
                result = _promptController.ShowDialog();
                _state = InternalState.Uncertain;
                durationToLog += timer.Stop();
                if (result == null || result.Value == false)
                {
                    _state = InternalState.Idle;
                    return LoggingResponse.SnoozeResponse;
                }
            }

            _state = InternalState.Logging;
            var timer2 = _timeFactory.CreateTimeTracker();
            timer2.Start();
            _loggerController.ViewModel.Reset((int)Math.Ceiling(durationToLog.TotalMinutes));
            do
            {
                result = _loggerController.ShowDialog();
            } while (result == null || result.Value == false);
            var timeSpentLogging = timer2.Stop();

            var response = LoggingResponse.WithLogs(_loggerController.ViewModel.Logs);
            response.Logs.Add(new WorkLog()
                {
                    Comment = "Logging Time",
                    Minutes = (int)Math.Ceiling(timeSpentLogging.TotalMinutes),
                    TicketCodes = LoggingTicketCodes
                });

            _state = InternalState.Idle;
            return response;
        }

        public List<string> LoggingTicketCodes { private get; set; }

        public bool TryUpdateRunningThread(TimeSpan durationToLog)
        {
            //we want to correct the other thread:
            switch (_state)
            {
                case InternalState.Logging:
                    _loggerController.ViewModel.Update((int)Math.Ceiling(durationToLog.TotalMinutes));
                    return true;
                case InternalState.Prompting:
                    
                    break;
                case InternalState.Idle:
                    break;
                case InternalState.Uncertain:
                    var sleep = TimeSpan.FromTicks(100);
                    Thread.Sleep(sleep);
                    return TryUpdateRunningThread(durationToLog + sleep);
            }
            return false;
        }
    }
}