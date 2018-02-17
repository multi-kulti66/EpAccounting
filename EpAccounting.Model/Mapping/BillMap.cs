// ///////////////////////////////////
// File: BillMap.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.Model.Mapping
{
    using FluentNHibernate.Mapping;


    public class BillMap : ClassMap<Bill>
    {
        #region Constructors

        public BillMap()
        {
            this.Id(x => x.Id);
            this.References(x => x.Client).Fetch.Join();
            this.Map(x => x.Printed);
            this.Map(x => x.KindOfBill);
            this.Map(x => x.KindOfVat);
            this.Map(x => x.VatPercentage);
            this.Map(x => x.Date);
            this.HasMany(x => x.BillItems).Cascade.All().Not.LazyLoad();
        }

        #endregion
    }
}