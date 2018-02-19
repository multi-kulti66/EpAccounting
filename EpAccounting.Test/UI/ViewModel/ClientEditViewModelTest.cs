// ///////////////////////////////////
// File: ClientEditViewModelTest.cs
// Last Change: 17.02.2018, 19:57
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
    public class ClientEditViewModelTest
    {
        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._clientEditViewModel = new ClientEditViewModel(this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._mockDialogService = null;
            this._clientEditViewModel = null;
            GC.Collect();
        }

        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private ClientEditViewModel _clientEditViewModel;

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(ClientEditViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void CanCreateClientEditViewModelObject()
        {
            // Assert
            this._clientEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void GetExistingCurrentClientDetailViewModelAfterCreation()
        {
            // Assert
            this._clientEditViewModel.CurrentClientDetailViewModel.Should().NotBeNull();
        }

        [Test]
        public void GetExistingClientStateAfterCreation()
        {
            // Assert
            this._clientEditViewModel.CurrentState.Should().NotBeNull().And.BeOfType<ClientEmptyState>();
        }

        [Test]
        public void CanInsertClientIdShouldReturnTrue()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this._clientEditViewModel.ChangeToSearchMode();

            // Assert
            this._clientEditViewModel.CanInsertClientId.Should().BeTrue();
        }

        [Test]
        public void CanInsertClientIdShouldReturnFalseWhenClientStateNotSearchState()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this._clientEditViewModel.CanInsertClientId.Should().BeFalse();
        }

        [Test]
        public void CanInsertClientIdShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Act
            this._clientEditViewModel.ChangeToSearchMode();

            // Assert
            this._clientEditViewModel.CanInsertClientId.Should().BeFalse();
        }

        [Test]
        public void CanEditClientDataShouldReturnTrue()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this._clientEditViewModel.ChangeToSearchMode();

            // Assert
            this._clientEditViewModel.CanEditClientData.Should().BeTrue();
        }

        [Test]
        public void CanEditClientDataShouldReturnFalseWhenClientStateCanNotCommit()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this._clientEditViewModel.CanEditClientData.Should().BeFalse();
        }

        [Test]
        public void CanEditClientDataShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Act
            this._clientEditViewModel.ChangeToSearchMode();

            // Assert
            this._clientEditViewModel.CanEditClientData.Should().BeFalse();
        }

        [Test]
        public void CanEditCompanyNameShouldReturnTrue()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            this._clientEditViewModel.ChangeToEditMode();
            this._clientEditViewModel.CurrentClientDetailViewModel.Title = ClientTitle.Firma;

            // Assert
            this._clientEditViewModel.CanEditCompanyName.Should().BeTrue();
        }

        [Test]
        public void CanEditCompanyNameShouldReturnTrueInSearchMode()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this._clientEditViewModel.ChangeToSearchMode();

            // Assert
            this._clientEditViewModel.CanEditCompanyName.Should().BeTrue();
        }

        [Test]
        public void CanEditCompanyNameShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(false);
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.ChangeToEditMode();

            // Act
            this._clientEditViewModel.CurrentClientDetailViewModel.Title = ClientTitle.Firma;

            // Assert
            this._clientEditViewModel.CanEditCompanyName.Should().BeFalse();
        }

        [Test]
        public void CanEditCompanyNameShouldReturnFalseWhenNotInEditMode()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            this._clientEditViewModel.CurrentClientDetailViewModel.Title = ClientTitle.Firma;

            // Assert
            this._clientEditViewModel.CanEditCompanyName.Should().BeFalse();
        }

        [Test]
        public void CanEditCompanyNameShouldReturnFalseWhenTitleNotCompany()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            this._clientEditViewModel.CurrentClientDetailViewModel.Title = ClientTitle.Familie;

            // Assert
            this._clientEditViewModel.CanEditCompanyName.Should().BeFalse();
        }

        [Test]
        public void CanLoadBillsShouldReturnTrue()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this._clientEditViewModel.CanLoadBills.Should().BeTrue();
        }

        [Test]
        public void CanLoadBillsShouldReturnFalseWhenClientStateNotLoadedState()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this._clientEditViewModel.CanLoadBills.Should().BeFalse();
        }

        [Test]
        public void CanLoadBillsShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this._mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this._clientEditViewModel.CanLoadBills.Should().BeFalse();
        }

        [Test]
        public void CanChangeCurrentClientDetailViewModel()
        {
            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this._clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().Be("Andre");
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentClientDetailViewModelChanges()
        {
            // Arrange
            this._clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this._clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
        }

        [Test]
        public void RaisePropertyChangedEventWhenEqualClientDetailViewModelWillBeSet()
        {
            // Arrange
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this._clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
        }

        [Test]
        public void RaisesPropertyChangedWhenClientWillBeReloaded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.CurrentClientDetailViewModel.FirstName = "Alfred";
            this._clientEditViewModel.CurrentClientDetailViewModel.LastName = "Hugendubel";
            this._clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._clientEditViewModel.Load();

            // Assert
            this._clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
            this._clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this._clientEditViewModel.CurrentClientDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
        }

        [Test]
        public void ShowsExceptionMessageWhenClientCouldNotBeReloaded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Throws<Exception>();

            // Act
            this._clientEditViewModel.Load();

            // Assert
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CanChangeToCreationMode()
        {
            // Act
            this._clientEditViewModel.ChangeToCreationMode();

            // Assert
            this._clientEditViewModel.CurrentState.Should().BeOfType<ClientCreationState>();
        }

        [Test]
        public void CanChangeToLoadedModeWhenClientIsNull()
        {
            // Act
            this._clientEditViewModel.ChangeToLoadedMode();

            // Assert
            this._clientEditViewModel.CurrentState.Should().BeOfType<ClientLoadedState>();
        }

        [Test]
        public void CanChangeToEditMode()
        {
            // Arrange
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            this._clientEditViewModel.ChangeToEditMode();

            // Assert
            this._clientEditViewModel.CurrentState.Should().BeOfType<ClientEditState>();
        }

        [Test]
        public void CanNotChangeToEditMode()
        {
            // Act
            this._clientEditViewModel.ChangeToEditMode();

            // Assert
            this._clientEditViewModel.CurrentState.Should().BeOfType<ClientEmptyState>();
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentClientStateChanges()
        {
            // Arrange
            this._clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._clientEditViewModel.ChangeToCreationMode();

            // Assert
            this._clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentState);
        }

        [Test]
        public void RaisePropertyChangedEvenWhenEqualClientStateWillBeSet()
        {
            // Arrange
            this._clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._clientEditViewModel.ChangeToEmptyMode();

            // Assert
            this._clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentState);
        }

        [Test]
        public void GetInstancesOfClientStates()
        {
            // Assert
            this._clientEditViewModel.GetClientEmptyState().Should().NotBeNull().And.BeOfType<ClientEmptyState>();
            this._clientEditViewModel.GetClientCreationState().Should().NotBeNull().And.BeOfType<ClientCreationState>();
            this._clientEditViewModel.GetClientSearchState().Should().NotBeNull().And.BeOfType<ClientSearchState>();
            this._clientEditViewModel.GetClientLoadedState().Should().NotBeNull().And.BeOfType<ClientLoadedState>();
            this._clientEditViewModel.GetClientEditState().Should().NotBeNull().And.BeOfType<ClientEditState>();
        }

        [Test]
        public void ClientCommandsAreInitialized()
        {
            // Assert
            this._clientEditViewModel.StateCommands.Should().HaveCount(6);
        }

        [Test]
        public async Task DoNotAddNewClientWhenClientHasMissingNamePart()
        {
            // Act
            bool result = await this._clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            this._mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient,
                                                             It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotAddNewClientWhenClientHasMissingPostalCode()
        {
            // Arrange
            this._clientEditViewModel.CurrentClientDetailViewModel.FirstName = "Andre";
            this._clientEditViewModel.CurrentClientDetailViewModel.LastName = "Multerer";

            // Act
            bool result = await this._clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            this._mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient,
                                                             It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task AddClientToDatabaseIfNoEqualClientExists()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client>());
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this._clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeTrue();
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public async Task AddClientToDatabaseIfEqualClientExistsAndDialogResultIsYes()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client> { new Client { CityToPostalCode = new CityToPostalCode() } });
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(() => Task.FromResult(true));
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this._clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeTrue();
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Client>()), Times.Once);
            this._mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotAddClientToDatabaseIfEqualClientExistsAndDialogResultIsNo()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client> { new Client { CityToPostalCode = new CityToPostalCode() } });
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this._clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Client>()), Times.Never);
            this._mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotRaisePropertyChangedWhenNewClientWasAdded()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client>());
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            await this._clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            this._clientEditViewModel.ShouldNotRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
        }

        [Test]
        public async Task ShowMessageWhenClientCouldNotBeAddedToDatabaseBecauseOfAnException()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client>());
            this._mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<Client>())).Throws(new Exception());
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this._clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Client>()), Times.Once);
            this._mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient,
                                                             It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotDeleteClientWhenDialogResultIsNo()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            // Act
            bool result = await this._clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeFalse();
            this._mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteClientWhenDialogResultIsYes()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this._clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeTrue();
            this._mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this._mockRepository.Verify(x => x.Delete(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public async Task SendRemoveMessagesWhenClientWasDeleted()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            List<string> notificationMessages = new List<string>();
            Messenger.Default.Register<NotificationMessage>(this, x => notificationMessages.Add(x.Notification));
            Messenger.Default.Register<NotificationMessage<int>>(this, x => notificationMessages.Add(x.Notification));

            // Act
            await this._clientEditViewModel.DeleteClientAsync();

            // Assert
            notificationMessages.Count.Should().Be(4);
            notificationMessages.Should().Contain(Resources.Message_ReloadClientsForClientSearchVM);
            notificationMessages.Should().Contain(Resources.Message_RemoveClientForBillEditVM);
            notificationMessages.Should().Contain(Resources.Message_RemoveClientForBillSearchVM);
            notificationMessages.Should().Contain(Resources.Message_RemoveClientForBillItemEditVM);
        }

        [Test]
        public async Task ShowMessageWhenClientCouldNotBeDeletedBecauseOfAnException()
        {
            // Arrange
            this._mockRepository.Setup(x => x.Delete(It.IsAny<Client>())).Throws(new Exception());
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this._clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeFalse();
            this._mockRepository.Verify(x => x.Delete(It.IsAny<Client>()), Times.Once);
            this._mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotDeleteClient, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SendClientSearchCriterionThatSearchesJustForASpecificClientId()
        {
            // Arrange
            const int expectedId = 2;
            Client client = new Client { Id = expectedId, FirstName = "Andre", LastName = "Multerer" };
            this._clientEditViewModel.ChangeToLoadedMode(client);

            ICriterion criterion = null;
            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, x => criterion = x.Content);

            Conjunction expectedCriterion = Restrictions.Conjunction();
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Id == expectedId));

            // Act
            this._clientEditViewModel.SendClientSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedCriterion.ToString());
        }

        [Test]
        public void SendClientSearchCriterionThatSearchesWithAllEnteredClientValues()
        {
            // Arrange
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            ICriterion criterion = null;
            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, x => criterion = x.Content);

            Conjunction expectedCriterion = Restrictions.Conjunction();
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.CompanyName.IsLike(ModelFactory.DefaultClientCompanyName, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.FirstName.IsLike(ModelFactory.DefaultClientFirstName, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.LastName.IsLike(ModelFactory.DefaultClientLastName, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Street.IsLike(ModelFactory.DefaultClientStreet, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.HouseNumber.IsLike(ModelFactory.DefaultClientHouseNumber, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.CityToPostalCode.PostalCode.IsLike(ModelFactory.DefaultCityToPostalCodePostalCode, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.CityToPostalCode.City.IsLike(ModelFactory.DefaultCityToPostalCodeCity, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.DateOfBirth.IsLike(ModelFactory.DefaultClientDateOfBirth, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.PhoneNumber1.IsLike(ModelFactory.DefaultClientPhoneNumber1, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.PhoneNumber2.IsLike(ModelFactory.DefaultClientPhoneNumber2, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.MobileNumber.IsLike(ModelFactory.DefaultClientMobileNumber, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Telefax.IsLike(ModelFactory.DefaultClientTelefax, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Email.IsLike(ModelFactory.DefaultClientEmail, MatchMode.Anywhere)));

            // Act
            this._clientEditViewModel.SendClientSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedCriterion.ToString());
        }

        [Test]
        public void LoadClientViaMessengerMessage()
        {
            // Arrange
            const int expectedId = 2;
            this._mockRepository.Setup(x => x.GetById<Client>(expectedId)).Returns(new Client { CityToPostalCode = new CityToPostalCode() });

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(expectedId, Resources.Message_LoadClientForClientEditVM));

            // Assert
            this._mockRepository.Verify(x => x.GetById<Client>(expectedId), Times.Once);
            this._clientEditViewModel.CurrentState.Should().BeOfType<ClientLoadedState>();
        }

        [Test]
        public void SendEnabledStateForClientSearchWhenInLoadedMode()
        {
            // Arrange
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(new Client { CityToPostalCode = new CityToPostalCode() });

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[0].Should().BeTrue();
        }

        [Test]
        public void SendEnabledStateForClientSearchWhenInCreationMode()
        {
            // Arrange
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this._clientEditViewModel.ChangeToCreationMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[0].Should().BeFalse();
        }

        [Test]
        public void SendNotEnabledStateForClientSearchWhenInEmptyMode()
        {
            // Arrange
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this._clientEditViewModel.ChangeToEmptyMode();

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
            this._clientEditViewModel.ChangeToEmptyMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[1].Should().BeTrue();
        }

        [Test]
        public void SendNotEnabledStateForWorkspaceChange()
        {
            // Arrange
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this._clientEditViewModel.ChangeToEditMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[1].Should().BeFalse();
        }

        [Test]
        public void SendNotEnabledStateForClientSearchWhenInSearchMode()
        {
            // Arrange
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this._clientEditViewModel.ChangeToSearchMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[0].Should().BeTrue();
        }

        [Test]
        public void CanNotSendCreateNewBillMessageWhenNotInLoadedClientState()
        {
            // Assert
            this._clientEditViewModel.CreateNewBillCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanSendCreateNewBillMessageWhenInLoadedClientState()
        {
            // Act
            this._clientEditViewModel.ChangeToLoadedMode(new Client { CityToPostalCode = new CityToPostalCode() });

            // Assert
            this._clientEditViewModel.CreateNewBillCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendChangeToBillWorkspaceMessageWhenNewBillShouldBeCreated()
        {
            // Arrange
            string notificationMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => notificationMessage = x.Notification);

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.CreateNewBillCommand.Execute(null);

            // Assert
            notificationMessage.Should().Be(Resources.Message_ChangeToBillWorkspaceForMainVM);
        }

        [Test]
        public void SendCreateNewBillMessageWhenNewBillShouldBeCreated()
        {
            // Arrange
            string notificationMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage<int>>(this, x => notificationMessage = x.Notification);

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.CreateNewBillCommand.Execute(null);

            // Assert
            notificationMessage.Should().Be(Resources.Message_CreateNewBillForBillEditVM);
        }

        [Test]
        public void CanNotSendLoadBillsFromClientMessageWhenNotInLoadedClientState()
        {
            // Assert
            this._clientEditViewModel.LoadBillsFromClientCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanSendLoadBillsFromClientMessageWhenInLoadedClientState()
        {
            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this._clientEditViewModel.LoadBillsFromClientCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendChangeToBillWorkspaceMessageWhenBillsFromClientShouldBeLoaded()
        {
            // Arrange
            string notificationMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => notificationMessage = x.Notification);

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.LoadBillsFromClientCommand.Execute(null);

            // Assert
            notificationMessage.Should().Be(Resources.Message_ChangeToBillWorkspaceForMainVM);
        }

        [Test]
        public void SendLoadBillsFromClientMessageWhenBillsFromClientShouldBeLoaded()
        {
            // Arrange
            string notificationMessage = this.ToString();
            Messenger.Default.Register<NotificationMessage<int>>(this, x => notificationMessage = x.Notification);

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.LoadBillsFromClientCommand.Execute(null);

            // Assert
            notificationMessage.Should().Be(Resources.Message_LoadBillsFromClientForBillSearchVM);
        }

        [Test]
        public void SendSwitchToSearchModeMessageWhenBillsFromClientSholdBeLoaded()
        {
            // Arrange
            string notificationMessage = this.ToString();
            Messenger.Default.Register<NotificationMessage<Client>>(this, x => notificationMessage = x.Notification);

            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.LoadBillsFromClientCommand.Execute(null);

            // Assert
            notificationMessage.Should().Be(Resources.Message_SwitchToSearchModeAndLoadClientDataForBillEditVM);
        }

        [Test]
        public void SendsUpdateClientMessages()
        {
            // Arrange
            List<string> notificationMessages = new List<string>();
            Messenger.Default.Register<NotificationMessage<int>>(this, x => notificationMessages.Add(x.Notification));

            // Act
            this._clientEditViewModel.SendUpdateClientValuesMessage();

            // Assert
            notificationMessages.Count.Should().Be(3);
            notificationMessages.Should().Contain(Resources.Message_UpdateClientValuesForClientSearchVM);
            notificationMessages.Should().Contain(Resources.Message_UpdateClientValuesForBillEditVM);
            notificationMessages.Should().Contain(Resources.Message_UpdateClientValuesForBillSearchVM);
        }

        [Test]
        public void ReloadClientWhenUpdateClientMessageWasReceived()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(1)).Returns(ModelFactory.GetDefaultClient);
            this._clientEditViewModel.ChangeToLoadedMode(new Client { Id = 1, FirstName = "Michael" });

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_UpdateClientForClientEditVM));

            // Assert
            this._clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
        }

        [Test]
        public void CanNotClearFieldsWhenNotInLoadedState()
        {
            // Assert
            this._clientEditViewModel.ClearFieldsCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanClearFieldsWhenInLoadedState()
        {
            // Act
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this._clientEditViewModel.ClearFieldsCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void ClearsFields()
        {
            // Arrange
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            this._clientEditViewModel.ClearFieldsCommand.Execute(null);

            // Assert
            this._clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().BeNullOrEmpty();
            this._clientEditViewModel.CurrentClientDetailViewModel.LastName.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task DeletesPostalCodeWhenDeletedClientWasTheOnlyReference()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>())).Returns(0);

            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this._clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeTrue();
            this._mockRepository.Verify(x => x.Delete(It.IsAny<CityToPostalCode>()), Times.Once());
        }

        [Test]
        public async Task DoesNotDeletePostalCodeWhenDeletedClientWasNotTheOnlyReference()
        {
            // Arrange
            this._mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>())).Returns(1);

            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this._clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeTrue();
            this._mockRepository.Verify(x => x.Delete(It.IsAny<CityToPostalCode>()), Times.Never);
        }

        [Test]
        public async Task DeletesPreviousPostalCodeWhenChangedClientWasTheOnlyReference()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient());
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>())).Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>())).Returns(0);

            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.ChangeToEditMode();

            // Act
            bool result = await this._clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeTrue();
            this._mockRepository.Verify(x => x.Delete(It.IsAny<CityToPostalCode>()), Times.Once());
        }

        [Test]
        public async Task DoesNotDeletePreviousPostalCodeWhenChangedClientWasNotTheOnlyReference()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient());
            this._mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), It.IsAny<int>())).Returns(new List<Client>());
            this._mockRepository.Setup(x => x.GetQuantityByCriteria<Client>(It.IsAny<ICriterion>())).Returns(1);

            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this._clientEditViewModel.ChangeToEditMode();

            // Act
            bool result = await this._clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeTrue();
            this._mockRepository.Verify(x => x.Delete(It.IsAny<CityToPostalCode>()), Times.Never);
        }

        [Test]
        public void UpdatesCompanyNameEnableStateWhenTitleChanges()
        {
            // Arrange
            this._clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();
            this._mockRepository.Setup(x => x.IsConnected).Returns(true);
            this._clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            this._clientEditViewModel.ChangeToEditMode();
            this._clientEditViewModel.CurrentClientDetailViewModel.Title = ClientTitle.Firma;

            // Assert
            this._clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CanEditCompanyName);
        }
    }
}