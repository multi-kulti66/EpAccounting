// ///////////////////////////////////
// File: ArticleMap.cs
// Last Change: 16.08.2017  18:27
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model.Mapping
{
    using FluentNHibernate.Mapping;



    public class ArticleMap : ClassMap<Article>
    {
        public ArticleMap()
        {
            this.Id(x => x.Id);
            this.Map(x => x.ArticleNumber);
            this.Map(x => x.Description);
            this.Map(x => x.Amount);
            this.Map(x => x.Price);
        }
    }
}