using System.Windows.Input;

namespace Scrabblos.Core;

internal class RelayCommand : ICommand {
    private readonly Predicate<object> _canExecute;
    private readonly Action<object?> _execute;

    public RelayCommand(Action<object?> execute, Predicate<object> canExecute) {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) {
        return _canExecute(parameter);
    }

    public void Execute(object? parameter) {
        _execute(parameter);
    }

    public event EventHandler CanExecuteChanged {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}