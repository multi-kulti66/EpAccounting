// ///////////////////////////////////
// File: ArticleViewModel.cs
// Last Change: 18.09.2017  20:41
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System;
    using EpAccounting.Business;
    using EpAccounting.Model;



    public class ArticleViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly Article article;
        private readonly IRepository repository;

        #endregion



        #region Constructors / Destructor

        public ArticleViewModel(Article article, IRepository repository)
        {
            this.article = article;
            this.repository = repository;
        }

        #endregion



        #region Properties

        public int Id
        {
            get { return this.article.Id; }
        }

        public int ArticleNumber
        {
            get { return this.article.ArticleNumber; }
            set { this.SetProperty(() => this.article.ArticleNumber = value, () => this.article.ArticleNumber == value); }
        }

        public string Description
        {
            get { return this.article.Description; }
            set { this.SetProperty(() => this.article.Description = value, () => this.article.Description == value); }
        }

        public double Amount
        {
            get { return this.article.Amount; }
            set { this.SetProperty(() => this.article.Amount = value, () => Math.Abs(this.article.Amount - value) < 0.01); }
        }

        public decimal Price
        {
            get { return this.article.Price; }
            set { this.SetProperty(() => this.article.Price = value, () => this.article.Price == value); }
        }

        #endregion



        public void Save()
        {
            this.repository.SaveOrUpdate(this.article);
        }
    }
}