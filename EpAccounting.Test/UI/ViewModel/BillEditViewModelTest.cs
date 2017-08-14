// ///////////////////////////////////
// File: BillEditViewModelTest.cs
// Last Change: 21.06.2017  16:46
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Test.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class BillEditViewModelTest
    {
        #region Test Methods

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(BillEditViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void CanCreateBillEditViewModelObject()
        {
            // Act
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();

            // Assert
            billEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void CurrentBillDetailViewModelShouldBeNullAfterCreation()
        {
            // Act
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();

            // Assert
            billEditViewModel.CurrentBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void BillStateShouldBeNullAfterCreation()
        {
            // Act
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();

            // Assert
            billEditViewModel.CurrentBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void IsInSearchModeShouldReturnTrue()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);

            // Act
            billEditViewModel.Load(null, billEditViewModel.GetBillSearchState());

            // Assert
            billEditViewModel.IsInSearchMode.Should().BeTrue();
        }

        [Test]
        public void IsInSearchModeShouldReturnFalseWhenBillStateNotSearchState()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);

            // Assert
            billEditViewModel.IsInSearchMode.Should().BeFalse();
        }

        [Test]
        public void IsInSearchModeShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(false);
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);

            // Assert
            billEditViewModel.IsInSearchMode.Should().BeFalse();
        }

        [Test]
        public void CanEditBillDataShouldReturnTrue()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);

            // Act
            billEditViewModel.Load(null, billEditViewModel.GetBillSearchState());

            // Assert
            billEditViewModel.CanEditBillData.Should().BeTrue();
        }

        [Test]
        public void CanEditBillDataShouldReturnFalseWhenBillStateCanNotCommit()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);

            // Assert
            billEditViewModel.CanEditBillData.Should().BeFalse();
        }

        [Test]
        public void CanEditBillDataShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(false);
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);

            // Act
            billEditViewModel.Load(null, billEditViewModel.GetBillSearchState());

            // Assert
            billEditViewModel.CanEditBillData.Should().BeFalse();
        }

        [Test]
        public void CanChangeCurrentBillDetailViewModel()
        {
            // Arrange
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();

            // Act
            billEditViewModel.Load(ModelFactory.GetDefaultBill(), billEditViewModel.GetBillLoadedState());

            // Assert
            billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentBillDetailViewModelChanges()
        {
            // Arrange
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billEditViewModel.Load(ModelFactory.GetDefaultBill(), billEditViewModel.GetBillLoadedState());

            // Assert
            billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillDetailViewModel);
        }

        [Test]
        public void RaisePropertyChangedEventWhenEqualBillDetailViewModelWillBeSet()
        {
            // Arrange
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            Bill bill = ModelFactory.GetDefaultBill();
            billEditViewModel.Load(bill, billEditViewModel.GetBillLoadedState());
            billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billEditViewModel.Load(bill, billEditViewModel.GetBillLoadedState());

            // Assert
            billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillDetailViewModel);
        }

        [Test]
        public void CanChangeCurrentBillState()
        {
            // Arrange
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();

            // Act
            billEditViewModel.Load(new Bill(), billEditViewModel.GetBillCreationState());

            // Assert
            billEditViewModel.CurrentBillState.Should().BeOfType<BillCreationState>();
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentBillStateChanges()
        {
            // Arrange
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billEditViewModel.Load(new Bill(), billEditViewModel.GetBillCreationState());

            // Assert
            billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillState);
        }

        [Test]
        public void RaisesPropertyChangedWhenBillWillBeReloaded()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultBill);
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModelWithBillDetailViewModelValues(mockRepository,
                                                                                                                ModelFactory.DefaultBillDate,
                                                                                                                ModelFactory.DefaultBillKindOfBill);
            billEditViewModel.CurrentBillDetailViewModel.Date = "--.--.----";
            billEditViewModel.CurrentBillDetailViewModel.KindOfBill = "Gutschein";
            billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billEditViewModel.Reload();

            // Assert
            billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillDetailViewModel);
            billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
            billEditViewModel.CurrentBillDetailViewModel.KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void RaisePropertyChangedEvenWhenEqualBillStateWillBeSet()
        {
            // Arrange
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billEditViewModel.Load(new Bill(), billEditViewModel.GetBillEmptyState());

            // Assert
            billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillState);
        }

        [Test]
        public void DoNotChangeBillAndBillStateWhenParametersAreNull()
        {
            // Arrange
            const string ExpectedDate = "07.01.2015";
            const string ExpectedKindOfBill = "Gutschein";
            BillEditViewModel BillEditViewModel = this.GetDefaultBillEditViewModel();

            BillEditViewModel.Load(new Bill { Date = ExpectedDate, KindOfBill = ExpectedKindOfBill }, BillEditViewModel.GetBillLoadedState());

            // Act
            BillEditViewModel.Load(null, null);

            // Assert
            BillEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(ExpectedDate);
            BillEditViewModel.CurrentBillDetailViewModel.KindOfBill.Should().Be(ExpectedKindOfBill);
            BillEditViewModel.CurrentBillState.Should().Be(BillEditViewModel.GetBillLoadedState());
        }

        [Test]
        public void GetInstancesOfBillStates()
        {
            // Arrange
            BillEditViewModel bill = this.GetDefaultBillEditViewModel();

            // Act
            IBillState billEmptyState = bill.GetBillEmptyState();
            IBillState billCreationState = bill.GetBillCreationState();
            IBillState billSearchState = bill.GetBillSearchState();
            IBillState billLoadedState = bill.GetBillLoadedState();
            IBillState billEditState = bill.GetBillEditState();

            // Assert
            billEmptyState.Should().BeOfType<BillEmptyState>();
            billCreationState.Should().BeOfType<BillCreationState>();
            billSearchState.Should().BeOfType<BillSearchState>();
            billLoadedState.Should().BeOfType<BillLoadedState>();
            billEditState.Should().BeOfType<BillEditState>();
        }

        [Test]
        public void BillCommandsAreInitialized()
        {
            // Arrange
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();

            // Assert
            billEditViewModel.BillCommands.Should().HaveCount(5);
        }

        [Test]
        public async Task UpdatesBill()
        {
            // Arrange
            Mock<IRepository> mockRepsoitory = new Mock<IRepository>();
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModelWithBillDetailViewModelValues(mockRepsoitory, "01", "Gutschien");

            // Act
            bool result = await billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            result.Should().BeTrue();
            mockRepsoitory.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once());
        }

        [Test]
        public async Task ShowMessageWhenBillCouldNotBeAddedToDatabaseBecauseOfAnException()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<Bill>())).Throws(new Exception());
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository, mockDialogService);

            // Act
            bool result = await billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            result.Should().BeFalse();
            mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once());
            mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateBill, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotDeleteBillWhenDialogResultIsNo()
        {
            // Arrange
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockDialogService);

            // Act
            bool result = await billEditViewModel.DeleteBillAsync();

            // Assert
            result.Should().BeFalse();
            mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteBillWhenDialogResultIsYes()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModelWithBillDetailViewModelValues(mockRepository, mockDialogService,
                                                                                                                "01.02.2017", "Rechnung");

            // Act
            bool result = await billEditViewModel.DeleteBillAsync();

            // Assert
            result.Should().BeTrue();
            mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockRepository.Verify(x => x.Delete(It.IsAny<Bill>()), Times.Once);
        }

        [Test]
        public async Task ShowMessageWhenBillCouldNotBeDeletedBecauseOfAnException()
        {
            // Arrange
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockDialogService);

            // Act
            bool result = await billEditViewModel.DeleteBillAsync();

            // Assert
            result.Should().BeFalse();
            mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotDeleteBill, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SendBillSearchCriterionThatSearchesJustForASpecificBillId()
        {
            // Arrange
            const int ExpectedId = 2;
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            Bill bill = new Bill { BillId = ExpectedId, Date = "01.02.2017", KindOfBill = "Gutschein" };
            billEditViewModel.Load(bill, billEditViewModel.GetBillLoadedState());

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> criterion = null;
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>>(this, x => criterion = x.Content);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.BillId == ExpectedId));
            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, null, null);

            // Act
            billEditViewModel.SendBillSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedTuple.ToString());
        }

        [Test]
        public void SendBillSearchCriterionThatSearchesForBillsWithSpecificBillDataAndClientNumber()
        {
            // Arrange
            const int ExpectedId = 2;
            const string KindOfBill = "Gutschein";
            const string Date = "01.02.2017";
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            Client client = new Client() { ClientId = ExpectedId };
            Bill bill = new Bill { KindOfBill = KindOfBill, Date = Date, Client = client };
            billEditViewModel.Load(bill, billEditViewModel.GetBillLoadedState());

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> criterion = null;
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>>(this, x => criterion = x.Content);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(b => b.KindOfBill.IsLike(KindOfBill, MatchMode.Anywhere)));
            billConjunction.Add(Restrictions.Where<Bill>(b => b.Date.IsLike(Date, MatchMode.Anywhere)));

            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.ClientId == ExpectedId));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            billEditViewModel.SendBillSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedTuple.ToString());
        }

        [Test]
        public void SendBillSearchCriterionThatSearchesWithAllEnteredBillValues()
        {
            // Arrange
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            Bill bill = ModelFactory.GetDefaultBill();
            billEditViewModel.Load(bill, billEditViewModel.GetBillLoadedState());

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> criterion = null;
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>>(this, x => criterion = x.Content);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill.IsLike(ModelFactory.DefaultBillKindOfBill, MatchMode.Anywhere)));
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfVat.IsLike(ModelFactory.DefaultBillKindOfVat, MatchMode.Anywhere)));
            billConjunction.Add(Restrictions.Where<Bill>(c => c.Date.IsLike(ModelFactory.DefaultBillDate, MatchMode.Anywhere)));

            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(ModelFactory.DefaultClientTitle, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.FirstName.IsLike(ModelFactory.DefaultClientFirstName, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.LastName.IsLike(ModelFactory.DefaultClientLastName, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Street.IsLike(ModelFactory.DefaultClientStreet, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.HouseNumber.IsLike(ModelFactory.DefaultClientHouseNumber, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.PostalCode.IsLike(ModelFactory.DefaultClientPostalCode, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.City.IsLike(ModelFactory.DefaultClientCity, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            billEditViewModel.SendBillSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedTuple.ToString());
        }

        [Test]
        public void ExecutesCommandsWhenCanConditionsMet()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);

            Mock<IBillState> mockBillState = new Mock<IBillState>();
            mockBillState.Setup(x => x.CanSwitchToSearchMode()).Returns(true);
            mockBillState.Setup(x => x.CanSwitchToEditMode()).Returns(true);
            mockBillState.Setup(x => x.CanCommit()).Returns(true);
            mockBillState.Setup(x => x.CanCancel()).Returns(true);
            mockBillState.Setup(x => x.CanDelete()).Returns(true);

            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);
            billEditViewModel.Load(new Bill(), mockBillState.Object);

            // Act
            foreach (ImageCommandViewModel command in billEditViewModel.BillCommands)
            {
                command.RelayCommand.Execute(this);
            }

            // Assert
            mockBillState.Verify(x => x.SwitchToSearchMode(), Times.Once);
            mockBillState.Verify(x => x.SwitchToEditMode(), Times.Once);
            mockBillState.Verify(x => x.Commit(), Times.Once);
            mockBillState.Verify(x => x.Cancel(), Times.Once);
            mockBillState.Verify(x => x.Delete(), Times.Once);
        }

        [Test]
        public void DoesNotExecuteCommandsWhenCanConditionsDoNotMet()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(true);

            Mock<IBillState> mockBillState = new Mock<IBillState>();
            mockBillState.Setup(x => x.CanSwitchToSearchMode()).Returns(false);
            mockBillState.Setup(x => x.CanSwitchToEditMode()).Returns(false);
            mockBillState.Setup(x => x.CanCommit()).Returns(false);
            mockBillState.Setup(x => x.CanCancel()).Returns(false);
            mockBillState.Setup(x => x.CanDelete()).Returns(false);

            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);
            billEditViewModel.Load(new Bill(), mockBillState.Object);

            // Act
            foreach (ImageCommandViewModel command in billEditViewModel.BillCommands)
            {
                command.RelayCommand.Execute(this);
            }

            // Assert
            mockBillState.Verify(x => x.SwitchToSearchMode(), Times.Never);
            mockBillState.Verify(x => x.SwitchToEditMode(), Times.Never);
            mockBillState.Verify(x => x.Commit(), Times.Never);
            mockBillState.Verify(x => x.Cancel(), Times.Never);
            mockBillState.Verify(x => x.Delete(), Times.Never);
        }

        [Test]
        public void DoesNotExecuteCommandsWhenRepositoryNotInitialized()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.IsConnected).Returns(false);

            Mock<IBillState> mockBillState = new Mock<IBillState>();
            mockBillState.Setup(x => x.CanSwitchToSearchMode()).Returns(true);
            mockBillState.Setup(x => x.CanSwitchToEditMode()).Returns(true);
            mockBillState.Setup(x => x.CanCommit()).Returns(true);
            mockBillState.Setup(x => x.CanCancel()).Returns(true);
            mockBillState.Setup(x => x.CanDelete()).Returns(true);

            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);
            billEditViewModel.Load(new Bill(), mockBillState.Object);

            // Act
            foreach (ImageCommandViewModel command in billEditViewModel.BillCommands)
            {
                command.RelayCommand.Execute(this);
            }

            // Assert
            mockBillState.Verify(x => x.SwitchToSearchMode(), Times.Never);
            mockBillState.Verify(x => x.SwitchToEditMode(), Times.Never);
            mockBillState.Verify(x => x.Commit(), Times.Never);
            mockBillState.Verify(x => x.Cancel(), Times.Never);
            mockBillState.Verify(x => x.Delete(), Times.Never);
        }

        [Test]
        public void LoadBillViaMessengerMessage()
        {
            // Arrange
            const int ExpectedId = 2;
            const string Date = "01.01.2017";
            Mock<IRepository> mockRepsoitory = new Mock<IRepository>();
            mockRepsoitory.Setup(x => x.GetById<Bill>(ExpectedId)).Returns(new Bill() { Date = Date});
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepsoitory);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(ExpectedId, Resources.Messenger_Message_LoadSelectedBillForBillEditVM));

            // Assert
            billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(Date);
        }

        [Test]
        public void ReloadBillViaMessengerMessage()
        {
            // Arrange
            const string Date = "01.01.2017";
            Bill bill = new Bill() { Date = Date };

            Mock<IRepository> mockRepsoitory = new Mock<IRepository>();
            mockRepsoitory.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(new Bill() {Date = Date});
            mockRepsoitory.Setup(x => x.IsConnected).Returns(true);
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepsoitory);
            billEditViewModel.Load(bill, billEditViewModel.GetBillEditState());

            // Act
            billEditViewModel.CurrentBillDetailViewModel.Date = "02.02.2016";
            billEditViewModel.BillCommands.Find(x => x.CommandMessage == Resources.Command_Message_Bill_Cancel).RelayCommand.Execute(null);
            
            // Assert
            billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(Date);
        }

        [Test]
        public void ReloadBillWhenClientValuesOfLoadedBillChanged()
        {
            // Arrange
            const string FirstName = "Annalena";
            Bill bill = ModelFactory.GetDefaultBill();
            bill.Client.FirstName = FirstName;

            Mock<IRepository> mockRepsoitory = new Mock<IRepository>();
            mockRepsoitory.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(bill);

            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepsoitory);
            billEditViewModel.Load(ModelFactory.GetDefaultBill(), billEditViewModel.GetBillLoadedState());

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Messenger_Message_UpdateClientValuesMessageForBillEditVM));

            // Assert
            billEditViewModel.CurrentBillDetailViewModel.FirstName.Should().Be(FirstName);

        }

        [Test]
        public void SendsUpdateBillValuesMessage()
        {
            // Arrange
            const int ExpectedId = 10;
            int billId = 0;
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();

            billEditViewModel.Load(new Bill { BillId = ExpectedId }, billEditViewModel.GetBillLoadedState());
            Messenger.Default.Register<NotificationMessage<int>>(this, x => billId = x.Content);

            // Act
            billEditViewModel.SendUpdateBillValuesMessage();

            // Assert
            billId.Should().Be(ExpectedId);
        }

        [Test]
        public void SendNotEnabledStateForBillItemEditing()
        {
            // Arrange
            bool enableState = true;
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => enableState = x.Content);

            // Act
            billEditViewModel.Load(null, billEditViewModel.GetBillEmptyState());

            // Assert
            enableState.Should().BeFalse();
        }

        [Test]
        public void SendEnabledStateForBillItemEditing()
        {
            // Arrange
            bool enableState = true;
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => enableState = x.Content);

            // Act
            billEditViewModel.Load(null, billEditViewModel.GetBillEditState());

            // Assert
            enableState.Should().BeTrue();
        }

        [Test]
        public void CreatesNewBillWithSpecificClientByCreateBillMessage()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Messenger_Message_CreateNewBillMessageForBillEditVM));

            // Assert
            mockRepository.Verify(x => x.GetById<Client>(It.IsAny<int>()), Times.Once);
        }

        #endregion



        private BillEditViewModel GetDefaultBillEditViewModel()
        {
            return new BillEditViewModel(new Mock<IRepository>().Object, new Mock<IDialogService>().Object);
        }

        private BillEditViewModel GetDefaultBillEditViewModel(Mock<IRepository> mockRepository)
        {
            return new BillEditViewModel(mockRepository.Object, new Mock<IDialogService>().Object);
        }

        private BillEditViewModel GetDefaultBillEditViewModel(Mock<IDialogService> mockDialogService)
        {
            return new BillEditViewModel(new Mock<IRepository>().Object, mockDialogService.Object);
        }

        private BillEditViewModel GetDefaultBillEditViewModel(Mock<IRepository> mockRepository, Mock<IDialogService> mockDialogService)
        {
            return new BillEditViewModel(mockRepository.Object, mockDialogService.Object);
        }

        private BillEditViewModel GetDefaultBillEditViewModelWithBillDetailViewModelValues(string date, string kindOfBill)
        {
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel();
            billEditViewModel.Load(new Bill() { Date = date, KindOfBill = kindOfBill }, billEditViewModel.GetBillLoadedState());

            return billEditViewModel;
        }

        private BillEditViewModel GetDefaultBillEditViewModelWithBillDetailViewModelValues(Mock<IRepository> mockRepository, string date, string kindOfBill)
        {
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository);
            billEditViewModel.Load(new Bill() { Date = date, KindOfBill = kindOfBill }, billEditViewModel.GetBillLoadedState());

            return billEditViewModel;
        }

        private BillEditViewModel GetDefaultBillEditViewModelWithBillDetailViewModelValues(Mock<IRepository> mockRepository, Mock<IDialogService> mockDialogService, string date, string kindOfBill)
        {
            BillEditViewModel billEditViewModel = this.GetDefaultBillEditViewModel(mockRepository, mockDialogService);
            billEditViewModel.Load(new Bill() { Date = date, KindOfBill = kindOfBill }, billEditViewModel.GetBillLoadedState());

            return billEditViewModel;
        }
    }
}