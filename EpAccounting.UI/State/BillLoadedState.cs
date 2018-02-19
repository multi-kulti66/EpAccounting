// ///////////////////////////////////
// File: BillLoadedState.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using GalaSoft.MvvmLight.Messaging;
    using Properties;
    using ViewModel;


    public class BillLoadedState : IBillState
    {
        #region Fields

        private readonly BillEditViewModel _billEditViewModel;

        #endregion



        #region Constructors

        public BillLoadedState(BillEditViewModel billEditViewModel)
        {
            this._billEditViewModel = billEditViewModel;
        }

        #endregion



        #region IBillState Members

        public bool CanSwitchToSearchMode()
        {
            return true;
        }

        public void SwitchToSearchMode()
        {
            this._billEditViewModel.ChangeToSearchMode();
            Messenger.Default.Send(new NotificationMessage(Resources.Message_LoadBillSearchViewModelMessageForBillVM));
        }

        public bool CanSwitchToEditMode()
        {
            return true;
        }

        public void SwitchToEditMode()
        {
            this._billEditViewModel.ChangeToEditMode();
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
            if (await this._billEditViewModel.DeleteBillAsync())
            {
                this._billEditViewModel.ChangeToEmptyMode();
                Messenger.Default.Send(new NotificationMessage(Resources.Message_LoadBillItemEditViewModelForBillVM));
            }
        }

        #endregion
    }
}