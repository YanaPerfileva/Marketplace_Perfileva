using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace User.ViewModels
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Устанавливает значение свойства и уведомляет UI об изменениях.
        /// </summary>
        /// <typeparam name="T">Тип поля.</typeparam>
        /// <param name="storage">Ссылка на приватное поле (_searchText).</param>
        /// <param name="value">Новое значение.</param>
        /// <param name="propertyName">Имя свойства (подставляется автоматически).</param>
        /// <returns>True, если значение изменилось.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Генерирует событие изменения свойства.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
