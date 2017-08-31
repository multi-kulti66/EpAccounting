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
        #region Constructors / Destructor

        public ArticleMap()
        {
            this.Id(x => x.ArticleId);
            this.Map(x => x.Description);
        }

        #endregion
    }
}