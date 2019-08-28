using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using ISB_BIA_IMPORT1.Services.Interfaces;
using System;

namespace ISB_BIA_IMPORT1.Services
{
    public class NavigationService : ObservableObject, INavigationService
    {
        public ObservableCollection<ViewModelBase> VMHistory { get; set; }

        public NavigationService()
        {
            VMHistory = new ObservableCollection<ViewModelBase>();
        }

        public void NavigateBack(bool refresh = false)
        {
            VMHistory.RemoveAt(0);
            GC.Collect();
            if (refresh)
            {
                Messenger.Default.Send(new NotificationMessage<string>(this,"Refresh",null), MessageToken.RefreshData);
            }
            Messenger.Default.Send(new NotificationMessage<ViewModelBase>(this, VMHistory.FirstOrDefault(),null), MessageToken.ChangeCurrentVM);
        }

        public void NavigateTo<T>() where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send(new NotificationMessage<ViewModelBase>(this,cv,null), MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(int id, ProcAppMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send(new NotificationMessage<int>(this,id,null), mode);
            Messenger.Default.Send(new NotificationMessage<ViewModelBase>(this, cv, null), MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(ProcAppListMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send(new NotificationMessage<ProcAppListMode>(this,mode,null));
            Messenger.Default.Send(new NotificationMessage<ViewModelBase>(this,cv,null), MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(ISISAttributeMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send(new NotificationMessage<ISISAttributeMode>(this, mode, null));
            Messenger.Default.Send(new NotificationMessage<ViewModelBase>(this, cv, null), MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

        public void NavigateTo<T>(int id, ISISAttributeMode mode) where T : ViewModelBase
        {
            ViewModelBase cv = ServiceLocator.Current.GetInstance<T>();
            Messenger.Default.Send(new NotificationMessage<int>(this, id, null), mode);
            Messenger.Default.Send(new NotificationMessage<ViewModelBase>(this, cv, null), MessageToken.ChangeCurrentVM);
            VMHistory.Insert(0, cv);
        }

    }
}
