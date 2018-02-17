// ///////////////////////////////////
// File: CityToPostalCodeMap.cs
// Last Change: 17.02.2018, 14:25
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.Model.Mapping
{
    using FluentNHibernate.Mapping;

    public class CityToPostalCodeMap : ClassMap<CityToPostalCode>
    {
        #region Constructors

        public CityToPostalCodeMap()
        {
            this.Id(x => x.PostalCode);
            this.Map(x => x.City);
        }

        #endregion
    }
}