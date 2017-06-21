// ///////////////////////////////////
// File: ModelFactory.cs
// Last Change: 03.06.2017  20:07
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Model
{
    using System.Collections.Generic;
    using EpAccounting.Model;



    public static class ModelFactory
    {
        public const int DefaultId = 1;

        public const string DefaultArticleDescription = "Testartikel - Blitzschaden";

        public const string DefaultClientTitle = "Herr";
        public const string DefaultClientFirstName = "Andre";
        public const string DefaultClientLastName = "Multerer";
        public const string DefaultClientStreet = "Schmidstraße";
        public const string DefaultClientHouseNumber = "14";
        public const string DefaultClientPostalCode = "94234";
        public const string DefaultClientCity = "Viechtach";
        public const string DefaultClientDateOfBirth = "06.11.1993";
        public const string DefaultClientPhoneNumber1 = "09942/902097";
        public const string DefaultClientPhoneNumber2 = "09942 - 902099";
        public const string DefaultClientMobileNumber = "+49 160 55-07-869";
        public const string DefaultClientTelefax = "09942 / 902098";
        public const string DefaultClientEmail = "andre.multerer@gmail.com";

        public const string DefaultBillKindOfBill = "Rechnung";
        public const string DefaultBillKindOfVat = "inkl. MwSt";
        public const double DefaultBillVatPercentage = 19;
        public const string DefaultBillDate = "01.01.2017";

        public const int DefaultBillItemPosition = 2;
        public const int DefaultBillItemArticleNumber = 1;
        public const string DefaultBillItemDescription = "Test Description";
        public const double DefaultBillItemAmount = 3;
        public const double DefaultBillItemPrice = 23.32;
        public const double DefaultBillItemDiscount = 5.5;

        public static Client GetDefaultClient()
        {
            return new Client
                   {
                       Title = DefaultClientTitle,
                       FirstName = DefaultClientFirstName,
                       LastName = DefaultClientLastName,
                       Street = DefaultClientStreet,
                       HouseNumber = DefaultClientHouseNumber,
                       PostalCode = DefaultClientPostalCode,
                       City = DefaultClientCity,
                       DateOfBirth = DefaultClientDateOfBirth,
                       PhoneNumber1 = DefaultClientPhoneNumber1,
                       PhoneNumber2 = DefaultClientPhoneNumber2,
                       MobileNumber = DefaultClientMobileNumber,
                       Telefax = DefaultClientTelefax,
                       Email = DefaultClientEmail
                   };
        }

        public static Bill GetDefaultBill()
        {
            return new Bill()
                   {
                       KindOfBill = DefaultBillKindOfBill,
                       KindOfVat = DefaultBillKindOfVat,
                       VatPercentage = DefaultBillVatPercentage,
                       Date = DefaultBillDate,
                       Client = GetDefaultClient(),
                       BillItems = new List<BillItem>() { GetDefaultBillItem() }
                   };
        }

        public static BillItem GetDefaultBillItem()
        {
            return new BillItem()
                   {
                       Position = DefaultBillItemPosition,
                       ArticleNumber = DefaultBillItemArticleNumber,
                       Description = DefaultBillItemDescription,
                       Amount = DefaultBillItemAmount,
                       Price = DefaultBillItemPrice,
                       Discount = DefaultBillItemDiscount
                   };
        }

        public static Article GetDefaultArticle()
        {
            return new Article
                   {
                       Description = DefaultArticleDescription
                   };
        }
    }
}