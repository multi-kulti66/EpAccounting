// ///////////////////////////////////
// File: ClientSearchViewModel.cs
// Last Change: 17.02.2018, 20:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Business;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Model;
    using NHibernate.Criterion;
    using Properties;


    public class ClientSearchViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly IRepository repository;

        private int _numberOfAllPages;
        private int _currentPage;
        private ObservableCollection<ClientDetailViewModel> _foundClients;
        private ClientDetailViewModel _selectedClientDetailViewModel;

        private bool _isClientLoadingEnabled = true;

        private RelayCommand _loadSelectedClientCommand;
        private ImageCommandViewModel _loadFirstPageCommand;
        private ImageCommandViewModel _loadLastPageCommand;
        private ImageCommandViewModel _loadNextPageCommand;
        private ImageCommandViewModel _loadPreviousPageCommand;

        #endregion



        #region Constructors

        public ClientSearchViewModel(IRepository repository)
        {
            this.repository = repository;

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<bool>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties, Indexers

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

        public ImageCommandViewModel LoadFirstPageCommand
        {
            get
            {
                if (this._loadFirstPageCommand == null)
                {
                    this._loadFirstPageCommand = new ImageCommandViewModel(Resources.img_arrow_first,
                                                                          Resources.Command_DisplayName_First_Page,
                                                                          new RelayCommand(this.LoadFirstPage, this.CanLoadFirstPage));
                }

                return this._loadFirstPageCommand;
            }
        }

        public ImageCommandViewModel LoadLastPageCommand
        {
            get
            {
                if (this._loadLastPageCommand == null)
                {
                    this._loadLastPageCommand = new ImageCommandViewModel(Resources.img_arrow_last,
                                                                          Resources.Command_DisplayName_Last_Page,
                                                                          new RelayCommand(this.LoadLastPage, this.CanLoadLastPage));
                }

                return this._loadLastPageCommand;
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
                                                                              new RelayCommand(this.LoadPreviousPage, this.CanLoadPreviousPage));
                }

                return this._loadPreviousPageCommand;
            }
        }

        private ICriterion LastCriterion { get; set; }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Message_ReloadClientsForClientSearchVM)
            {
                this.ReloadClients();
            }
        }

        private void LoadSearchedClients(ICriterion criterion, int page = 1)
        {
            // ReSharper disable once PossibleLossOfFraction
            int numberOfClients = this.repository.GetQuantityByCriteria<Client>(criterion);
            this.NumberOfAllPages = (numberOfClients - 1) / Settings.Default.PageSize + 1;
            this.CurrentPage = this.CurrentPage > this.NumberOfAllPages ? this.NumberOfAllPages : page;
            this.ReloadCommands();

            this.FoundClients.Clear();

            foreach (Client client in this.repository.GetByCriteria<Client>(criterion, this.CurrentPage).ToList())
            {
                this.FoundClients.Add(new ClientDetailViewModel(client, this.repository));
            }
        }

        private void UpdateClient(int id)
        {
            Client client = this.repository.GetById<Client>(id);

            for (int i = 0; i < this.FoundClients.Count; i++)
            {
                if (this.FoundClients[i].Id == id)
                {
                    this.FoundClients[i] = new ClientDetailViewModel(client, this.repository);
                }
                else if (this.FoundClients[i].PostalCode == client.CityToPostalCode.PostalCode &&
                         this.FoundClients[i].City != client.CityToPostalCode.City)
                {
                    this.FoundClients[i] = new ClientDetailViewModel(this.repository.GetById<Client>(this.FoundClients[i].Id), this.repository);
                }
            }
        }

        private void ReloadClients()
        {
            this.LoadSearchedClients(this.LastCriterion, this.CurrentPage);
        }

        private void ReloadCommands()
        {
            this.LoadFirstPageCommand.RelayCommand.RaiseCanExecuteChanged();
            this.LoadPreviousPageCommand.RelayCommand.RaiseCanExecuteChanged();
            this.LoadNextPageCommand.RelayCommand.RaiseCanExecuteChanged();
            this.LoadLastPageCommand.RelayCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteNotificationMessage(NotificationMessage<ICriterion> message)
        {
            if (message.Notification == Resources.Message_ClientSearchCriteriaForClientSearchVM)
            {
                this.LastCriterion = message.Content;
                this.LoadSearchedClients(message.Content);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Message_UpdateClientValuesForClientSearchVM)
            {
                this.UpdateClient(message.Content);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<bool> message)
        {
            if (message.Notification == Resources.Message_EnableStateForClientSearchVM)
            {
                this.IsClientLoadingEnabled = message.Content;
            }
        }

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
            Messenger.Default.Send(new NotificationMessage<int>(this.SelectedClientDetailViewModel.Id, Resources.Message_LoadClientForClientEditVM));
        }

        private bool CanLoadFirstPage()
        {
            return this.CurrentPage > 1;
        }

        private void LoadFirstPage()
        {
            this.CurrentPage = 1;
            this.LoadSearchedClients(this.LastCriterion, this.CurrentPage);
        }

        private bool CanLoadLastPage()
        {
            return this.CurrentPage < this.NumberOfAllPages;
        }

        private void LoadLastPage()
        {
            this.CurrentPage = this.NumberOfAllPages;
            this.LoadSearchedClients(this.LastCriterion, this.CurrentPage);
        }

        private bool CanLoadNextPage()
        {
            return this.CurrentPage < this.NumberOfAllPages;
        }

        private void LoadNextPage()
        {
            this.CurrentPage++;
            this.LoadSearchedClients(this.LastCriterion, this.CurrentPage);
        }

        private bool CanLoadPreviousPage()
        {
            return this.CurrentPage > 1;
        }

        private void LoadPreviousPage()
        {
            this.CurrentPage--;
            this.LoadSearchedClients(this.LastCriterion, this.CurrentPage);
        }
    }
}