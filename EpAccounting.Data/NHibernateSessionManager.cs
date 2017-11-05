// ///////////////////////////////////
// File: NHibernateSessionManager.cs
// Last Change: 28.10.2017  12:25
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Data
{
    using System.Data.SQLite;
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
            ISessionFactory sessionFactory = Fluently.Configure()
                                                     .Database(SQLiteConfiguration.Standard.UsingFile(filePath))
                                                     .Mappings(m => m.FluentMappings.AddFromAssembly(this.assembly))
                                                     .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true))
                                                     .BuildSessionFactory();

            using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", filePath)))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    using (SQLiteTransaction tr = conn.BeginTransaction())
                    {
                        cmd.Transaction = tr;

                        const string clientIdInsert = "INSERT INTO SQLITE_SEQUENCE (name, seq) VALUES (\'Client\', 25000)";
                        const string billIdInsert = "INSERT INTO SQLITE_SEQUENCE (name, seq) VALUES (\'Bill\', 25000)";

                        cmd.CommandText = clientIdInsert;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = billIdInsert;
                        cmd.ExecuteNonQuery();

                        tr.Commit();
                    }
                }
            }

            return sessionFactory;
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