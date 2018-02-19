// ///////////////////////////////////
// File: ClientViewModel.cs
// Last Change: 19.02.2018, 20:05
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

        private readonly IRepository _repository;
        private readonly IDialogService _dialogService;

        #endregion



        #region Constructors

        public ClientViewModel(string title, Bitmap image, IRepository repository, IDialogService dialogService) : base(title, image)
        {
            this._repository = repository;
            this._dialogService = dialogService;

            this.InitClientViewModels();
        }

        #endregion



        #region Properties, Indexers

        public ClientEditViewModel ClientEditViewModel { get; private set; }

        public ClientSearchViewModel ClientSearchViewModel { get; private set; }

        #endregion



        private void InitClientViewModels()
        {
            this.ClientEditViewModel = new ClientEditViewModel(this._repository, this._dialogService);
            this.ClientSearchViewModel = new ClientSearchViewModel(this._repository, this._dialogService);
        }
    }
}