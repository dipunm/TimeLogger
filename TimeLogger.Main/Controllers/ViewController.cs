using System;
using System.Windows;
using MVVM.Extensions;

namespace TimeLogger.Wpf.Domain.Controllers
{
    public abstract class ViewController<TViewModel>
    {
        private readonly TViewModel _viewModel;

        protected ViewController(TViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public TViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected void ApplyViewModel(FrameworkElement element)
        {
            if(element is IViewModelHandler<TViewModel>)
                ApplyViewModel((IViewModelHandler<TViewModel>)element);
            else
                element.Dispatcher.Invoke(new Action(() => element.DataContext = ViewModel));
        }

        protected void ApplyViewModel(IViewModelHandler<TViewModel> element)
        {
            element.SetViewModel(ViewModel);
        }


    }
}