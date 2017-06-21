// ///////////////////////////////////
// File: WorkspaceViewModel.cs
// Last Change: 13.03.2017  15:45
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System.Drawing;



    public class WorkspaceViewModel : BindableViewModelBase
    {
        #region Constructors / Destructor

        public WorkspaceViewModel(string title, Bitmap image)
        {
            this.Title = title;
            this.Image = image;
        }

        #endregion



        #region Properties

        public string Title { get; private set; }

        public Bitmap Image { get; private set; }

        #endregion
    }
}