using NHibernate;
using NHibernate.Cfg;
using System;
using System.Configuration;
using System.Reflection;

namespace cocina
{
    public static class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public static ISession OpenSession()
        {
            if (_sessionFactory == null)
            {
                var configuration = new NHibernate.Cfg.Configuration();
                try
                {
                    configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, ConfigurationManager.AppSettings["hibernate.connection.provider"]);
                    configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, ConfigurationManager.AppSettings["hibernate.connection.driver_class"]);
                    configuration.SetProperty(NHibernate.Cfg.Environment.Dialect, ConfigurationManager.AppSettings["hibernate.dialect"]);
                    configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, ConfigurationManager.ConnectionStrings["NHibernateConnection"].ConnectionString);

                    configuration.AddAssembly(Assembly.GetExecutingAssembly());
                    _sessionFactory = configuration.BuildSessionFactory();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("NHibernate configuration error: " + ex.Message);
                    throw;
                }
            }
            return _sessionFactory.OpenSession();
        }
    }
}
