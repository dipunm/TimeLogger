using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using MVVM.Extensions;
using TimeLogger.Data.Core;

namespace TimeLogger.Wpf.ViewModels
{
    public class WorkEntryViewModel : ObservableObject, IDataErrorInfo
    {
        private string _comment;
        private bool _isBreak;
        private int? _minutesWorked;
        private string _ticketCodes;

        public WorkEntryViewModel()
        {
            SaveWorkLog = new DelegateCommand(SaveWorkLogAction);
        }

        public ICommand SaveWorkLog { get; private set; }

        public string TicketCodes
        {
            get { return _ticketCodes; }
            set
            {
                _ticketCodes = value;
                OnPropertyChanged("TicketCodes");
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                OnPropertyChanged("Comment");
            }
        }

        public int? MinutesWorked
        {
            get { return _minutesWorked; }
            set
            {
                _minutesWorked = value;
                OnPropertyChanged("MinutesWorked");
            }
        }

        public bool IsBreak
        {
            get { return _isBreak; }
            set
            {
                if (value.Equals(_isBreak)) return;
                _isBreak = value;
                OnPropertyChanged("IsBreak");
            }
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Comment":
                        if (String.IsNullOrEmpty(Comment))
                            return "Please enter a message.";
                        return null;
                    case "TicketCodes":
                        if (!IsBreak)
                        {
                            if (String.IsNullOrEmpty(TicketCodes))
                                return "Please enter one or more ticket codes";
                            else if (!Regex.IsMatch(TicketCodes, @"^([a-zA-Z]+\-\d+\s*,?\s*)+$"))
                                return "Tickets should be in format: AAA-NNN where A is alphabetic " +
                                       "characters and N are digits. Separate multiples with commas.";
                        }
                        return null;
                    case "MinutesWorked":
                        if (MinutesWorked == null || MinutesWorked < 1)
                            return "Enter number of minutes worked";
                        return null;
                    default:
                        return null;
                }
            }
        }

        private void SaveWorkLogAction()
        {
            if (!String.IsNullOrEmpty(this["Comment"] + this["MinutesWorked"] + this["TicketCodes"]))
            {
                //error;
                return;
            }

            List<string> ticketCodes = IsBreak ? null : TicketCodes.Split(' ').Select(c => c.Trim()).ToList();

            //if (!ticketCodes.Any())
            //{
            //    //logging a break
            //}

            var work = new WorkLog
                {
                    Comment = Comment,
// ReSharper disable PossibleInvalidOperationException
// MinutesWorked should be validated by now.
                    Minutes = MinutesWorked.Value,
// ReSharper restore PossibleInvalidOperationException
                    TicketCodes = ticketCodes
                };

            if (WorklogSubmitted != null)
                WorklogSubmitted(work);

            ResetProperties();
        }

        public void ResetProperties()
        {
            Comment = null;
            MinutesWorked = null;
            TicketCodes = null;
            IsBreak = false;
        }

        public event Action<WorkLog> WorklogSubmitted;
    }
}