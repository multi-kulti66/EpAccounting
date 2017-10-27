// ///////////////////////////////////
// File: BillSearchViewModelTest.cs
// Last Change: 26.10.2017  20:39
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class BillSearchViewModelTest
    {
        #region Fields

        private Mock<IRepository> mockRepository;
        private BillSearchViewModel billSearchViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.billSearchViewModel = new BillSearchViewModel(this.mockRepository.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.billSearchViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            // Assert
            typeof(BillSearchViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void GetEmptyListAfterCreation()
        {
            // Assert
            this.billSearchViewModel.FoundBills.Should().HaveCount(0);
        }

        [Test]
        public void AddSingleBillToListThatMatchesBillCriterion()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill> { ModelFactory.GetDefaultBill() });

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, null, null);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this.billSearchViewModel.FoundBills.Should().HaveCount(1);
            this.billSearchViewModel.FoundBills[0].Date.Should().Be(ModelFactory.DefaultBillDate);
            this.billSearchViewModel.FoundBills[0].KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void AddSingleBillToListThatMatchesBillAndClientCriterion()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill> { ModelFactory.GetDefaultBill() });

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this.billSearchViewModel.FoundBills.Should().HaveCount(1);
            this.billSearchViewModel.FoundBills[0].Date.Should().Be(ModelFactory.DefaultBillDate);
            this.billSearchViewModel.FoundBills[0].KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void SelectedBillReturnsNullAfterCreation()
        {
            // Assert
            this.billSearchViewModel.SelectedBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void RaisePropertyChangedWhenSelectedBillChanges()
        {
            // Arrange
            this.billSearchViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(new Bill(), this.mockRepository.Object);

            // Assert
            this.billSearchViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedBillDetailViewModel);
        }

        [Test]
        public void UpdatesBillWhenUpdateBillNotificationReceived()
        {
            // Arrange
            const string ExpectedDate = "01.01.2015";
            Bill expectedBill = new Bill { Id = 1, Date = ExpectedDate, KindOfVat = ModelFactory.DefaultBillKindOfVat };
            this.mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);

            this.billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { Id = 1, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }, this.mockRepository.Object));
            this.billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { Id = 2, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }, this.mockRepository.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_UpdateBillValuesMessageForBillSearchVM));

            // Assert
            this.billSearchViewModel.FoundBills.Should().HaveCount(2);
            this.billSearchViewModel.FoundBills[0].Date.Should().Be(ExpectedDate);
            this.billSearchViewModel.FoundBills[1].Date.Should().Be(ModelFactory.DefaultBillDate);
        }

        [Test]
        public void UpdatesBillWhenUpdateClientNotificationReceived()
        {
            // Arrange
            const string ExpectedFirstName = "Matthias";
            Bill expectedBill = ModelFactory.GetDefaultBill();
            expectedBill.Client.FirstName = ExpectedFirstName;
            this.mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);

            this.billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { Id = 1, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat, Client = new Client() }, this.mockRepository.Object));
            this.billSearchViewModel.FoundBills.Add(new BillDetailViewModel(ModelFactory.GetDefaultBill(), this.mockRepository.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_UpdateClientValuesForBillSearchVM));

            // Assert
            this.billSearchViewModel.FoundBills.Should().HaveCount(2);
            this.billSearchViewModel.FoundBills[1].FirstName.Should().Be(ExpectedFirstName);
        }

        [Test]
        public void RemovesBillWhenUpdateNotificationReceived()
        {
            // Arrange
            const int Id = 1;
            const string Date = "01.01.2015";
            KindOfBill KindOfBill = KindOfBill.Gutschrift;

            Bill expectedBill = new Bill { Id = Id, Date = Date, KindOfBill = KindOfBill };
            this.mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);
            this.mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            this.billSearchViewModel.FoundBills.Add(new BillDetailViewModel(expectedBill, this.mockRepository.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_RemoveBillForBillSearchVM));

            // Assert
            this.billSearchViewModel.FoundBills.Should().HaveCount(0);
        }

        [Test]
        public void RemoveBillsFromRemovedClient()
        {
            // Arrange
            this.billSearchViewModel.FoundBills.Add(new BillDetailViewModel(ModelFactory.GetDefaultBill(), this.mockRepository.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_RemoveClientForBillSearchVM));

            // Assert
            this.billSearchViewModel.FoundBills.Count.Should().Be(0);
        }

        [Test]
        public void CanNotLoadBill()
        {
            // Assert
            this.billSearchViewModel.LoadSelectedBillCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadBill()
        {
            // Act
            this.billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(ModelFactory.GetDefaultBill(), this.mockRepository.Object);

            // Assert
            this.billSearchViewModel.LoadSelectedBillCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendsLoadBillMessage()
        {
            // Arrange
            this.billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(ModelFactory.GetDefaultBill(), this.mockRepository.Object);

            string notificationMessage = null;
            Messenger.Default.Register<NotificationMessage<int>>(this, x => notificationMessage = x.Notification);

            // Act
            this.billSearchViewModel.LoadSelectedBillCommand.Execute(null);

            // Assert
            notificationMessage.Should().Be(Resources.Message_LoadSelectedBillForBillEditVM);
        }

        [Test]
        public void CanNotLoadNextPageWhenJustOnePage()
        {
            // Arrange
            const int NumberOfElements = 10;

            this.mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Bill>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this.billSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadNexPage()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this.billSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsNexPage()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this.billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this.billSearchViewModel.CurrentPage.Should().Be(2);
        }

        [Test]
        public void CanNotLoadPreviousPageWhenOnPage1()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria<Bill>(It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this.billSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadPreviousPageWhenNotOnPage1()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this.billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this.billSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsPreviousPage()
        {
            // Arrange
            const int NumberOfElements = 55;

            this.mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this.billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this.billSearchViewModel.LoadPreviousPageCommand.RelayCommand.Execute(null);

            // Assert
            this.billSearchViewModel.CurrentPage.Should().Be(1);
        }

        [Test]
        public void ChangesToPreviousPageWhenBillWasDeletedAndNoBillLeftOnPage()
        {
            // Arrange
            const int NumberOfElements = 51;

            this.mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this.mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                .Returns(NumberOfElements);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this.billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            this.mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                .Returns(NumberOfElements - 1);
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this.billSearchViewModel.CurrentPage.Should().Be(1);
            this.billSearchViewModel.NumberOfAllPages.Should().Be(1);
        }

        [Test]
        public void LoadBillsFromSpecificClientWhenLoadBillsFromClientMessageReceived()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>() { ModelFactory.GetDefaultBill() });
            this.mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                .Returns(1);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_LoadBillsFromClientForBillSearchVM));

            // Assert
            this.billSearchViewModel.FoundBills.Count.Should().Be(1);
        }

        #endregion
    }
}