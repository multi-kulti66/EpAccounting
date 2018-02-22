// ///////////////////////////////////
// File: NHibernateRepository.cs
// Last Change: 22.02.2018, 20:44
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Data;
    using NHibernate;
    using NHibernate.Criterion;


    public class NHibernateRepository : IRepository
    {
        #region Fields

        private const int PageSize = 50;
        private readonly ISessionManager _sessionManager;

        #endregion



        #region Constructors

        public NHibernateRepository(ISessionManager sessionManager)
        {
            this._sessionManager = sessionManager;
        }

        #endregion



        #region IRepository Members

        public bool IsConnected
        {
            get { return this._sessionManager.IsConnected; }
        }

        public string FilePath
        {
            get { return this._sessionManager.FilePath; }
        }

        public void CreateDatabase(string filePath)
        {
            this._sessionManager.CreateDatabase(filePath);
        }

        public void LoadDatabase(string filePath)
        {
            this._sessionManager.LoadDatabase(filePath);
        }

        public void CloseDatabase()
        {
            this._sessionManager.CloseDatabase();
        }

        public void SaveOrUpdate<T>(T t) where T : class
        {
            using (ISession session = this._sessionManager.OpenSession())
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
            using (ISession session = this._sessionManager.OpenSession())
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
            using (ISession session = this._sessionManager.OpenSession())
            {
                return session.QueryOver<T>().RowCount();
            }
        }

        public ICollection<T> GetAll<T>(int page) where T : class
        {
            using (ISession session = this._sessionManager.OpenSession())
            {
                return session.QueryOver<T>().Skip((page - 1) * PageSize).Take(PageSize).List();
            }
        }

        public T GetById<T>(int id) where T : class
        {
            using (ISession session = this._sessionManager.OpenSession())
            {
                return session.Get<T>(id);
            }
        }

        public int GetQuantityByCriteria<T>(ICriterion criterion) where T : class
        {
            using (ISession session = this._sessionManager.OpenSession())
            {
                return session.QueryOver<T>().Where(criterion).RowCount();
            }
        }

        public int GetQuantityByCriteria<T, U>(ICriterion criterion1,
                                               Expression<Func<T, U>> combinationCriterion,
                                               ICriterion criterion2) where T : class
        {
            using (ISession session = this._sessionManager.OpenSession())
            {
                return session.QueryOver<T>().Where(criterion1).JoinQueryOver(combinationCriterion).Where(criterion2).RowCount();
            }
        }

        public int GetQuantityByCriteria<T, U, V>(ICriterion criterion1,
                                                  Expression<Func<T, U>> combinationCriterion1,
                                                  ICriterion criterion2,
                                                  Expression<Func<U, V>> combinationCriterion2,
                                                  ICriterion criterion3) where T : class
        {
            using (ISession session = this._sessionManager.OpenSession())
            {
                return session.QueryOver<T>().Where(criterion1).JoinQueryOver(combinationCriterion1).Where(criterion2).JoinQueryOver(combinationCriterion2).Where(criterion3).RowCount();
            }
        }

        public ICollection<T> GetByCriteria<T>(ICriterion criterion, int page) where T : class
        {
            using (ISession session = this._sessionManager.OpenSession())
            {
                return session.QueryOver<T>().Where(criterion).Skip((page - 1) * PageSize).Take(PageSize).List();
            }
        }

        public ICollection<T> GetByCriteria<T, TU>(ICriterion criterion1,
                                                   Expression<Func<T, TU>> combinationCriterion,
                                                   ICriterion criterion2, int page) where T : class
        {
            using (ISession session = this._sessionManager.OpenSession())
            {
                return session.QueryOver<T>().Where(criterion1).JoinQueryOver(combinationCriterion).Where(criterion2).Skip((page - 1) * PageSize).Take(PageSize).List();
            }
        }

        public ICollection<T> GetByCriteria<T, U, V>(ICriterion criterion1,
                                                     Expression<Func<T, U>> combinationCriterion1,
                                                     ICriterion criterion2,
                                                     Expression<Func<U, V>> combinationCriterion2,
                                                     ICriterion criterion3, int page) where T : class
        {
            using (ISession session = this._sessionManager.OpenSession())
            {
                return session.QueryOver<T>().Where(criterion1).JoinQueryOver(combinationCriterion1).Where(criterion2).JoinQueryOver(combinationCriterion2).Where(criterion3).Skip((page - 1) * PageSize).Take(PageSize).List();
            }
        }

        #endregion
    }
}