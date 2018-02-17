// ///////////////////////////////////
// File: DatabaseFactory.cs
// Last Change: 28.10.2017  11:17
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test
{
    using System.IO;
    using EpAccounting.UI.Properties;



    public static class DatabaseFactory
    {
        private const string DIRECTORY_DESKTOP = @"D:\Downloads";
        private const string DIRECTORY_SURFACE = @"C:\Users\Andre\Desktop";
        private const string DIRECTORY_FOLDERNAME = "TestFolder";


        public static string TestFolderPath
        {
            get
            {
                if (Directory.Exists(DIRECTORY_DESKTOP))
                {
                    return Path.Combine(DIRECTORY_DESKTOP, DIRECTORY_FOLDERNAME);
                }

                return Path.Combine(DIRECTORY_SURFACE, DIRECTORY_FOLDERNAME);
            }
        }

        public static string TestFilePath
        {
            get { return Path.Combine(TestFolderPath, Resources.Database_NameWithExtension); }
        }


        public static void CreateTestFolder()
        {
            if (Directory.Exists(TestFolderPath))
            {
                return;
            }

            Directory.CreateDirectory(TestFolderPath);
        }

        public static void CreateTestFile()
        {
            CreateTestFolder();

            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }

            FileStream fileStream = File.Create(TestFilePath);
            fileStream.Close();
        }

        public static void DeleteTestFolderAndFile()
        {
            if (Directory.Exists(TestFolderPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(TestFolderPath);
                directoryInfo.Delete(true);
            }
        }

        public static void SetSavedFilePath()
        {
            Settings.Default.DatabaseFilePath = TestFilePath;
            Settings.Default.Save();
        }

        public static void ClearSavedFilePath()
        {
            Settings.Default.DatabaseFilePath = string.Empty;
            Settings.Default.Save();
        }
    }
}