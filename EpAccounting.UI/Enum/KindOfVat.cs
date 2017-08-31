// ///////////////////////////////////
// File: KindOfVat.cs
// Last Change: 16.08.2017  18:33
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