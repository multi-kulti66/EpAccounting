// ///////////////////////////////////
// File: DoubleToCurrencyConverter.cs
// Last Change: 20.03.2017  21:16
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;



    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToCurrencyConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
            {
                return Binding.DoNothing;
            }

            return string.Format("{0:C}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}