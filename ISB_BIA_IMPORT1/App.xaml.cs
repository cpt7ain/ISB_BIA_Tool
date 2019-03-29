using System;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using ISB_BIA_IMPORT1.Services.Interfaces;
using Microsoft.WindowsAPICodePack.ApplicationServices;

namespace ISB_BIA_IMPORT1
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application//, ISingleInstanceApp
    {
        private const string Unique = "4fa4cae9-ddc9-4a5b-8c49-8f9e06521f69";
        private static App app = new App();
        static Mutex instanceMutex;
        /// <summary>
        /// Explizite Angabe der Main Methode, welche sonst standartmäßig durch den c# Compiler erstellt wird
        /// Verwendet um SingleInstance-Mechanismus zu implementieren
        /// Projekt -> Eigenschaften -> Startobjekt auf ~App Namen~ anstatt auf "Nicht Festgelegt"
        /// App.xaml -> Eigenschaften -> Buildvorgang auf "Page" statt "Application Definition"
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (instanceMutex = new Mutex(false, @"Global\ISB_BIA_Tool" + Environment.UserName + Unique))
            {
                var isOwned = false;
                try
                {
                    try
                    {
                        if (!instanceMutex.WaitOne(1000,false))
                        {
                            MessageBox.Show("ISB_BIA-Tool läuft bereits");
                            Application.Current.Shutdown();
                        }
                    }
                    catch (AbandonedMutexException)
                    {
                        isOwned = true;
                    }
                    app.RegisterAppRecovery();
                    app.InitializeComponent();
                    app.Run();
                }
                finally
                {
                    ApplicationRestartRecoveryManager.UnregisterApplicationRecovery();
                    if (isOwned) instanceMutex.ReleaseMutex();
                }
            }
        }

        #region Applikations Recovery Funktionen
        /// <summary>
        /// Registrieren der Anwendung für die Application-Recovery
        /// </summary>
        public void RegisterAppRecovery()
        {
            try
            {
                RecoveryData recData = new RecoveryData(UnlockRecovery, null);
                RecoverySettings recSett = new RecoverySettings(recData, 5000);
                ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(recSett);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Funktion, die bei Absturz der Anwendung ausgeführt wird 
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> Status der Recovery-Aktion </returns>
        private int UnlockRecovery(object parameter)
        {
            try
            {
                ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress();
                //Entsperrt alle vom User gesperrten Objekte falls die Anwendung crashed
                var myLock = SimpleIoc.Default.GetInstance<ILockService>();
                myLock.Unlock_AllObjectsForUserOnMachine();
                var myDia = SimpleIoc.Default.GetInstance<IDialogService>();
                myDia.ShowError("Die Anwendung wurde aus einem unbekannten Grund beendet");
                ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(true);
            }
            catch
            {
                ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(false);
            }
            return 0;
        }
        #endregion
    }
}
