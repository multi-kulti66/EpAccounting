// ///////////////////////////////////
// File: ImageCommandViewModel.cs
// Last Change: 14.03.2017  19:53
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Drawing;
    using GalaSoft.MvvmLight.Command;



    public class ImageCommandViewModel : CommandViewModel
    {
        #region Constructors / Destructor



        #region Constructors

        public ImageCommandViewModel(Bitmap image, string displayName, string commandMessage, RelayCommand relayCommand)
            : base(displayName, commandMessage, relayCommand)
        {
            this.Image = image;
        }

        #endregion



        #endregion



        #region Properties

        public Bitmap Image { get; set; }

        #endregion
    }
}