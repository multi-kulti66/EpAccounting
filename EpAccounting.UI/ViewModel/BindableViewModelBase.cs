// ///////////////////////////////////
// File: BindableViewModelBase.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using GalaSoft.MvvmLight;


    public abstract class BindableViewModelBase : ViewModelBase, IDataErrorInfo
    {
        #region Fields

        protected PropertyInfo[] _propertyInfos;

        #endregion



        #region Properties, Indexers

        public bool IsValid
        {
            get
            {
                foreach (PropertyInfo propertyinfo in this.GetPropertyInfos())
                {
                    string error = (this as IDataErrorInfo)[propertyinfo.Name];

                    if (!string.IsNullOrEmpty(error))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        #endregion



        #region IDataErrorInfo Members

        public string this[string propertyName]
        {
            get { return this.Validate(propertyName); }
        }

        public string Error
        {
            get { throw new NotSupportedException(); }
        }

        #endregion



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

        protected virtual bool SetProperty(Action action, Func<bool> equals, [CallerMemberName] string propertyName = null)
        {
            if (equals())
            {
                return false;
            }

            action();
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual string Validate(string propertyName)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateProperty(this.GetPropertyValue(propertyName),
                                                         new ValidationContext(this, null, null) {MemberName = propertyName},
                                                         results);

            return isValid ? string.Empty : results.First().ErrorMessage;
        }

        private object GetPropertyValue(string propertyName)
        {
            Type type = this.GetPropertyInfos().First(x => x.Name == propertyName).PropertyType;
            object value = this.GetPropertyInfos().First(x => x.Name == propertyName).GetValue(this);

            if (type == typeof(string) && value == null)
            {
                value = string.Empty;
            }

            return value;
        }

        private PropertyInfo[] GetPropertyInfos()
        {
            if (this._propertyInfos == null)
            {
                this._propertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
            }

            return this._propertyInfos;
        }
    }
}