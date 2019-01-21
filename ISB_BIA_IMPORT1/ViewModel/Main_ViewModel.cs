using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using ISB_BIA_IMPORT1.View;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using System.Data.Sql;

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
        none,
        /// <summary>
        /// niedrige Schutzzieleinstufung
        /// </summary>
        low,
        /// <summary>
        /// mittlere Schutzzieleinstufung
        /// </summary>
        medium,
        /// <summary>
        /// hohe Schutzzieleinstufung
        /// </summary>
        high,
        /// <summary>
        /// sehr hohe Schutzzieleinstufung
        /// </summary>
        veryhigh
    }
    public enum MessageToken
    {
        ChangeCurrentVM,
        ChangedToCriticalNotification,
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
                        myShared.User.UserGroup = (UserGroups)k;
                        myNavi.NavigateTo<Menu_ViewModel>();
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
        IMyDialogService myDia;
        IMyNavigationService myNavi;
        IMySharedResourceService myShared;
        IMyDataService myData;
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
            #region Services
            myDia = myDialogService;
            myNavi = myNavigationService;
            myShared = mySharedResourceService;
            myData = myDataService;
            #endregion

            //Messenger Registrierung für den Empfang Viewmodel-bestimmender Nachrichten
            Messenger.Default.Register<ViewModelBase>(this, MessageToken.ChangeCurrentVM, s => { if (s != null) CurrentViewModel = s; else ShutDown(); });
            //Messenger Registrierung für den Empfang Schriftgrößen-bestimmender Nachrichten
            Messenger.Default.Register<int>(this, MessageToken.ChangeTextSize, s => { MyFontSize = s; });
            //Messenger Registrierung für den Empfang Anwendungs-beendender Nachrichten
            Messenger.Default.Register<string>(this, MessageToken.WindowClosingRequest, s => { ShutDown(); });

            // Standard-Schriftgröße
            MyFontSize = 14;
            //Fenstertitel
            WindowTitle = (myShared.Current_Environment == Current_Environment.Test) ? "ISB BIA Tool - Testumgebung" : "ISB BIA Tool";

            if (myShared.ConstructionMode)
            {
                myShared.User = new Login_Model()
                {
                    Username = Environment.UserName +" in Construction Mode",
                    UserGroup = UserGroups.CISO,
                    OE = "0",
                    Givenname = "Tim",
                    Surname = "Wolf"
                };
                myNavi.NavigateTo<Menu_ViewModel>();
            }
            else
                try
                {
                    // test-user
                    if (myShared.Current_Environment == Current_Environment.Local_Test)
                        myShared.User = new Login_Model()
                        {
                            Username = Environment.UserName,
                            UserGroup = UserGroups.CISO,
                            OE = "1.1",
                            Givenname = "Tim",
                            Surname = "Wolf"
                        };
                    else
                        myShared.User = new Login_Model()
                        {
                            Username = Environment.UserName,
                            UserGroup = UserGroups.Normal_User,
                            OE = "0",
                            Givenname = "",
                            Surname = ""
                        };

                    //Prüfen der Datenbankverbindung
                    if (!myData.CheckDBConnection())
                    {
                        if (!myShared.Admin) Environment.Exit(0);
                    }
                    //Prüfen der Active-Directory -> EXIT wenn Fehler (Wenn erfolgreich: Usergroup ist gesetzt; entweder nach Standard oder nach Einstellung)
                    if (myShared.Current_Environment != Current_Environment.Local_Test)
                    {
                        GetUserFromAD();
                        GetGroups(myShared.User.Username);
                    }
                    //Prüfen, ob alle Tabellen in Datenbank vorhanden
                    CheckDataModel();
                    //Test für alle Usergruppen

                    //Direkt weiterleiten wenn nicht im Admin Modus
                    if (!myShared.Admin)
                    {
                        myNavi.NavigateTo<Menu_ViewModel>();
                    }
                }
                catch (Exception ex)
                {
                    myDia.ShowError("Es ist ein Fehler aufgetreten.\nDas Programm wird geschlossen.", ex);
                    if (!myShared.Admin) Environment.Exit(0);
                }
        }



        /// <summary>
        /// Userdaten aus AD abrufen
        /// </summary>
        private void GetUserFromAD()
        {
            try
            {
                using (DirectorySearcher searcher = new DirectorySearcher(new DirectoryEntry(string.Empty)))
                {
                    searcher.Filter = "(&(objectClass=user)(objectCategory=Person)(sAMAccountName=" + myShared.User.Username + "))";
                    SearchResult result = searcher.FindOne();
                    if (result.Properties.Contains("department"))
                    {
                        var department = result.Properties["department"];
                        if (department.Count > 0)
                        {
                            myShared.User.OE = department[0].ToString();
                        }
                    }
                    if (result.Properties.Contains("givenname"))
                    {
                        var firstName = result.Properties["givenname"];
                        myShared.User.Givenname = (firstName.Count > 0) ? firstName[0].ToString() : "n/a";
                    }
                    if (result.Properties.Contains("sn"))
                    {
                        var surname = result.Properties["sn"];
                        myShared.User.Surname = (surname.Count > 0) ? surname[0].ToString() : "n/a";
                    }
                }
            }
            catch (Exception ex)
            {
                //Wenn kein test und Verbindung zu AD nicht möglich dann Schließen
                myDia.ShowError("Es konnte keine Verbindung zur Active Directory hergestellt werden.\nDie Anwendung wird geschlossen.", ex);
                if (!myShared.Admin) Environment.Exit(0);
            }
        }

        /// <summary>
        /// Gruppenzugehörigkeiten eines Users auswerten (zuteilen einer Rolle in der Anwendung)
        /// </summary>
        /// <param name="userName"></param>
        private void GetGroups(string userName)
        {
            try
            {
                List<string> result = new List<string>();
                WindowsIdentity wi = new WindowsIdentity(userName);

                foreach (IdentityReference group in wi.Groups)
                {
                    // Übsersetzen der SID in den Gruppennnamen
                    //Fehler abfangen falls SID nicht mehr verfügbar
                    try
                    {
                        result.Add(group.Translate(typeof(NTAccount)).ToString());
                    }
                    catch { }
                }
                //Gruppenabfrage für Produktivumgebung
                if(myShared.Current_Environment == Current_Environment.Prod)
                {
                    //Gruppenabfrage von Gruppen mit vielen Rechten nach Gruppen mit wenigen (CISO->..->Normaler user) damit immer die Rolle mit den meisten Rechten angenommen wird falls mehrere zutreffen
                    if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_CISO_PROD"])) myShared.User.UserGroup = UserGroups.CISO;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_Admin_PROD"])) myShared.User.UserGroup = UserGroups.Admin;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_SBA_PROD"])) myShared.User.UserGroup = UserGroups.SBA_User;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_Normal_PROD"])) myShared.User.UserGroup = UserGroups.Normal_User;
                    else
                    {
                        myDia.ShowWarning("Sie haben keine Berechtigungen für dieses Programm.\nUser:  " + userName);
                        if (!myShared.Admin) Environment.Exit(0);
                    }
                }
                //Gruppenabfrage für Testumgebung
                else if (myShared.Current_Environment == Current_Environment.Test)
                {
                    if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_CISO_TEST"])) myShared.User.UserGroup = UserGroups.CISO;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_Admin_TEST"])) myShared.User.UserGroup = UserGroups.Admin;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_SBA_TEST"])) myShared.User.UserGroup = UserGroups.SBA_User;
                    else if (result.Contains(@System.Configuration.ConfigurationManager.AppSettings["AD_Group_Normal_TEST"])) myShared.User.UserGroup = UserGroups.Normal_User;
                    else
                    {
                        myDia.ShowWarning("Sie haben keine Berechtigungen für dieses Programm.\nUser:  " + userName);
                        if (!myShared.Admin) Environment.Exit(0);
                    }
                }
                else
                {
                    myDia.ShowWarning("Sie haben keine Berechtigungen für dieses Programm.\nUser:  " + userName);
                    if (!myShared.Admin) Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                myDia.ShowError("AD-Gruppe konnte nicht aufgelöst werden für User\n"+userName, ex);
                if (!myShared.Admin) Environment.Exit(0);
            }
        }

        /// <summary>
        /// Prüfen ob das Datenmodell Fehler aufweist
        /// </summary>
        private void CheckDataModel()
        {
            //Prüfen, ob alle Tabellen in Datenbank vorhanden
            try
            {
                string sql1 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_Applikationen + "]')";
                string sql2 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_Delta + "]')";
                string sql3 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_IS + "]')";
                string sql4 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_IS_Attribute + "]')";
                string sql5 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_Log + "]')";
                string sql6 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_OEs + "]')";
                string sql7 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_Prozesse + "]')";
                string sql8 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_Proz_App + "]')";
                string sql9 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_Settings + "]')";
                string sql10 = "SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + myShared.Tbl_Lock + "]')";

                //Abfrage auf Existenz jeder einzelnen Tabelle
                using (MyLinqContextDataContext db = new MyLinqContextDataContext(myShared.ConnectionString))
                {
                    int res1 = db.ExecuteQuery<int>(sql1).Single();
                    int res2 = db.ExecuteQuery<int>(sql2).Single();
                    int res3 = db.ExecuteQuery<int>(sql3).Single();
                    int res4 = db.ExecuteQuery<int>(sql4).Single();
                    int res5 = db.ExecuteQuery<int>(sql5).Single();
                    int res6 = db.ExecuteQuery<int>(sql6).Single();
                    int res7 = db.ExecuteQuery<int>(sql7).Single();
                    int res8 = db.ExecuteQuery<int>(sql8).Single();
                    int res9 = db.ExecuteQuery<int>(sql9).Single();
                    int res10 = db.ExecuteQuery<int>(sql10).Single();

                    //Prüfen ob alle Tabellen vorhanden
                    if (res1 == 1 && res2 == 1 && res3 == 1 && res4 == 1 && res5 == 1 && res6 == 1 && res7 == 1 && res8 == 1 && res9 == 1 && res10 == 1)
                    {
                        //Wenn Tabelle existiert werden bei Start der Anwendung werden alle möglichen Locks, die durch diesen User in vergangenen Sessions verursacht wurden und Fehlerhafterweise nicht entfernt wurden, gelöscht
                        myData.UnlockAllObjectsForUser();
                    }
                    else
                    {
                        myDia.ShowError("Fehler im Datenmodell.\nBitte kontaktieren Sie ggf. die IT.\nDie Anwendung wird nun geschlossen.\n");
                        if (!myShared.Admin) Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                myDia.ShowError("Fehler im Datenmodell.\nBitte kontaktieren Sie ggf. die IT.\nDie Anwendung wird nun geschlossen.\n", ex);
                if (!myShared.Admin) Environment.Exit(0);
            }
        }

        /// <summary>
        /// Methode um das Programm zu beenden. 
        /// Entfernt alle Locks des Users.
        /// </summary>
        private void ShutDown()
        {
            myData.UnlockAllObjectsForUser();
            Environment.Exit(0);
        }
       
        /*
        Messenger.Default.Register<NotificationMessage>(this, NotifyUser);
        Messenger.Default.Register<NotificationMessageAction<MessageBoxResult>>(this, (m) =>
            {
                if ((string)(m.Target) == "cancel")
                {
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            m.Execute(MessageBox.Show(m.Notification, "Bestätigen", btnMessageBox));
        }
        });

        }

    private void NotifyUser(NotificationMessage msg)
    {
        MessageBox.Show(msg.Notification);
    }
    */
    }
}
