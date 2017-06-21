// ///////////////////////////////////
// File: ObjectEqualityComparer.cs
// Last Change: 13.03.2017  20:44
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Model.Mapping
{
    using System;
    using System.Collections;
    using EpAccounting.Model;



    public class ObjectEqualityComparer : IEqualityComparer
    {
        private const double MAX_VARIATION = 0.01;

        public new bool Equals(object firstObject, object secondObject)
        {
            if ((firstObject == null) || (secondObject == null))
            {
                return false;
            }

            Client client1 = firstObject as Client;
            Client client2 = secondObject as Client;

            if (client1 != null && client2 != null)
            {
                return AreEqualClients(client1, client2);
            }

            Bill bill1 = firstObject as Bill;
            Bill bill2 = secondObject as Bill;

            if (bill1 != null && bill2 != null)
            {
                return AreEqualBills(bill1, bill2);
            }

            BillItem billDetail1 = firstObject as BillItem;
            BillItem billDetail2 = secondObject as BillItem;

            if (billDetail1 != null && billDetail2 != null)
            {
                return AreEqualBillItems(billDetail1, billDetail2);
            }

            return firstObject.Equals(secondObject);
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        private static bool AreEqualClients(Client client1, Client client2)
        {
            if ((client1.ClientId == client2.ClientId) &&
                (client1.Title == client2.Title) &&
                (client1.FirstName == client2.FirstName) &&
                (client1.LastName == client2.LastName) &&
                (client1.Street == client2.Street) &&
                (client1.HouseNumber == client2.HouseNumber) &&
                (client1.PostalCode == client2.PostalCode) &&
                (client1.City == client2.City) &&
                (client1.DateOfBirth == client2.DateOfBirth) &&
                (client1.PhoneNumber1 == client2.PhoneNumber1) &&
                (client1.PhoneNumber2 == client2.PhoneNumber2) &&
                (client1.MobileNumber == client2.MobileNumber) &&
                (client1.Telefax == client2.Telefax) &&
                (client1.Email == client2.Email))
            {
                return true;
            }

            return false;
        }

        private static bool AreEqualBills(Bill bill1, Bill bill2)
        {
            if ((bill1.BillId == bill2.BillId) &&
                (bill1.KindOfBill == bill2.KindOfBill) &&
                (bill1.KindOfVat == bill2.KindOfVat) &&
                (Math.Abs(bill1.VatPercentage - bill2.VatPercentage) < MAX_VARIATION) &&
                (bill1.Date == bill2.Date))
            {
                return true;
            }

            return false;
        }

        private static bool AreEqualBillItems(BillItem billDetail1, BillItem billDetail2)
        {
            if ((billDetail1.BillItemId == billDetail2.BillItemId) &&
                (billDetail1.Position == billDetail2.Position) &&
                (billDetail1.ArticleNumber == billDetail2.ArticleNumber) &&
                (billDetail1.Description == billDetail2.Description) &&
                (Math.Abs(billDetail1.Amount - billDetail2.Amount) < MAX_VARIATION) &&
                (Math.Abs(billDetail1.Discount - billDetail2.Discount) < MAX_VARIATION) &&
                (Math.Abs(billDetail1.Price - billDetail2.Price) < MAX_VARIATION))
            {
                return true;
            }

            return false;
        }
    }
}