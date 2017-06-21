// ///////////////////////////////////
// File: BillLoadedState.cs
// Last Change: 09.05.2017  19:07
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using GalaSoft.MvvmLight.Messaging;



    public class BillLoadedState : IBillState
    {
        #region Fields

        private readonly BillEditViewModel billEditViewModel;

        #endregion



        #region Constructors / Destructor

        public BillLoadedState(BillEditViewModel billEditViewModel)
        {
            this.billEditViewModel = billEditViewModel;
        }

        #endregion



        #region IBillState Members

        public bool CanSwitchToSearchMode()
        {
            return true;
        }

        public void SwitchToSearchMode()
        {
            this.billEditViewModel.Load(new Bill() {Client = new Client()}, this.billEditViewModel.GetBillSearchState());
            Messenger.Default.Send(new NotificationMessage(Resources.Messenger_Message_LoadBillSearchViewModel));
        }

        public bool CanSwitchToEditMode()
        {
            return true;
        }

        public void SwitchToEditMode()
        {
            this.billEditViewModel.Load(null, this.billEditViewModel.GetBillEditState());
        }

        public bool CanCommit()
        {
            return false;
        }

        public Task Commit()
        {
            return Task.FromResult<object>(null);
        }

        public bool CanCancel()
        {
            return false;
        }

        public void Cancel()
        {
            // do nothing
        }

        public bool CanDelete()
        {
            return true;
        }

        public async Task Delete()
        {
            if (await this.billEditViewModel.DeleteBillAsync())
            {
                this.billEditViewModel.Load(new Bill() { Client = new Client()}, this.billEditViewModel.GetBillEmptyState());
                Messenger.Default.Send(new NotificationMessage(Resources.Messenger_Message_LoadBillItemEditViewModel));
            }
        }

        #endregion
    }
}