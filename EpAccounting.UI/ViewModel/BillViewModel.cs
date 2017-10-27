// ///////////////////////////////////
// File: BillViewModel.cs
// Last Change: 22.10.2017  16:05
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

        private readonly BillEditViewModel _billEditViewModel;
        private readonly BillItemEditViewModel _billItemEditViewModel;
        private readonly BillSearchViewModel _billSearchViewModel;

        private BillWorkspaceViewModel _billWorkspaceViewModel;

        #endregion



        #region Constructors / Destructor

        public BillViewModel(string title, Bitmap image, IRepository repository, IDialogService dialogService) : base(title, image)
        {
            this._billEditViewModel = new BillEditViewModel(repository, dialogService);
            this._billItemEditViewModel = new BillItemEditViewModel(repository, dialogService, new WordService());
            this._billSearchViewModel = new BillSearchViewModel(repository);
            this._billWorkspaceViewModel = this.BillSearchViewModel;

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<Bill>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public BillEditViewModel BillEditViewModel
        {
            get { return this._billEditViewModel; }
        }

        public BillWorkspaceViewModel BillWorkspaceViewModel
        {
            get { return this._billWorkspaceViewModel; }
            set { this.SetProperty(ref this._billWorkspaceViewModel, value); }
        }

        private BillItemEditViewModel BillItemEditViewModel
        {
            get { return this._billItemEditViewModel; }
        }

        private BillSearchViewModel BillSearchViewModel
        {
            get { return this._billSearchViewModel; }
        }

        #endregion



        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Message_LoadBillSearchViewModelMessageForBillVM)
            {
                this.BillWorkspaceViewModel = this.BillSearchViewModel;
            }
            else if (message.Notification == Resources.Message_ResetBillItemEditVMAndChangeToSearchWorkspaceForBillVM)
            {
                this.BillItemEditViewModel.Clear();
                this.BillWorkspaceViewModel = this.BillSearchViewModel;
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<Bill> message)
        {
            if (message.Notification == Resources.Message_LoadBillItemEditViewModelForBillVM)
            {
                this.BillWorkspaceViewModel = this.BillItemEditViewModel;
                this.BillItemEditViewModel.LoadBill(message.Content);
            }
        }
    }
}