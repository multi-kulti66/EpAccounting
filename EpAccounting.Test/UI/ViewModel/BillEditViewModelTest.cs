// ///////////////////////////////////
// File: BillEditViewModelTest.cs
// Last Change: 17.02.2018, 21:21
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
        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._billEditViewModel = new BillEditViewModel(this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._mockDialogService = null;
            this._billEditViewModel = null;
            GC.Collect();
        }

        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private BillEditViewModel _billEditViewModel;

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(BillEditViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void CanCreateBillEditViewModelObject()
        {
            // Assert
            this._billEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void CurrentBillDetailViewModelShouldBeNullAfterCreation()
        {
            // Assert
            this._billEditViewModel.CurrentBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void BillStateShouldBeNullAfterCreation()
        {
            // Assert
            this._billEditViewModel.CurrentBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void IsInSearchModeShouldReturnTrue()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this._billEditViewModel.ChangeToSearchMode();

            // Assert
            this._billEditViewModel.CanInsertIDs.Should().BeTrue();
        }

        [Test]
        public void IsInSearchModeShouldReturnFalseWhenBillStateNotSearchState()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this._billEditViewModel.CanInsertIDs.Should().BeFalse();
        }

        [Test]
        public void IsInSearchModeShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Assert
            this._billEditViewModel.CanInsertIDs.Should().BeFalse();
        }

        [Test]
        public void CanEditBillDataShouldReturnTrue()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this._billEditViewModel.ChangeToSearchMode();

            // Assert
            this._billEditViewModel.CanEditData.Should().BeTrue();
        }

        [Test]
        public void CanEditPrintedStatusShouldReturnTrueInSearchMode()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this._billEditViewModel.ChangeToSearchMode();

            // Assert
            this._billEditViewModel.CanEditPrintedStatus.Should().BeTrue();
        }

        [Test]
        public void CanEditPrintedStatusShouldReturnTrueInEditMode()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());
            this._billEditViewModel.ChangeToEditMode();

            // Assert
            this._billEditViewModel.CanEditPrintedStatus.Should().BeTrue();
        }

        [Test]
        public void CanEditPrintedStatusShouldReturnFalseInCreationMode()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_CreateNewBillForBillEditVM));

            // Assert
            this._billEditViewModel.CanEditPrintedStatus.Should().BeFalse();
        }

        [Test]
        public void CanEditBillDataShouldReturnFalseWhenBillStateCanNotCommit()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this._billEditViewModel.CanEditData.Should().BeFalse();
        }

        [Test]
        public void CanEditBillDataShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Act
            this._billEditViewModel.ChangeToSearchMode();

            // Assert
            this._billEditViewModel.CanEditData.Should().BeFalse();
        }

        [Test]
        public void CanChangeCurrentBillDetailViewModel()
        {
            // Act
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Assert
            this._billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentBillDetailViewModelChanges()
        {
            // Arrange
            this._billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Assert
            this._billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillDetailViewModel);
        }

        [Test]
        public void RaisePropertyChangedEventWhenEqualBillDetailViewModelWillBeSet()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();
            this._billEditViewModel.ChangeToLoadedMode(bill);
            this._billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billEditViewModel.ChangeToLoadedMode(bill);

            // Assert
            this._billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillDetailViewModel);
        }

        [Test]
        public void CanChangeCurrentBillState()
        {
            // Act
            this._billEditViewModel.ChangeToSearchMode();

            // Assert
            this._billEditViewModel.CurrentBillState.Should().BeOfType<BillSearchState>();
        }

        [Test]
        public void CanChangeToLoadedModeWhenBillIsNull()
        {
            // Act
            this._billEditViewModel.ChangeToLoadedMode();

            // Assert
            this._billEditViewModel.CurrentBillState.Should().BeOfType<BillLoadedState>();
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentBillStateChanges()
        {
            // Arrange
            this._billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billEditViewModel.ChangeToSearchMode();

            // Assert
            this._billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillState);
        }

        [Test]
        public void RaisesPropertyChangedWhenBillWillBeReloaded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultBill);
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            this._billEditViewModel.CurrentBillDetailViewModel.Date = "05.09.2017";
            this._billEditViewModel.CurrentBillDetailViewModel.KindOfBill = KindOfBill.Rechnung;
            this._billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billEditViewModel.Reload();

            // Assert
            this._billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillDetailViewModel);
            this._billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(ModelFactory.DefaultBillDate);
            this._billEditViewModel.CurrentBillDetailViewModel.KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void ShowsExceptionMessageWhenBillCouldNotBeReloaded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Throws<Exception>();

            // Act
            this._billEditViewModel.Reload();

            // Assert
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void RaisePropertyChangedEvenWhenEqualBillStateWillBeSet()
        {
            // Arrange
            this._billEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billEditViewModel.ChangeToEmptyMode();

            // Assert
            this._billEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentBillState);
        }

        [Test]
        public void GetInstancesOfBillStates()
        {
            // Act
            IBillState billEmptyState = this._billEditViewModel.GetBillEmptyState();
            IBillState billCreationState = this._billEditViewModel.GetBillCreationState();
            IBillState billSearchState = this._billEditViewModel.GetBillSearchState();
            IBillState billLoadedState = this._billEditViewModel.GetBillLoadedState();
            IBillState billEditState = this._billEditViewModel.GetBillEditState();

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
            this._billEditViewModel.BillCommands.Should().HaveCount(5);
        }

        [Test]
        public async Task UpdatesBill()
        {
            // Arrange
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Act
            bool result = await this._billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            result.Should().BeTrue();
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once());
        }

        [Test]
        public async Task SendUpdateClientMessageWhenBillWillBeSavedOrUpdated()
        {
            // Arrange
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            string messenerMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage<int>>(this, x => messenerMessage = x.Notification);

            // Act
            await this._billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once());
            messenerMessage.Should().Be(Resources.Message_UpdateClientForClientEditVM);
        }

        [Test]
        public async Task ShowMessageWhenBillCouldNotBeAddedToDatabaseBecauseOfAnException()
        {
            // Arrange
            this._mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<Bill>())).Throws(new Exception());

            // Act
            bool result = await this._billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            result.Should().BeFalse();
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once());
            this._mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateBill, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotDeleteBillWhenDialogResultIsNo()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            // Act
            bool result = await this._billEditViewModel.DeleteBillAsync();

            // Assert
            result.Should().BeFalse();
            this._mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteBillWhenDialogResultIsYes()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            Bill bill = new Bill { Date = "01.02.2017", KindOfBill = KindOfBill.Rechnung };
            this._billEditViewModel.ChangeToLoadedMode(bill);

            // Act
            bool result = await this._billEditViewModel.DeleteBillAsync();

            // Assert
            result.Should().BeTrue();
            this._mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this._mockRepository.Verify(x => x.Delete(It.IsAny<Bill>()), Times.Once);
        }

        [Test]
        public async Task ShowMessageWhenBillCouldNotBeDeletedBecauseOfAnException()
        {
            // Arrange
            this._mockRepository.Setup(x => x.Delete(It.IsAny<Bill>())).Throws(new Exception());
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            // Act
            bool result = await this._billEditViewModel.DeleteBillAsync();

            // Assert
            result.Should().BeFalse();
            this._mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotDeleteBill, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SendBillSearchCriterionThatSearchesJustForASpecificBillId()
        {
            // Arrange
            const int expectedId = 2;
            Bill bill = new Bill { Id = expectedId, Date = "01.02.2017", KindOfBill = KindOfBill.Gutschrift };
            this._billEditViewModel.ChangeToLoadedMode(bill);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> criterion = null;
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>>(this, x => criterion = x.Content);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(c => c.Id == expectedId));
            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, null, null);

            // Act
            this._billEditViewModel.SendBillSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedTuple.ToString());
        }

        [Test]
        public void SendBillSearchCriterionThatSearchesForBillsWithSpecificBillDataAndClientNumber()
        {
            // Arrange
            const int expectedId = 2;
            KindOfBill kindOfBill = KindOfBill.Gutschrift;
            const string date = "01.02.2017";
            Client client = new Client() { Id = expectedId };
            Bill bill = new Bill { KindOfBill = kindOfBill, Date = date, Client = client };
            this._billEditViewModel.ChangeToLoadedMode(bill);

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> criterion = null;
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>>>(this, x => criterion = x.Content);

            Conjunction billConjunction = Restrictions.Conjunction();
            billConjunction.Add(Restrictions.Where<Bill>(b => b.KindOfBill == kindOfBill));
            billConjunction.Add(Restrictions.Where<Bill>(b => b.Date.IsLike(date, MatchMode.Anywhere)));

            Conjunction clientConjunction = Restrictions.Conjunction();
            clientConjunction.Add(Restrictions.Where<Client>(c => c.Id == expectedId));

            Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion> expectedTuple = new Tuple<ICriterion, Expression<Func<Bill, Client>>, ICriterion>(billConjunction, b => b.Client, clientConjunction);

            // Act
            this._billEditViewModel.SendBillSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedTuple.ToString());
        }

        [Test]
        public void SendBillSearchCriterionThatSearchesWithAllEnteredBillValues()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();
            this._billEditViewModel.ChangeToLoadedMode(bill);

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
            this._billEditViewModel.SendBillSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedTuple.ToString());
        }

        [Test]
        public void LoadBillViaMessengerMessage()
        {
            // Arrange
            const int expectedId = 2;
            const string date = "01.02.2017";
            this._mockRepository.Setup(x => x.GetById<Bill>(expectedId)).Returns(new Bill { Date = date });

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(expectedId, Resources.Message_LoadSelectedBillForBillEditVM));

            // Assert
            this._billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(date);
        }

        [Test]
        public void ReloadBillViaMessengerMessage()
        {
            // Arrange
            const string date = "01.02.2017";
            Bill bill = new Bill { Date = date };

            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(new Bill() { Date = date });
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);
            this._billEditViewModel.ChangeToLoadedMode(bill);
            this._billEditViewModel.ChangeToEditMode();

            // Act
            this._billEditViewModel.CurrentBillDetailViewModel.Date = "02.02.2016";
            this._billEditViewModel.BillCommands.Find(x => x.DisplayName == Resources.Command_DisplayName_Cancel).RelayCommand.Execute(null);

            // Assert
            this._billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(date);
        }

        [Test]
        public void ReloadBillWhenClientValuesOfLoadedBillChanged()
        {
            // Arrange
            const string firstName = "Annalena";
            Bill bill = ModelFactory.GetDefaultBill();
            bill.Client.FirstName = firstName;

            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(bill);
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_UpdateClientValuesForBillEditVM));

            // Assert
            this._billEditViewModel.CurrentBillDetailViewModel.FirstName.Should().Be(firstName);
        }

        [Test]
        public void SendsUpdateBillValuesMessage()
        {
            // Arrange
            const int expectedId = 10;
            int billId = 0;

            this._billEditViewModel.ChangeToLoadedMode(new Bill { Id = expectedId });
            Messenger.Default.Register<NotificationMessage<int>>(this, x => billId = x.Content);

            // Act
            this._billEditViewModel.SendUpdateBillValuesMessage();

            // Assert
            billId.Should().Be(expectedId);
        }

        [Test]
        public void SendNotEnabledStateForBillItemEditing()
        {
            // Arrange
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this._billEditViewModel.ChangeToEmptyMode();

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
            this._billEditViewModel.ChangeToEditMode();

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
            this._billEditViewModel.ChangeToEmptyMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[1].Should().BeTrue();
        }

        [Test]
        public void SendNotEnabledStateForWorkspaceChange()
        {
            // Arrange
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this._billEditViewModel.ChangeToEditMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[1].Should().BeFalse();
        }

        [Test]
        public void CreatesNewBillWithSpecificClientByCreateBillMessage()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_CreateNewBillForBillEditVM));

            // Assert
            this._mockRepository.Verify(x => x.GetById<Client>(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void CreatesNewBillWithDefaultValues()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_CreateNewBillForBillEditVM));

            // Assert
            this._billEditViewModel.CurrentBillDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this._billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(DateTime.Now.Date.ToShortDateString());
            this._billEditViewModel.CurrentBillDetailViewModel.KindOfBill.Should().Be(KindOfBill.Rechnung);
            this._billEditViewModel.CurrentBillDetailViewModel.KindOfVat.Should().Be(KindOfVat.inkl_MwSt);
            this._billEditViewModel.CurrentBillDetailViewModel.VatPercentage.Should().Be(Settings.Default.VatPercentage);
        }

        [Test]
        public void DoesNotChangeToCreationModeWhenReferencedClientCouldNotBeLoaded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Throws<Exception>();

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_CreateNewBillForBillEditVM));

            // Assert
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
            this._billEditViewModel.CurrentBillState.Should().NotBeOfType<BillCreationState>();
        }

        [Test]
        public void SwitchesToSearchModeAndInsertsClientDataWhenMessageReceived()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<Client>(ModelFactory.GetDefaultClient(), Resources.Message_SwitchToSearchModeAndLoadClientDataForBillEditVM));

            // Assert
            this._billEditViewModel.CurrentBillState.Should().BeOfType<BillSearchState>();
            this._billEditViewModel.CurrentBillDetailViewModel.ClientId.Should().Be(0);
            this._billEditViewModel.CurrentBillDetailViewModel.Title.Should().Be(ModelFactory.DefaultClientTitle);
            this._billEditViewModel.CurrentBillDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this._billEditViewModel.CurrentBillDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
            this._billEditViewModel.CurrentBillDetailViewModel.Street.Should().Be(ModelFactory.DefaultClientStreet);
            this._billEditViewModel.CurrentBillDetailViewModel.HouseNumber.Should().Be(ModelFactory.DefaultClientHouseNumber);
            this._billEditViewModel.CurrentBillDetailViewModel.PostalCode.Should().Be(ModelFactory.DefaultCityToPostalCodePostalCode);
            this._billEditViewModel.CurrentBillDetailViewModel.City.Should().Be(ModelFactory.DefaultCityToPostalCodeCity);
        }

        [Test]
        public void ChangeToEmptyModeWhenLoadedClientWasDeleted()
        {
            // Arrange
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_RemoveClientForBillEditVM));

            // Assert
            this._billEditViewModel.CurrentBillState.Should().BeOfType<BillEmptyState>();
        }

        [Test]
        public void CanNotClearFieldsWhenNotInLoadedState()
        {
            // Assert
            this._billEditViewModel.ClearFieldsCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanClearFieldsWhenInLoadedState()
        {
            // Act
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Assert
            this._billEditViewModel.ClearFieldsCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void ClearsFields()
        {
            // Arrange
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());

            // Act
            this._billEditViewModel.ClearFieldsCommand.Execute(null);

            // Assert
            this._billEditViewModel.CurrentBillDetailViewModel.KindOfBill.Should().Be(null);
            this._billEditViewModel.CurrentBillDetailViewModel.KindOfVat.Should().Be(null);
            this._billEditViewModel.CurrentBillDetailViewModel.Date.Should().Be(null);
        }

        [Test]
        public void SendsChangeToBillSearchViewMessageWhenFieldsAreCleared()
        {
            // Arrange
            List<string> notificationMessages = new List<string>();
            this._billEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultBill());
            Messenger.Default.Register<NotificationMessage>(this, x => notificationMessages.Add(x.Notification));

            // Act
            this._billEditViewModel.ClearFieldsCommand.Execute(null);

            // Assert
            notificationMessages.Should().Contain(Resources.Message_LoadBillSearchViewModelMessageForBillVM);
        }

        [Test]
        public async Task SetsPrintedToFalseAfterCreation()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient());
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_CreateNewBillForBillEditVM));

            // Act
            await this._billEditViewModel.SaveOrUpdateBillAsync();

            // Assert
            this._billEditViewModel.CurrentBillDetailViewModel.Printed.Should().BeFalse();
        }
    }
}