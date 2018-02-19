// ///////////////////////////////////
// File: BillItemDetailViewModel.cs
// Last Change: 19.02.2018, 19:41
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Linq;
    using Business;
    using Model;
    using Model.Enum;
    using NHibernate.Criterion;


    public class BillItemDetailViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly BillItem _billItem;
        private readonly IRepository _repository;

        #endregion



        #region Constructors

        public BillItemDetailViewModel(BillItem billItem, IRepository repository)
        {
            this._billItem = billItem;
            this._repository = repository;
        }

        #endregion



        #region Properties, Indexers

        public int Id
        {
            get { return this._billItem.Id; }
        }

        public int Position
        {
            get { return this._billItem.Position; }
            set { this.SetProperty(() => this._billItem.Position = value, () => this._billItem.Position == value); }
        }

        public int ArticleNumber
        {
            get { return this._billItem.ArticleNumber; }
            set
            {
                if (this.SetProperty(() => this._billItem.ArticleNumber = value, () => this._billItem.ArticleNumber == value))
                {
                    this.FillArticleValues();
                }
            }
        }

        public string Description
        {
            get { return this._billItem.Description; }
            set { this.SetProperty(() => this._billItem.Description = value, () => this._billItem.Description == value); }
        }

        public double Amount
        {
            get { return this._billItem.Amount; }
            set
            {
                this.SetProperty(() => this._billItem.Amount = value, () => Math.Abs(this._billItem.Amount - value) < 0.01);
                this.RaisePropertyChanged(() => this.Sum);
            }
        }

        public decimal Price
        {
            get { return this._billItem.Price; }
            set
            {
                this.SetProperty(() => this._billItem.Price = value, () => this._billItem.Price == value);
                this.RaisePropertyChanged(() => this.Sum);
            }
        }

        public double Discount
        {
            get { return this._billItem.Discount; }
            set
            {
                this.SetProperty(() => this._billItem.Discount = value, () => Math.Abs(this._billItem.Discount - value) < 0.01);
                this.RaisePropertyChanged(() => this.Sum);
            }
        }

        public decimal Sum
        {
            get { return ((decimal) this._billItem.Amount * this._billItem.Price) / 100 * (100 - (decimal) this._billItem.Discount); }
        }

        #endregion



        private void FillArticleValues()
        {
            Article article = null;

            try
            {
                article = this._repository.GetByCriteria<Article>(Restrictions.Where<Article>(x => x.ArticleNumber == this.ArticleNumber), 1).FirstOrDefault();
            }
            catch
            {
                // do nothing
            }

            if (article != null)
            {
                this.Description = article.Description;
                this.Amount = article.Amount;

                if (this._billItem.Bill.KindOfVat == KindOfVat.InklMwSt)
                {
                    this.Price = article.Price * (100 + (decimal) this._billItem.Bill.VatPercentage) / 100;
                }
                else
                {
                    this.Price = article.Price;
                }
            }
        }
    }
}