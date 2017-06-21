// ///////////////////////////////////
// File: IRepository.cs
// Last Change: 13.03.2017  20:58
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Business
{
    using System.Collections.Generic;
    using NHibernate.Criterion;



    public interface IRepository
    {
        #region Properties

        bool IsConnected { get; }

        string FilePath { get; }

        #endregion



        void CreateDatabase(string folderPath);

        void LoadDatabase(string filePath);

        void CloseDatabase();

        void SaveOrUpdate<T>(T t) where T : class;

        void Delete<T>(T t) where T : class;

        int GetQuantity<T>() where T : class;

        ICollection<T> GetAll<T>() where T : class;

        T GetById<T>(int id) where T : class;

        ICollection<T> GetByCriteria<T>(ICriterion criterion) where T : class;
    }
}