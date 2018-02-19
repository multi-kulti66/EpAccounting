// ///////////////////////////////////
// File: KindOfVat.cs
// Last Change: 19.02.2018, 21:01
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.Model.Enum
{
    using System.ComponentModel;
    using Converter;


    // ReSharper disable InconsistentNaming
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