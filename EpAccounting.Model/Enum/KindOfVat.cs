// ///////////////////////////////////
// File: KindOfVat.cs
// Last Change: 05.09.2017  19:30
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model.Enum
{
    using System.ComponentModel;
    using Converter;


    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum KindOfVat
    {
        [Description("inkl.")]
        InklMwSt,

        [Description("zzgl.")]
        ZzglMwSt,

        [Description("ohne")]
        WithoutMwSt
    }
}