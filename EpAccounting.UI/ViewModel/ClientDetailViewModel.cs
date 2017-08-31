// ///////////////////////////////////
// File: ClientDetailViewModel.cs
// Last Change: 23.08.2017  20:23
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using EpAccounting.Model;



    public class ClientDetailViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly Client client;

        #endregion



        #region Constructors / Destructor

        public ClientDetailViewModel(Client client)
        {
            this.client = client;
        }

        #endregion



        #region Properties

        public int ClientId
        {
            get { return this.client.ClientId; }
            set { this.SetProperty(this.client.ClientId, value, () => this.client.ClientId = value); }
        }

        public string Title
        {
            get { return this.client.Title; }
            set { this.SetProperty(this.client.Title, value, () => this.client.Title = value); }
        }

        public string FirstName
        {
            get { return this.client.FirstName; }
            set { this.SetProperty(this.client.FirstName, value, () => this.client.FirstName = value); }
        }

        public string LastName
        {
            get { return this.client.LastName; }
            set { this.SetProperty(this.client.LastName, value, () => this.client.LastName = value); }
        }

        public string Street
        {
            get { return this.client.Street; }
            set { this.SetProperty(this.client.Street, value, () => this.client.Street = value); }
        }

        public string HouseNumber
        {
            get { return this.client.HouseNumber; }
            set { this.SetProperty(this.client.HouseNumber, value, () => this.client.HouseNumber = value); }
        }

        public string PostalCode
        {
            get { return this.client.PostalCode; }
            set { this.SetProperty(this.client.PostalCode, value, () => this.client.PostalCode = value); }
        }

        public string City
        {
            get { return this.client.City; }
            set { this.SetProperty(this.client.City, value, () => this.client.City = value); }
        }

        public string DateOfBirth
        {
            get { return this.client.DateOfBirth; }
            set { this.SetProperty(this.client.DateOfBirth, value, () => this.client.DateOfBirth = value); }
        }

        public string PhoneNumber1
        {
            get { return this.client.PhoneNumber1; }
            set { this.SetProperty(this.client.PhoneNumber1, value, () => this.client.PhoneNumber1 = value); }
        }

        public string PhoneNumber2
        {
            get { return this.client.PhoneNumber2; }
            set { this.SetProperty(this.client.PhoneNumber2, value, () => this.client.PhoneNumber2 = value); }
        }

        public string MobileNumber
        {
            get { return this.client.MobileNumber; }
            set { this.SetProperty(this.client.MobileNumber, value, () => this.client.MobileNumber = value); }
        }

        public string Telefax
        {
            get { return this.client.Telefax; }
            set { this.SetProperty(this.client.Telefax, value, () => this.client.Telefax = value); }
        }

        public string Email
        {
            get { return this.client.Email; }
            set { this.SetProperty(this.client.Email, value, () => this.client.Email = value); }
        }

        public int NumberOfBills
        {
            get { return this.client.Bills.Count; }
        }

        public double Sales
        {
            get
            {
                double sum = 0;

                foreach (Bill bill in this.client.Bills)
                {
                    foreach (BillItem billDetail in bill.BillItems)
                    {
                        sum += billDetail.Price * ((100 - billDetail.Discount) / 100) * billDetail.Amount;
                    }
                }

                return sum;
            }
        }

        public bool HasMissingLastName
        {
            get
            {
                if (string.IsNullOrEmpty(this.LastName))
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
    }
}