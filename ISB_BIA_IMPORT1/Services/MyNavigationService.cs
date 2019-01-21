using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyNavigationService : ObservableObject, IMyNavigationService
    {
        public ObservableCollection<ViewModelBase> _vMHistory;

        public MyNavigationService()
        {
            _vMHistory = new ObservableCollection<ViewModelBase>();
        }

        public ObservableCollection<ViewModelBase> VMHistory
        {
            get => _vMHistory;
            set => Set(()=>VMHistory, ref _vMHistory, value);
        }

        public void NavigateBack()
        {
            VMHistory.RemoveAt(0);
            Messenger.Default.Send<ViewModelBase>(VMHistory.FirstOrDefault(), MessageToken.ChangeCurrentVM);
        }

        public void NavigateBack(bool refresh)
        {
            VMHistory.RemoveAt(0);
            if (refresh)
            {
                Messenger.Default.Send<string>("Refresh", MessageToken.RefreshData);
            }
            Messenger.Default.Send<ViewModelBase>(VMHistory.FirstOrDefault(), MessageToken.ChangeCurrentVM);
        }

        public void NavigateTo<T>() where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(int id, ProcAppMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send<int>(id, mode);
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(ProcAppListMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send<ProcAppListMode>(mode);
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(ISISAttributeMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send<ISISAttributeMode>(mode);
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(int id, ISISAttributeMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send<int>(id, mode);
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

    }
}
