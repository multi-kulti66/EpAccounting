// ///////////////////////////////////
// File: BillItemMap.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.Model.Mapping
{
    using FluentNHibernate.Mapping;


    public class BillItemMap : ClassMap<BillItem>
    {
        #region Constructors

        public BillItemMap()
        {
            this.Id(x => x.Id);
            this.References(x => x.Bill).Fetch.Join();
            this.Map(x => x.Position);
            this.Map(x => x.ArticleNumber);
            this.Map(x => x.Description);
            this.Map(x => x.Amount);
            this.Map(x => x.Discount);
            this.Map(x => x.Price);
        }

        #endregion
    }
}