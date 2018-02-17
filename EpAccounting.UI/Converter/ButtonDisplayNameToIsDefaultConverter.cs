// ///////////////////////////////////
// File: ButtonDisplayNameToIsDefaultConverter.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Properties;


    [ValueConversion(typeof(string), typeof(bool))]
    public class ButtonDisplayNameToIsDefaultConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;

            if (string.IsNullOrEmpty(stringValue) || stringValue != Resources.Command_DisplayName_SaveOrUpdate)
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }

            return value;
        }

        #endregion
    }
}