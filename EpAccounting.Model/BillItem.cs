// ///////////////////////////////////
// File: BillItem.cs
// Last Change: 18.09.2017  20:38
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model
{
    using System;



    public class BillItem : ICloneable
    {
        #region Properties

        public virtual int Id { get; set; }

        public virtual Bill Bill { get; set; }

        public virtual int Position { get; set; }

        public virtual int ArticleNumber { get; set; }

        public virtual string Description { get; set; }

        public virtual double Amount { get; set; }

        public virtual decimal Price { get; set; }

        public virtual double Discount { get; set; }

        #endregion



        #region ICloneable Members

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion



        public override bool Equals(object obj)
        {
            BillItem otherBillItem = obj as BillItem;

            if (otherBillItem == null)
            {
                return false;
            }

            return this.Id == otherBillItem.Id && this.Bill?.Id == otherBillItem.Bill?.Id &&
                   this.Position == otherBillItem.Position && this.ArticleNumber == otherBillItem.ArticleNumber &&
                   string.Equals(this.Description, otherBillItem.Description) && this.Amount.Equals(otherBillItem.Amount) &&
                   this.Price.Equals(otherBillItem.Price) && this.Discount.Equals(otherBillItem.Discount);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Id;
                hashCode = (hashCode * 397) ^ (this.Bill?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ this.Position;
                hashCode = (hashCode * 397) ^ this.ArticleNumber;
                hashCode = (hashCode * 397) ^ (this.Description?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ this.Amount.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Price.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Discount.GetHashCode();
                return hashCode;
            }
        }
    }
}