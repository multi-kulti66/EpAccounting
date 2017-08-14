// ///////////////////////////////////
// File: ClientSearchViewModel.cs
// Last Change: 28.07.2017  20:19
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using NHibernate.Criterion;



    public class ClientSearchViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly IRepository repository;

        private int _numberOfAllPages;
        private int _currentPage;
        private ICriterion _lastCriterion;
        private ObservableCollection<ClientDetailViewModel> _foundClients;
        private ClientDetailViewModel _selectedClientDetailViewModel;

        private bool _isClientLoadingEnabled = true;

        private RelayCommand _loadSelectedClientCommand;
        private ImageCommandViewModel _loadNextPageCommand;
        private ImageCommandViewModel _loadPreviousPageCommand;

        #endregion



        #region Constructors / Destructor

        public ClientSearchViewModel(IRepository repository)
        {
            this.repository = repository;

            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<bool>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public bool IsClientLoadingEnabled
        {
            get { return this._isClientLoadingEnabled; }
            private set { this.SetProperty(ref this._isClientLoadingEnabled, value); }
        }

        public int NumberOfAllPages
        {
            get { return this._numberOfAllPages; }
            private set { this.SetProperty(ref this._numberOfAllPages, value); }
        }

        public int CurrentPage
        {
            get { return this._currentPage; }
            private set { this.SetProperty(ref this._currentPage, value); }
        }

        public ObservableCollection<ClientDetailViewModel> FoundClients
        {
            get
            {
                if (this._foundClients == null)
                {
                    this._foundClients = new ObservableCollection<ClientDetailViewModel>();
                }

                return this._foundClients;
            }
        }

        public ClientDetailViewModel SelectedClientDetailViewModel
        {
            get { return this._selectedClientDetailViewModel; }
            set { this.SetProperty(ref this._selectedClientDetailViewModel, value); }
        }

        public RelayCommand LoadSelectedClientCommand
        {
            get
            {
                if (this._loadSelectedClientCommand == null)
                {
                    this._loadSelectedClientCommand = new RelayCommand(this.SendLoadSelectedClientMessage, this.CanLoadSelectedClient);
                }

                return this._loadSelectedClientCommand;
            }
        }

        public ImageCommandViewModel LoadNextPageCommand
        {
            get
            {
                if (this._loadNextPageCommand == null)
                {
                    this._loadNextPageCommand = new ImageCommandViewModel(Resources.img_arrow_right,
                                                                          Resources.Command_DisplayName_Next_Page,
                                                                          Resources.Command_Message_Next_Page,
                                                                          new RelayCommand(this.LoadNextPage, this.CanLoadNextPage));
                }

                return this._loadNextPageCommand;
            }
        }

        public ImageCommandViewModel LoadPreviousPageCommand
        {
            get
            {
                if (this._loadPreviousPageCommand == null)
                {
                    this._loadPreviousPageCommand = new ImageCommandViewModel(Resources.img_arrow_left,
                                                                              Resources.Command_DisplayName_Previous_Page,
                                                                              Resources.Command_Message_Previous_Page,
                                                                              new RelayCommand(this.LoadPreviousPage, this.CanLoadPreviousPage));
                }

                return this._loadPreviousPageCommand;
            }
        }

        private ICriterion LastCriterion
        {
            get { return this._lastCriterion; }
            set { this._lastCriterion = value; }
        }

        #endregion



        private bool CanLoadSelectedClient()
        {
            if (this.SelectedClientDetailViewModel == null)
            {
                return false;
            }

            return true;
        }

        private void SendLoadSelectedClientMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.SelectedClientDetailViewModel.ClientId, Resources.Messenger_Message_LoadSelectedClientMessageForClientEditVM));
        }

        private bool CanLoadNextPage()
        {
            if (this.CurrentPage < this.NumberOfAllPages)
            {
                return true;
            }

            return false;
        }

        private void LoadNextPage()
        {
            this.CurrentPage++;
            this.LoadSearchedClients(this.LastCriterion, this.CurrentPage);
        }

        private bool CanLoadPreviousPage()
        {
            if (this.CurrentPage > 1)
            {
                return true;
            }

            return false;
        }

        private void LoadPreviousPage()
        {
            this.CurrentPage--;
            this.LoadSearchedClients(this.LastCriterion, this.CurrentPage);
        }

        private void ExecuteNotificationMessage(NotificationMessage<ICriterion> message)
        {
            if (message.Notification == Resources.Messenger_Message_ClientSearchCriteriaForClientSearchVM)
            {
                this.LastCriterion = message.Content;
                this.LoadSearchedClients(message.Content, 1);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Messenger_Message_UpdateClientValuesMessageForClientSearchVM)
            {
                this.UpdateClient(message.Content);
            }
            else if (message.Notification == Resources.Messenger_Message_RemoveClientMessageForClientSearchVM)
            {
                this.RemoveClient(message.Content);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<bool> message)
        {
            if (message.Notification == Resources.Messenger_Message_EnableStateMessageForClientSearchVM)
            {
                this.IsClientLoadingEnabled = message.Content;
            }
        }

        private void LoadSearchedClients(ICriterion criterion, int page)
        {
            // ReSharper disable once PossibleLossOfFraction
            int numberOfClients = this.repository.GetQuantityByCriteria<Client>(criterion);
            this.NumberOfAllPages = (numberOfClients - 1) / Settings.Default.PageSize + 1;
            this.CurrentPage = this.CurrentPage > this.NumberOfAllPages ? this.NumberOfAllPages : page;
            this.ReloadCommands();

            this.FoundClients.Clear();

            foreach (Client client in this.repository.GetByCriteria<Client>(criterion, this.CurrentPage).ToList())
            {
                this.FoundClients.Add(new ClientDetailViewModel(client));
            }
        }

        private void UpdateClient(int id)
        {
            for (int i = 0; i < this.FoundClients.Count; i++)
            {
                if (this.FoundClients[i].ClientId == id)
                {
                    Client client = this.repository.GetById<Client>(id);
                    this.FoundClients[i] = new ClientDetailViewModel(client);
                }
            }
        }

        private void RemoveClient(int id)
        {
            for (int i = 0; i < this.FoundClients.Count; i++)
            {
                if (this.FoundClients[i].ClientId == id)
                {
                    this.FoundClients.RemoveAt(i);
                }
            }

            this.LoadSearchedClients(this.LastCriterion, this.CurrentPage);
        }

        private void ReloadCommands()
        {
            this.LoadPreviousPageCommand.RelayCommand.RaiseCanExecuteChanged();
            this.LoadNextPageCommand.RelayCommand.RaiseCanExecuteChanged();
        }
    }
}