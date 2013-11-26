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
}
