// ///////////////////////////////////
// File: KindOfVat.cs
// Last Change: 10.04.2017  20:36
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Enum
{
    using System.ComponentModel;
    using EpAccounting.UI.Converter;



    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum KindOfVat
    {
        [Description("inkl.")]
        inkl_MwSt,

        [Description("zzgl.")]
        zzgl_MwSt,

        [Description("ohne")]
        without_MwSt
    }
}