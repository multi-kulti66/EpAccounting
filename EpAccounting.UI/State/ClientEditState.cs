// ///////////////////////////////////
// File: ClientEditState.cs
// Last Change: 21.04.2017  21:10
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using EpAccounting.UI.ViewModel;



    public class ClientEditState : IClientState
    {
        #region Fields

        private readonly ClientEditViewModel clientEditViewModel;

        #endregion



        #region Constructors / Destructor

        public ClientEditState(ClientEditViewModel clientEditViewModel)
        {
            this.clientEditViewModel = clientEditViewModel;
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
            if (await this.clientEditViewModel.SaveOrUpdateClientAsync())
            {
                this.clientEditViewModel.Load(null, this.clientEditViewModel.GetClientLoadedState());
                this.clientEditViewModel.SendUpdateClientValuesMessage();
            }
        }

        public bool CanCancel()
        {
            return true;
        }

        public void Cancel()
        {
            this.clientEditViewModel.Reload();
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