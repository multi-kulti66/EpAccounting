// ///////////////////////////////////
// File: ISessionManager.cs
// Last Change: 13.03.2017  20:59
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Data
{
    using NHibernate;



    public interface ISessionManager
    {
        #region Properties

        bool IsConnected { get; }

        string FilePath { get; }

        #endregion



        void CreateDatabase(string folderPath);

        void LoadDatabase(string filePath);

        void CloseDatabase();

        ISession OpenSession();
    }
}