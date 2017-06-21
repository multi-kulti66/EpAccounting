// ///////////////////////////////////
// File: BillSearchState.cs
// Last Change: 09.05.2017  19:06
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using GalaSoft.MvvmLight.Messaging;



    public class BillSearchState : IBillState
    {
        #region Fields

        private readonly BillEditViewModel billEditViewModel;

        #endregion



        #region Constructors / Destructor

        public BillSearchState(BillEditViewModel billEditViewModel)
        {
            this.billEditViewModel = billEditViewModel;
        }

        #endregion



        #region IBillState Members

        public bool CanSwitchToSearchMode()
        {
            return false;
        }

        public void SwitchToSearchMode()
        {
            // do nothing
        }

        public bool CanSwitchToEditMode()
        {
            return false;
        }

        public void SwitchToEditMode()
        {
            // do nothing
        }

        public bool CanCommit()
        {
            return true;
        }

        public Task Commit()
        {
            this.billEditViewModel.SendBillSearchCriterionMessage();
            return Task.FromResult<object>(null);
        }

        public bool CanCancel()
        {
            return true;
        }

        public void Cancel()
        {
            this.billEditViewModel.Load(new Bill() {Client = new Client()}, this.billEditViewModel.GetBillEmptyState());
            Messenger.Default.Send(new NotificationMessage(Resources.Messenger_Message_LoadBillSearchViewModel));
        }

        public bool CanDelete()
        {
            return false;
        }

        public Task Delete()
        {
            return Task.FromResult<object>(null);
        }

        #endregion
    }
}