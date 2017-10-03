// ///////////////////////////////////
// File: ImageCommandViewModel.cs
// Last Change: 16.09.2017  11:33
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

        public ImageCommandViewModel(Bitmap image, string displayName, RelayCommand relayCommand)
            : base(displayName, relayCommand)
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