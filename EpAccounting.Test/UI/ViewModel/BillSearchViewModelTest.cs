// ///////////////////////////////////
// File: BillSearchViewModelTest.cs
// Last Change: 22.02.2018, 21:20
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
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;


    [TestFixture]
    public class BillSearchViewModelTest
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private BillSearchViewModel _billSearchViewModel;



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._billSearchViewModel = new BillSearchViewModel(this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._billSearchViewModel = null;
            GC.Collect();
        }

        #endregion



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
            this._billSearchViewModel.FoundBills.Should().HaveCount(0);
        }

        [Test]
        public void AddSingleBillToListThatMatchesBillCriterion()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill> { ModelFactory.GetDefaultBill() });

            Conjunction billConjunction = Restrictions.Conjunction();

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(billConjunction, null, null, null, null);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._billSearchViewModel.FoundBills.Should().HaveCount(1);
            this._billSearchViewModel.FoundBills[0].Date.Should().Be(ModelFactory.DefaultBillDate);
            this._billSearchViewModel.FoundBills[0].KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void ShowMessageWhenSearchedClientsCouldNotBeLoaded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Bill>(It.IsAny<ICriterion>())).Throws<Exception>();

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(billConjunction, null, null, null, null);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void AddSingleBillToListThatMatchesBillAndClientCriterion()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill> { ModelFactory.GetDefaultBill() });

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._billSearchViewModel.FoundBills.Should().HaveCount(1);
            this._billSearchViewModel.FoundBills[0].Date.Should().Be(ModelFactory.DefaultBillDate);
            this._billSearchViewModel.FoundBills[0].KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void SelectedBillReturnsNullAfterCreation()
        {
            // Assert
            this._billSearchViewModel.SelectedBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void RaisePropertyChangedWhenSelectedBillChanges()
        {
            // Arrange
            this._billSearchViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(new Bill(), this._mockRepository.Object, this._mockDialogService.Object);

            // Assert
            this._billSearchViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedBillDetailViewModel);
        }

        [Test]
        public void UpdatesBillWhenUpdateBillNotificationReceived()
        {
            // Arrange
            const string expectedDate = "01.01.2015";
            Bill expectedBill = new Bill { Id = 1, Date = expectedDate, KindOfVat = ModelFactory.DefaultBillKindOfVat };
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);

            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { Id = 1, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }, this._mockRepository.Object, this._mockDialogService.Object));
            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { Id = 2, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }, this._mockRepository.Object, this._mockDialogService.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_UpdateBillValuesMessageForBillSearchVM));

            // Assert
            this._billSearchViewModel.FoundBills.Should().HaveCount(2);
            this._billSearchViewModel.FoundBills[0].Date.Should().Be(expectedDate);
            this._billSearchViewModel.FoundBills[1].Date.Should().Be(ModelFactory.DefaultBillDate);
        }

        [Test]
        public void ShowMessageWhenBillCouldNotBeUpdatedViaNotificationMessage()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Throws<Exception>();

            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { Id = 1, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }, this._mockRepository.Object, this._mockDialogService.Object));
            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { Id = 2, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }, this._mockRepository.Object, this._mockDialogService.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_UpdateBillValuesMessageForBillSearchVM));

            // Assert
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void UpdatesBillWhenUpdateClientNotificationReceived()
        {
            // Arrange
            const string expectedFirstName = "Matthias";
            Bill expectedBill = ModelFactory.GetDefaultBill();
            expectedBill.Client.FirstName = expectedFirstName;
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);

            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { Id = 1, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat, Client = new Client() }, this._mockRepository.Object, this._mockDialogService.Object));
            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(ModelFactory.GetDefaultBill(), this._mockRepository.Object, this._mockDialogService.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_UpdateClientValuesForBillSearchVM));

            // Assert
            this._billSearchViewModel.FoundBills.Should().HaveCount(2);
            this._billSearchViewModel.FoundBills[1].FirstName.Should().Be(expectedFirstName);
        }

        [Test]
        public void ShowMessageWhenBillCouldNotBeUpdatedByClientIdViaNotificationMessage()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Throws<Exception>();

            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { Id = 1, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat, Client = new Client() { Id = 1 } }, this._mockRepository.Object, this._mockDialogService.Object));
            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(ModelFactory.GetDefaultBill(), this._mockRepository.Object, this._mockDialogService.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_UpdateClientValuesForBillSearchVM));

            // Assert
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void RemovesBillWhenUpdateNotificationReceived()
        {
            // Arrange
            const int id = 1;
            const string date = "01.01.2015";
            KindOfBill kindOfBill = KindOfBill.Gutschrift;

            Bill expectedBill = new Bill { Id = id, Date = date, KindOfBill = kindOfBill };
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);
            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(expectedBill, this._mockRepository.Object, this._mockDialogService.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ReloadBillForBillSearchVM));

            // Assert
            this._billSearchViewModel.FoundBills.Should().HaveCount(0);
        }

        [Test]
        public void RemoveBillsFromRemovedClient()
        {
            // Arrange
            this._billSearchViewModel.FoundBills.Add(new BillDetailViewModel(ModelFactory.GetDefaultBill(), this._mockRepository.Object, this._mockDialogService.Object));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_RemoveClientForBillSearchVM));

            // Assert
            this._billSearchViewModel.FoundBills.Count.Should().Be(0);
        }

        [Test]
        public void CanNotLoadBill()
        {
            // Assert
            this._billSearchViewModel.LoadSelectedBillCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadBill()
        {
            // Act
            this._billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(ModelFactory.GetDefaultBill(), this._mockRepository.Object, this._mockDialogService.Object);

            // Assert
            this._billSearchViewModel.LoadSelectedBillCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendsLoadBillMessage()
        {
            // Arrange
            this._billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(ModelFactory.GetDefaultBill(), this._mockRepository.Object, this._mockDialogService.Object);

            string notificationMessage = null;
            Messenger.Default.Register<NotificationMessage<int>>(this, x => notificationMessage = x.Notification);

            // Act
            this._billSearchViewModel.LoadSelectedBillCommand.Execute(null);

            // Assert
            notificationMessage.Should().Be(Resources.Message_LoadSelectedBillForBillEditVM);
        }

        [Test]
        public void CanNotLoadNextPageWhenJustOnePage()
        {
            // Arrange
            const int numberOfElements = 10;

            this._mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Bill>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._billSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadNexPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._billSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsNexPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this._billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this._billSearchViewModel.CurrentPage.Should().Be(2);
        }

        [Test]
        public void CanLoadLastPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._billSearchViewModel.LoadLastPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsLastPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this._billSearchViewModel.LoadLastPageCommand.RelayCommand.Execute(null);

            // Assert
            this._billSearchViewModel.CurrentPage.Should().Be(Convert.ToInt32(Math.Ceiling((double) numberOfElements / Settings.Default.PageSize)));
        }

        [Test]
        public void CanNotLoadPreviousPageWhenOnPage1()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Bill>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._billSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadPreviousPageWhenNotOnPage1()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this._billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this._billSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsPreviousPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this._billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._billSearchViewModel.LoadPreviousPageCommand.RelayCommand.Execute(null);

            // Assert
            this._billSearchViewModel.CurrentPage.Should().Be(1);
        }

        [Test]
        public void CanNotLoadFirstPageWhenOnPage1()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Bill>(It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._billSearchViewModel.LoadFirstPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadFirstPageWhenNotOnPage1()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this._billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            this._billSearchViewModel.LoadFirstPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsFirstPage()
        {
            // Arrange
            const int numberOfElements = 55;

            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this._billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            this._billSearchViewModel.LoadFirstPageCommand.RelayCommand.Execute(null);

            // Assert
            this._billSearchViewModel.CurrentPage.Should().Be(1);
        }

        [Test]
        public void ChangesToPreviousPageWhenBillWasDeletedAndNoBillLeftOnPage()
        {
            // Arrange
            const int numberOfElements = 51;

            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));
            this._billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(numberOfElements - 1);
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._billSearchViewModel.CurrentPage.Should().Be(1);
            this._billSearchViewModel.NumberOfAllPages.Should().Be(1);
        }

        [Test]
        public void LoadBillsFromSpecificClientWhenLoadBillsFromClientMessageReceived()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                .Returns(new List<Bill>() { ModelFactory.GetDefaultBill() });
            this._mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Client, CityToPostalCode>>>(), It.IsAny<ICriterion>()))
                .Returns(1);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>(new Conjunction(), b => b.Client, new Conjunction(), c => c.CityToPostalCode, new Conjunction());

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>(expectedTuple, Resources.Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            this._billSearchViewModel.FoundBills.Count.Should().Be(1);
        }
    }
}