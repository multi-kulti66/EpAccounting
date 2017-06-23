// ///////////////////////////////////
// File: BillViewModel.cs
// Last Change: 09.05.2017  19:27
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Drawing;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using GalaSoft.MvvmLight.Messaging;



    public class BillViewModel : WorkspaceViewModel
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private BillEditViewModel _billEditViewModel;

        private BillWorkspaceViewModel _billWorkspaceViewModel;
        private BillItemEditViewModel _billItemEditViewModel;
        private BillSearchViewModel _billSearchViewModel;

        #endregion



        #region Constructors / Destructor

        public BillViewModel(string title, Bitmap image, IRepository repository, IDialogService dialogService) : base(title, image)
        {
            this.repository = repository;
            this.dialogService = dialogService;

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<int>>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<Bill>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public BillEditViewModel BillEditViewModel
        {
            get
            {
                if (this._billEditViewModel == null)
                {
                    this._billEditViewModel = new BillEditViewModel(this.repository, this.dialogService);
                }

                return this._billEditViewModel;
            }
        }

        public BillWorkspaceViewModel BillWorkspaceViewModel
        {
            get
            {
                if (this._billWorkspaceViewModel == null)
                {
                    this._billWorkspaceViewModel = this.BillSearchViewModel;
                }

                return this._billWorkspaceViewModel;
            }
            set { this.SetProperty(ref this._billWorkspaceViewModel, value); }
        }

        private BillItemEditViewModel BillItemEditViewModel
        {
            get
            {
                if (this._billItemEditViewModel == null)
                {
                    this._billItemEditViewModel = new BillItemEditViewModel();
                }

                return this._billItemEditViewModel;
            }
        }

        private BillSearchViewModel BillSearchViewModel
        {
            get
            {
                if (this._billSearchViewModel == null)
                {
                    this._billSearchViewModel = new BillSearchViewModel(this.repository);
                }

                return this._billSearchViewModel;
            }
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Messenger_Message_LoadBillSearchViewModel)
            {
                this.BillWorkspaceViewModel = this.BillSearchViewModel;
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<int> message)
        {
            if (message.Notification == Resources.Messenger_Message_RemoveBill)
            {
                this.BillItemEditViewModel.Clear();
                this.BillWorkspaceViewModel = this.BillSearchViewModel;
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<Bill> message)
        {
            if (message.Notification == Resources.Messenger_Message_LoadBillItemEditViewModel)
            {
                this.BillWorkspaceViewModel = this.BillItemEditViewModel;
                this.BillItemEditViewModel.LoadBill(message.Content);
            }
        }
    }
}