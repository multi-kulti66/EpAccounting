// ///////////////////////////////////
// File: BillEditViewModelTest.cs
// Last Change: 22.10.2017  12:07
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
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
        #region Fields

        private Mock<IRepository> mockRepository;
        private Mock<IDialogService> mockDialogService;
        private BillEditViewModel billEditViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.mockDialogService = new Mock<IDialogService>();
            this.billEditViewModel = new BillEditViewModel(this.mockRepository.Object, this.mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.mockDialogService = null;
            this.billEditViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(BillEditViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void CanCreateBillEditViewModelObject()
        {
            // Assert
            this.billEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void CurrentBillDetailViewModelShouldBeNullAfterCreation()
        {
            // Assert
            this.billEditViewModel.CurrentBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void BillStateShouldBeNullAfterCreation()
        {
            // Assert
            this.billEditViewModel.CurrentBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void IsInSearchModeShouldReturnTrue()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this.billEditViewModel.ChangeToSearchMode();

            // Assert
            this.billEditViewModel.CanInsertIDs.Should().BeTrue();
        }

        [Test]
        public void IsInSearchModeShouldReturnFalseWhenBillStateNotSearchState()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this.billEditViewModel.CanInsertIDs.Should().BeFalse();
        }

        [Test]
        public void IsInSearchModeShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Assert
            this.billEditViewModel.CanInsertIDs.Should().BeFalse();
        }

        [Test]
        public void CanEditBillDataShouldReturnTrue()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this.billEditViewModel.ChangeToSearchMode();

            // Assert
            this.billEditViewModel.CanEditData.Should().BeTrue();
        }

        [Test]
        public void CanEditPrintedStatusShouldReturnTrueInSearchMode()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this.billEditViewModel.ChangeToSearchMode();

            // Assert
            this.billEditViewModel.CanEditPrintedStatus.Should().BeTrue();
        }

        [Test]
        public void CanEditPrintedStatusShouldReturnTrueInEditMode()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());
            this.billEditViewModel.ChangeToEditMode();

            // Assert
            this.billEditViewModel.CanEditPrintedStatus.Should().BeTrue();
        }

        [Test]
        public void CanEditPrintedStatusShouldReturnFalseInCreationMode()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);
            this.mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_CreateNewBillForBillEditVM));

            // Assert
            this.billEditViewModel.CanEditPrintedStatus.Should().BeFalse();
        }

        [Test]
        public void CanEditBillDataShouldReturnFalseWhenBillStateCanNotCommit()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this.billEditViewModel.CanEditData.Should().BeFalse();
        }

        [Test]
        public void CanEditBillDataShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Act
            this.billEditViewModel.ChangeToSearchMode();

            // Assert
            this.billEditViewModel.CanEditData.Should().BeFalse();
        }

        [Test]
        public void CanChangeCurrentBillDetailViewModel()
        {
            // Act
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Assert
            this.billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentBillDetailViewModelChanges()
        {
            // Arrange
            this.billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Assert
            this.billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillDetailViewModel);
        }

        [Test]
        public void RaisePropertyChangedEventWhenEqualBillDetailViewModelWillBeSet()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();
            this.billEditViewModel.ChangeToLoadedMode(bill);
            this.billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billEditViewModel.ChangeToLoadedMode(bill);

            // Assert
            this.billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillDetailViewModel);
        }

        [Test]
        public void CanChangeCurrentBillState()
        {
            // Act
            this.billEditViewModel.ChangeToSearchMode();

            // Assert
            this.billEditViewModel.CurrentBillState.Should().BeOfType<BillSearchState>();
        }

        [Test]
        public void CanChangeToLoadedModeWhenBillIsNull()
        {
            // Act
            this.billEditViewModel.ChangeToLoadedMode();

            // Assert
            this.billEditViewModel.CurrentBillState.Should().BeOfType<BillLoadedState>();
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentBillStateChanges()
        {
            // Arrange
            this.billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billEditViewModel.ChangeToSearchMode();

            // Assert
            this.billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillState);
        }

        [Test]
        public void RaisesPropertyChangedWhenBillWillBeReloaded()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultBill);
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            this.billEditViewModel.CurrentBillDetailViewModel.Date = "05.09.2017";
            this.billEditViewModel.CurrentBillDetailViewModel.KindOfBill = KindOfBill.Rechnung;
            this.billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billEditViewModel.Reload();

            // Assert
            this.billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillDetailViewModel);
            this.billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
            this.billEditViewModel.CurrentBillDetailViewModel.KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void RaisePropertyChangedEvenWhenEqualBillStateWillBeSet()
        {
            // Arrange
            this.billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billEditViewModel.ChangeToEmptyMode();

            // Assert
            this.billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillState);
        }

        [Test]
        public void GetInstancesOfBillStates()
        {
            // Act
            IBillState billEmptyState = this.billEditViewModel.GetBillEmptyState();
            IBillState billCreationState = this.billEditViewModel.GetBillCreationState();
            IBillState billSearchState = this.billEditViewModel.GetBillSearchState();
            IBillState billLoadedState = this.billEditViewModel.GetBillLoadedState();
            IBillState billEditState = this.billEditViewModel.GetBillEditState();

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
            // Assert
            this.billEditViewModel.BillCommands.Should().HaveCount(5);
        }

        [Test]
        public async Task UpdatesBill()
        {
            // Arrange
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Act
            bool result = await this.billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            result.Should().BeTrue();
            this.mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once());
        }

        [Test]
        public async Task SendUpdateClientMessageWhenBillWillBeSavedOrUpdated()
        {
            // Arrange
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            string messenerMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage<int>>(this, x => messenerMessage = x.Notification);

            // Act
            await this.billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            this.mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once());
            messenerMessage.Should().Be(Resources.Message_UpdateClientForClientEditVM);
        }

        [Test]
        public async Task ShowMessageWhenBillCouldNotBeAddedToDatabaseBecauseOfAnException()
        {
            // Arrange
            this.mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<Bill>())).Throws(new Exception());

            // Act
            bool result = await this.billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            result.Should().BeFalse();
            this.mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once());
            this.mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateBill, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotDeleteBillWhenDialogResultIsNo()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            // Act
            bool result = await this.billEditViewModel.DeleteBillAsync();

            // Assert
            result.Should().BeFalse();
            this.mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteBillWhenDialogResultIsYes()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            Bill bill = new Bill { Date = "01.02.2017", KindOfBill = KindOfBill.Rechnung };
            this.billEditViewModel.ChangeToLoadedMode(bill);

            // Act
            bool result = await this.billEditViewModel.DeleteBillAsync();

            // Assert
            result.Should().BeTrue();
            this.mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this.mockRepository.Verify(x => x.Delete(It.IsAny<Bill>()), Times.Once);
        }

        [Test]
        public async Task ShowMessageWhenBillCouldNotBeDeletedBecauseOfAnException()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            // Act
            bool result = await this.billEditViewModel.DeleteBillAsync();

            // Assert
            result.Should().BeFalse();
            this.mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotDeleteBill, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SendBillSearchCriterionThatSearchesJustForASpecificBillId()
        {
            // Arrange
            const int ExpectedId = 2;
            Bill bill = new Bill { Id = ExpectedId, Date = "01.02.2017", KindOfBill = KindOfBill.Gutschrift };
            this.billEditViewModel.ChangeToLoadedMode(bill);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> criterion = null;
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>>(this, x => criterion = x.Content);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.Id == ExpectedId));
            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, null, null);

            // Act
            this.billEditViewModel.SendBillSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedTuple.ToString());
        }

        [Test]
        public void SendBillSearchCriterionThatSearchesForBillsWithSpecificBillDataAndClientNumber()
        {
            // Arrange
            const int ExpectedId = 2;
            KindOfBill KindOfBill = KindOfBill.Gutschrift;
            const string Date = "01.02.2017";
            Client client = new Client() { Id = ExpectedId };
            Bill bill = new Bill { KindOfBill = KindOfBill, Date = Date, Client = client };
            this.billEditViewModel.ChangeToLoadedMode(bill);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> criterion = null;
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>>(this, x => criterion = x.Content);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(b => b.KindOfBill == KindOfBill));
            billConjunction.Add(Restrictions.Where<Bill>(b => b.Date.IsLike(Date, MatchMode.Anywhere)));

            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Id == ExpectedId));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            this.billEditViewModel.SendBillSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedTuple.ToString());
        }

        [Test]
        public void SendBillSearchCriterionThatSearchesWithAllEnteredBillValues()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();
            this.billEditViewModel.ChangeToLoadedMode(bill);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> criterion = null;
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>>(this, x => criterion = x.Content);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfBill == ModelFactory.DefaultBillKindOfBill));
            billConjunction.Add(Restrictions.Where<Bill>(c => c.KindOfVat == ModelFactory.DefaultBillKindOfVat));
            billConjunction.Add(Restrictions.Where<Bill>(c => c.Date.IsLike(ModelFactory.DefaultBillDate, MatchMode.Anywhere)));
            billConjunction.Add(Restrictions.Where<Bill>(c => c.Printed == ModelFactory.DefaultBillPrinted));

            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.CompanyName.IsLike(ModelFactory.DefaultClientCompanyName, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.FirstName.IsLike(ModelFactory.DefaultClientFirstName, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.LastName.IsLike(ModelFactory.DefaultClientLastName, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Street.IsLike(ModelFactory.DefaultClientStreet, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.HouseNumber.IsLike(ModelFactory.DefaultClientHouseNumber, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.CityToPostalCode.PostalCode.IsLike(ModelFactory.DefaultCityToPostalCodePostalCode, MatchMode.Anywhere)));
            clientConjunction.Add(Restrictions.Where<Client>(c => c.CityToPostalCode.City.IsLike(ModelFactory.DefaultCityToPostalCodeCity, MatchMode.Anywhere)));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            this.billEditViewModel.SendBillSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedTuple.ToString());
        }

        [Test]
        public void LoadBillViaMessengerMessage()
        {
            // Arrange
            const int ExpectedId = 2;
            const string Date = "01.02.2017";
            this.mockRepository.Setup(x => x.GetById<Bill>(ExpectedId)).Returns(new Bill { Date = Date });

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(ExpectedId, Resources.Message_LoadSelectedBillForBillEditVM));

            // Assert
            this.billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(Date);
        }

        [Test]
        public void ReloadBillViaMessengerMessage()
        {
            // Arrange
            const string Date = "01.02.2017";
            Bill bill = new Bill { Date = Date };

            this.mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(new Bill() { Date = Date });
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);
            this.billEditViewModel.ChangeToLoadedMode(bill);
            this.billEditViewModel.ChangeToEditMode();

            // Act
            this.billEditViewModel.CurrentBillDetailViewModel.Date = "02.02.2016";
            this.billEditViewModel.BillCommands.Find(x => x.DisplayName == Resources.Command_DisplayName_Cancel).RelayCommand.Execute(null);

            // Assert
            this.billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(Date);
        }

        [Test]
        public void ReloadBillWhenClientValuesOfLoadedBillChanged()
        {
            // Arrange
            const string FirstName = "Annalena";
            Bill bill = ModelFactory.GetDefaultBill();
            bill.Client.FirstName = FirstName;

            this.mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(bill);
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_UpdateClientValuesForBillEditVM));

            // Assert
            this.billEditViewModel.CurrentBillDetailViewModel.FirstName.Should().Be(FirstName);
        }

        [Test]
        public void SendsUpdateBillValuesMessage()
        {
            // Arrange
            const int ExpectedId = 10;
            int billId = 0;

            this.billEditViewModel.ChangeToLoadedMode(new Bill { Id = ExpectedId });
            Messenger.Default.Register<NotificationMessage<int>>(this, x => billId = x.Content);

            // Act
            this.billEditViewModel.SendUpdateBillValuesMessage();

            // Assert
            billId.Should().Be(ExpectedId);
        }

        [Test]
        public void SendNotEnabledStateForBillItemEditing()
        {
            // Arrange
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this.billEditViewModel.ChangeToEmptyMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[0].Should().BeFalse();
        }

        [Test]
        public void SendEnabledStateForBillItemEditing()
        {
            // Arrange
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this.billEditViewModel.ChangeToEditMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[0].Should().BeTrue();
        }

        [Test]
        public void SendEnabledStateForWorkspaceChange()
        {
            // Arrange
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this.billEditViewModel.ChangeToEmptyMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[1].Should().BeTrue();
        }

        [Test]
        public void SendNotEnabledStateForWorkspaceChange()
        {
            // Arrange
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this.billEditViewModel.ChangeToEditMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[1].Should().BeFalse();
        }

        [Test]
        public void CreatesNewBillWithSpecificClientByCreateBillMessage()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_CreateNewBillForBillEditVM));

            // Assert
            this.mockRepository.Verify(x => x.GetById<Client>(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void CreatesNewBillWithDefaultValues()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_CreateNewBillForBillEditVM));

            // Assert
            this.billEditViewModel.CurrentBillDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this.billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(DateTime.Now.Date.ToShortDateString());
            this.billEditViewModel.CurrentBillDetailViewModel.KindOfBill.Should().Be(KindOfBill.Rechnung);
            this.billEditViewModel.CurrentBillDetailViewModel.KindOfVat.Should().Be(KindOfVat.inkl_MwSt);
            this.billEditViewModel.CurrentBillDetailViewModel.VatPercentage.Should().Be(Settings.Default.VatPercentage);
        }

        [Test]
        public void SwitchesToSearchModeAndInsertsClientDataWhenMessageReceived()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<Client>(ModelFactory.GetDefaultClient(), Resources.Message_SwitchToSearchModeAndLoadClientDataForBillEditVM));

            // Assert
            this.billEditViewModel.CurrentBillState.Should().BeOfType<BillSearchState>();
            this.billEditViewModel.CurrentBillDetailViewModel.ClientId.Should().Be(0);
            this.billEditViewModel.CurrentBillDetailViewModel.Title.Should().Be(ModelFactory.DefaultClientTitle);
            this.billEditViewModel.CurrentBillDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this.billEditViewModel.CurrentBillDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
            this.billEditViewModel.CurrentBillDetailViewModel.Street.Should().Be(ModelFactory.DefaultClientStreet);
            this.billEditViewModel.CurrentBillDetailViewModel.HouseNumber.Should().Be(ModelFactory.DefaultClientHouseNumber);
            this.billEditViewModel.CurrentBillDetailViewModel.PostalCode.Should().Be(ModelFactory.DefaultCityToPostalCodePostalCode);
            this.billEditViewModel.CurrentBillDetailViewModel.City.Should().Be(ModelFactory.DefaultCityToPostalCodeCity);
        }

        [Test]
        public void ChangeToEmptyModeWhenLoadedClientWasDeleted()
        {
            // Arrange
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_RemoveClientForBillEditVM));

            // Assert
            this.billEditViewModel.CurrentBillState.Should().BeOfType<BillEmptyState>();
        }

        [Test]
        public void CanNotClearFieldsWhenNotInLoadedState()
        {
            // Assert
            this.billEditViewModel.ClearFieldsCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanClearFieldsWhenInLoadedState()
        {
            // Act
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Assert
            this.billEditViewModel.ClearFieldsCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void ClearsFields()
        {
            // Arrange
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Act
            this.billEditViewModel.ClearFieldsCommand.Execute(null);

            // Assert
            this.billEditViewModel.CurrentBillDetailViewModel.KindOfBill.Should().Be(null);
            this.billEditViewModel.CurrentBillDetailViewModel.KindOfVat.Should().Be(null);
            this.billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(null);
        }

        [Test]
        public void SendsChangeToBillSearchViewMessageWhenFieldsAreCleared()
        {
            // Arrange
            List<string> notificationMessages = new List<string>();
            this.billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());
            Messenger.Default.Register<NotificationMessage>(this, x => notificationMessages.Add(x.Notification));

            // Act
            this.billEditViewModel.ClearFieldsCommand.Execute(null);

            // Assert
            notificationMessages.Should().Contain(Resources.Message_LoadBillSearchViewModelMessageForBillVM);
        }

        [Test]
        public async Task SetsPrintedToFalseAfterCreation()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient());
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_CreateNewBillForBillEditVM));

            // Act
            await this.billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            this.billEditViewModel.CurrentBillDetailViewModel.Printed.Should().BeFalse();
        }

        #endregion
    }
}