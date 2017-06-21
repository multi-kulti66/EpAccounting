// ///////////////////////////////////
// File: NHibernateRepositoryTest.cs
// Last Change: 18.03.2017  22:25
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Business
{
    using System;
    using EpAccounting.Business;
    using EpAccounting.Data;
    using EpAccounting.Model;
    using EpAccounting.Test.Model;
    using FluentAssertions;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class NHibernateRepositoryTest
    {
        private IRepository repository;

        [SetUp]
        public void TestInit()
        {
            DatabaseFactory.DeleteTestFolderAndFile();
            DatabaseFactory.ClearSavedFilePath();
        }

        [TearDown]
        public void TestCleanup()
        {
            DatabaseFactory.DeleteTestFolderAndFile();
            DatabaseFactory.ClearSavedFilePath();
        }

        [Test]
        public void EmptyFilePathIfNotConnected()
        {
            // Act
            this.CreateRepository();

            // Assert
            this.repository.FilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void CreateDatabaseIfDatabaseNotAlreadyExists()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();
            this.CreateRepository();

            // Act
            this.repository.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            FileAssert.Exists(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void OverwritesDatabaseByCreationWhenFolderContainsEquallyNamedDatabase()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();

            // Act
            Action action = () => this.repository.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            action.ShouldNotThrow<Exception>();
        }

        [Test]
        public void IsInitializedAfterDatabaseCreation()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();
            this.CreateRepository();

            // Act
            this.repository.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this.repository.IsConnected.Should().BeTrue();
        }

        [Test]
        public void FilePathSetIfDatabaseCreated()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();
            this.CreateRepository();

            // Act
            this.repository.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this.repository.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void LoadDatabaseIfDatabaseAlreadyExists()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();

            // Act
            Action action = () => this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            action.ShouldNotThrow<Exception>();
        }

        [Test]
        public void ThrowExceptionIfDatabaseShouldBeLoadedInFolderWhereDatabaseNotExists()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();
            this.CreateRepository();

            // Act
            Action action = () => this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void IsConnectedAfterDatabaseLoaded()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();

            // Act
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this.repository.IsConnected.Should().BeTrue();
        }

        [Test]
        public void GetFilePathIfDatabaseLoaded()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();

            // Act
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this.repository.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void NotConnectedAfterClosingDatabaseConnection()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            this.repository.CloseDatabase();

            // Assert
            this.repository.IsConnected.Should().BeFalse();
        }

        [Test]
        public void GetEmptyFilePathAfterClosingDatabaseConnection()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            this.repository.CloseDatabase();

            // Assert
            this.repository.FilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void AddClientToDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();

            // Act
            this.repository.SaveOrUpdate(client);

            // Assert
            this.repository.GetById<Client>(1).FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this.repository.GetById<Client>(1).LastName.Should().Be(ModelFactory.DefaultClientLastName);
        }

        [Test]
        public void AddBillToDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            this.repository.SaveOrUpdate(bill.Client);
            this.repository.SaveOrUpdate(bill);

            // Assert
            Assert.AreEqual(ModelFactory.DefaultBillDate, this.repository.GetById<Bill>(1).Date);
            Assert.AreEqual(ModelFactory.DefaultBillKindOfBill, this.repository.GetById<Bill>(1).KindOfBill);
        }

        [Test]
        public void ThrowExceptionWhenInvalidObjectWillBeAddedToDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);
            Person person = new Person { Name = "Andre Multerer" };

            // Act
            Action action = () => this.repository.SaveOrUpdate(person);

            // Assert
            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void ThrowExceptionWhenClientCanNotBeUpdatedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();
            Client client = new Client();

            // Act
            Action action = () => this.repository.SaveOrUpdate(client);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenBillCanNotBeUpdatedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();
            Bill bill = new Bill();

            // Act
            Action action = () => this.repository.SaveOrUpdate(bill);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void UpdateExistingClient()
        {
            // Arrange
            const string NewFirstName = "Maximilian";

            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();
            this.repository.SaveOrUpdate(client);

            // Act
            client.FirstName = NewFirstName;
            this.repository.SaveOrUpdate(client);

            // Assert
            this.repository.GetById<Client>(1).FirstName.Should().Be(NewFirstName);
            this.repository.GetById<Client>(1).LastName.Should().Be(ModelFactory.DefaultClientLastName);
        }

        [Test]
        public void UpdateExistingBill()
        {
            // Arrange
            const string NewBillDate = "02.01.2017";
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();
            this.repository.SaveOrUpdate(bill.Client);
            this.repository.SaveOrUpdate(bill);

            // Act
            bill.Date = NewBillDate;
            this.repository.SaveOrUpdate(bill);

            // Assert
            this.repository.GetById<Bill>(1).Date.Should().Be(NewBillDate);
        }

        [Test]
        public void ThrowExceptionWhenClientCanNotBeDeletedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();
            Client client = new Client();

            // Act
            Action action = () => this.repository.Delete(client);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenBillCanNotBeDeletedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();
            Bill bill = new Bill();

            // Act
            Action action = () => this.repository.Delete(bill);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenClientCanNotBeDeletedBecauseClientNotInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();

            // Act
            Action action = () => this.repository.Delete(client);

            // Assert
            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void ThrowExceptionWhenBillCanNotBeDeletedBecauseBillNotInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            Action action = () => this.repository.Delete(bill);

            // Assert
            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void DeleteClientFromDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();
            this.repository.SaveOrUpdate(client);

            // Act
            this.repository.Delete(client);

            // Assert
            this.repository.GetAll<Client>().Should().HaveCount(0);
        }

        [Test]
        public void DeleteBillFromDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();
            this.repository.SaveOrUpdate(bill.Client);
            this.repository.SaveOrUpdate(bill);

            // Act
            this.repository.Delete(bill);

            // Assert
            this.repository.GetAll<Bill>().Should().HaveCount(0);
        }

        [Test]
        public void ThrowExceptionWhenClientsCanNotBeCountedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            TestDelegate methodDelegate = () => this.repository.GetQuantity<Client>();

            // Assert
            Assert.That(methodDelegate, Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void ThrowExceptionWhenBillsCanNotBeCountedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            TestDelegate methodDelegate = () => this.repository.GetQuantity<Bill>();

            // Assert
            Assert.That(methodDelegate, Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void ReturnZeroQuantityWhenNoClientsInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this.repository.GetQuantity<Client>().Should().Be(0);
        }

        [Test]
        public void ReturnZeroQuantityWhenNoBillsInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this.repository.GetQuantity<Bill>().Should().Be(0);
        }

        [Test]
        public void GetNumberOfClientsFromDatabase()
        {
            // Arrange
            const int quantity = 5;
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client();
                this.repository.SaveOrUpdate(client);
            }

            // Assert
            this.repository.GetQuantity<Client>().Should().Be(quantity);
        }

        [Test]
        public void GetNumberOfBillsFromDatabase()
        {
            // Arrange
            const int quantity = 10;
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill();
                this.repository.SaveOrUpdate(bill);
            }

            // Assert
            this.repository.GetQuantity<Bill>().Should().Be(quantity);
        }

        [Test]
        public void ThrowExceptionWhenAllClientsCanNotBeLoadedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            Action action = () => this.repository.GetAll<Client>();

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenAllBillsCanNotBeLoadedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            Action action = () => this.repository.GetAll<Bill>();

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void GetEmptyClientListWhenNoClientsInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this.repository.GetAll<Client>().Should().HaveCount(0);
        }

        [Test]
        public void GetEmptyBillListWhenNoBillsInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this.repository.GetAll<Bill>().Should().HaveCount(0);
        }

        [Test]
        public void GetAllClientsFromDatabase()
        {
            // Arrange
            const int quantity = 6;
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client();
                this.repository.SaveOrUpdate(client);
            }

            // Assert
            this.repository.GetAll<Client>().Should().HaveCount(quantity);
        }

        [Test]
        public void GetAllBillsFromDatabase()
        {
            // Arrange
            const int quantity = 14;
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill();
                this.repository.SaveOrUpdate(bill);
            }

            // Assert
            this.repository.GetAll<Bill>().Should().HaveCount(quantity);
        }

        [Test]
        public void ThrowExceptionWhenClientCanNotBeGetByIdBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            Action action = () => this.repository.GetById<Client>(1);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenBillCanNotBeGetByIdBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            Action action = () => this.repository.GetById<Bill>(1);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ReturnNullByNotExistingClientId()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            Client actualClient = this.repository.GetById<Client>(1);

            // Assert
            actualClient.Should().BeNull();
        }

        [Test]
        public void ReturnNullByNotExistingBillId()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            Bill actualBill = this.repository.GetById<Bill>(1);

            // Assert
            actualBill.Should().BeNull();
        }

        [Test]
        public void GetClientById()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();

            const int ExpectedEmptyClients = 10;

            for (int i = 0; i < ExpectedEmptyClients; i++)
            {
                this.repository.SaveOrUpdate(new Client());
            }

            this.repository.SaveOrUpdate(client);

            // Act
            Client actualClient = this.repository.GetById<Client>(ExpectedEmptyClients + 1);

            // Assert
            actualClient.Should().NotBeNull();
            actualClient.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            actualClient.LastName.Should().Be(ModelFactory.DefaultClientLastName);
        }

        [Test]
        public void GetBillById()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();

            const int ExpectedEmptyBills = 5;

            for (int i = 0; i < ExpectedEmptyBills; i++)
            {
                this.repository.SaveOrUpdate(new Bill());
            }

            this.repository.SaveOrUpdate(bill.Client);
            this.repository.SaveOrUpdate(bill);

            // Act
            Bill actualBill = this.repository.GetById<Bill>(ExpectedEmptyBills + 1);

            // Assert
            actualBill.Should().NotBeNull();
            actualBill.Date.Should().Be(ModelFactory.DefaultBillDate);
            actualBill.KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void ThrowExceptionWhenClientCanNotBeGetByCriteriaBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Client>(client => client.FirstName.IsLike("test")));

            // Act
            Action action = () => this.repository.GetByCriteria<Client>(conjunction);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenBillCanNotBeGetByCriteriaBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Bill>(bill => bill.KindOfBill.IsLike("test")));

            // Act
            Action action = () => this.repository.GetByCriteria<Bill>(conjunction);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void GetEmptyClientListWhenNoClientsEqualCriteria()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Client>(client => client.FirstName.IsLike("test", MatchMode.Anywhere)));

            // Act
            for (int i = 0; i < 5; i++)
            {
                Client client = new Client();
                this.repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < 5; i++)
            {
                Client client = ModelFactory.GetDefaultClient();
                this.repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < 5; i++)
            {
                Client client = new Client() { FirstName = "Andreas" };
                this.repository.SaveOrUpdate(client);
            }

            // Assert
            this.repository.GetByCriteria<Client>(conjunction).Should().HaveCount(0);
        }

        [Test]
        public void GetEmptyBillListWhenNoBillsEqualCriteria()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Bill>(bill => bill.KindOfBill == "test"));

            // Act
            for (int i = 0; i < 5; i++)
            {
                Bill bill = new Bill();
                this.repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < 5; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                this.repository.SaveOrUpdate(bill.Client);
                this.repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < 5; i++)
            {
                Bill bill = new Bill() { KindOfBill = "Bescheinigung" };
                this.repository.SaveOrUpdate(bill);
            }

            // Assert
            this.repository.GetByCriteria<Bill>(conjunction).Should().HaveCount(0);
        }

        [Test]
        public void GetClientsByCriteria()
        {
            // Arrange
            const int quantity = 7;
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Client>(client => client.FirstName.IsLike("ndr", MatchMode.Anywhere)));

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client();
                this.repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < quantity; i++)
            {
                Client client = ModelFactory.GetDefaultClient();
                this.repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client() { FirstName = "Andreas" };
                this.repository.SaveOrUpdate(client);
            }

            // Assert
            this.repository.GetByCriteria<Client>(conjunction).Should().HaveCount(2 * quantity);
        }

        [Test]
        public void GetBillsByCriteria()
        {
            // Arrange
            const int quantity = 6;
            this.CreateRepositoryWithLoadedDatabase();
            this.repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Bill>(bill => bill.KindOfBill == ModelFactory.DefaultBillKindOfBill));

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill();
                this.repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                this.repository.SaveOrUpdate(bill.Client);
                this.repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill() { KindOfBill = "Bescheinigung" };
                this.repository.SaveOrUpdate(bill);
            }

            // Assert
            this.repository.GetByCriteria<Bill>(conjunction).Should().HaveCount(quantity);
        }

        private void CreateDatabaseAndSetPath()
        {
            DatabaseFactory.CreateTestFile();
            DatabaseFactory.SetSavedFilePath();
        }

        private void CreateRepository()
        {
            this.repository = new NHibernateRepository(new NHibernateSessionManager());
        }

        private void CreateRepositoryWithLoadedDatabase()
        {
            this.CreateDatabaseAndSetPath();
            this.CreateRepository();
        }

        private class Person
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Name { get; set; }
        }
    }
}