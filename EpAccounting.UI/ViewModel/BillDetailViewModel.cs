// ///////////////////////////////////
// File: BillDetailViewModel.cs
// Last Change: 26.10.2017  21:26
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Linq;
    using System.Reflection;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.UI.Properties;
    using GalaSoft.MvvmLight.Messaging;



    public class BillDetailViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly IRepository repository;
        private Bill bill;

        #endregion



        #region Constructors / Destructor

        public BillDetailViewModel(Bill bill, IRepository repository)
        {
            this.repository = repository;
            this.bill = bill;

            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public int Id
        {
            get { return this.bill.Id; }
            set { this.SetProperty(() => this.bill.Id = value, () => this.bill.Id == value); }
        }

        public bool? Printed
        {
            get { return this.bill.Printed; }
            set { this.SetProperty(() => this.bill.Printed = value, () => this.bill.Printed == value); }
        }

        public KindOfBill? KindOfBill
        {
            get { return this.bill.KindOfBill; }
            set { this.SetProperty(() => this.bill.KindOfBill = value, () => this.bill.KindOfBill == value); }
        }

        public KindOfVat? KindOfVat
        {
            get { return this.bill.KindOfVat; }
            set
            {
                this.SetProperty(() => this.bill.KindOfVat = value, () => this.bill.KindOfVat == value);
                Messenger.Default.Send(new NotificationMessage(Resources.Message_OnVatChangeRecalculatePricesForBillItemEditVM));
            }
        }

        public double VatPercentage
        {
            get { return this.bill.VatPercentage; }
            set { this.SetProperty(() => this.bill.VatPercentage = value, () => Math.Abs(this.bill.VatPercentage - value) < 0.01); }
        }

        public string Date
        {
            get { return this.bill.Date; }
            set { this.SetProperty(() => this.bill.Date = value, () => this.bill.Date == value); }
        }

        public int ClientId
        {
            get { return this.bill.Client.Id; }
            set { this.SetProperty(() => this.bill.Client.Id = value, () => this.bill.Client.Id == value); }
        }

        public ClientTitle? Title
        {
            get { return this.bill.Client.Title; }
            set { this.SetProperty(() => this.bill.Client.Title = value, () => this.bill.Client.Title == value); }
        }

        public string FirstName
        {
            get { return this.bill.Client.FirstName; }
            set { this.SetProperty(() => this.bill.Client.FirstName = value, () => this.bill.Client.FirstName == value); }
        }

        public string LastName
        {
            get { return this.bill.Client.LastName; }
            set { this.SetProperty(() => this.bill.Client.LastName = value, () => this.bill.Client.LastName == value); }
        }

        public string Street
        {
            get { return this.bill.Client.Street; }
            set { this.SetProperty(() => this.bill.Client.Street = value, () => this.bill.Client.Street == value); }
        }

        public string HouseNumber
        {
            get { return this.bill.Client.HouseNumber; }
            set { this.SetProperty(() => this.bill.Client.HouseNumber = value, () => this.bill.Client.HouseNumber == value); }
        }

        public string PostalCode
        {
            get { return this.bill.Client.CityToPostalCode.PostalCode; }
            set { this.SetProperty(() => this.bill.Client.CityToPostalCode.PostalCode = value, () => this.bill.Client.CityToPostalCode.PostalCode == value); }
        }

        public string City
        {
            get { return this.bill.Client.CityToPostalCode.City; }
            set { this.SetProperty(() => this.bill.Client.CityToPostalCode.City = value, () => this.bill.Client.CityToPostalCode.City == value); }
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Message_ReloadBillBecauseOfPrintedStateChangeForBillDetailVM)
            {
                if (this.Id == message.Content)
                {
                    this.bill = this.repository.GetById<Bill>(message.Content);
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