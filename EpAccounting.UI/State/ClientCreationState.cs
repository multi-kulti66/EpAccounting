﻿// ///////////////////////////////////
// File: ClientCreationState.cs
// Last Change: 21.04.2017  21:16
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.State
{
    using System.Threading.Tasks;
    using EpAccounting.Model;
    using EpAccounting.UI.ViewModel;



    public class ClientCreationState : IClientState
    {
        #region Fields

        private readonly ClientEditViewModel clientEditViewModel;

        #endregion



        #region Constructors / Destructor

        public ClientCreationState(ClientEditViewModel clientEditViewModel)
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
            }
        }

        public bool CanCancel()
        {
            return true;
        }

        public void Cancel()
        {
            this.clientEditViewModel.Load(new Client(), this.clientEditViewModel.GetClientEmptyState());
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