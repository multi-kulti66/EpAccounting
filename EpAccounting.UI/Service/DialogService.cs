﻿// ///////////////////////////////////
// File: DialogService.cs
// Last Change: 13.03.2017  21:04
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Service
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using EpAccounting.UI.Properties;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using Application = System.Windows.Application;



    public class DialogService : IDialogService
    {
        #region IDialogService Members

        public async Task ShowMessage(string title, string message)
        {
            MetroWindow metroWindow = Application.Current.MainWindow as MetroWindow;
            await metroWindow.ShowMessageAsync(title, message);
        }

        public async Task<bool> ShowDialogYesNo(string title, string message)
        {
            MetroWindow metroWindow = Application.Current.MainWindow as MetroWindow;

            MessageDialogResult result = await metroWindow.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ShowCustomDialog(string title, string message, string button1, string button2)
        {
            MetroWindow metroWindow = Application.Current.MainWindow as MetroWindow;

            MetroDialogSettings settings = new MetroDialogSettings
                                           {
                                               AffirmativeButtonText = button1,
                                               NegativeButtonText = button2
                                           };

            MessageDialogResult result = await metroWindow.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, settings);

            if (result == MessageDialogResult.Affirmative)
            {
                return true;
            }

            return false;
        }

        public string ShowDatabaseFileDialog()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = @"Geben Sie den Pfad zur Datenbank an.";
                ofd.Filter = @"db file (*.db)|*.db";
                ofd.InitialDirectory = Environment.UserName;
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileName;
                }

                return null;
            }
        }

        public string ShowFolderDialog()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = @"Geben Sie den gewünschten Ordnerpfad an.";
                fbd.SelectedPath = Settings.Default.RecentFolderPath;
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Settings.Default.RecentFolderPath = fbd.SelectedPath;
                    return fbd.SelectedPath;
                }

                return null;
            }
        }

        #endregion
    }
}