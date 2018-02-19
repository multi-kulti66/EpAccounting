// ///////////////////////////////////
// File: MainViewModel.cs
// Last Change: 19.02.2018, 20:06
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using Business;
    using GalaSoft.MvvmLight.Messaging;
    using Properties;
    using Service;


    public class MainViewModel : BindableViewModelBase, IDisposable
    {
        #region Fields

        private readonly IRepository _repository;
        private readonly IDialogService _dialogService;

        private bool _canChangeWorkspace = true;

        private WorkspaceViewModel _currentWorkspace;
        private ClientViewModel _clientWorkspace;
        private BillViewModel _billWorkspace;
        private ArticlesOptionViewModel _articleWorkspace;
        private OptionViewModel _optionWorkspace;

        #endregion



        #region Constructors

        public MainViewModel(IRepository repository, IDialogService dialogService)
        {
            this._repository = repository;
            this._dialogService = dialogService;

            this.TryConnectingAtStartup();
            this.InitWorkspaces();

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<bool>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties, Indexers

        public bool IsConnected
        {
            get { return this._repository.IsConnected; }
        }

        public bool CanChangeWorkspace
        {
            get { return this._canChangeWorkspace; }
            private set { this.SetProperty(ref this._canChangeWorkspace, value); }
        }

        public ObservableCollection<WorkspaceViewModel> WorkspaceViewModels { get; private set; }

        public WorkspaceViewModel CurrentWorkspace
        {
            get { return this._currentWorkspace; }
            set { this.SetProperty(ref this._currentWorkspace, value); }
        }

        private ClientViewModel ClientWorkspace
        {
            get
            {
                if (this._clientWorkspace == null)
                {
                    this._clientWorkspace = new ClientViewModel(Resources.Workspace_Title_Clients, Resources.img_clients,
                                                                this._repository, this._dialogService);
                }

                return this._clientWorkspace;
            }
        }

        private BillViewModel BillWorkspace
        {
            get
            {
                if (this._billWorkspace == null)
                {
                    this._billWorkspace = new BillViewModel(Resources.Workspace_Title_Bills, Resources.img_bills,
                                                            this._repository, this._dialogService);
                }

                return this._billWorkspace;
            }
        }

        private ArticlesOptionViewModel ArticleWorkspace
        {
            get
            {
                if (this._articleWorkspace == null)
                {
                    this._articleWorkspace = new ArticlesOptionViewModel(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                         this._repository, this._dialogService);
                }

                return this._articleWorkspace;
            }
        }

        private OptionViewModel OptionWorkspace
        {
            get
            {
                if (this._optionWorkspace == null)
                {
                    this._optionWorkspace = new OptionViewModel(Resources.Workspace_Title_Options, Resources.img_options,
                                                                this._repository, this._dialogService);
                }

                return this._optionWorkspace;
            }
        }

        #endregion



        #region IDisposable Members

        public void Dispose()
        {
            this._billWorkspace?.Dispose();
            Messenger.Default.Unregister(this);
        }

        #endregion



        private void TryConnectingAtStartup()
        {
            try
            {
                this._repository.LoadDatabase(Settings.Default.DatabaseFilePath);
            }
            catch
            {
                // just try to connect at beginning
                // do not throw exception when no database path was saved
                Settings.Default.DatabaseFilePath = string.Empty;
                Settings.Default.Save();
            }
        }

        private void UpdateConnectionState()
        {
            this.RaisePropertyChanged(() => this.IsConnected);
        }

        private void ChangeToBillWorkspace()
        {
            this.CurrentWorkspace = this.BillWorkspace;
        }

        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Message_UpdateConnectionStateForMainVM)
            {
                this.UpdateConnectionState();
            }
            else if (message.Notification == Resources.Message_ChangeToBillWorkspaceForMainVM)
            {
                this.ChangeToBillWorkspace();
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<bool> message)
        {
            if (message.Notification == Resources.Message_WorkspaceEnableStateForMainVM)
            {
                this.CanChangeWorkspace = message.Content;
            }
        }

        private void InitWorkspaces()
        {
            this.WorkspaceViewModels = new ObservableCollection<WorkspaceViewModel>
                                       {
                                           this.ClientWorkspace,
                                           this.BillWorkspace,
                                           this.ArticleWorkspace,
                                           this.OptionWorkspace
                                       };

            this.CurrentWorkspace = this.ClientWorkspace;
        }
    }
}