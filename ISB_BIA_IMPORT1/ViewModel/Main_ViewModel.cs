using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Principal;
using System.Windows.Input;
using ISB_BIA_IMPORT1.View;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;
using System.Windows;

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
    /// Hier werden die momentan "angzuzeigenden" VM's eingesetzt <see cref="ViewModelCurrent"/> und Starteinstellungen / Überprüfungen vorgenommen
    /// </summary>
    public class Main_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private ViewModelBase _viewModelCurrent;
        private int _globalFontSize;
        private string _str_WindowTitle;
        #endregion

        /// <summary>
        /// Aktuelles Viewmodel, welches an die Usercontrol im <see cref="MainWindow"/> gebunden ist
        /// </summary>
        public ViewModelBase ViewModelCurrent
        {
            get => _viewModelCurrent;
            set => Set(()=> ViewModelCurrent, ref _viewModelCurrent, value);
        }

        /// <summary>
        /// Command um für Testzwecke Nutzergruppe manuell zu wählen
        /// </summary>
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
        public int GlobalFontSize
        {
            get => _globalFontSize;
            set => Set(() => GlobalFontSize, ref _globalFontSize, value);           
        }

        /// <summary>
        /// Titel des Fensters
        /// </summary>
        public string Str_WindowTitle
        {
            get => _str_WindowTitle;
            set => Set(() => Str_WindowTitle, ref _str_WindowTitle, value);
        }

        public Login_Model ConstructionUser
        {
            get => new Login_Model()
            {
                Username = Environment.UserName + " in Construction Mode",
                UserGroup = UserGroups.CISO,
                OE = "0",
                Givenname = "Tim",
                Surname = "Wolf"
            };
        }

        public Login_Model LocalUser
        {
            get => new Login_Model()
            {
                Username = "TestUser",
                UserGroup = UserGroups.CISO,
                OE = "1.1",
                Givenname = "Max",
                Surname = "Mustermann"
            };
        }

        public Login_Model InitialUser
        {
            get => new Login_Model()
            {
                Username = Environment.UserName,
                UserGroup = UserGroups.Normal_User,
                OE = "",
                Givenname = "",
                Surname = ""
            };
        }

        #region Services
        private readonly IDialogService _myDia;
        private readonly INavigationService _myNavi;
        private readonly ISharedResourceService _myShared;
        private readonly ILockService _myLock;
        #endregion 

        /// <summary>
        /// Haupt-VM Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myShared"></param>
        /// <param name="myLock"></param>
        public Main_ViewModel(IDialogService myDia, INavigationService myNavi, ISharedResourceService myShared, ILockService myLock)
        {
            Mouse.OverrideCursor = null;

            #region Services
            _myDia = myDia;
            _myNavi = myNavi;
            _myShared = myShared;
            _myLock = myLock;
            #endregion

            //Messenger Registrierung für den Empfang Viewmodel-bestimmender Nachrichten
            MessengerInstance.Register<NotificationMessage<ViewModelBase>>(this, MessageToken.ChangeCurrentVM, s =>
            {
                if (!(s.Sender is INavigationService)) return;
                if (s.Content != null) ViewModelCurrent = s.Content;
                else ShutDown();
            });
            //Messenger Registrierung für den Empfang Schriftgrößen-bestimmender Nachrichten
            MessengerInstance.Register<NotificationMessage<int>>(this, MessageToken.ChangeTextSize, s =>
            {
                if(!(s.Sender is Menu_ViewModel)) return;
                GlobalFontSize = s.Content;
            });
            //Messenger Registrierung für den Empfang Anwendungs-beendender Nachrichten
            MessengerInstance.Register<NotificationMessage<string>>(this, MessageToken.WindowClosingRequest, s =>
            {
                if (!(s.Sender is MainWindow)) return;
                ShutDown();
            });

            // Standard-Schriftgröße
            GlobalFontSize = 14;
            //Fenstertitel
            Str_WindowTitle = (_myShared.Conf_CurrentEnvironment == Current_Environment.Test) ? "ISB BIA Tool - Testumgebung" : "ISB BIA Tool";
            
            // Wenn Konstruktionsmodus
            if (_myShared.Conf_ConstructionMode)
            {
                _myShared.User = ConstructionUser;
                _myNavi.NavigateTo<Menu_ViewModel>();
            }
            else
            {
                // Wenn lokaler Test
                if (_myShared.Conf_CurrentEnvironment == Current_Environment.Local_Test)
                {
                    // TestUser
                    _myShared.User = LocalUser;
                }
                else
                {
                    // User für ISB Test/Prod Umgebung, wird des Weiteren durch Daten aus AD gefüllt
                    _myShared.User = InitialUser;
                }
                try
                {
                    //Prüfen der Datenbankverbindung
                    if (!_myLock.CheckDBConnection())
                    {
                        //Exit wenn kein Admin
                        if (!_myShared.Conf_Admin) Environment.Exit(0);
                    }

                    //Prüfen der Active-Directory -> EXIT wenn Fehler (Wenn erfolgreich: Usergroup ist gesetzt; entweder nach Standard oder nach Einstellung)
                    if (_myShared.Conf_CurrentEnvironment != Current_Environment.Local_Test)
                    {
                        if ((!GetUserFromAD() || !GetGroups()) && !_myShared.Conf_Admin) Environment.Exit(0);
                    }

                    //Direkt weiterleiten wenn nicht im Admin Modus
                    if (!_myShared.Conf_Admin)
                    {
                        _myNavi.NavigateTo<Menu_ViewModel>();
                    }
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("Es ist ein Fehler aufgetreten.\nDas Programm wird geschlossen.", ex);
                    if (!_myShared.Conf_Admin) Environment.Exit(0);
                }
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
                        ResultPropertyCollection res = result.Properties;
                        if (result.Properties.Contains("department"))
                        {
                            var department = res["department"];
                            if (department.Count > 0)
                            {
                                _myShared.User.OE = department[0].ToString();
                            }
                        }
                        if (result.Properties.Contains("givenname"))
                        {
                            var firstName = res["givenname"];
                            _myShared.User.Givenname = (firstName.Count > 0) ? firstName[0].ToString() : "n/a";
                        }
                        if (result.Properties.Contains("sn"))
                        {
                            var surname = res["sn"];
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
        /// <returns> User gefunden </returns>
        private bool GetGroups()
        {
            try
            {
                List<string> result = new List<string>();
                WindowsIdentity wi = new WindowsIdentity(_myShared.User.Username);
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

                //Gruppenabfrage von Gruppen mit vielen Rechten nach Gruppen mit wenigen (CISO->..->Normaler user) damit immer die Rolle mit den meisten Rechten angenommen wird falls mehrere zutreffen
                //Service fragt je nach PROD/TEST Einstellungen richtige Gruppe ab
                if (result.Contains(_myShared.Conf_AD_Group_CISO)) _myShared.User.UserGroup = UserGroups.CISO;
                else if (result.Contains(_myShared.Conf_AD_Group_Admin)) _myShared.User.UserGroup = UserGroups.Admin;
                else if (result.Contains(_myShared.Conf_AD_Group_SBA)) _myShared.User.UserGroup = UserGroups.SBA_User;
                else if (result.Contains(_myShared.Conf_AD_Group_Normal)) _myShared.User.UserGroup = UserGroups.Normal_User;
                else
                {
                    string env = (_myShared.Conf_CurrentEnvironment == Current_Environment.Prod) ? "[Prod]" : "[Test]";
                    _myDia.ShowWarning("Sie haben keine Berechtigungen für dieses Programm"+env+".\nUser:  " + _myShared.User.Username);
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                _myDia.ShowError("AD-Gruppe konnte nicht gefunden werden für User\n"+ _myShared.User.Username, ex);
                return false;
            }
        }

        /// <summary>
        /// Methode um das Programm zu beenden. 
        /// Entfernt alle Locks des Users und löscht die Registrierung von der Application Recovery
        /// </summary>
        private void ShutDown()
        {
            try
            {
                _myLock.Unlock_AllObjectsForUserOnMachine();
            }
            catch
            {
            }
            try
            {
                ApplicationRestartRecoveryManager.UnregisterApplicationRecovery();
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                _myDia.ShowError(ex.ToString());
                Application.Current.Shutdown();
            }
        }
    }
}
