﻿// ///////////////////////////////////
// File: WorkspaceViewModel.cs
// Last Change: 19.02.2018, 20:07
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System.Drawing;


    public class WorkspaceViewModel : BindableViewModelBase
    {
        #region Constructors

        public WorkspaceViewModel(string title, Bitmap image)
        {
            this.Title = title;
            this.Image = image;
        }

        #endregion



        #region Properties, Indexers

        public string Title { get; }

        public Bitmap Image { get; }

        #endregion
    }
}