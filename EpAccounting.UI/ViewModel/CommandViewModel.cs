// ///////////////////////////////////
// File: CommandViewModel.cs
// Last Change: 16.09.2017  11:33
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using GalaSoft.MvvmLight.Command;



    public class CommandViewModel : BindableViewModelBase
    {
        #region Constructors / Destructor

        public CommandViewModel(string displayName, RelayCommand relayCommand)
        {
            this.DisplayName = displayName;
            this.RelayCommand = relayCommand;
        }

        #endregion



        #region Properties

        public string DisplayName { get; private set; }

        public RelayCommand RelayCommand { get; private set; }

        #endregion
    }
}