using Mvvm;
using System.Runtime.CompilerServices;
using MaterialDesignThemes.Wpf;

namespace Manager.ViewModels.Base
{
    public abstract class UpdatableViewModelBase<T> : BindableBase
        where T : class, new()
    {
        /// <summary>
        /// Сохранённые значения после последнего обновления
        /// </summary>
        protected T BenchMark;
        
        protected bool IsRefreshing;
        protected bool HasChanged;

        public UpdatableViewModelBase(T model = null)
        {
            Refresh(model);
        }

        public void Refresh(T model)
        {
            // обнуляем признак изменений
            HasChanged = false;

            if (model == null)
                return;

            IsRefreshing = true;
            
            RefreshOverride(model);

            IsRefreshing = false;
            
            // сохраняем копию модели
            BenchMark = model;
        }
        public abstract T ToModel();

        protected abstract void RefreshOverride(T model);

        protected override bool SetProperty<T1>(ref T1 storage, T1 value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);

            // Если флажок опущен, не в режиме обновления и что-то изменили
            if (!HasChanged && !IsRefreshing && result)
            {
                HasChanged = true;
            }

            return result;
        }

        public bool TheSame(T model)
        {
            return Equals(model, ToModel());
        }
    }
}
