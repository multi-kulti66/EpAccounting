// ///////////////////////////////////
// File: BillDetailViewModel.cs
// Last Change: 19.02.2018, 19:35
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Business;
    using GalaSoft.MvvmLight.Messaging;
    using Model;
    using Model.Enum;
    using Properties;
    using Service;


    public class BillDetailViewModel : BindableViewModelBase, IDisposable
    {
        #region Fields

        private readonly IRepository _repository;
        private readonly IDialogService _dialogService;
        private Bill _bill;

        #endregion



        #region Constructors

        public BillDetailViewModel(Bill bill, IRepository repository, IDialogService dialogService)
        {
            this._dialogService = dialogService;
            this._repository = repository;
            this._bill = bill;

            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties, Indexers

        public int Id
        {
            get { return this._bill.Id; }
            set { this.SetProperty(() => this._bill.Id = value, () => this._bill.Id == value); }
        }

        public bool? Printed
        {
            get { return this._bill.Printed; }
            set { this.SetProperty(() => this._bill.Printed = value, () => this._bill.Printed == value); }
        }

        public KindOfBill? KindOfBill
        {
            get { return this._bill.KindOfBill; }
            set { this.SetProperty(() => this._bill.KindOfBill = value, () => this._bill.KindOfBill == value); }
        }

        public KindOfVat? KindOfVat
        {
            get { return this._bill.KindOfVat; }
            set
            {
                this.SetProperty(() => this._bill.KindOfVat = value, () => this._bill.KindOfVat == value);
                Messenger.Default.Send(new NotificationMessage(Resources.Message_OnVatChangeRecalculatePricesForBillItemEditVM));
            }
        }

        public double VatPercentage
        {
            get { return this._bill.VatPercentage; }
            set { this.SetProperty(() => this._bill.VatPercentage = value, () => Math.Abs(this._bill.VatPercentage - value) < 0.01); }
        }

        public string Date
        {
            get { return this._bill.Date; }
            set { this.SetProperty(() => this._bill.Date = value, () => this._bill.Date == value); }
        }

        public int ClientId
        {
            get { return this._bill.Client.Id; }
            set { this.SetProperty(() => this._bill.Client.Id = value, () => this._bill.Client.Id == value); }
        }

        public ClientTitle? Title
        {
            get { return this._bill.Client.Title; }
            set { this.SetProperty(() => this._bill.Client.Title = value, () => this._bill.Client.Title == value); }
        }

        public string CompanyName
        {
            get { return this._bill.Client.CompanyName; }
            set { this.SetProperty(() => this._bill.Client.CompanyName = value, () => this._bill.Client.CompanyName == value); }
        }

        public string FirstName
        {
            get { return this._bill.Client.FirstName; }
            set { this.SetProperty(() => this._bill.Client.FirstName = value, () => this._bill.Client.FirstName == value); }
        }

        public string LastName
        {
            get { return this._bill.Client.LastName; }
            set { this.SetProperty(() => this._bill.Client.LastName = value, () => this._bill.Client.LastName == value); }
        }

        public string Street
        {
            get { return this._bill.Client.Street; }
            set { this.SetProperty(() => this._bill.Client.Street = value, () => this._bill.Client.Street == value); }
        }

        public string HouseNumber
        {
            get { return this._bill.Client.HouseNumber; }
            set { this.SetProperty(() => this._bill.Client.HouseNumber = value, () => this._bill.Client.HouseNumber == value); }
        }

        public string PostalCode
        {
            get { return this._bill.Client.CityToPostalCode.PostalCode; }
            set { this.SetProperty(() => this._bill.Client.CityToPostalCode.PostalCode = value, () => this._bill.Client.CityToPostalCode.PostalCode == value); }
        }

        public string City
        {
            get { return this._bill.Client.CityToPostalCode.City; }
            set { this.SetProperty(() => this._bill.Client.CityToPostalCode.City = value, () => this._bill.Client.CityToPostalCode.City == value); }
        }

        #endregion



        #region IDisposable Members

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Message_ReloadBillBecauseOfPrintedStateChangeForBillDetailVM)
            {
                if (this.Id == message.Content)
                {
                    try
                    {
                        this._bill = this._repository.GetById<Bill>(message.Content);
                    }
                    catch (Exception e)
                    {
                        this._dialogService.ShowExceptionMessage(e, string.Format("Could not load bill with id '{0}'", message.Content));
                    }

                    this.UpdateProperties();
                }
            }
        }

        private void UpdateProperties()
        {
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                      .Where(x => x.DeclaringType == this.GetType()))
            {
                this.RaisePropertyChanged(propertyInfo.Name);
            }
        }
    }
}