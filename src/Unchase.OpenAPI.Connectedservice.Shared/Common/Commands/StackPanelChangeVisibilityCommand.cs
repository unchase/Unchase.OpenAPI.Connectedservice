using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Unchase.OpenAPI.ConnectedService.Common.Commands
{
    class StackPanelChangeVisibilityCommand :
        ICommand
    {

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            (parameter as StackPanel)?.ChangeStackPanelVisibility();
        }

        event EventHandler ICommand.CanExecuteChanged { add { } remove { } }
    }
}