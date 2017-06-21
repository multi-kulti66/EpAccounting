// ///////////////////////////////////
// File: BillItemMap.cs
// Last Change: 28.03.2017  18:57
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model.Mapping
{
    using FluentNHibernate.Mapping;



    public class BillItemMap : ClassMap<BillItem>
    {
        #region Constructors / Destructor

        public BillItemMap()
        {
            this.Id(x => x.BillItemId);
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