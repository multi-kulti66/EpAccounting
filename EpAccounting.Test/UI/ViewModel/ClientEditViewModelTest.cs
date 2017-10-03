﻿// ///////////////////////////////////
// File: ClientEditViewModelTest.cs
// Last Change: 31.08.2017  20:09
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
        #region Fields

        private Mock<IRepository> mockRepository;
        private Mock<IDialogService> mockDialogService;
        private ClientEditViewModel clientEditViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.mockDialogService = new Mock<IDialogService>();
            this.clientEditViewModel = new ClientEditViewModel(this.mockRepository.Object, this.mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.mockDialogService = null;
            this.clientEditViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            typeof(ClientEditViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void CanCreateClientEditViewModelObject()
        {
            // Assert
            this.clientEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void GetExistingCurrentClientDetailViewModelAfterCreation()
        {
            // Assert
            this.clientEditViewModel.CurrentClientDetailViewModel.Should().NotBeNull();
        }

        [Test]
        public void GetExistingClientStateAfterCreation()
        {
            // Assert
            this.clientEditViewModel.CurrentState.Should().NotBeNull().And.BeOfType<ClientEmptyState>();
        }

        [Test]
        public void CanInsertClientIdShouldReturnTrue()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this.clientEditViewModel.ChangeToSearchMode();

            // Assert
            this.clientEditViewModel.CanInsertClientId.Should().BeTrue();
        }

        [Test]
        public void CanInsertClientIdShouldReturnFalseWhenClientStateNotSearchState()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this.clientEditViewModel.CanInsertClientId.Should().BeFalse();
        }

        [Test]
        public void CanInsertClientIdShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Act
            this.clientEditViewModel.ChangeToSearchMode();

            // Assert
            this.clientEditViewModel.CanInsertClientId.Should().BeFalse();
        }

        [Test]
        public void CanEditClientDataShouldReturnTrue()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this.clientEditViewModel.ChangeToSearchMode();

            // Assert
            this.clientEditViewModel.CanEditClientData.Should().BeTrue();
        }

        [Test]
        public void CanEditClientDataShouldReturnFalseWhenClientStateCanNotCommit()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this.clientEditViewModel.CanEditClientData.Should().BeFalse();
        }

        [Test]
        public void CanEditClientDataShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Act
            this.clientEditViewModel.ChangeToSearchMode();

            // Assert
            this.clientEditViewModel.CanEditClientData.Should().BeFalse();
        }

        [Test]
        public void CanLoadBillsShouldReturnTrue()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Act
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this.clientEditViewModel.CanLoadBills.Should().BeTrue();
        }

        [Test]
        public void CanLoadBillsShouldReturnFalseWhenClientStateNotLoadedState()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(true);

            // Assert
            this.clientEditViewModel.CanLoadBills.Should().BeFalse();
        }

        [Test]
        public void CanLoadBillsShouldReturnFalseWhenNotConnectedToDatabase()
        {
            // Arrange
            this.mockRepository.Setup(x => x.IsConnected).Returns(false);

            // Act
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this.clientEditViewModel.CanLoadBills.Should().BeFalse();
        }

        [Test]
        public void CanChangeCurrentClientDetailViewModel()
        {
            // Act
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this.clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().Be("Andre");
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentClientDetailViewModelChanges()
        {
            // Arrange
            this.clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this.clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
        }

        [Test]
        public void RaisePropertyChangedEventWhenEqualClientDetailViewModelWillBeSet()
        {
            // Arrange
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this.clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this.clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
        }

        [Test]
        public void RaisesPropertyChangedWhenClientWillBeReloaded()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetById<Client>(It.IsAny<int>())).Returns(ModelFactory.GetDefaultClient);
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this.clientEditViewModel.CurrentClientDetailViewModel.FirstName = "Alfred";
            this.clientEditViewModel.CurrentClientDetailViewModel.LastName = "Hugendubel";
            this.clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.clientEditViewModel.Reload();

            // Assert
            this.clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
            this.clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
            this.clientEditViewModel.CurrentClientDetailViewModel.LastName.Should().Be(ModelFactory.DefaultClientLastName);
        }

        [Test]
        public void CanChangeToCreationMode()
        {
            // Act
            this.clientEditViewModel.ChangeToCreationMode();

            // Assert
            this.clientEditViewModel.CurrentState.Should().BeOfType<ClientCreationState>();
        }

        [Test]
        public void CanChangeToLoadedModeWhenClientIsNull()
        {
            // Act
            this.clientEditViewModel.ChangeToLoadedMode();

            // Assert
            this.clientEditViewModel.CurrentState.Should().BeOfType<ClientLoadedState>();
        }

        [Test]
        public void CanChangeToEditMode()
        {
            // Arrange
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            this.clientEditViewModel.ChangeToEditMode();

            // Assert
            this.clientEditViewModel.CurrentState.Should().BeOfType<ClientEditState>();
        }

        [Test]
        public void CanNotChangeToEditMode()
        {
            // Act
            this.clientEditViewModel.ChangeToEditMode();

            // Assert
            this.clientEditViewModel.CurrentState.Should().BeOfType<ClientEmptyState>();
        }

        [Test]
        public void RaisesPropertyChangedWhenCurrentClientStateChanges()
        {
            // Arrange
            this.clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.clientEditViewModel.ChangeToCreationMode();

            // Assert
            this.clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentState);
        }

        [Test]
        public void RaisePropertyChangedEvenWhenEqualClientStateWillBeSet()
        {
            // Arrange
            this.clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.clientEditViewModel.ChangeToEmptyMode();

            // Assert
            this.clientEditViewModel.ShouldRaisePropertyChangeFor(x => x.CurrentState);
        }

        [Test]
        public void GetInstancesOfClientStates()
        {
            // Assert
            this.clientEditViewModel.GetClientEmptyState().Should().NotBeNull().And.BeOfType<ClientEmptyState>();
            this.clientEditViewModel.GetClientCreationState().Should().NotBeNull().And.BeOfType<ClientCreationState>();
            this.clientEditViewModel.GetClientSearchState().Should().NotBeNull().And.BeOfType<ClientSearchState>();
            this.clientEditViewModel.GetClientLoadedState().Should().NotBeNull().And.BeOfType<ClientLoadedState>();
            this.clientEditViewModel.GetClientEditState().Should().NotBeNull().And.BeOfType<ClientEditState>();
        }

        [Test]
        public void ClientCommandsAreInitialized()
        {
            // Assert
            this.clientEditViewModel.StateCommands.Should().HaveCount(6);
        }

        [Test]
        public async Task DoNotAddNewClientWhenClientHasMissingNamePart()
        {
            // Act
            bool result = await this.clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            this.mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient,
                                                             It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task AddClientToDatabaseIfNoEqualClientExists()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client>());
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this.clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeTrue();
            this.mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public async Task AddClientToDatabaseIfEqualClientExistsAndDialogResultIsYes()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client> { new Client() });
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(() => Task.FromResult(true));
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this.clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeTrue();
            this.mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Client>()), Times.Once);
            this.mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotAddClientToDatabaseIfEqualClientExistsAndDialogResultIsNo()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client> { new Client() });
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this.clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            this.mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Client>()), Times.Never);
            this.mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotRaisePropertyChangedWhenNewClientWasAdded()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client>());
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this.clientEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            await this.clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            this.clientEditViewModel.ShouldNotRaisePropertyChangeFor(x => x.CurrentClientDetailViewModel);
        }

        [Test]
        public async Task ShowMessageWhenClientCouldNotBeAddedToDatabaseBecauseOfAnException()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria<Client>(It.IsAny<ICriterion>(), 1)).Returns(() => new List<Client>());
            this.mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<Client>())).Throws(new Exception());
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this.clientEditViewModel.SaveOrUpdateClientAsync();

            // Assert
            result.Should().BeFalse();
            this.mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Client>()), Times.Once);
            this.mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient,
                                                             It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DoNotDeleteClientWhenDialogResultIsNo()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            // Act
            bool result = await this.clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeFalse();
            this.mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteClientWhenDialogResultIsYes()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            // Act
            bool result = await this.clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeTrue();
            this.mockDialogService.Verify(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this.mockRepository.Verify(x => x.Delete(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public async Task SendRemoveMessagesWhenClientWasDeleted()
        {
            // Arrange
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            List<string> notificationMessages = new List<string>();
            Messenger.Default.Register<NotificationMessage<int>>(this, x => notificationMessages.Add(x.Notification));

            // Act
            await this.clientEditViewModel.DeleteClientAsync();

            // Assert
            notificationMessages.Count.Should().Be(4);
            notificationMessages.Should().Contain(Resources.Message_RemoveClientForClientSearchVM);
            notificationMessages.Should().Contain(Resources.Message_RemoveClientForBillEditVM);
            notificationMessages.Should().Contain(Resources.Message_RemoveClientForBillSearchVM);
            notificationMessages.Should().Contain(Resources.Message_RemoveClientForBillItemEditVM);
        }

        [Test]
        public async Task ShowMessageWhenClientCouldNotBeDeletedBecauseOfAnException()
        {
            // Arrange
            this.mockRepository.Setup(x => x.Delete(It.IsAny<Client>())).Throws(new Exception());
            this.mockDialogService.Setup(x => x.ShowDialogYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            bool result = await this.clientEditViewModel.DeleteClientAsync();

            // Assert
            result.Should().BeFalse();
            this.mockRepository.Verify(x => x.Delete(It.IsAny<Client>()), Times.Once);
            this.mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_CanNotDeleteClient, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SendClientSearchCriterionThatSearchesJustForASpecificClientId()
        {
            // Arrange
            const int ExpectedId = 2;
            Client client = new Client { Id = ExpectedId, FirstName = "Andre", LastName = "Multerer" };
            this.clientEditViewModel.ChangeToLoadedMode(client);

            ICriterion criterion = null;
            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, x => criterion = x.Content);

            Conjunction expectedCriterion = Restrictions.Conjunction();
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Id == ExpectedId));

            // Act
            this.clientEditViewModel.SendClientSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedCriterion.ToString());
        }

        [Test]
        public void SendClientSearchCriterionThatSearchesWithAllEnteredClientValues()
        {
            // Arrange
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            ICriterion criterion = null;
            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, x => criterion = x.Content);

            Conjunction expectedCriterion = Restrictions.Conjunction();
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Title == ModelFactory.DefaultClientTitle));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.FirstName.IsLike(ModelFactory.DefaultClientFirstName, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.LastName.IsLike(ModelFactory.DefaultClientLastName, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Street.IsLike(ModelFactory.DefaultClientStreet, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.HouseNumber.IsLike(ModelFactory.DefaultClientHouseNumber, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.PostalCode.IsLike(ModelFactory.DefaultClientPostalCode, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.City.IsLike(ModelFactory.DefaultClientCity, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.DateOfBirth.IsLike(ModelFactory.DefaultClientDateOfBirth, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.PhoneNumber1.IsLike(ModelFactory.DefaultClientPhoneNumber1, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.PhoneNumber2.IsLike(ModelFactory.DefaultClientPhoneNumber2, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.MobileNumber.IsLike(ModelFactory.DefaultClientMobileNumber, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Telefax.IsLike(ModelFactory.DefaultClientTelefax, MatchMode.Anywhere)));
            expectedCriterion.Add(Restrictions.Where<Client>(c => c.Email.IsLike(ModelFactory.DefaultClientEmail, MatchMode.Anywhere)));

            // Act
            this.clientEditViewModel.SendClientSearchCriterionMessage();

            // Assert
            criterion.Should().NotBeNull();
            criterion.ToString().Should().Be(expectedCriterion.ToString());
        }

        [Test]
        public void LoadClientViaMessengerMessage()
        {
            // Arrange
            const int ExpectedId = 2;
            this.mockRepository.Setup(x => x.GetById<Client>(ExpectedId)).Returns(new Client());

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(ExpectedId, Resources.Message_LoadClientForClientEditVM));

            // Assert
            this.mockRepository.Verify(x => x.GetById<Client>(ExpectedId), Times.Once);
            this.clientEditViewModel.CurrentState.Should().BeOfType<ClientLoadedState>();
        }

        [Test]
        public void SendEnabledStateForClientSearchWhenInLoadedMode()
        {
            // Arrange
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this.clientEditViewModel.ChangeToLoadedMode(new Client());

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
            this.clientEditViewModel.ChangeToCreationMode();

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
            this.clientEditViewModel.ChangeToEmptyMode();

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
            this.clientEditViewModel.ChangeToEmptyMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[1].Should().BeTrue();
        }

        [Test]
        public void SendNotEnabledStateForWorkspaceChange()
        {
            // Arrange
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            List<bool> areEnabled = new List<bool>();
            Messenger.Default.Register<NotificationMessage<bool>>(this, x => areEnabled.Add(x.Content));

            // Act
            this.clientEditViewModel.ChangeToEditMode();

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
            this.clientEditViewModel.ChangeToSearchMode();

            // Assert
            areEnabled.Should().HaveCount(2);
            areEnabled[0].Should().BeTrue();
        }

        [Test]
        public void CanNotSendCreateNewBillMessageWhenNotInLoadedClientState()
        {
            // Assert
            this.clientEditViewModel.CreateNewBillCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanSendCreateNewBillMessageWhenInLoadedClientState()
        {
            // Act
            this.clientEditViewModel.ChangeToLoadedMode(new Client());

            // Assert
            this.clientEditViewModel.CreateNewBillCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendChangeToBillWorkspaceMessageWhenNewBillShouldBeCreated()
        {
            // Arrange
            string notificationMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => notificationMessage = x.Notification);

            // Act
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this.clientEditViewModel.CreateNewBillCommand.Execute(null);

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
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this.clientEditViewModel.CreateNewBillCommand.Execute(null);

            // Assert
            notificationMessage.Should().Be(Resources.Message_CreateNewBillForBillEditVM);
        }

        [Test]
        public void CanNotSendLoadBillsFromClientMessageWhenNotInLoadedClientState()
        {
            // Assert
            this.clientEditViewModel.LoadBillsFromClientCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanSendLoadBillsFromClientMessageWhenInLoadedClientState()
        {
            // Act
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this.clientEditViewModel.LoadBillsFromClientCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void SendChangeToBillWorkspaceMessageWhenBillsFromClientShouldBeLoaded()
        {
            // Arrange
            string notificationMessage = string.Empty;
            Messenger.Default.Register<NotificationMessage>(this, x => notificationMessage = x.Notification);

            // Act
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this.clientEditViewModel.LoadBillsFromClientCommand.Execute(null);

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
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this.clientEditViewModel.LoadBillsFromClientCommand.Execute(null);

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
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());
            this.clientEditViewModel.LoadBillsFromClientCommand.Execute(null);

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
            this.clientEditViewModel.SendUpdateClientValuesMessage();

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
            this.mockRepository.Setup(x => x.GetById<Client>(1)).Returns(ModelFactory.GetDefaultClient);
            this.clientEditViewModel.ChangeToLoadedMode(new Client { Id = 1, FirstName = "Michael" });

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Message_UpdateClientForClientEditVM));

            // Assert
            this.clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().Be(ModelFactory.DefaultClientFirstName);
        }

        [Test]
        public void CanNotClearFieldsWhenNotInLoadedState()
        {
            // Assert
            this.clientEditViewModel.ClearFieldsCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanClearFieldsWhenInLoadedState()
        {
            // Act
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Assert
            this.clientEditViewModel.ClearFieldsCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void ClearsFields()
        {
            // Arrange
            this.clientEditViewModel.ChangeToLoadedMode(ModelFactory.GetDefaultClient());

            // Act
            this.clientEditViewModel.ClearFieldsCommand.Execute(null);

            // Assert
            this.clientEditViewModel.CurrentClientDetailViewModel.FirstName.Should().BeNullOrEmpty();
            this.clientEditViewModel.CurrentClientDetailViewModel.LastName.Should().BeNullOrEmpty();
        }

        #endregion
    }
}