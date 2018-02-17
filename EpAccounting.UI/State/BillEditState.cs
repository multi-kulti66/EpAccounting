// ///////////////////////////////////
// File: BillEditState.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using ViewModel;


    public class BillEditState : IBillState
    {
        #region Fields

        private readonly BillEditViewModel billEditViewModel;

        #endregion



        #region Constructors

        public BillEditState(BillEditViewModel billEditViewModel)
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

        public async Task Commit()
        {
            if (await this.billEditViewModel.SaveOrUpdateBillAsync())
            {
                this.billEditViewModel.ChangeToLoadedMode();
                this.billEditViewModel.SendUpdateBillValuesMessage();
            }
        }

        public bool CanCancel()
        {
            return true;
        }

        public void Cancel()
        {
            this.billEditViewModel.Reload();
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