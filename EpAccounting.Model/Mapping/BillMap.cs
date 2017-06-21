// ///////////////////////////////////
// File: BillMap.cs
// Last Change: 28.03.2017  18:58
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model.Mapping
{
    using FluentNHibernate.Mapping;



    public class BillMap : ClassMap<Bill>
    {
        #region Constructors / Destructor

        public BillMap()
        {
            this.Id(x => x.BillId);
            this.References(x => x.Client).Fetch.Join();
            this.Map(x => x.KindOfBill);
            this.Map(x => x.KindOfVat);
            this.Map(x => x.VatPercentage);
            this.Map(x => x.Date);
            this.HasMany(x => x.BillItems).Cascade.All().Not.LazyLoad();
        }

        #endregion
    }
}