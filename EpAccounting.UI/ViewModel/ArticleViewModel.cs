// ///////////////////////////////////
// File: ArticleViewModel.cs
// Last Change: 19.02.2018, 19:26
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using Business;
    using Model;
    using Service;


    public class ArticleViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly Article _article;
        private readonly IRepository _repository;
        private readonly IDialogService _dialogService;

        #endregion



        #region Constructors

        public ArticleViewModel(Article article, IRepository repository, IDialogService dialogService)
        {
            this._article = article;
            this._repository = repository;
            this._dialogService = dialogService;
        }

        #endregion



        #region Properties, Indexers

        public int Id
        {
            get { return this._article.Id; }
        }

        public int ArticleNumber
        {
            get { return this._article.ArticleNumber; }
            set { this.SetProperty(() => this._article.ArticleNumber = value, () => this._article.ArticleNumber == value); }
        }

        public string Description
        {
            get { return this._article.Description; }
            set { this.SetProperty(() => this._article.Description = value, () => this._article.Description == value); }
        }

        public double Amount
        {
            get { return this._article.Amount; }
            set { this.SetProperty(() => this._article.Amount = value, () => Math.Abs(this._article.Amount - value) < 0.01); }
        }

        public decimal Price
        {
            get { return this._article.Price; }
            set { this.SetProperty(() => this._article.Price = value, () => this._article.Price == value); }
        }

        #endregion



        public void Save()
        {
            try
            {
                this._repository.SaveOrUpdate(this._article);
            }
            catch (Exception e)
            {
                this._dialogService.ShowExceptionMessage(e, string.Format("Could not save article '{0}'", this._article.Description));
            }
        }
    }
}