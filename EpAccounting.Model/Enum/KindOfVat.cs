// ///////////////////////////////////
// File: KindOfVat.cs
// Last Change: 05.09.2017  19:30
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model.Enum
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