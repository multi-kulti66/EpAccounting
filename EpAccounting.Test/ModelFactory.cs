// ///////////////////////////////////
// File: ModelFactory.cs
// Last Change: 23.10.2017  21:04
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test
{
    using System.Collections.Generic;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;



    public static class ModelFactory
    {
        #region Fields

        public const int DefaultId = 1001;

        public const int DefaultArticleNumber = 3;
        public const string DefaultArticleDescription = "Testartikel - Blitzschaden";
        public const double DefaultArticleAmount = 4;
        public const decimal DefaultArticlePrice = 2.5m;

        public const ClientTitle DefaultClientTitle = ClientTitle.Herr;
        public const string DefaultClientFirstName = "Andre";
        public const string DefaultClientLastName = "Multerer";
        public const string DefaultClientStreet = "Schmidstraße";
        public const string DefaultClientHouseNumber = "14";
        public const string DefaultClientDateOfBirth = "06.11.1993";
        public const string DefaultClientPhoneNumber1 = "09942/902097";
        public const string DefaultClientPhoneNumber2 = "09942 - 902099";
        public const string DefaultClientMobileNumber = "+49 160 55-07-869";
        public const string DefaultClientTelefax = "09942 / 902098";
        public const string DefaultClientEmail = "andre.multerer@gmail.com";

        public const string DefaultCityToPostalCodePostalCode = "94234";
        public const string DefaultCityToPostalCodeCity = "Viechtach";

        public const bool DefaultBillPrinted = true;
        public const KindOfBill DefaultBillKindOfBill = KindOfBill.Rechnung;
        public const KindOfVat DefaultBillKindOfVat = KindOfVat.inkl_MwSt;
        public const double DefaultBillVatPercentage = 19;

        public const int DefaultBillItemPosition = 1;
        public const int DefaultBillItemArticleNumber = 1;
        public const string DefaultBillItemDescription = "Test Description";
        public const double DefaultBillItemAmount = 3;
        public const decimal DefaultBillItemPrice = 23.32m;
        public const double DefaultBillItemDiscount = 5.5;
        public const string DefaultBillDate = "01.01.2017";

        #endregion



        public static Client GetDefaultClient()
        {
            return new Client
                   {
                       Title = DefaultClientTitle,
                       FirstName = DefaultClientFirstName,
                       LastName = DefaultClientLastName,
                       Street = DefaultClientStreet,
                       HouseNumber = DefaultClientHouseNumber,
                       CityToPostalCode = GetDefaultCityToPostalCode(),
                       DateOfBirth = DefaultClientDateOfBirth,
                       PhoneNumber1 = DefaultClientPhoneNumber1,
                       PhoneNumber2 = DefaultClientPhoneNumber2,
                       MobileNumber = DefaultClientMobileNumber,
                       Telefax = DefaultClientTelefax,
                       Email = DefaultClientEmail
                   };
        }

        public static CityToPostalCode GetDefaultCityToPostalCode()
        {
            return new CityToPostalCode
                   {
                       PostalCode = DefaultCityToPostalCodePostalCode,
                       City = DefaultCityToPostalCodeCity
                   };
        }

        public static Bill GetDefaultBill()
        {
            return new Bill()
                   {
                        Printed = DefaultBillPrinted,
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
                       ArticleNumber = DefaultArticleNumber,
                       Description = DefaultArticleDescription,
                       Amount = DefaultArticleAmount,
                       Price = DefaultArticlePrice
                   };
        }
    }
}