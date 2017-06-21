// ///////////////////////////////////
// File: BillItemDetailViewModel.cs
// Last Change: 18.04.2017  20:09
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using EpAccounting.Model;



    public class BillItemDetailViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly BillItem billItem;

        #endregion



        #region Constructors / Destructor

        public BillItemDetailViewModel(BillItem billItem)
        {
            this.billItem = billItem;
        }

        #endregion



        #region Properties

        public int Position
        {
            get { return this.billItem.Position; }
            set { this.SetProperty(this.billItem.Position, value, () => this.billItem.Position = value); }
        }

        public int ArticleNumber
        {
            get { return this.billItem.ArticleNumber; }
            set { this.SetProperty(this.billItem.ArticleNumber, value, () => this.billItem.ArticleNumber = value); }
        }

        public string Description
        {
            get { return this.billItem.Description; }
            set { this.SetProperty(this.billItem.Description, value, () => this.billItem.Description = value); }
        }

        public double Amount
        {
            get { return this.billItem.Amount; }
            set
            {
                this.SetProperty(this.billItem.Amount, value, () => this.billItem.Amount = value);
                this.RaisePropertyChanged(() => this.Sum);
            }
        }

        public double Price
        {
            get { return this.billItem.Price; }
            set
            {
                this.SetProperty(this.billItem.Price, value, () => this.billItem.Price = value);
                this.RaisePropertyChanged(() => this.Sum);
            }
        }

        public double Discount
        {
            get { return this.billItem.Discount; }
            set
            {
                this.SetProperty(this.billItem.Discount, value, () => this.billItem.Discount = value);
                this.RaisePropertyChanged(() => this.Sum);
            }
        }

        public double Sum
        {
            get { return (this.billItem.Amount * this.billItem.Price) / 100 * (100 - this.billItem.Discount); }
        }

        #endregion
    }
}