﻿// ///////////////////////////////////
// File: IRepository.cs
// Last Change: 22.02.2018, 20:34
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using NHibernate.Criterion;


    public interface IRepository
    {
        #region Properties, Indexers

        bool IsConnected { get; }

        string FilePath { get; }

        #endregion



        void CreateDatabase(string filePath);

        void LoadDatabase(string filePath);

        void CloseDatabase();

        void SaveOrUpdate<T>(T t) where T : class;

        void Delete<T>(T t) where T : class;

        int GetQuantity<T>() where T : class;

        ICollection<T> GetAll<T>(int page) where T : class;

        T GetById<T>(int id) where T : class;

        int GetQuantityByCriteria<T>(ICriterion criterion) where T : class;

        int GetQuantityByCriteria<T, TU>(ICriterion criterion1,
                                         Expression<Func<T, TU>> combinationCriterion,
                                         ICriterion criterion2) where T : class;

        int GetQuantityByCriteria<T, U, V>(ICriterion criterion1,
                                           Expression<Func<T, U>> combinationCriterion1,
                                           ICriterion criterion2,
                                           Expression<Func<U, V>> combinationCriterion2,
                                           ICriterion criterion3) where T : class;

        ICollection<T> GetByCriteria<T>(ICriterion criterion, int page) where T : class;

        ICollection<T> GetByCriteria<T, TU>(ICriterion criterion1,
                                            Expression<Func<T, TU>> combinationCriterion,
                                            ICriterion criterion2, int page) where T : class;

        ICollection<T> GetByCriteria<T, U, V>(ICriterion criterion1,
                                              Expression<Func<T, U>> combinationCriterion1,
                                              ICriterion criterion2,
                                              Expression<Func<U, V>> combinationCriterion2,
                                              ICriterion criterion3, int page) where T : class;
    }
}