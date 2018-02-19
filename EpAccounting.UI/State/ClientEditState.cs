// ///////////////////////////////////
// File: ClientEditState.cs
// Last Change: 19.02.2018, 19:15
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using ViewModel;


    public class ClientEditState : IClientState
    {
        #region Fields

        private readonly ClientEditViewModel _clientEditViewModel;

        #endregion



        #region Constructors

        public ClientEditState(ClientEditViewModel clientEditViewModel)
        {
            this._clientEditViewModel = clientEditViewModel;
        }

        #endregion



        #region IClientState Members

        public bool CanSwitchToSearchMode()
        {
            return false;
        }

        public void SwitchToSearchMode()
        {
            // do nothing
        }

        public bool CanSwitchToAddMode()
        {
            return false;
        }

        public void SwitchToAddMode()
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
            if (await this._clientEditViewModel.SaveOrUpdateClientAsync())
            {
                this._clientEditViewModel.ChangeToLoadedMode();
                this._clientEditViewModel.SendUpdateClientValuesMessage();
            }
        }

        public bool CanCancel()
        {
            return true;
        }

        public void Cancel()
        {
            this._clientEditViewModel.Load();
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