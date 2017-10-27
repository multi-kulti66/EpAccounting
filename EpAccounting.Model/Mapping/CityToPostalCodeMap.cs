// ///////////////////////////////////
// File: AddressMap.cs
// Last Change: 22.10.2017  11:32
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model.Mapping
{
    using FluentNHibernate.Mapping;



    public class CityToPostalCodeMap : ClassMap<CityToPostalCode>
    {
        #region Constructors / Destructor

        public CityToPostalCodeMap()
        {
            this.Id(x => x.PostalCode);
            this.Map(x => x.City);
        }

        #endregion
    }
}