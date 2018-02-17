// ///////////////////////////////////
// File: ImageCommandViewModel.cs
// Last Change: 17.02.2018, 14:29
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System.Drawing;
    using GalaSoft.MvvmLight.Command;


    public class ImageCommandViewModel : CommandViewModel
    {
        #region Constructors

        public ImageCommandViewModel(Bitmap image, string displayName, RelayCommand relayCommand)
            : base(displayName, relayCommand)
        {
            this.Image = image;
        }

        #endregion



        #region Properties, Indexers

        public Bitmap Image { get; set; }

        #endregion
    }
}