// ///////////////////////////////////
// File: ValidDateAttribute.cs
// Last Change: 22.09.2017  20:51
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EpAccounting.UI.Properties;



    public class ValidDateAttribute : ValidationAttribute
    {
        #region Constructors / Destructor

        public ValidDateAttribute()
        {
            this.ErrorMessage = Resources.ValidationError_InvalidDate;
        }

        #endregion



        public override bool IsValid(object value)
        {
            string stringValue = value as string;
            DateTime dateTime;

            if (string.IsNullOrEmpty(stringValue) || DateTime.TryParse(stringValue, out dateTime))
            {
                return true;
            }

            return false;
        }
    }
}