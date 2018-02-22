// ///////////////////////////////////
// File: ClientSearchViewModel.cs
// Last Change: 22.02.2018, 19:59
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Business;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Model;
    using NHibernate.Criterion;
    using Properties;
    using Service;


    public class ClientSearchViewModel : BindableViewModelBase, IDisposable
    {
        #region Fields

        private readonly IRepository _repository;
        private readonly IDialogService _dialogService;

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

        public ClientSearchViewModel(IRepository repository, IDialogService dialogService)
        {
            this._repository = repository;
            this._dialogService = dialogService;

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<Tuple<ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>>>(this, this.ExecuteNotificationMessage);
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

        private Tuple<ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> LastCriterion { get; set; }

        #endregion



        #region IDisposable Members

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Message_ReloadClientsForClientSearchVM)
            {
                this.ReloadClients();
            }
        }

        private void LoadSearchedClients(Tuple<ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion> tupleCriterion, int page = 1)
        {
            try
            {
                int numberOfClients;

                if (tupleCriterion.Item2 == null || tupleCriterion.Item3 == null)
                {
                    numberOfClients = this._repository.GetQuantityByCriteria<Client>(tupleCriterion.Item1);
                }
                else
                {
                    numberOfClients = this._repository.GetQuantityByCriteria(tupleCriterion.Item1, tupleCriterion.Item2, tupleCriterion.Item3);
                }


                this.NumberOfAllPages = (numberOfClients - 1) / Settings.Default.PageSize + 1;
                this.CurrentPage = this.CurrentPage > this.NumberOfAllPages ? this.NumberOfAllPages : page;
                this.ReloadCommands();

                this.FoundClients.Clear();

                if (tupleCriterion.Item2 == null || tupleCriterion.Item3 == null)
                {
                    foreach (Client client in this._repository.GetByCriteria<Client>(tupleCriterion.Item1, this.CurrentPage).ToList())
                    {
                        this.FoundClients.Add(new ClientDetailViewModel(client, this._repository));
                    }
                }
                else
                {
                    foreach (Client client in this._repository.GetByCriteria(tupleCriterion.Item1, tupleCriterion.Item2, tupleCriterion.Item3, this.CurrentPage).ToList())
                    {
                        this.FoundClients.Add(new ClientDetailViewModel(client, this._repository));
                    }
                }
            }
            catch (Exception e)
            {
                this._dialogService.ShowExceptionMessage(e, "Could not load required clients!");
            }
        }

        private void UpdateClient(int id)
        {
            try
            {
                Client client = this._repository.GetById<Client>(id);

                for (int i = 0; i < this.FoundClients.Count; i++)
                {
                    if (this.FoundClients[i].Id == id)
                    {
                        this.FoundClients[i] = new ClientDetailViewModel(client, this._repository);
                    }
                    else if (this.FoundClients[i].PostalCode == client.CityToPostalCode.PostalCode &&
                             this.FoundClients[i].City != client.CityToPostalCode.City)
                    {
                        this.FoundClients[i] = new ClientDetailViewModel(this._repository.GetById<Client>(this.FoundClients[i].Id), this._repository);
                    }
                }
            }
            catch (Exception e)
            {
                this._dialogService.ShowExceptionMessage(e, $"Could not update client with id = '{id}'");
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

        private void ExecuteNotificationMessage(NotificationMessage<Tuple<ICriterion, Expression<Func<Client, CityToPostalCode>>, ICriterion>> message)
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