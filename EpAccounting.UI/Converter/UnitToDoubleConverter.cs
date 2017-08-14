// ///////////////////////////////////
// File: UnitToDoubleConverter.cs
// Last Change: 06.08.2017  15:08
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;



    public class UnitToDoubleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return 0;
            }

            if (value is double)
            {
                return (double)value;
            }

            if (value is decimal)
            {
                return (decimal)value;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return 0;
            }

            string trimmedValue = value.ToString().TrimEnd('%', '€');

            if (targetType == typeof(double))
            {
                double result;

                if (double.TryParse(trimmedValue, out result))
                {
                    return result;
                }

                return value;
            }

            if (targetType == typeof(decimal))
            {
                decimal result;

                if (decimal.TryParse(trimmedValue, out result))
                {
                    return result;
                }

                return value;
            }

            return value;
        }

        #endregion
    }
}