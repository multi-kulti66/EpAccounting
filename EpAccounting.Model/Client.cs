// ///////////////////////////////////
// File: Client.cs
// Last Change: 28.10.2017  11:50
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model
{
    using System;
    using System.Collections.Generic;
    using EpAccounting.Model.Enum;
    using EpAccounting.Model.Properties;



    public class Client : ICloneable
    {
        #region Properties

        public virtual int Id { get; set; }

        public virtual ClientTitle? Title { get; set; }

        public virtual string CompanyName { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Street { get; set; }

        public virtual string HouseNumber { get; set; }

        public virtual string DateOfBirth { get; set; }

        public virtual string PhoneNumber1 { get; set; }

        public virtual string PhoneNumber2 { get; set; }

        public virtual string MobileNumber { get; set; }

        public virtual string Telefax { get; set; }

        public virtual string Email { get; set; }

        public virtual CityToPostalCode CityToPostalCode { get; set; }

        public virtual IList<Bill> Bills { get; set; } = new List<Bill>();

        #endregion



        #region ICloneable Members

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion



        public virtual void AddBill(Bill bill)
        {
            if (bill == null)
            {
                throw new ArgumentNullException(Resources.Exception_Message_Client_BillHasNoReference);
            }

            bill.Client = this;
            this.Bills.Add(bill);
        }

        public override bool Equals(object obj)
        {
            Client otherClient = obj as Client;

            if (otherClient == null)
            {
                return false;
            }

            bool equalClientData = this.Id == otherClient.Id && Equals(this.Title, otherClient.Title) &&
                                   string.Equals(this.FirstName, otherClient.FirstName) && string.Equals(this.LastName, otherClient.LastName) &&
                                   string.Equals(this.Street, otherClient.Street) && string.Equals(this.HouseNumber, otherClient.HouseNumber) &&
                                   Equals(this.CityToPostalCode.PostalCode, otherClient.CityToPostalCode.PostalCode) && string.Equals(this.DateOfBirth, otherClient.DateOfBirth) &&
                                   string.Equals(this.PhoneNumber1, otherClient.PhoneNumber1) && string.Equals(this.PhoneNumber2, otherClient.PhoneNumber2)
                                   && string.Equals(this.MobileNumber, otherClient.MobileNumber) && string.Equals(this.Telefax, otherClient.Telefax) &&
                                   string.Equals(this.Email, otherClient.Email);

            if (!equalClientData || (this.Bills.Count != otherClient.Bills.Count))
            {
                return false;
            }

            for (int i = 0; i < this.Bills.Count; i++)
            {
                if (!this.Bills[i].Equals(otherClient.Bills[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Id;
                hashCode = (hashCode * 397) ^ (this.Title.GetHashCode());
                hashCode = (hashCode * 397) ^ (this.FirstName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.LastName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Street?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.HouseNumber?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.CityToPostalCode.PostalCode?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.DateOfBirth?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.PhoneNumber1?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.PhoneNumber2?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.MobileNumber?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Telefax?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Email?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Bills?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format(Resources.Client_ToString, this.Id, this.FirstName, this.LastName,
                                 this.Street, this.HouseNumber, this.CityToPostalCode.PostalCode, this.CityToPostalCode.City);
        }
    }
}