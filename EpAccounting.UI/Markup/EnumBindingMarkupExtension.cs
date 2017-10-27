// ///////////////////////////////////
// File: EnumBindingMarkupExtension.cs
// Last Change: 22.10.2017  16:05
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Markup
{
    using System;
    using System.Windows.Markup;



    public class EnumBindingMarkupExtension : MarkupExtension
    {
        #region Fields

        private Type _enumType;

        #endregion



        #region Constructors / Destructor

        public EnumBindingMarkupExtension()
        { }

        public EnumBindingMarkupExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        #endregion



        #region Properties

        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (this._enumType != value)
                {
                    if (value != null)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                        if (!enumType.IsEnum)
                        {
                            throw new ArgumentException("Type must be for an Enum.");
                        }
                    }

                    this._enumType = value;
                }
            }
        }

        #endregion



        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this._enumType == null)
            {
                throw new InvalidOperationException("The EnumType must be specified.");
            }

            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (this._enumType == actualEnumType)
            {
                return enumValues;
            }

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}