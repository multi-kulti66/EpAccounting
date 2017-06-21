// ///////////////////////////////////
// File: BitmapToBitmapImageConverter.cs
// Last Change: 10.04.2017  20:12
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Converter
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;



    [ValueConversion(typeof(Bitmap), typeof(BitmapImage))]
    public class BitmapToBitmapImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Bitmap))
            {
                return Binding.DoNothing;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Bitmap bitmap = (Bitmap)value;
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}