// ///////////////////////////////////
// File: Article.cs
// Last Change: 18.09.2017  20:40
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model
{
    using System;



    public class Article
    {
        public virtual int Id { get; set; }

        public virtual int ArticleNumber { get; set; }

        public virtual string Description { get; set; }

        public virtual double Amount { get; set; }

        public virtual decimal Price { get; set; }


        public override bool Equals(object obj)
        {
            Article otherArticle = obj as Article;

            if (otherArticle == null)
            {
                return false;
            }

            return this.Id == otherArticle.Id && this.ArticleNumber == otherArticle.ArticleNumber &&
                   string.Equals(this.Description, otherArticle.Description) &&
                   Math.Abs(this.Amount - otherArticle.Amount) < 0.01 && this.Price == otherArticle.Price;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Id;
                hashCode = (hashCode * 397) ^ this.ArticleNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Description.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Amount.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Price.GetHashCode();
                return hashCode;
            }
        }
    }
}