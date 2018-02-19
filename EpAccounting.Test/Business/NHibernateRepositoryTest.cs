// ///////////////////////////////////
// File: NHibernateRepositoryTest.cs
// Last Change: 05.09.2017  20:20
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Business
{
    using System;
    using EpAccounting.Business;
    using EpAccounting.Data;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using FluentAssertions;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class NHibernateRepositoryTest
    {
        private IRepository _repository;


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
            this._repository.FilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void CreateDatabaseIfDatabaseNotAlreadyExists()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();
            this.CreateRepository();

            // Act
            this._repository.CreateDatabase(DatabaseFactory.TestFilePath);

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
            Action action = () => this._repository.CreateDatabase(DatabaseFactory.TestFilePath);

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
            this._repository.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this._repository.IsConnected.Should().BeTrue();
        }

        [Test]
        public void FilePathSetIfDatabaseCreated()
        {
            // Arrange
            DatabaseFactory.CreateTestFolder();
            this.CreateRepository();

            // Act
            this._repository.CreateDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this._repository.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void LoadDatabaseIfDatabaseAlreadyExists()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();

            // Act
            Action action = () => this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

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
            Action action = () => this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

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
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this._repository.IsConnected.Should().BeTrue();
        }

        [Test]
        public void GetFilePathIfDatabaseLoaded()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();

            // Act
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this._repository.FilePath.Should().Be(DatabaseFactory.TestFilePath);
        }

        [Test]
        public void NotConnectedAfterClosingDatabaseConnection()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            this._repository.CloseDatabase();

            // Assert
            this._repository.IsConnected.Should().BeFalse();
        }

        [Test]
        public void GetEmptyFilePathAfterClosingDatabaseConnection()
        {
            // Arrange
            DatabaseFactory.CreateTestFile();
            this.CreateRepository();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            this._repository.CloseDatabase();

            // Assert
            this._repository.FilePath.Should().BeNullOrEmpty();
        }

        [Test]
        public void AddClientToDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();

            // Act
            this._repository.SaveOrUpdate(client);

            // Assert
            this._repository.GetById<Client>(1).FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this._repository.GetById<Client>(1).LastName.Should().Be(ModelFactory.DefaultClientLastName);
        }

        [Test]
        public void AddBillToDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            this._repository.SaveOrUpdate(bill.Client);
            this._repository.SaveOrUpdate(bill);

            // Assert
            Assert.AreEqual(ModelFactory.DefaultBillDate, this._repository.GetById<Bill>(1).Date);
            Assert.AreEqual(ModelFactory.DefaultBillKindOfBill, this._repository.GetById<Bill>(1).KindOfBill);
        }

        [Test]
        public void ThrowExceptionWhenInvalidObjectWillBeAddedToDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);
            Person person = new Person { Name = "Andre Multerer" };

            // Act
            Action action = () => this._repository.SaveOrUpdate(person);

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
            Action action = () => this._repository.SaveOrUpdate(client);

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
            Action action = () => this._repository.SaveOrUpdate(bill);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void UpdateExistingClient()
        {
            // Arrange
            const string newFirstName = "Maximilian";

            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();
            this._repository.SaveOrUpdate(client);

            // Act
            client.FirstName = newFirstName;
            this._repository.SaveOrUpdate(client);

            // Assert
            this._repository.GetById<Client>(1).FirstName.Should().Be(newFirstName);
            this._repository.GetById<Client>(1).LastName.Should().Be(ModelFactory.DefaultClientLastName);
        }

        [Test]
        public void UpdateExistingBill()
        {
            // Arrange
            const string newBillDate = "01.02.2017";
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();
            this._repository.SaveOrUpdate(bill.Client);
            this._repository.SaveOrUpdate(bill);

            // Act
            bill.Date = newBillDate;
            this._repository.SaveOrUpdate(bill);

            // Assert
            this._repository.GetById<Bill>(1).Date.Should().Be(newBillDate);
        }

        [Test]
        public void ThrowExceptionWhenClientCanNotBeDeletedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();
            Client client = new Client();

            // Act
            Action action = () => this._repository.Delete(client);

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
            Action action = () => this._repository.Delete(bill);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenClientCanNotBeDeletedBecauseClientNotInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();

            // Act
            Action action = () => this._repository.Delete(client);

            // Assert
            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void ThrowExceptionWhenBillCanNotBeDeletedBecauseBillNotInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            Action action = () => this._repository.Delete(bill);

            // Assert
            action.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void DeleteClientFromDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();
            this._repository.SaveOrUpdate(client);

            // Act
            this._repository.Delete(client);

            // Assert
            this._repository.GetAll<Client>(1).Should().HaveCount(0);
        }

        [Test]
        public void DeleteBillFromDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();
            this._repository.SaveOrUpdate(bill.Client);
            this._repository.SaveOrUpdate(bill);

            // Act
            this._repository.Delete(bill);

            // Assert
            this._repository.GetAll<Bill>(1).Should().HaveCount(0);
        }

        [Test]
        public void ThrowExceptionWhenClientsCanNotBeCountedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            TestDelegate methodDelegate = () => this._repository.GetQuantity<Client>();

            // Assert
            Assert.That(methodDelegate, Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void ThrowExceptionWhenBillsCanNotBeCountedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            TestDelegate methodDelegate = () => this._repository.GetQuantity<Bill>();

            // Assert
            Assert.That(methodDelegate, Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void ReturnZeroQuantityWhenNoClientsInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this._repository.GetQuantity<Client>().Should().Be(0);
        }

        [Test]
        public void ReturnZeroQuantityWhenNoBillsInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this._repository.GetQuantity<Bill>().Should().Be(0);
        }

        [Test]
        public void GetNumberOfClientsFromDatabase()
        {
            // Arrange
            const int quantity = 5;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client();
                this._repository.SaveOrUpdate(client);
            }

            // Assert
            this._repository.GetQuantity<Client>().Should().Be(quantity);
        }

        [Test]
        public void GetNumberOfBillsFromDatabase()
        {
            // Arrange
            const int quantity = 10;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill();
                this._repository.SaveOrUpdate(bill);
            }

            // Assert
            this._repository.GetQuantity<Bill>().Should().Be(quantity);
        }

        [Test]
        public void ThrowExceptionWhenAllClientsCanNotBeLoadedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            Action action = () => this._repository.GetAll<Client>(1);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenAllBillsCanNotBeLoadedBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            Action action = () => this._repository.GetAll<Bill>(1);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void GetEmptyClientListWhenNoClientsInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this._repository.GetAll<Client>(1).Should().HaveCount(0);
        }

        [Test]
        public void GetEmptyBillListWhenNoBillsInDatabase()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Assert
            this._repository.GetAll<Bill>(1).Should().HaveCount(0);
        }

        [Test]
        public void GetAllClientsFromDatabase()
        {
            // Arrange
            const int quantity = 6;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client();
                this._repository.SaveOrUpdate(client);
            }

            // Assert
            this._repository.GetAll<Client>(1).Should().HaveCount(quantity);
        }

        [Test]
        public void GetAllBillsFromDatabase()
        {
            // Arrange
            const int quantity = 14;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill();
                this._repository.SaveOrUpdate(bill);
            }

            // Assert
            this._repository.GetAll<Bill>(1).Should().HaveCount(quantity);
        }

        [Test]
        public void ThrowExceptionWhenClientCanNotBeGetByIdBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            Action action = () => this._repository.GetById<Client>(1);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenBillCanNotBeGetByIdBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            // Act
            Action action = () => this._repository.GetById<Bill>(1);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ReturnNullByNotExistingClientId()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            Client actualClient = this._repository.GetById<Client>(1);

            // Assert
            actualClient.Should().BeNull();
        }

        [Test]
        public void ReturnNullByNotExistingBillId()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            // Act
            Bill actualBill = this._repository.GetById<Bill>(1);

            // Assert
            actualBill.Should().BeNull();
        }

        [Test]
        public void GetClientById()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Client client = ModelFactory.GetDefaultClient();

            const int expectedEmptyClients = 10;

            for (int i = 0; i < expectedEmptyClients; i++)
            {
                this._repository.SaveOrUpdate(new Client());
            }

            this._repository.SaveOrUpdate(client);

            // Act
            Client actualClient = this._repository.GetById<Client>(expectedEmptyClients + 1);

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
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Bill bill = ModelFactory.GetDefaultBill();

            const int expectedEmptyBills = 5;

            for (int i = 0; i < expectedEmptyBills; i++)
            {
                this._repository.SaveOrUpdate(new Bill());
            }

            this._repository.SaveOrUpdate(bill.Client);
            this._repository.SaveOrUpdate(bill);

            // Act
            Bill actualBill = this._repository.GetById<Bill>(expectedEmptyBills + 1);

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
            Action action = () => this._repository.GetByCriteria<Client>(conjunction, 1);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void ThrowExceptionWhenBillCanNotBeGetByCriteriaBecauseDatabaseNotConnected()
        {
            // Arrange
            this.CreateRepository();

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Bill>(bill => bill.Date.IsLike("23", MatchMode.Anywhere)));

            // Act
            Action action = () => this._repository.GetByCriteria<Bill>(conjunction, 1);

            // Assert
            action.ShouldThrow<NullReferenceException>();
        }

        [Test]
        public void GetEmptyClientListWhenNoClientsEqualCriteria()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Client>(client => client.FirstName.IsLike("test", MatchMode.Anywhere)));

            // Act
            for (int i = 0; i < 5; i++)
            {
                Client client = new Client();
                this._repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < 5; i++)
            {
                Client client = ModelFactory.GetDefaultClient();
                this._repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < 5; i++)
            {
                Client client = new Client() { FirstName = "Andreas" };
                this._repository.SaveOrUpdate(client);
            }

            // Assert
            this._repository.GetByCriteria<Client>(conjunction, 1).Should().HaveCount(0);
        }

        [Test]
        public void GetEmptyBillListWhenNoBillsEqualCriteria()
        {
            // Arrange
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Bill>(bill => bill.Date.IsLike("05.08.2017", MatchMode.Anywhere)));

            // Act
            for (int i = 0; i < 5; i++)
            {
                Bill bill = new Bill();
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < 5; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                this._repository.SaveOrUpdate(bill.Client);
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < 5; i++)
            {
                Bill bill = new Bill() { KindOfBill = KindOfBill.Kostenvoranschlag };
                this._repository.SaveOrUpdate(bill);
            }

            // Assert
            this._repository.GetByCriteria<Bill>(conjunction, 1).Should().HaveCount(0);
        }

        [Test]
        public void GetClientsByCriteria()
        {
            // Arrange
            const int quantity = 7;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Client>(client => client.FirstName.IsLike("ndr", MatchMode.Anywhere)));

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client();
                this._repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < quantity; i++)
            {
                Client client = ModelFactory.GetDefaultClient();
                this._repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client() { FirstName = "Andreas" };
                this._repository.SaveOrUpdate(client);
            }

            // Assert
            this._repository.GetByCriteria<Client>(conjunction, 1).Should().HaveCount(2 * quantity);
        }

        [Test]
        public void GetBillsByCriteria()
        {
            // Arrange
            const int quantity = 6;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Bill>(bill => bill.KindOfBill == ModelFactory.DefaultBillKindOfBill));

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill();
                bill.KindOfBill = KindOfBill.Angebot;
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                this._repository.SaveOrUpdate(bill.Client);
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill() { KindOfBill = KindOfBill.Kostenvoranschlag };
                this._repository.SaveOrUpdate(bill);
            }

            // Assert
            this._repository.GetByCriteria<Bill>(conjunction, 1).Should().HaveCount(quantity);
        }

        [Test]
        public void GetBillsByCriteriaForClientAndBill()
        {
            // Arrange
            const int quantity = 6;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(bill => bill.KindOfBill == ModelFactory.DefaultBillKindOfBill));

            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.FirstName == ModelFactory.DefaultClientFirstName));

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                this._repository.SaveOrUpdate(bill.Client);
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                bill.Client.FirstName = "Manuel";
                this._repository.SaveOrUpdate(bill.Client);
                this._repository.SaveOrUpdate(bill);
            }

            // Assert
            this._repository.GetByCriteria<Bill, Client>(billConjunction, b => b.Client, clientConjunction, 1).Should().HaveCount(quantity);
        }

        [Test]
        public void RestrictBillsByPageSize()
        {
            // Arrange
            const int pageSize = 50;
            const int quantity = 54;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Bill>(bill => bill.KindOfBill == ModelFactory.DefaultBillKindOfBill));

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill();
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                this._repository.SaveOrUpdate(bill.Client);
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill() { KindOfBill = KindOfBill.Kostenvoranschlag };
                this._repository.SaveOrUpdate(bill);
            }

            // Assert
            this._repository.GetByCriteria<Bill>(conjunction, 1).Should().HaveCount(pageSize);
        }

        [Test]
        public void GetQuantityOfClientsByCriteria()
        {
            // Arrange
            const int quantity = 7;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Client>(client => client.FirstName.IsLike("ndr", MatchMode.Anywhere)));

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client();
                this._repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < quantity; i++)
            {
                Client client = ModelFactory.GetDefaultClient();
                this._repository.SaveOrUpdate(client);
            }

            for (int i = 0; i < quantity; i++)
            {
                Client client = new Client() { FirstName = "Andreas" };
                this._repository.SaveOrUpdate(client);
            }

            // Assert
            this._repository.GetQuantityByCriteria<Client>(conjunction).Should().Be(2 * quantity);
        }

        [Test]
        public void GetQuantityOfBillsByCriteria()
        {
            // Arrange
            const int quantity = 2;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction conjunction = Restrictions.Conjunction();
            conjunction.Add(Restrictions.Where<Bill>(bill => bill.KindOfBill == ModelFactory.DefaultBillKindOfBill));

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill();
                bill.KindOfBill = KindOfBill.Gutschrift;
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                this._repository.SaveOrUpdate(bill.Client);
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = new Bill() { KindOfBill = KindOfBill.Kostenvoranschlag };
                this._repository.SaveOrUpdate(bill);
            }

            // Assert
            this._repository.GetQuantityByCriteria<Bill>(conjunction).Should().Be(quantity);
        }

        [Test]
        public void GetQuantityOfBillsByCriteriasForClientAndBill()
        {
            // Arrange
            const int quantity = 3;
            this.CreateRepositoryWithLoadedDatabase();
            this._repository.LoadDatabase(DatabaseFactory.TestFilePath);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(bill => bill.KindOfBill == ModelFactory.DefaultBillKindOfBill));

            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.FirstName == ModelFactory.DefaultClientFirstName));

            // Act
            for (int i = 0; i < quantity; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                this._repository.SaveOrUpdate(bill.Client);
                this._repository.SaveOrUpdate(bill);
            }

            for (int i = 0; i < quantity; i++)
            {
                Bill bill = ModelFactory.GetDefaultBill();
                bill.Client.FirstName = "Manuel";
                this._repository.SaveOrUpdate(bill.Client);
                this._repository.SaveOrUpdate(bill);
            }

            // Assert
            this._repository.GetQuantityByCriteria<Bill, Client>(billConjunction, b => b.Client, clientConjunction).Should().Be(quantity);
        }


        private void CreateDatabaseAndSetPath()
        {
            DatabaseFactory.CreateTestFile();
            DatabaseFactory.SetSavedFilePath();
        }

        private void CreateRepository()
        {
            this._repository = new NHibernateRepository(new NHibernateSessionManager());
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