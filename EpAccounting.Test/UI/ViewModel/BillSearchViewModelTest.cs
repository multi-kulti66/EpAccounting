// ///////////////////////////////////
// File: BillSearchViewModelTest.cs
// Last Change: 14.08.2017  11:37
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
    using EpAccounting.Test.Model;
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
            // Arrange
            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel();

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(0);
        }

        [Test]
        public void AddSingleBillToListThatMatchesBillCriterion()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                          .Returns(new List<Bill> { ModelFactory.GetDefaultBill() });

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill.IsLike(ModelFactory.DefaultBillKindOfBill, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, null, null);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(1);
            billSearchViewModel.FoundBills[0].Date.Should().Be(ModelFactory.DefaultBillDate);
            billSearchViewModel.FoundBills[0].KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void AddSingleBillToListThatMatchesBillAndClientCriterion()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), 1))
                          .Returns(new List<Bill> { ModelFactory.GetDefaultBill() });

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill.IsLike(ModelFactory.DefaultBillKindOfBill, MatchMode.Anywhere)));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(ModelFactory.DefaultClientTitle, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(1);
            billSearchViewModel.FoundBills[0].Date.Should().Be(ModelFactory.DefaultBillDate);
            billSearchViewModel.FoundBills[0].KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void SelectedBillReturnsNullAfterCreation()
        {
            // Act
            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel();

            // Assert
            billSearchViewModel.SelectedBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void RaisePropertyChangedWhenSelectedBillChanges()
        {
            // Arrange
            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel();
            billSearchViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(new Bill());

            // Assert
            billSearchViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedBillDetailViewModel);
        }

        [Test]
        public void UpdatesBillWhenUpdateBillNotificationReceived()
        {
            // Arrange
            const string ExpectedDate = "01.01.2015";
            Bill expectedBill = new Bill { BillId = 1, Date = ExpectedDate, KindOfVat = ModelFactory.DefaultBillKindOfVat };
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);
            billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { BillId = 1, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }));
            billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { BillId = 2, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Messenger_Message_UpdateBillValuesMessageForBillSearchVM));

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(2);
            billSearchViewModel.FoundBills[0].Date.Should().Be(ExpectedDate);
            billSearchViewModel.FoundBills[1].Date.Should().Be(ModelFactory.DefaultBillDate);
        }

        [Test]
        public void UpdatesBillWhenUpdateClientNotificationReceived()
        {
            // Arrange
            const string ExpectedFirstName = "Matthias";
            Bill expectedBill = ModelFactory.GetDefaultBill();
            expectedBill.Client.FirstName = ExpectedFirstName;
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);
            billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { BillId = 1, Date = "01.01.2017", KindOfVat = "Gutschein", Client = new Client() }));
            billSearchViewModel.FoundBills.Add(new BillDetailViewModel(ModelFactory.GetDefaultBill()));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Messenger_Message_UpdateClientValuesMessageForBillSearchVM));

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(2);
            billSearchViewModel.FoundBills[1].FirstName.Should().Be(ExpectedFirstName);
        }

        [Test]
        public void RemovesBillWhenUpdateNotificationReceived()
        {
            // Arrange
            const int Id = 1;
            const string Date = "01.01.2015";
            const string KindOfBill = "Gutschrift";

            Bill expectedBill = new Bill { BillId = Id, Date = Date, KindOfBill = KindOfBill };
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);
            mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), 1))
                          .Returns(new List<Bill>());

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill.IsLike(ModelFactory.DefaultBillKindOfBill, MatchMode.Anywhere)));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(ModelFactory.DefaultClientTitle, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));

            billSearchViewModel.FoundBills.Add(new BillDetailViewModel(expectedBill));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Messenger_Message_RemoveBillMessageForBillSearchVM));

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(0);
        }

        [Test]
        public void CanNotLoadBill()
        {
            // Act
            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel();

            // Assert
            billSearchViewModel.LoadSelectedBillCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadBill()
        {
            // Arrange
            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel();

            // Act
            billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(ModelFactory.GetDefaultBill());

            // Assert
            billSearchViewModel.LoadSelectedBillCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendsLoadBillMessage()
        {
            // Arrange
            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel();
            billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(ModelFactory.GetDefaultBill());
            string message = null;

            Messenger.Default.Register<NotificationMessage<int>>(this, x => message = x.Notification);

            // Act
            billSearchViewModel.LoadSelectedBillCommand.Execute(null);

            // Assert
            message.Should().Be(Resources.Messenger_Message_LoadSelectedBillForBillEditVM);
        }

        [Test]
        public void CanNotLoadNextPageWhenJustOnePage()
        {
            // Arrange
            const int NumberOfElements = 10;

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                          .Returns(new List<Bill>());
            mockRepository.Setup(x => x.GetQuantityByCriteria<Bill>(It.IsAny<ICriterion>()))
                          .Returns(NumberOfElements);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            billSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadNexPage()
        {
            // Arrange
            const int NumberOfElements = 55;

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), 1))
                          .Returns(new List<Bill>());
            mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                          .Returns(NumberOfElements);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill.IsLike(ModelFactory.DefaultBillKindOfBill, MatchMode.Anywhere)));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(ModelFactory.DefaultClientTitle, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            billSearchViewModel.LoadNextPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsNexPage()
        {
            // Arrange
            const int NumberOfElements = 55;

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                          .Returns(new List<Bill>());
            mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                          .Returns(NumberOfElements);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill.IsLike(ModelFactory.DefaultBillKindOfBill, MatchMode.Anywhere)));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(ModelFactory.DefaultClientTitle, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));
            billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            billSearchViewModel.CurrentPage.Should().Be(2);
        }

        [Test]
        public void CanNotLoadPreviousPageWhenOnPage1()
        {
            // Arrange
            const int NumberOfElements = 55;

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>(), 1))
                          .Returns(new List<Bill>());
            mockRepository.Setup(x => x.GetQuantityByCriteria<Bill>(It.IsAny<ICriterion>()))
                          .Returns(NumberOfElements);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            billSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanLoadPreviousPageWhenNotOnPage1()
        {
            // Arrange
            const int NumberOfElements = 55;

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                          .Returns(new List<Bill>());
            mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                          .Returns(NumberOfElements);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill.IsLike(ModelFactory.DefaultBillKindOfBill, MatchMode.Anywhere)));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(ModelFactory.DefaultClientTitle, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));
            billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            // Assert
            billSearchViewModel.LoadPreviousPageCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void LoadsPreviousPage()
        {
            // Arrange
            const int NumberOfElements = 55;

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                          .Returns(new List<Bill>());
            mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                          .Returns(NumberOfElements);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill.IsLike(ModelFactory.DefaultBillKindOfBill, MatchMode.Anywhere)));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(ModelFactory.DefaultClientTitle, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));
            billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);
            billSearchViewModel.LoadPreviousPageCommand.RelayCommand.Execute(null);

            // Assert
            billSearchViewModel.CurrentPage.Should().Be(1);
        }

        [Test]
        public void ChangesToPreviousPageWhenBillWasDeletedAndNoBillLeftOnPage()
        {
            // Arrange
            const int NumberOfElements = 51;

            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), It.IsAny<int>()))
                          .Returns(new List<Bill>());
            mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                          .Returns(NumberOfElements);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill.IsLike(ModelFactory.DefaultBillKindOfBill, MatchMode.Anywhere)));
            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(ModelFactory.DefaultClientTitle, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));
            billSearchViewModel.LoadNextPageCommand.RelayCommand.Execute(null);

            mockRepository.Setup(x => x.GetQuantityByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>()))
                          .Returns(NumberOfElements - 1);
            Messenger.Default.Send(new NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>(expectedTuple, Resources.Messenger_Message_BillSearchCriteriaForBillSearchVM));

            // Assert
            billSearchViewModel.CurrentPage.Should().Be(1);
            billSearchViewModel.NumberOfAllPages.Should().Be(1);
        }

        #endregion



        private BillSearchViewModel GetDefaultViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            return new BillSearchViewModel(mockRepository.Object);
        }

        private BillSearchViewModel GetDefaultViewModel(Mock<IRepository> repository)
        {
            return new BillSearchViewModel(repository.Object);
        }
    }
}