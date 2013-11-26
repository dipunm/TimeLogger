using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Domain.Computer;
using TimeLogger.Domain.Data;
using TimeLogger.Domain.UI;
using TimeLogger.Models;

namespace TimeLogger.Domain.OfficeManager
{
    public interface IOfficeManager
    {
        void RegisterEmployee(IUserInterface userInterface, IWorkRepository storage, IComputer computer);
        void RemindMeInABit();
        void SubmitWork(IList<WorkLog> work);
    }

    public class OfficeManager : IOfficeManager
    {
        private IUserInterface _userInterface;
        private IWorkRepository _storage;
        private IComputer _computer;

        public void RegisterEmployee(IUserInterface userInterface, IWorkRepository storage, IComputer computer)
        {
            _userInterface = userInterface;
            _storage = storage;
            _computer = computer;



            _computer.UserLeft += ComputerOnUserLeft;
            _computer.UserReturned += ComputerOnUserReturned;

            _userInterface.GetTimings();
            _userInterface.GetStartTime();
        }

        private void ComputerOnUserReturned(IComputer sender)
        {
            
        }

        private void ComputerOnUserLeft(IComputer sender)
        {
            
        }

        private void WakeUp()
        {
            _userInterface.LogTime(this, TimeSpan.FromMinutes(9), TimeSpan.FromMinutes(9));
        }

        public void RemindMeInABit()
        {
            
        }

        public void SubmitWork(IList<WorkLog> work)
        {
            foreach (var item in work)
            {
                _storage.AddLog(item);
            }
        }
    }
}
