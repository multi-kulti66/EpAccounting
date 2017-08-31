// ///////////////////////////////////
// File: EnumDescriptionTypeConverter.cs
// Last Change: 16.08.2017  18:32
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Converter
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;



    public class EnumDescriptionTypeConverter : EnumConverter
    {
        #region Constructors / Destructor

        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        { }

        #endregion



        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                    }
                }

                return string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}