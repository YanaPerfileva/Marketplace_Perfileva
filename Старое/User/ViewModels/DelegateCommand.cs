using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace User.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        // Это событие сообщает WPF, что нужно перепроверить, активна ли кнопка
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public DelegateCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Проверяет, можно ли нажать кнопку (например, CurrentPage < TotalPages)
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();

        // Выполняет основную логику (например, LoadProductsAsync)
        public void Execute(object? parameter) => _execute();

        // Принудительное обновление состояния кнопок
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
