// ///////////////////////////////////
// File: ClientDetailViewModel.cs
// Last Change: 08.12.2017  13:57
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Linq;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.UI.Properties;
    using GalaSoft.MvvmLight.Messaging;
    using NHibernate.Criterion;



    public class ClientDetailViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly Client client;
        private readonly IRepository repository;

        #endregion



        #region Constructors / Destructor

        public ClientDetailViewModel(Client client, IRepository repository)
        {
            this.client = client;
            this.repository = repository;
        }

        #endregion



        #region Properties

        public int Id
        {
            get { return this.client.Id; }
            set { this.SetProperty(() => this.client.Id = value, () => this.client.Id == value); }
        }

        public ClientTitle? Title
        {
            get { return this.client.Title; }
            set
            {
                if (this.SetProperty(() => this.client.Title = value, () => this.client.Title == value))
                {
                    if (this.client.Title != ClientTitle.Firma)
                    {
                        this.CompanyName = string.Empty;
                    }

                    Messenger.Default.Send(new NotificationMessage(Resources.Message_UpdateCompanyNameEnableStateForClientEditVM));
                }
            }
        }

        public string CompanyName
        {
            get { return this.client.CompanyName; }
            set { this.SetProperty(() => this.client.CompanyName = value, () => this.client.CompanyName == value); }
        }

        public string FirstName
        {
            get { return this.client.FirstName; }
            set { this.SetProperty(() => this.client.FirstName = value, () => this.client.FirstName == value); }
        }

        public string LastName
        {
            get { return this.client.LastName; }
            set { this.SetProperty(() => this.client.LastName = value, () => this.client.LastName == value); }
        }

        public string Street
        {
            get { return this.client.Street; }
            set { this.SetProperty(() => this.client.Street = value, () => this.client.Street == value); }
        }

        public string HouseNumber
        {
            get { return this.client.HouseNumber; }
            set { this.SetProperty(() => this.client.HouseNumber = value, () => this.client.HouseNumber == value); }
        }

        public string PostalCode
        {
            get { return this.client.CityToPostalCode.PostalCode; }
            set
            {
                if (this.SetProperty(() => this.client.CityToPostalCode.PostalCode = value, () => this.client.CityToPostalCode.PostalCode == value))
                {
                    this.FillCity();
                }
            }
        }

        public string City
        {
            get { return this.client.CityToPostalCode.City; }
            set { this.SetProperty(() => this.client.CityToPostalCode.City = value, () => this.client.CityToPostalCode.City == value); }
        }

        public string DateOfBirth
        {
            get { return this.client.DateOfBirth; }
            set { this.SetProperty(() => this.client.DateOfBirth = value, () => this.client.DateOfBirth == value); }
        }

        public string PhoneNumber1
        {
            get { return this.client.PhoneNumber1; }
            set { this.SetProperty(() => this.client.PhoneNumber1 = value, () => this.client.PhoneNumber1 == value); }
        }

        public string PhoneNumber2
        {
            get { return this.client.PhoneNumber2; }
            set { this.SetProperty(() => this.client.PhoneNumber2 = value, () => this.client.PhoneNumber2 == value); }
        }

        public string MobileNumber
        {
            get { return this.client.MobileNumber; }
            set { this.SetProperty(() => this.client.MobileNumber = value, () => this.client.MobileNumber == value); }
        }

        public string Telefax
        {
            get { return this.client.Telefax; }
            set { this.SetProperty(() => this.client.Telefax = value, () => this.client.Telefax == value); }
        }

        public string Email
        {
            get { return this.client.Email; }
            set { this.SetProperty(() => this.client.Email = value, () => this.client.Email == value); }
        }

        public int NumberOfBills
        {
            get { return this.client.Bills.Count; }
        }

        public decimal Sales
        {
            get
            {
                decimal sum = 0;

                foreach (Bill bill in this.client.Bills)
                {
                    if (bill.KindOfVat == KindOfVat.inkl_MwSt || bill.KindOfVat == KindOfVat.without_MwSt)
                    {
                        foreach (BillItem billDetail in bill.BillItems)
                        {
                            sum += billDetail.Price * ((100 - (decimal)billDetail.Discount) / 100) * (decimal)billDetail.Amount;
                        }
                    }
                    else if (bill.KindOfVat == KindOfVat.zzgl_MwSt)
                    {
                        foreach (BillItem billDetail in bill.BillItems)
                        {
                            sum += billDetail.Price * ((100 - (decimal)billDetail.Discount) / 100) * (decimal)billDetail.Amount * (100 + (decimal)bill.VatPercentage) / 100;
                        }
                    }
                }

                return sum;
            }
        }

        public bool HasMissingValues
        {
            get
            {
                if (string.IsNullOrEmpty(this.LastName) || string.IsNullOrEmpty(this.PostalCode))
                {
                    return true;
                }

                return false;
            }
        }

        #endregion



        public override string ToString()
        {
            return this.client.ToString();
        }

        private void FillCity()
        {
            CityToPostalCode cityToPostalCode = null;

            try
            {
                cityToPostalCode = this.repository
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