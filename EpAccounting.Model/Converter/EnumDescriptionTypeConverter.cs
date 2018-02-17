// ///////////////////////////////////
// File: EnumDescriptionTypeConverter.cs
// Last Change: 17.02.2018, 14:31
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.Model.Converter
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;


    public class EnumDescriptionTypeConverter : EnumConverter
    {
        #region Constructors

        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        { }

        #endregion



        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            if (value == null)
            {
                return string.Empty;
            }

            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi == null)
            {
                return string.Empty;
            }

            DescriptionAttribute[] attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
        }
    }
}