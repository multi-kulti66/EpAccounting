// ///////////////////////////////////
// File: ClientDetailViewModel.cs
// Last Change: 19.02.2018, 19:55
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System.Linq;
    using Business;
    using GalaSoft.MvvmLight.Messaging;
    using Model;
    using Model.Enum;
    using NHibernate.Criterion;
    using Properties;


    public class ClientDetailViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly Client _client;
        private readonly IRepository _repository;

        #endregion



        #region Constructors

        public ClientDetailViewModel(Client client, IRepository repository)
        {
            this._client = client;
            this._repository = repository;
        }

        #endregion



        #region Properties, Indexers

        public int Id
        {
            get { return this._client.Id; }
            set { this.SetProperty(() => this._client.Id = value, () => this._client.Id == value); }
        }

        public ClientTitle? Title
        {
            get { return this._client.Title; }
            set
            {
                if (this.SetProperty(() => this._client.Title = value, () => this._client.Title == value))
                {
                    if (this._client.Title != ClientTitle.Firma)
                    {
                        this.CompanyName = string.Empty;
                    }

                    Messenger.Default.Send(new NotificationMessage(Resources.Message_UpdateCompanyNameEnableStateForClientEditVM));
                }
            }
        }

        public string CompanyName
        {
            get { return this._client.CompanyName; }
            set { this.SetProperty(() => this._client.CompanyName = value, () => this._client.CompanyName == value); }
        }

        public string FirstName
        {
            get { return this._client.FirstName; }
            set { this.SetProperty(() => this._client.FirstName = value, () => this._client.FirstName == value); }
        }

        public string LastName
        {
            get { return this._client.LastName; }
            set { this.SetProperty(() => this._client.LastName = value, () => this._client.LastName == value); }
        }

        public string Street
        {
            get { return this._client.Street; }
            set { this.SetProperty(() => this._client.Street = value, () => this._client.Street == value); }
        }

        public string HouseNumber
        {
            get { return this._client.HouseNumber; }
            set { this.SetProperty(() => this._client.HouseNumber = value, () => this._client.HouseNumber == value); }
        }

        public string PostalCode
        {
            get { return this._client.CityToPostalCode.PostalCode; }
            set
            {
                if (this.SetProperty(() => this._client.CityToPostalCode.PostalCode = value, () => this._client.CityToPostalCode.PostalCode == value))
                {
                    this.FillCity();
                }
            }
        }

        public string City
        {
            get { return this._client.CityToPostalCode.City; }
            set { this.SetProperty(() => this._client.CityToPostalCode.City = value, () => this._client.CityToPostalCode.City == value); }
        }

        public string DateOfBirth
        {
            get { return this._client.DateOfBirth; }
            set { this.SetProperty(() => this._client.DateOfBirth = value, () => this._client.DateOfBirth == value); }
        }

        public string PhoneNumber1
        {
            get { return this._client.PhoneNumber1; }
            set { this.SetProperty(() => this._client.PhoneNumber1 = value, () => this._client.PhoneNumber1 == value); }
        }

        public string PhoneNumber2
        {
            get { return this._client.PhoneNumber2; }
            set { this.SetProperty(() => this._client.PhoneNumber2 = value, () => this._client.PhoneNumber2 == value); }
        }

        public string MobileNumber
        {
            get { return this._client.MobileNumber; }
            set { this.SetProperty(() => this._client.MobileNumber = value, () => this._client.MobileNumber == value); }
        }

        public string Telefax
        {
            get { return this._client.Telefax; }
            set { this.SetProperty(() => this._client.Telefax = value, () => this._client.Telefax == value); }
        }

        public string Email
        {
            get { return this._client.Email; }
            set { this.SetProperty(() => this._client.Email = value, () => this._client.Email == value); }
        }

        public int NumberOfBills
        {
            get { return this._client.Bills.Count; }
        }

        public decimal Sales
        {
            get
            {
                decimal sum = 0;

                foreach (Bill bill in this._client.Bills)
                {
                    if (bill.KindOfVat == KindOfVat.InklMwSt || bill.KindOfVat == KindOfVat.WithoutMwSt)
                    {
                        foreach (BillItem billDetail in bill.BillItems)
                        {
                            sum += billDetail.Price * ((100 - (decimal) billDetail.Discount) / 100) * (decimal) billDetail.Amount;
                        }
                    }
                    else if (bill.KindOfVat == KindOfVat.ZzglMwSt)
                    {
                        foreach (BillItem billDetail in bill.BillItems)
                        {
                            sum += billDetail.Price * ((100 - (decimal) billDetail.Discount) / 100) * (decimal) billDetail.Amount * (100 + (decimal) bill.VatPercentage) / 100;
                        }
                    }
                }

                return sum;
            }
        }

        public bool HasMissingValues
        {
            get { return string.IsNullOrEmpty(this.LastName) || string.IsNullOrEmpty(this.PostalCode); }
        }

        #endregion



        public override string ToString()
        {
            return this._client.ToString();
        }

        private void FillCity()
        {
            CityToPostalCode cityToPostalCode = null;

            try
            {
                cityToPostalCode = this._repository
                                       .GetByCriteria<CityToPostalCode>(Restrictions.Where<CityToPostalCode>(x => x.PostalCode == this.PostalCode), 1)
                                       .FirstOrDefault();
            }
            catch
            {
                // do nothing
            }

            if (cityToPostalCode != null)
            {
                this.City = cityToPostalCode.City;
            }
        }
    }
}