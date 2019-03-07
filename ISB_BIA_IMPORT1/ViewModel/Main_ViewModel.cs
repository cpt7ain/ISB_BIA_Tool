using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Principal;
using System.Windows.Input;
using ISB_BIA_IMPORT1.View;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using MS.WindowsAPICodePack.Internal;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// Modi für die Steuerung der einzelnen Objekt-Ansichten (Neuanlage, Bearbeitung) von Prozessen und Anwendungen
    /// </summary>
    public enum ProcAppMode
    {
        /// <summary>
        /// Neuanlagemodus
        /// </summary>
        New,
        /// <summary>
        /// Bearbeitungsmodus
        /// </summary>
        Change
    }
    /// <summary>
    /// Modi für die Steuerung der Objekt-Listen-Ansichten (Bearbeitung, Löschung) von Prozessen und Anwendungen
    /// </summary>
    public enum ProcAppListMode
    {
        /// <summary>
        /// Bearbeitungsmodus
        /// </summary>
        Change,
        /// <summary>
        /// Löschungsmodus
        /// </summary>
        Delete
    }
    /// <summary>
    /// Modi für die Steuerung der Objekt-(/Listen)-Ansichten (Bearbeitung, Betrachten) von Informationssegmenten und Attributen
    /// </summary>
    public enum ISISAttributeMode
    {
        /// <summary>
        /// Ansichtsmodus
        /// </summary>
        View,
        /// <summary>
        /// Bearbeitungsmodus
        /// </summary>
        Edit
    }
    /// <summary>
    /// Modi für die Einstufungen der Schutzziele (Integer-Werte der Enumeration 1:1 auf Schutzziele abgebildet)
    /// </summary>
    public enum SZ_Values
    {
        /// <summary>
        /// Schutzziel nicht relevant
        /// </summary>
        None,
        /// <summary>
        /// niedrige Schutzzieleinstufung
        /// </summary>
        Low,
        /// <summary>
        /// mittlere Schutzzieleinstufung
        /// </summary>
        Medium,
        /// <summary>
        /// hohe Schutzzieleinstufung
        /// </summary>
        High,
        /// <summary>
        /// sehr hohe Schutzzieleinstufung
        /// </summary>
        Veryhigh
    }
    /// <summary>
    /// Enum für die Nachrichten Tokens des MVVM-Light Messengers
    /// </summary>
    public enum MessageToken
    {
        ChangeCurrentVM,
        ISAttributValidationError,
        WindowClosingRequest,
        RefreshData,
        ChangeTextSize,
    }
    /// <summary>
    /// Haupt-VM, dessen Datacontext an das Fenster der Anwendung <see cref="MainWindow"/> gebunden ist. 
    /// Hier werden die momentan "angzuzeigenden" VM's eingesetzt <see cref="CurrentViewModel"/> und Starteinstellungen / Überprüfungen vorgenommen
    /// </summary>
    public class Main_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private ViewModelBase _currentViewModel;
        private int _myFontSize;
        private string _windowTitle;
        #endregion

        /// <summary>
        /// Aktuelles Viewmodel, welches an die Usercontrol im <see cref="MainWindow"/> gebunden ist
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => Set(()=> CurrentViewModel, ref _currentViewModel, value);
        }

        public MyRelayCommand<int> AdminSelectGroupCommand
        {
            get => new MyRelayCommand<int>((k) =>
                    {
                        _myShared.User.UserGroup = (UserGroups)k;
                        _myNavi.NavigateTo<Menu_ViewModel>();
                    });
        }

        /// <summary>
        /// Property für die Standardschriftgröße im <see cref="MainWindow"/>, von der alle Views erben
        /// </summary>
        public int MyFontSize
        {
            get => _myFontSize;
            set => Set(() => MyFontSize, ref _myFontSize, value);           
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set => Set(() => WindowTitle, ref _windowTitle, value);
        }

        #region Services
        private readonly IMyDialogService _myDia;
        private readonly IMyNavigationService _myNavi;
        private readonly IMySharedResourceService _myShared;
        private readonly IMyDataService _myData;
        #endregion 

        /// <summary>
        /// Haupt-VM Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="mySharedResourceService"></param>
        /// <param name="myDataService"></param>
        public Main_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMySharedResourceService mySharedResourceService, IMyDataService myDataService)
        {
            Mouse.OverrideCursor = null;

            #region Services
            _myDia = myDialogService;
            _myNavi = myNavigationService;
            _myShared = mySharedResourceService;
            _myData = myDataService;
            #endregion

            //Messenger Registrierung für den Empfang Viewmodel-bestimmender Nachrichten
            MessengerInstance.Register<NotificationMessage<ViewModelBase>>(this, MessageToken.ChangeCurrentVM, s =>
            {
                if (!(s.Sender is IMyNavigationService)) return;
                if (s.Content != null) CurrentViewModel = s.Content;
                else ShutDown();
            });
            //Messenger Registrierung für den Empfang Schriftgrößen-bestimmender Nachrichten
            MessengerInstance.Register<NotificationMessage<int>>(this, MessageToken.ChangeTextSize, s =>
            {
                if(!(s.Sender is Menu_ViewModel)) return;
                MyFontSize = s.Content;
            });
            //Messenger Registrierung für den Empfang Anwendungs-beendender Nachrichten
            MessengerInstance.Register<NotificationMessage<string>>(this, MessageToken.WindowClosingRequest, s =>
            {
                if (!(s.Sender is MainWindow)) return;
                ShutDown();
            });

            // Standard-Schriftgröße
            MyFontSize = 14;
            //Fenstertitel
            WindowTitle = (_myShared.Current_Environment == Current_Environment.Test) ? "ISB BIA Tool - Testumgebung" : "ISB BIA Tool";

            if (_myShared.ConstructionMode)
            {
                _myShared.User = new Login_Model()
                {
                    Username = Environment.UserName +" in Construction Mode",
                    UserGroup = UserGroups.CISO,
                    OE = "0",
                    Givenname = "Tim",
                    Surname = "Wolf"
                };
                _myNavi.NavigateTo<Menu_ViewModel>();
            }
            else
                try
                {
                    // test-user
                    if (_myShared.Current_Environment == Current_Environment.Local_Test)
                        _myShared.User = new Login_Model()
                        {
                            Username = Environment.UserName,
                            UserGroup = UserGroups.CISO,
                            OE = "1.1",
                            Givenname = "Tim",
                            Surname = "Wolf"
                        };
                    else
                        _myShared.User = new Login_Model()
                        {
                            Username = Environment.UserName,
                            UserGroup = UserGroups.Normal_User,
                            OE = "0",
                            Givenname = "",
                            Surname = ""
                        };

                    //Prüfen der Datenbankverbindung
                    if (!_myData.CheckDBConnection())
                    {
                        if (!_myShared.Admin) Environment.Exit(0);
                    }
                    //Prüfen der Active-Directory -> EXIT wenn Fehler (Wenn erfolgreich: Usergroup ist gesetzt; entweder nach Standard oder nach Einstellung)
                    if (_myShared.Current_Environment != Current_Environment.Local_Test)
                    {
                        bool userExists = GetUserFromAD();
                        if (!userExists && !_myShared.Admin) Environment.Exit(0);
                        bool groupMatchesUser = GetGroups(_myShared.User.Username);
                        if (!groupMatchesUser && !_myShared.Admin) Environment.Exit(0);
                    }

                    //Alle möglicherweise gesperrten Objekten aus unsauber beendeten früheren Sessions entfernen
                    _myData.UnlockAllObjectsForUserOnMachine();

                    //Direkt weiterleiten wenn nicht im Admin Modus
                    if (!_myShared.Admin)
                    {
                        _myNavi.NavigateTo<Menu_ViewModel>();
                    }
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("Es ist ein Fehler aufgetreten.\nDas Programm wird geschlossen.", ex);
                    if (!_myShared.Admin) Environment.Exit(0);
                }
        }

        /// <summary>
        /// Userdaten aus AD abrufen
        /// </summary>
        /// <returns> true wenn User gefunden und Verbindung zu AD erfolgreich </returns>
        private bool GetUserFromAD()
        {
            try
            {
                using (DirectorySearcher searcher = new DirectorySearcher(new DirectoryEntry(string.Empty)))
                {
                    searcher.Filter = "(&(objectClass=user)(objectCategory=Person)(sAMAccountName=" + _myShared.User.Username + "))";
                    SearchResult result = searcher.FindOne();
                    if(result != null)
                    {
                        if (result.Properties.Contains("department"))
                        {
                            var department = result.Properties["department"];
                            if (department.Count > 0)
                            {
                                _myShared.User.OE = department[0].ToString();
                            }
                        }
                        if (result.Properties.Contains("givenname"))
                        {
                            var firstName = result.Properties["givenname"];
                            _myShared.User.Givenname = (firstName.Count > 0) ? firstName[0].ToString() : "n/a";
                        }
                        if (result.Properties.Contains("sn"))
                        {
                            var surname = result.Properties["sn"];
                            _myShared.User.Surname = (surname.Count > 0) ? surname[0].ToString() : "n/a";
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                //Wenn kein test und Verbindung zu AD nicht möglich dann Schließen
                _myDia.ShowError("Es konnte keine Verbindung zur Active Directory hergestellt werden.\nDie Anwendung wird geschlossen.", ex);
                return false;
            }
        }

        /// <summary>
        /// Gruppenzugehörigkeiten eines Users auswerten (zuteilen einer Rolle in der Anwendung)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns> User gefunden </returns>
        private bool GetGroups(string userName)
        {
            try
            {
                List<string> result = new List<string>();
                WindowsIdentity wi = new WindowsIdentity(userName);
                if (wi.Groups != null)
                    foreach (IdentityReference group in wi.Groups)
                    {
                        // Übsersetzen der SID in den Gruppennnamen
                        try
                        {
                            result.Add(@group.Translate(typeof(NTAccount)).ToString());
                        }
                        catch
                        {
                            //Fehler abfangen falls SID nicht mehr verfügbar
                        }
                    }

                //Gruppenabfrage für Produktivumgebung
                if(_myShared.Current_Environment == Current_Environment.Prod)
                {
                    //Gruppenabfrage von Gruppen mit vielen Rechten nach Gruppen mit wenigen (CISO->..->Normaler user) damit immer die Rolle mit den meisten Rechten angenommen wird falls mehrere zutreffen
                    if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_CISO_PROD"])) _myShared.User.UserGroup = UserGroups.CISO;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_Admin_PROD"])) _myShared.User.UserGroup = UserGroups.Admin;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_SBA_PROD"])) _myShared.User.UserGroup = UserGroups.SBA_User;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_Normal_PROD"])) _myShared.User.UserGroup = UserGroups.Normal_User;
                    else
                    {
                        _myDia.ShowWarning("Sie haben keine Berechtigungen für dieses Programm[Prod].\nUser:  " + userName);
                        return false;
                    }
                    return true;
                }
                //Gruppenabfrage für Testumgebung
                else if (_myShared.Current_Environment == Current_Environment.Test)
                {
                    if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_CISO_TEST"])) _myShared.User.UserGroup = UserGroups.CISO;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_Admin_TEST"])) _myShared.User.UserGroup = UserGroups.Admin;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_SBA_TEST"])) _myShared.User.UserGroup = UserGroups.SBA_User;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_Normal_TEST"])) _myShared.User.UserGroup = UserGroups.Normal_User;
                    else
                    {
                        _myDia.ShowWarning("Sie haben keine Berechtigungen für dieses Programm[Test].\nUser:  " + userName);
                        return false;
                    }
                    return true;
                }
                else
                {
                    _myDia.ShowWarning("Sie haben keine Berechtigungen für dieses Programm.\nUser:  " + userName+"\n[CurrentEnvironmentError]");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("AD-Gruppe konnte nicht gefunden werden für User\n"+userName, ex);
                return false;
            }
        }

        /// <summary>
        /// Methode um das Programm zu beenden. 
        /// Entfernt alle Locks des Users und löscht die Registrierung von der Application Recovery
        /// </summary>
        private void ShutDown()
        {
            _myData.UnlockAllObjectsForUserOnMachine();
            UnregisterApplicationRecovery();
            Environment.Exit(0);
        }

        /// <summary>
        /// Methode um die Registrierung von der Application Recovery zu löschen
        /// </summary>
        private void UnregisterApplicationRecovery()
        {
            if (!CoreHelpers.RunningOnVista)
            {
                return;
            }
            ApplicationRestartRecoveryManager.UnregisterApplicationRecovery();
        }
    }
}
