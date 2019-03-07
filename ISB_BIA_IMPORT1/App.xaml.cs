using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using ISB_BIA_IMPORT1.Services;
using Microsoft.Shell;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using MS.WindowsAPICodePack.Internal;

namespace ISB_BIA_IMPORT1
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "4fa4cae9-ddc9-4a5b-8c49-8f9e06521f69";
        private static App app = new App();
        /// <summary>
        /// Explizite Angabe der Main Methode, welche sonst standartmäßig durch den c# Compiler erstellt wird
        /// Verwendet um SingleInstance-Mechanismus zu implementieren
        /// Projekt -> Eigenschaften -> Startobjekt auf ~App Namen~ anstatt auf "Nicht Festgelegt"
        /// App.xaml -> Eigenschaften -> Buildvorgang auf "Page" statt "Application Definition"
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                app.RegisterApplicationRecovery();
                app.InitializeComponent();
                app.Run();
                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Bringt App in den Vordergrund falls nicht erste Instanz gestartet
        /// </summary>
        /// <param name="args"> </param>
        /// <returns></returns>
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (app.MainWindow != null)
            {
                app.MainWindow.Visibility = Visibility.Visible;
                app.MainWindow.WindowState = WindowState.Normal;
                app.MainWindow.Activate();
                app.MainWindow.BringIntoView();
            }
            return true;
        }

        #region Applikations Recovery Funktionen
        /// <summary>
        /// Registrieren der Anwendung für die Application-Recovery
        /// </summary>
        public void RegisterApplicationRecovery()
        {
            if (!CoreHelpers.RunningOnVista)
            {
                return;
            }

            // register for Application Recovery
            RecoverySettings recoverySettings =
                new RecoverySettings(new RecoveryData(PerformRecovery, null),5000);
            ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(recoverySettings);
        }

        /// <summary>
        /// Funktion, die bei Absturz der Anwendung ausgeführt wird 
        /// </summary>
        /// <param name="parameter">Unused.</param>
        /// <returns>Unused.</returns>
        private int PerformRecovery(object parameter)
        {
            try
            {
                ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress();
                //Entsperrt alle vom User gesperrten Objekte falls die Anwendung crashed
                IMyDataService myData = SimpleIoc.Default.GetInstance<IMyDataService>();
                myData.UnlockAllObjectsForUserOnMachine();
                IMyDialogService myDialog = SimpleIoc.Default.GetInstance<IMyDialogService>();
                myDialog.ShowError("Die Anwendung wurde aus einem unbekannten Grund beendet");
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
