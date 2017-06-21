// ///////////////////////////////////
// File: BillEditState.cs
// Last Change: 23.04.2017  20:17
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using EpAccounting.UI.ViewModel;



    public class BillEditState : IBillState
    {
        #region Fields

        private readonly BillEditViewModel billEditViewModel;

        #endregion



        #region Constructors / Destructor

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
                this.billEditViewModel.Load(null, this.billEditViewModel.GetBillLoadedState());
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