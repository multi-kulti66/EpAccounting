// ///////////////////////////////////
// File: SessionManager.cs
// Last Change: 17.02.2018, 19:15
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.Data
{
    using System;
    using System.IO;
    using NHibernate;
    using Properties;


    public abstract class SessionManager : ISessionManager
    {
        #region Properties, Indexers

        private ISessionFactory SessionFactory { get; set; }

        #endregion



        #region ISessionManager Members

        public bool IsConnected
        {
            get { return this.SessionFactory != null; }
        }

        public string FilePath { get; private set; }

        public void CreateDatabase(string filePath)
        {
            this.SessionFactory = this.CreateSchema(filePath);
            this.FilePath = filePath;
        }

        public void LoadDatabase(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException(Resources.Exception_Message_CanNotLoadNotExistingDatabase);
            }

            if (Path.GetExtension(filePath) != Resources.Database_Extension)
            {
                throw new InvalidOperationException(Resources.Exception_Message_CanNotLoadInvalidFile);
            }

            this.SessionFactory = this.LoadSchema(filePath);
            this.FilePath = filePath;
        }

        public ISession OpenSession()
        {
            if (this.SessionFactory == null)
            {
                throw new NullReferenceException(Resources.Exception_Message_NoDatabaseConnection);
            }

            return this.SessionFactory.OpenSession();
        }

        public void CloseDatabase()
        {
            if (!this.IsConnected)
            {
                return;
            }

            this.SessionFactory.Close();
            this.SessionFactory = null;
            this.FilePath = null;
        }

        #endregion



        protected abstract ISessionFactory CreateSchema(string filePath);

        protected abstract ISessionFactory LoadSchema(string filePath);
    }
}