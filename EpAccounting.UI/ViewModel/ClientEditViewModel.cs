// ///////////////////////////////////
// File: ClientEditViewModel.cs
// Last Change: 03.08.2017  20:39
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.State;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using NHibernate.Criterion;



    public class ClientEditViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private Client currentClient;
        private ClientDetailViewModel _currentClientDetailViewModel;
        private IClientState _currentClientState;

        private RelayCommand _createNewBillCommand;

        #endregion



        #region Constructors / Destructor

        public ClientEditViewModel(IRepository repository, IDialogService dialogService)
        {
            this.repository = repository;
            this.dialogService = dialogService;

            this.InitPropertyInfos();
            this.InitClientStateList();
            this.InitClientCommands();

            this._currentClientState = this.GetClientEmptyState();
            this.SetCurrentClient(new Client());

            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public List<ImageCommandViewModel> ClientCommands { get; private set; }

        public ClientDetailViewModel CurrentClientDetailViewModel
        {
            get { return this._currentClientDetailViewModel; }
            private set { this.SetProperty(ref this._currentClientDetailViewModel, value); }
        }

        public IClientState CurrentClientState
        {
            get { return this._currentClientState; }
            private set { this.SetProperty(ref this._currentClientState, value); }
        }

        public RelayCommand CreateNewBillCommand
        {
            get
            {
                if (this._createNewBillCommand == null)
                {
                    this._createNewBillCommand = new RelayCommand(this.SendCreateNewBillMessage, this.CanCreateNewBill);
                }

                return this._createNewBillCommand;
            }
        }

        public bool CanInsertClientId
        {
            get
            {
                if (this.repository.IsConnected && this.CurrentClientState is ClientSearchState)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanEditClientData
        {
            get
            {
                if (this.repository.IsConnected && this.CanCommit())
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanDoBillAction
        {
            get
            {
                if (this.repository.IsConnected && this.CurrentClientState is ClientLoadedState)
                {
                    return true;
                }

                return false;
            }
        }

        private List<IClientState> ClientStates { get; set; }

        private PropertyInfo[] PropertyInfos { get; set; }

        #endregion



        public virtual void Load(Client client, IClientState clientState)
        {
            this.LoadClient(client);
            this.LoadClientState(clientState);
            this.UpdateProperties();
            this.UpdateCommands();
            this.SendEnableStateForClientLoadingMessage();
        }

        public virtual void Reload()
        {
            this.Load(this.repository.GetById<Client>(this.currentClient.ClientId), this.GetClientLoadedState());
        }

        public virtual async Task<bool> SaveOrUpdateClientAsync()
        {
            if (this.CurrentClientDetailViewModel.HasMissingLastName)
            {
                await this.dialogService.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient, Resources.Dialog_Message_ClientHasMissingNamePart);
                return false;
            }

            try
            {
                bool shouldSaveOrUpdate = true;
                Client equalClient = this.GetEqualClient();

                if (equalClient != null)
                {
                    shouldSaveOrUpdate = await this.dialogService.ShowDialogYesNo(Resources.Dialog_Title_ClientExistsAlready,
                                                                                  string.Format(Resources.Dialog_Question_AddToExistingClient, equalClient));
                }

                if (shouldSaveOrUpdate)
                {
                    this.repository.SaveOrUpdate(this.currentClient);
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                await this.dialogService.ShowMessage(Resources.Dialog_Title_CanNotSaveOrUpdateClient, e.Message);
                return false;
            }
        }

        public virtual async Task<bool> DeleteClientAsync()
        {
            bool shouldDelete = await this.dialogService.ShowDialogYesNo(Resources.Dialog_Title_Attention,
                                                                         Resources.Dialog_Question_DeleteClient);

            if (!shouldDelete)
            {
                return false;
            }

            try
            {
                this.repository.Delete(this.currentClient);
                this.SendRemoveClientMessage();
                return true;
            }
            catch (Exception e)
            {
                await this.dialogService.ShowMessage(Resources.Dialog_Title_CanNotDeleteClient, e.Message);
                return false;
            }
        }

        public virtual void SendClientSearchCriterionMessage()
        {
            Messenger.Default.Send(new NotificationMessage<ICriterion>(this.GetClientSearchCriterion(), Resources.Messenger_Message_ClientSearchCriteriaForClientSearchVM));
        }

        public virtual void SendUpdateClientValuesMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId, Resources.Messenger_Message_UpdateClientValuesMessageForClientSearchVM));
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId, Resources.Messenger_Message_UpdateClientValuesMessageForBillEditVM));
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId, Resources.Messenger_Message_UpdateClientValuesMessageForBillSearchVM));
        }

        private void SendRemoveClientMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId, Resources.Messenger_Message_RemoveClientMessageForClientSearchVM));
        }

        private void SendEnableStateForClientLoadingMessage()
        {
            if (this.CurrentClientState.GetType() == typeof(ClientEditState) || this.CurrentClientState.GetType() == typeof(ClientCreationState))
            {
                Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Messenger_Message_EnableStateMessageForClientSearchVM));
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForClientSearchVM));
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Messenger_Message_LoadSelectedClientMessageForClientEditVM)
            {
                this.Load(this.repository.GetById<Client>(message.Content), this.GetClientLoadedState());
            }
        }

        private void LoadClient(Client client)
        {
            if (client == null)
            {
                return;
            }

            this.SetCurrentClient(client);
        }

        private void LoadClientState(IClientState clientState)
        {
            if (clientState == null)
            {
                return;
            }

            this.CurrentClientState = clientState;
        }

        private void UpdateCommands()
        {
            foreach (ImageCommandViewModel command in this.ClientCommands)
            {
                command.RelayCommand.RaiseCanExecuteChanged();
            }

            this.CreateNewBillCommand.RaiseCanExecuteChanged();
        }

        private void UpdateProperties()
        {
            foreach (PropertyInfo propertyInfo in this.PropertyInfos)
            {
                this.RaisePropertyChanged(propertyInfo.Name);
            }
        }

        private void SetCurrentClient(Client client)
        {
            this.currentClient = client;
            this.CurrentClientDetailViewModel = new ClientDetailViewModel(this.currentClient);
        }

        private Client GetEqualClient()
        {
            return this.repository.GetByCriteria<Client>(this.GetEqualClientCriterion(), 1).FirstOrDefault();
        }

        private ICriterion GetEqualClientCriterion()
        {
            return Restrictions.Where<Client>(c => (c.ClientId != this.CurrentClientDetailViewModel.ClientId &&
                                                    c.FirstName == this.CurrentClientDetailViewModel.FirstName) &&
                                                   (c.LastName == this.CurrentClientDetailViewModel.LastName));
        }

        private ICriterion GetClientSearchCriterion()
        {
            Conjunction conjunction = Restrictions.Conjunction();

            if (this.CurrentClientDetailViewModel.ClientId != 0)
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.ClientId == this.CurrentClientDetailViewModel.ClientId));
                return conjunction;
            }

            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Title))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Title.IsLike(this.CurrentClientDetailViewModel.Title, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.FirstName))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.FirstName.IsLike(this.CurrentClientDetailViewModel.FirstName, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.LastName))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.LastName.IsLike(this.CurrentClientDetailViewModel.LastName, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Street))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Street.IsLike(this.CurrentClientDetailViewModel.Street, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.HouseNumber))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.HouseNumber.IsLike(this.CurrentClientDetailViewModel.HouseNumber, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.PostalCode))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.PostalCode.IsLike(this.CurrentClientDetailViewModel.PostalCode, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.City))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.City.IsLike(this.CurrentClientDetailViewModel.City, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.DateOfBirth))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.DateOfBirth.IsLike(this.CurrentClientDetailViewModel.DateOfBirth, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.PhoneNumber1))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.PhoneNumber1.IsLike(this.CurrentClientDetailViewModel.PhoneNumber1, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.PhoneNumber2))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.PhoneNumber2.IsLike(this.CurrentClientDetailViewModel.PhoneNumber2, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.MobileNumber))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.MobileNumber.IsLike(this.CurrentClientDetailViewModel.MobileNumber, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Telefax))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Telefax.IsLike(this.CurrentClientDetailViewModel.Telefax, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrEmpty(this.CurrentClientDetailViewModel.Email))
            {
                conjunction.Add(Restrictions.Where<Client>(c => c.Email.IsLike(this.CurrentClientDetailViewModel.Email, MatchMode.Anywhere)));
            }

            return conjunction;
        }

        private void InitPropertyInfos()
        {
            this.PropertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => prop.DeclaringType == this.GetType() &&
                                                                                                                         prop.CanRead && prop.GetMethod.IsPublic &&
                                                                                                                         prop.PropertyType != typeof(RelayCommand)).ToArray();
        }

        private bool CanCreateNewBill()
        {
            if (this.CurrentClientState == this.GetClientLoadedState())
            {
                return true;
            }

            return false;
        }

        private void SendCreateNewBillMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.CurrentClientDetailViewModel.ClientId, Resources.Messenger_Message_CreateNewBillMessageForBillEditVM));
            Messenger.Default.Send(new NotificationMessage(Resources.Messenger_Message_CreateNewBillMessageForMainVM));
        }



        #region ClientState Methods

        private void InitClientStateList()
        {
            this.ClientStates = new List<IClientState>
                                {
                                    new ClientEmptyState(this),
                                    new ClientSearchState(this),
                                    new ClientCreationState(this),
                                    new ClientLoadedState(this),
                                    new ClientEditState(this)
                                };
        }

        public IClientState GetClientEmptyState()
        {
            return this.ClientStates.Find(x => x.GetType() == typeof(ClientEmptyState));
        }

        public IClientState GetClientSearchState()
        {
            return this.ClientStates.Find(x => x.GetType() == typeof(ClientSearchState));
        }

        public IClientState GetClientCreationState()
        {
            return this.ClientStates.Find(x => x.GetType() == typeof(ClientCreationState));
        }

        public IClientState GetClientLoadedState()
        {
            return this.ClientStates.Find(x => x.GetType() == typeof(ClientLoadedState));
        }

        public IClientState GetClientEditState()
        {
            return this.ClientStates.Find(x => x.GetType() == typeof(ClientEditState));
        }

        #endregion



        #region Command Methods

        private void InitClientCommands()
        {
            this.ClientCommands = new List<ImageCommandViewModel>
                                  {
                                      new ImageCommandViewModel(Resources.img_client_search,
                                                                Resources.Command_DisplayName_Search,
                                                                Resources.Command_Message_Client_Search,
                                                                new RelayCommand(this.SwitchToSearchMode, this.CanSwitchToSearchMode)),
                                      new ImageCommandViewModel(Resources.img_client_add,
                                                                Resources.Command_DisplayName_Add,
                                                                Resources.Command_Message_Client_Add,
                                                                new RelayCommand(this.SwitchToAddMode, this.CanSwitchToAddMode)),
                                      new ImageCommandViewModel(Resources.img_client_edit,
                                                                Resources.Command_DisplayName_Edit,
                                                                Resources.Command_Message_Client_Edit,
                                                                new RelayCommand(this.SwitchToEditMode, this.CanSwitchToEditMode)),
                                      new ImageCommandViewModel(Resources.img_client_saveOrUpdate,
                                                                Resources.Command_DisplayName_SaveOrUpdate,
                                                                Resources.Command_Message_Client_SaveOrUpdate,
                                                                new RelayCommand(this.Commit, this.CanCommit)),
                                      new ImageCommandViewModel(Resources.img_client_cancel,
                                                                Resources.Command_DisplayName_Cancel,
                                                                Resources.Command_Message_Client_Cancel,
                                                                new RelayCommand(this.Cancel, this.CanCancel)),
                                      new ImageCommandViewModel(Resources.img_delete,
                                                                Resources.Command_DisplayName_Delete,
                                                                Resources.Command_Message_Client_Delete,
                                                                new RelayCommand(this.Delete, this.CanDelete))
                                  };
        }

        private bool CanSwitchToSearchMode()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanSwitchToSearchMode())
            {
                return true;
            }

            return false;
        }

        private void SwitchToSearchMode()
        {
            this.CurrentClientState.SwitchToSearchMode();
        }

        private bool CanSwitchToAddMode()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanSwitchToAddMode())
            {
                return true;
            }

            return false;
        }

        private void SwitchToAddMode()
        {
            this.CurrentClientState.SwitchToAddMode();
        }

        private bool CanSwitchToEditMode()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanSwitchToEditMode())
            {
                return true;
            }

            return false;
        }

        private void SwitchToEditMode()
        {
            this.CurrentClientState.SwitchToEditMode();
        }

        private bool CanCommit()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanCommit())
            {
                return true;
            }

            return false;
        }

        private void Commit()
        {
            this.CurrentClientState.Commit();
        }

        private bool CanCancel()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanCancel())
            {
                return true;
            }

            return false;
        }

        private void Cancel()
        {
            this.CurrentClientState.Cancel();
        }

        private bool CanDelete()
        {
            if (this.repository.IsConnected && this.CurrentClientState.CanDelete())
            {
                return true;
            }

            return false;
        }

        private void Delete()
        {
            this.CurrentClientState.Delete();
        }

        #endregion
    }
}