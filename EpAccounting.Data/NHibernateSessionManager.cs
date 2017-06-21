// ///////////////////////////////////
// File: NHibernateSessionManager.cs
// Last Change: 13.03.2017  20:59
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Data
{
    using System.Reflection;
    using EpAccounting.Model;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using NHibernate;
    using NHibernate.Tool.hbm2ddl;



    public class NHibernateSessionManager : SessionManager
    {
        #region Fields

        private readonly Assembly assembly = typeof(Client).Assembly;

        #endregion



        protected override ISessionFactory CreateSchema(string filePath)
        {
            return Fluently.Configure()
                           .Database(SQLiteConfiguration.Standard.UsingFile(filePath))
                           .Mappings(m => m.FluentMappings.AddFromAssembly(this.assembly))
                           .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                           .BuildSessionFactory();
        }

        protected override ISessionFactory LoadSchema(string filePath)
        {
            return Fluently.Configure()
                           .Database(SQLiteConfiguration.Standard.UsingFile(filePath))
                           .Mappings(m => m.FluentMappings.AddFromAssembly(this.assembly))
                           .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                           .BuildSessionFactory();
        }
    }
}