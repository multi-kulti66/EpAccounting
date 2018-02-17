// ///////////////////////////////////
// File: BillItemDetailViewModel.cs
// Last Change: 17.02.2018, 14:28
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

        private readonly BillItem billItem;
        private readonly IRepository repository;

        #endregion



        #region Constructors

        public BillItemDetailViewModel(BillItem billItem, IRepository repository)
        {
            this.billItem = billItem;
            this.repository = repository;
        }

        #endregion



        #region Properties, Indexers

        public int Id
        {
            get { return this.billItem.Id; }
        }

        public int Position
        {
            get { return this.billItem.Position; }
            set { this.SetProperty(() => this.billItem.Position = value, () => this.billItem.Position == value); }
        }

        public int ArticleNumber
        {
            get { return this.billItem.ArticleNumber; }
            set
            {
                if (this.SetProperty(() => this.billItem.ArticleNumber = value, () => this.billItem.ArticleNumber == value))
                {
                    this.FillArticleValues();
                }
            }
        }

        public string Description
        {
            get { return this.billItem.Description; }
            set { this.SetProperty(() => this.billItem.Description = value, () => this.billItem.Description == value); }
        }

        public double Amount
        {
            get { return this.billItem.Amount; }
            set
            {
                this.SetProperty(() => this.billItem.Amount = value, () => Math.Abs(this.billItem.Amount - value) < 0.01);
                this.RaisePropertyChanged(() => this.Sum);
            }
        }

        public decimal Price
        {
            get { return this.billItem.Price; }
            set
            {
                this.SetProperty(() => this.billItem.Price = value, () => this.billItem.Price == value);
                this.RaisePropertyChanged(() => this.Sum);
            }
        }

        public double Discount
        {
            get { return this.billItem.Discount; }
            set
            {
                this.SetProperty(() => this.billItem.Discount = value, () => Math.Abs(this.billItem.Discount - value) < 0.01);
                this.RaisePropertyChanged(() => this.Sum);
            }
        }

        public decimal Sum
        {
            get { return ((decimal) this.billItem.Amount * this.billItem.Price) / 100 * (100 - (decimal) this.billItem.Discount); }
        }

        #endregion



        private void FillArticleValues()
        {
            Article article = null;

            try
            {
                article = this.repository.GetByCriteria<Article>(Restrictions.Where<Article>(x => x.ArticleNumber == this.ArticleNumber), 1).FirstOrDefault();
            }
            catch
            {
                // do nothing
            }

            if (article != null)
            {
                this.Description = article.Description;
                this.Amount = article.Amount;

                if (this.billItem.Bill.KindOfVat == KindOfVat.inkl_MwSt)
                {
                    this.Price = article.Price * (100 + (decimal) this.billItem.Bill.VatPercentage) / 100;
                }
                else
                {
                    this.Price = article.Price;
                }
            }
        }
    }
}