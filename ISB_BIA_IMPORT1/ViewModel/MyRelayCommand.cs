using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows.Input;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// Command-Klasse, erbt von <see cref="RelayCommand"/>. Ändert Cursor während der Execute-Funktion zu Cursor.Wait
    /// </summary>
    public class MyRelayCommand: RelayCommand
    {
        /// <summary>
        /// Erbt von <see cref="RelayCommand"/>
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="keepTargetAlive"></param>
        public MyRelayCommand(Action execute, bool keepTargetAlive = false) : base(execute, keepTargetAlive)
        {
        }

        /// <summary>
        /// Erbt von <see cref="RelayCommand"/>
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <param name="keepTargetAlive"></param>
        public MyRelayCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        {
        }

        /// <summary>
        /// Überschreibt <see cref="RelayCommand.Execute(object)"/>, Warte-Animation des Cursors
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            base.Execute(parameter);
            Mouse.OverrideCursor = null;
        }
    }

    /// <summary>
    /// Erbt von <see cref="RelayCommand{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyRelayCommand<T> : RelayCommand<T>
    {
        /// <summary>
        /// Erbt von <see cref="RelayCommand{T}"/>
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="keepTargetAlive"></param>
        public MyRelayCommand(Action<T> execute, bool keepTargetAlive = false) : base(execute, keepTargetAlive)
        {
        }

        /// <summary>
        /// Erbt von <see cref="RelayCommand{T}"/>
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <param name="keepTargetAlive"></param>
        public MyRelayCommand(Action<T> execute, Func<T, bool> canExecute, bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        {
        }

        /// <summary>
        /// Überschreibt <see cref="RelayCommand{T}.Execute(object)"/>, Warte-Animation des Cursors
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            base.Execute(parameter);
            Mouse.OverrideCursor = null;
        }
    }
}
