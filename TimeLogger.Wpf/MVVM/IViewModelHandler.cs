using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.MVVM
{
    public interface IViewModelHandler<in TViewModel>
    {
        void SetViewModel(TViewModel model);
    }
}
