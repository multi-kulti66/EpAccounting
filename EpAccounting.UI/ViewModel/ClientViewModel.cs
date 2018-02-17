// ///////////////////////////////////
// File: ClientViewModel.cs
// Last Change: 17.02.2018, 14:29
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System.Drawing;
    using Business;
    using Service;


    public class ClientViewModel : WorkspaceViewModel
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private ClientEditViewModel _clientEditViewModel;
        private ClientSearchViewModel _clientSearchViewModel;

        #endregion



        #region Constructors

        public ClientViewModel(string title, Bitmap image, IRepository repository, IDialogService dialogService) : base(title, image)
        {
            this.repository = repository;
            this.dialogService = dialogService;

            this.InitClientViewModels();
        }

        #endregion



        #region Properties, Indexers

        public ClientEditViewModel ClientEditViewModel
        {
            get { return this._clientEditViewModel; }
        }

        public ClientSearchViewModel ClientSearchViewModel
        {
            get { return this._clientSearchViewModel; }
        }

        #endregion



        private void InitClientViewModels()
        {
            this._clientEditViewModel = new ClientEditViewModel(this.repository, this.dialogService);
            this._clientSearchViewModel = new ClientSearchViewModel(this.repository);
        }
    }
}