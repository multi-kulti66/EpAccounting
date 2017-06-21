// ///////////////////////////////////
// File: BindableViewModelBase.cs
// Last Change: 13.03.2017  15:26
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Runtime.CompilerServices;
    using GalaSoft.MvvmLight;



    public abstract class BindableViewModelBase : ViewModelBase
    {
        #region Methods

        protected virtual bool SetProperty<T>(ref T storage, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, newValue))
            {
                return false;
            }

            storage = newValue;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual bool SetProperty<T>(T currentValue, T newValue, Action doSet, [CallerMemberName] string propertyName = null)
        {
            if (Equals(currentValue, newValue))
            {
                return false;
            }

            doSet.Invoke();
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        #endregion Methods
    }
}