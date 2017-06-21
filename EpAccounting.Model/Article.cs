// ///////////////////////////////////
// File: Article.cs
// Last Change: 28.03.2017  19:00
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model
{
    public class Article
    {
        #region Properties

        public virtual int ArticleId { get; set; }

        public virtual string Description { get; set; }

        #endregion



        public override bool Equals(object obj)
        {
            Article otherArticle = obj as Article;

            if (otherArticle == null)
            {
                return false;
            }

            return this.ArticleId == otherArticle.ArticleId && string.Equals(this.Description, otherArticle.Description);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.ArticleId * 397) ^ (this.Description?.GetHashCode() ?? 0);
            }
        }
    }
}