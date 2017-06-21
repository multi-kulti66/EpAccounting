// ///////////////////////////////////
// File: NHibernateRepository.cs
// Last Change: 13.03.2017  20:58
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Business
{
    using System;
    using System.Collections.Generic;
    using EpAccounting.Data;
    using NHibernate;
    using NHibernate.Criterion;



    public class NHibernateRepository : IRepository
    {
        #region Fields

        private readonly ISessionManager sessionManager;

        #endregion



        #region Constructors / Destructor

        public NHibernateRepository(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        #endregion



        #region IRepository Members

        public bool IsConnected
        {
            get { return this.sessionManager.IsConnected; }
        }

        public string FilePath
        {
            get { return this.sessionManager.FilePath; }
        }

        public void CreateDatabase(string folderPath)
        {
            this.sessionManager.CreateDatabase(folderPath);
        }

        public void LoadDatabase(string filePath)
        {
            this.sessionManager.LoadDatabase(filePath);
        }

        public void CloseDatabase()
        {
            this.sessionManager.CloseDatabase();
        }

        public void SaveOrUpdate<T>(T t) where T : class
        {
            using (ISession session = this.sessionManager.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.SaveOrUpdate(t);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException(e.Message, e.InnerException);
                    }
                }
            }
        }

        public void Delete<T>(T t) where T : class
        {
            using (ISession session = this.sessionManager.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        // necessary to check if object even exists in database
                        session.Update(t);
                        session.Delete(t);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException(e.Message, e.InnerException);
                    }
                }
            }
        }

        public int GetQuantity<T>() where T : class
        {
            using (ISession session = this.sessionManager.OpenSession())
            {
                return session.QueryOver<T>().RowCount();
            }
        }

        public ICollection<T> GetAll<T>() where T : class
        {
            using (ISession session = this.sessionManager.OpenSession())
            {
                return session.QueryOver<T>().List();
            }
        }

        public T GetById<T>(int id) where T : class
        {
            using (ISession session = this.sessionManager.OpenSession())
            {
                return session.Get<T>(id);
            }
        }

        public ICollection<T> GetByCriteria<T>(ICriterion criterion) where T : class
        {
            using (ISession session = this.sessionManager.OpenSession())
            {
                return session.QueryOver<T>().Where(criterion).List();
            }
        }

        #endregion
    }
}