// ///////////////////////////////////
// File: MappingTest.cs
// Last Change: 22.10.2017  13:34
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Model.Mapping
{
    using System.Collections.Generic;
    using EpAccounting.Data;
    using EpAccounting.Model;
    using FluentNHibernate.Testing;
    using NUnit.Framework;



    [TestFixture]
    public class MappingTest
    {
        #region Setup/Teardown

        [SetUp]
        public void TestInit()
        {
            DatabaseFactory.DeleteTestFolderAndFile();
            DatabaseFactory.CreateTestFolder();
        }

        [TearDown]
        public void TestCleanup()
        {
            DatabaseFactory.DeleteTestFolderAndFile();
        }

        #endregion



        #region Test Methods

        [Test]
        public void CanMapArticle()
        {
            // Arrange
            NHibernateSessionManager sessionManager = new NHibernateSessionManager();
            sessionManager.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            new PersistenceSpecification<Article>(sessionManager.OpenSession())
                    .CheckProperty(c => c.Id, ModelFactory.DefaultId)
                    .CheckProperty(c => c.ArticleNumber, ModelFactory.DefaultArticleNumber)
                    .CheckProperty(c => c.Description, ModelFactory.DefaultArticleDescription)
                    .CheckProperty(c => c.Amount, ModelFactory.DefaultArticleAmount)
                    .CheckProperty(c => c.Price, ModelFactory.DefaultArticlePrice)
                    .VerifyTheMappings();
        }

        [Test]
        public void CanCorrectlyMapBillItem()
        {
            // Arrange
            NHibernateSessionManager sessionManager = new NHibernateSessionManager();
            sessionManager.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            new PersistenceSpecification<BillItem>(sessionManager.OpenSession(), new ObjectEqualityComparer())
                    .CheckProperty(c => c.Id, ModelFactory.DefaultId)
                    .CheckProperty(c => c.Position, ModelFactory.DefaultBillItemPosition)
                    .CheckProperty(c => c.ArticleNumber, ModelFactory.DefaultBillItemArticleNumber)
                    .CheckProperty(c => c.Description, ModelFactory.DefaultBillItemDescription)
                    .CheckProperty(c => c.Amount, ModelFactory.DefaultBillItemAmount)
                    .CheckProperty(c => c.Discount, ModelFactory.DefaultBillItemDiscount)
                    .CheckProperty(c => c.Price, ModelFactory.DefaultBillItemPrice)
                    .VerifyTheMappings();
        }

        [Test]
        public void CanCorrectlyMapBill()
        {
            // Arrange
            NHibernateSessionManager sessionManager = new NHibernateSessionManager();
            sessionManager.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            new PersistenceSpecification<Bill>(sessionManager.OpenSession(), new ObjectEqualityComparer())
                    .CheckProperty(c => c.Id, ModelFactory.DefaultId)
                    .CheckReference(c => c.Client, ModelFactory.GetDefaultClient())
                    .CheckProperty(c => c.Printed, ModelFactory.DefaultBillPrinted)
                    .CheckProperty(c => c.KindOfBill, ModelFactory.DefaultBillKindOfBill)
                    .CheckProperty(c => c.KindOfVat, ModelFactory.DefaultBillKindOfVat)
                    .CheckProperty(c => c.VatPercentage, ModelFactory.DefaultBillVatPercentage)
                    .CheckProperty(c => c.Date, ModelFactory.DefaultBillDate)
                    .CheckList(c => c.BillItems, new List<BillItem>() { ModelFactory.GetDefaultBillItem() })
                    .VerifyTheMappings();
        }

        [Test]
        public void CanCorrectlyMapClient()
        {
            // Arrange
            NHibernateSessionManager sessionManager = new NHibernateSessionManager();
            sessionManager.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            new PersistenceSpecification<Client>(sessionManager.OpenSession(), new ObjectEqualityComparer())
                    .CheckProperty(c => c.Id, ModelFactory.DefaultId)
                    .CheckProperty(c => c.Title, ModelFactory.DefaultClientTitle)
                    .CheckProperty(c => c.FirstName, ModelFactory.DefaultClientFirstName)
                    .CheckProperty(c => c.LastName, ModelFactory.DefaultClientLastName)
                    .CheckProperty(c => c.Street, ModelFactory.DefaultClientStreet)
                    .CheckProperty(c => c.HouseNumber, ModelFactory.DefaultClientHouseNumber)
                    .CheckReference(c => c.CityToPostalCode, ModelFactory.GetDefaultCityToPostalCode())
                    .CheckProperty(c => c.DateOfBirth, ModelFactory.DefaultClientDateOfBirth)
                    .CheckProperty(c => c.PhoneNumber1, ModelFactory.DefaultClientPhoneNumber1)
                    .CheckProperty(c => c.PhoneNumber2, ModelFactory.DefaultClientPhoneNumber2)
                    .CheckProperty(c => c.MobileNumber, ModelFactory.DefaultClientMobileNumber)
                    .CheckProperty(c => c.Telefax, ModelFactory.DefaultClientTelefax)
                    .CheckProperty(c => c.Email, ModelFactory.DefaultClientEmail)
                    .VerifyTheMappings();
        }

        [Test]
        public void CanCorrectlyMapCityToPostalCode()
        {
            // Arrange
            NHibernateSessionManager sessionManager = new NHibernateSessionManager();
            sessionManager.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            new PersistenceSpecification<CityToPostalCode>(sessionManager.OpenSession(), new ObjectEqualityComparer())
                    .CheckProperty(c => c.PostalCode, ModelFactory.DefaultCityToPostalCodePostalCode)
                    .CheckProperty(c => c.City, ModelFactory.DefaultCityToPostalCodeCity)
                    .VerifyTheMappings();
        }

        #endregion
    }
}