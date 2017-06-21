// ///////////////////////////////////
// File: ClientSearchViewModel.cs
// Last Change: 23.04.2017  19:44
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using GalaSoft.MvvmLight.Messaging;
    using NHibernate.Criterion;



    public class ClientSearchViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly IRepository repository;

        private ObservableCollection<ClientDetailViewModel> _foundClients;
        private ClientDetailViewModel _selectedClientDetailViewModel;

        private bool _isClientLoadingEnabled = true;

        #endregion



        #region Constructors / Destructor

        public ClientSearchViewModel(IRepository repository)
        {
            this.repository = repository;

            Messenger.Default.Register<NotificationMessage<ICriterion>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
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
            set
            {
                this.SetProperty(ref this._selectedClientDetailViewModel, value);

                if (this._selectedClientDetailViewModel != null)
                {
                    this.SendLoadSelectedClientMessage();
                }
            }
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage<ICriterion> message)
        {
            if (message.Notification == Resources.Messenger_Message_ClientSearchCriteria)
            {
                this.LoadSearchedClients(message.Content);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Messenger_Message_UpdateClientValues)
            {
                this.UpdateClient(message.Content);
            }
            else if (message.Notification == Resources.Messenger_Message_RemoveClient)
            {
                this.RemoveClient(message.Content);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<bool> message)
        {
            if (message.Notification == Resources.Messenger_Message_EnableStateForClientLoading)
            {
                this.IsClientLoadingEnabled = message.Content;
            }
        }

        private void LoadSearchedClients(ICriterion criterion)
        {
            this.FoundClients.Clear();

            foreach (Client client in this.repository.GetByCriteria<Client>(criterion).ToList())
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
        }

        private void SendLoadSelectedClientMessage()
        {
            Messenger.Default.Send(new NotificationMessage<int>(this.SelectedClientDetailViewModel.ClientId, Resources.Messenger_Message_LoadSelectedClient));
        }
    }
}