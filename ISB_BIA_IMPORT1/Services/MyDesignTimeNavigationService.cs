using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.ViewModel;

namespace ISB_BIA_IMPORT1.Services
{
    public class TargetViewModel: ViewModelBase
    {

    }

    public class SourceViewModel : ViewModelBase
    {

    }

    public class MyDesignTimeNavigationService : ObservableObject, IMyNavigationService
    {
        public ObservableCollection<ViewModelBase> _vMHistory;

        public MyDesignTimeNavigationService()
        {
            _vMHistory = new ObservableCollection<ViewModelBase>();
            _vMHistory.Add(new SourceViewModel());
        }

        public ObservableCollection<ViewModelBase> VMHistory
        {
            get => _vMHistory;
            set => Set(() => VMHistory, ref _vMHistory, value);
        }

        public void NavigateBack(bool refresh = false)
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
            ViewModelBase cv = new TargetViewModel();
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(int id, ProcAppMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = new TargetViewModel();
            Messenger.Default.Send<int>(id, mode);
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(ProcAppListMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = new TargetViewModel();
            Messenger.Default.Send<ProcAppListMode>(mode);
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(ISISAttributeMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = new TargetViewModel();
            Messenger.Default.Send<ISISAttributeMode>(mode);
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(int id, ISISAttributeMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = new TargetViewModel();
            Messenger.Default.Send<int>(id, mode);
            Messenger.Default.Send<ViewModelBase>(cv, MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }
    }
}
