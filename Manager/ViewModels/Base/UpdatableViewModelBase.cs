using Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Manager.ViewModels.Base
{
    public abstract class UpdatableViewModelBase<T> : BindableBase
        where T : class, new()
    {
        protected T benchMark;
        protected bool _isRefreshing;
        protected bool hasChanged;

        public UpdatableViewModelBase(T model = null)
        {
            Refresh(model);
        }

        public void Refresh(T model)
        {
            // сохраняем оригинал
            benchMark = model;
            // обнуляем признак изменений
            hasChanged = false;

            if (model == null)
                return;

            _isRefreshing = true;

            _isRefreshing = false;
        }
        public abstract T ToModel();

        protected abstract void RefreshOverride(T model);


        protected override bool SetProperty<T1>(ref T1 storage, T1 value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);

            hasChanged = result;

            return result;
        }
    }
}
