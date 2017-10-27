// ///////////////////////////////////
// File: BoolInverterConverter.cs
// Last Change: 26.10.2017  22:18
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