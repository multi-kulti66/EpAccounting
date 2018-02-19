// ///////////////////////////////////
// File: ObjectEqualityComparer.cs
// Last Change: 22.10.2017  13:33
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Model.Mapping
{
    using System;
    using System.Collections;
    using EpAccounting.Model;



    public class ObjectEqualityComparer : IEqualityComparer
    {
        private const double MaxVariation = 0.01;


        public new bool Equals(object firstObject, object secondObject)
        {
            if (firstObject == null || secondObject == null)
            {
                return false;
            }

            if (firstObject is Client client1 && secondObject is Client client2)
            {
                return AreEqualClients(client1, client2);
            }

            if (firstObject is CityToPostalCode cityToPostalCode1 && secondObject is CityToPostalCode cityToPostalCode2)
            {
                return AreEqualCityToPostalCodes(cityToPostalCode1, cityToPostalCode2);
            }

            if (firstObject is Bill bill1 && secondObject is Bill bill2)
            {
                return AreEqualBills(bill1, bill2);
            }

            if (firstObject is BillItem billDetail1 && secondObject is BillItem billDetail2)
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
            if (client1.Id == client2.Id &&
                client1.Title == client2.Title &&
                client1.FirstName == client2.FirstName &&
                client1.LastName == client2.LastName &&
                client1.Street == client2.Street &&
                client1.HouseNumber == client2.HouseNumber &&
                client1.DateOfBirth == client2.DateOfBirth &&
                client1.PhoneNumber1 == client2.PhoneNumber1 &&
                client1.PhoneNumber2 == client2.PhoneNumber2 &&
                client1.MobileNumber == client2.MobileNumber &&
                client1.Telefax == client2.Telefax &&
                client1.Email == client2.Email)
            {
                return true;
            }

            return false;
        }

        private static bool AreEqualCityToPostalCodes(CityToPostalCode cityToPostalCode1, CityToPostalCode cityToPostalCode2)
        {
            if (cityToPostalCode1.PostalCode == cityToPostalCode2.PostalCode &&
                cityToPostalCode1.City == cityToPostalCode2.City)
            {
                return true;
            }

            return false;
        }

        private static bool AreEqualBills(Bill bill1, Bill bill2)
        {
            if (bill1.Id == bill2.Id &&
                bill1.KindOfBill == bill2.KindOfBill &&
                bill1.KindOfVat == bill2.KindOfVat &&
                Math.Abs(bill1.VatPercentage - bill2.VatPercentage) < MaxVariation &&
                bill1.Date == bill2.Date)
            {
                return true;
            }

            return false;
        }

        private static bool AreEqualBillItems(BillItem billDetail1, BillItem billDetail2)
        {
            if (billDetail1.Id == billDetail2.Id &&
                billDetail1.Position == billDetail2.Position &&
                billDetail1.ArticleNumber == billDetail2.ArticleNumber &&
                billDetail1.Description == billDetail2.Description &&
                Math.Abs(billDetail1.Amount - billDetail2.Amount) < MaxVariation &&
                Math.Abs(billDetail1.Discount - billDetail2.Discount) < MaxVariation &&
                billDetail1.Price == billDetail2.Price)
            {
                return true;
            }

            return false;
        }
    }
}