// ///////////////////////////////////
// File: ClientEmptyState.cs
// Last Change: 19.02.2018, 19:15
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using ViewModel;


    public class ClientEmptyState : IClientState
    {
        #region Fields

        private readonly ClientEditViewModel _clientEditViewModel;

        #endregion



        #region Constructors

        public ClientEmptyState(ClientEditViewModel clientEditViewModel)
        {
            this._clientEditViewModel = clientEditViewModel;
        }

        #endregion



        #region IClientState Members

        public bool CanSwitchToSearchMode()
        {
            return true;
        }

        public void SwitchToSearchMode()
        {
            this._clientEditViewModel.ChangeToSearchMode();
        }

        public bool CanSwitchToAddMode()
        {
            return true;
        }

        public void SwitchToAddMode()
        {
            this._clientEditViewModel.ChangeToCreationMode();
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
            return false;
        }

        public Task Delete()
        {
            return Task.FromResult<object>(null);
        }

        #endregion
    }
}