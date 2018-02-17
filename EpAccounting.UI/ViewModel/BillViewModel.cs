// ///////////////////////////////////
// File: BillViewModel.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System.Drawing;
    using Business;
    using GalaSoft.MvvmLight.Messaging;
    using Model;
    using Properties;
    using Service;


    public class BillViewModel : WorkspaceViewModel
    {
        #region Fields

        private readonly BillEditViewModel _billEditViewModel;
        private readonly BillItemEditViewModel _billItemEditViewModel;
        private readonly BillSearchViewModel _billSearchViewModel;

        private BillWorkspaceViewModel _billWorkspaceViewModel;

        #endregion



        #region Constructors

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



        #region Properties, Indexers

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