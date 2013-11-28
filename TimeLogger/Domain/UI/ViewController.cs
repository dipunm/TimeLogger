using System.Windows;
using TimeLogger.MVVM;

namespace TimeLogger.Domain.UI
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
                element.DataContext = ViewModel;
        }

        protected void ApplyViewModel(IViewModelHandler<TViewModel> element)
        {
            element.SetViewModel(ViewModel);
        }


    }
}