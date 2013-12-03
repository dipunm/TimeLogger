namespace MVVM.Extensions
{
    public interface IViewModelHandler<in TViewModel>
    {
        void SetViewModel(TViewModel model);
    }
}
