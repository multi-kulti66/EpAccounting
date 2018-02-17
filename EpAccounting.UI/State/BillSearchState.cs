// ///////////////////////////////////
// File: BillSearchState.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using GalaSoft.MvvmLight.Messaging;
    using Properties;
    using ViewModel;


    public class BillSearchState : IBillState
    {
        #region Fields

        private readonly BillEditViewModel billEditViewModel;

        #endregion



        #region Constructors

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
            this.billEditViewModel.ChangeToEmptyMode();
            Messenger.Default.Send(new NotificationMessage(Resources.Message_LoadBillSearchViewModelMessageForBillVM));
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