// ///////////////////////////////////
// File: ImageCommandViewModel.cs
// Last Change: 22.10.2017  16:05
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