// ///////////////////////////////////
// File: OptionViewModel.cs
// Last Change: 17.02.2018, 14:29
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Drawing;
    using System.IO;
    using Business;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Properties;
    using Service;


    public class OptionViewModel : WorkspaceViewModel
    {
        #region Fields

        private readonly IRepository _repository;
        private readonly IDialogService _dialogService;

        private RelayCommand _createDatabaseCommand;
        private RelayCommand _loadDatabaseCommand;

        #endregion



        #region Constructors

        public OptionViewModel(string title, Bitmap image,
                               IRepository repository, IDialogService dialogService) : base(title, image)
        {
            this._repository = repository;
            this._dialogService = dialogService;
        }

        #endregion



        #region Properties, Indexers

        public string FilePath
        {
            get { return Settings.Default.DatabaseFilePath; }
            private set
            {
                if (this.SetProperty(() => Settings.Default.DatabaseFilePath = value, () => Settings.Default.DatabaseFilePath == value))
                {
                    Settings.Default.Save();
                }
            }
        }

        public RelayCommand CreateDatabaseCommand
        {
            get
            {
                if (this._createDatabaseCommand == null)
                {
                    this._createDatabaseCommand = new RelayCommand(this.CreateDatabase);
                }

                return this._createDatabaseCommand;
            }
        }

        public RelayCommand LoadDatabaseCommand
        {
            get
            {
                if (this._loadDatabaseCommand == null)
                {
                    this._loadDatabaseCommand = new RelayCommand(this.LoadDatabase);
                }

                return this._loadDatabaseCommand;
            }
        }

        #endregion



        private async void CreateDatabase()
        {
            string databaseFolderPath = this._dialogService.ShowFolderDialog();

            if (string.IsNullOrEmpty(databaseFolderPath))
            {
                return;
            }

            try
            {
                string databaseFilePath = Path.Combine(databaseFolderPath, Resources.Database_NameWithExtension);

                if (File.Exists(databaseFilePath))
                {
                    bool shouldOverwrite = await this._dialogService.ShowDialogYesNo(Resources.Dialog_Title_DatabaseAlreadyExists,
                                                                                    string.Format(Resources.Dialog_Question_ShouldOverwriteDatabase, databaseFilePath));

                    if (!shouldOverwrite)
                    {
                        return;
                    }
                }

                this._repository.CreateDatabase(databaseFilePath);
                this.UpdatePathAndConnectionState(databaseFilePath);
            }
            catch (Exception e)
            {
                await this._dialogService.ShowMessage(Resources.Exception_Message_CouldNotCreateDatabase, e.Message);
            }
        }

        private void LoadDatabase()
        {
            string databaseFilePath = this._dialogService.ShowDatabaseFileDialog();

            if (string.IsNullOrEmpty(databaseFilePath))
            {
                return;
            }

            try
            {
                this._repository.LoadDatabase(databaseFilePath);
                this.UpdatePathAndConnectionState(databaseFilePath);
            }
            catch (Exception e)
            {
                this._dialogService.ShowMessage(Resources.Exception_Message_CouldNotLoadDatabase, e.Message);
            }
        }

        private void UpdatePathAndConnectionState(string databaseFilePath)
        {
            this.FilePath = databaseFilePath;
            Messenger.Default.Send(new NotificationMessage(Resources.Message_UpdateConnectionStateForMainVM));
        }
    }
}