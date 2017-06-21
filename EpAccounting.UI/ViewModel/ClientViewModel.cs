// ///////////////////////////////////
// File: ClientViewModel.cs
// Last Change: 19.04.2017  19:39
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Drawing;
    using EpAccounting.Business;
    using EpAccounting.UI.Service;



    public class ClientViewModel : WorkspaceViewModel
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private ClientEditViewModel _clientEditViewModel;
        private ClientSearchViewModel _clientSearchViewModel;

        #endregion



        #region Constructors / Destructor

        public ClientViewModel(string title, Bitmap image, IRepository repository, IDialogService dialogService) : base(title, image)
        {
            this.repository = repository;
            this.dialogService = dialogService;
        }

        #endregion



        #region Properties

        public ClientEditViewModel ClientEditViewModel
        {
            get
            {
                if (this._clientEditViewModel == null)
                {
                    this._clientEditViewModel = new ClientEditViewModel(this.repository, this.dialogService);
                }

                return this._clientEditViewModel;
            }
        }

        public ClientSearchViewModel ClientSearchViewModel
        {
            get
            {
                if (this._clientSearchViewModel == null)
                {
                    this._clientSearchViewModel = new ClientSearchViewModel(this.repository);
                }
                return this._clientSearchViewModel;
            }
        }

        #endregion
    }
}