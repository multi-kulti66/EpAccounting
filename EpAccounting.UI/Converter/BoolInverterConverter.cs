// ///////////////////////////////////
// File: BoolInverterConverter.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;


    [ValueConversion(typeof(bool?), typeof(bool?))]
    public class BoolInverterConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }

            return value;
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