// ///////////////////////////////////
// File: Bill.cs
// Last Change: 28.10.2017  11:50
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model
{
    using System;
    using System.Collections.Generic;
    using EpAccounting.Model.Enum;
    using EpAccounting.Model.Properties;



    public class Bill : ICloneable
    {
        #region Properties

        public virtual int Id { get; set; }

        public virtual Client Client { get; set; }

        public virtual bool? Printed { get; set; }

        public virtual KindOfBill? KindOfBill { get; set; }

        public virtual KindOfVat? KindOfVat { get; set; }

        public virtual double VatPercentage { get; set; }

        public virtual string Date { get; set; }

        public virtual IList<BillItem> BillItems { get; set; } = new List<BillItem>();

        #endregion



        #region ICloneable Members

        public virtual object Clone()
        {
            return (Bill)this.MemberwiseClone();
        }

        #endregion



        public virtual void AddBillItem(BillItem billItem)
        {
            if (billItem == null)
            {
                throw new ArgumentNullException(Resources.Exception_Message_Bill_BillItemHasNoReference);
            }

            billItem.Bill = this;
            this.BillItems.Add(billItem);
        }

        public override bool Equals(object obj)
        {
            Bill otherBill = obj as Bill;

            if (otherBill == null)
            {
                return false;
            }

            bool equalBillItem = this.Id == otherBill.Id && this.Client?.Id == otherBill.Client?.Id &&
                                 Equals(this.KindOfBill, otherBill.KindOfBill) && Equals(this.KindOfVat, otherBill.KindOfVat) &&
                                 this.VatPercentage.Equals(otherBill.VatPercentage) && Equals(this.Date, otherBill.Date);

            if (!equalBillItem || (this.BillItems.Count != otherBill.BillItems.Count))
            {
                return false;
            }

            for (int i = 0; i < this.BillItems.Count; i++)
            {
                if (!this.BillItems[i].Equals(otherBill.BillItems[i]))
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
                hashCode = (hashCode * 397) ^ (this.Client?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ this.KindOfBill.GetHashCode();
                hashCode = (hashCode * 397) ^ this.KindOfVat.GetHashCode();
                hashCode = (hashCode * 397) ^ this.VatPercentage.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Date.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.BillItems?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}