// ///////////////////////////////////
// File: WorkspaceViewModel.cs
// Last Change: 23.08.2017  20:49
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

        public string Title { get; }

        public Bitmap Image { get; }

        #endregion
    }
}