// ///////////////////////////////////
// File: ClientLoadedState.cs
// Last Change: 21.04.2017  21:16
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using EpAccounting.Model;
    using EpAccounting.UI.ViewModel;



    public class ClientLoadedState : IClientState
    {
        #region Fields

        private readonly ClientEditViewModel clientEditViewModel;

        #endregion



        #region Constructors / Destructor

        public ClientLoadedState(ClientEditViewModel clientEditViewModel)
        {
            this.clientEditViewModel = clientEditViewModel;
        }

        #endregion



        #region IClientState Members

        public bool CanSwitchToSearchMode()
        {
            return true;
        }

        public void SwitchToSearchMode()
        {
            this.clientEditViewModel.Load(new Client(), this.clientEditViewModel.GetClientSearchState());
        }

        public bool CanSwitchToAddMode()
        {
            return true;
        }

        public void SwitchToAddMode()
        {
            this.clientEditViewModel.Load(new Client(), this.clientEditViewModel.GetClientCreationState());
        }

        public bool CanSwitchToEditMode()
        {
            return true;
        }

        public void SwitchToEditMode()
        {
            this.clientEditViewModel.Load(null, this.clientEditViewModel.GetClientEditState());
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
            if (await this.clientEditViewModel.DeleteClientAsync())
            {
                this.clientEditViewModel.Load(new Client(), this.clientEditViewModel.GetClientEmptyState());
            }
        }

        #endregion
    }
}