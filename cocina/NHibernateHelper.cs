using NHibernate;
using NHibernate.Cfg;

namespace cocina
{
    public static class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public static ISession OpenSession()
        {
            if (_sessionFactory == null)
            {
                var configuration = new Configuration();
                configuration.Configure(); // Esto carga el archivo hibernate.cfg.xml
                configuration.AddAssembly("cocina"); // Asegúrate de que el ensamblado es correcto
                _sessionFactory = configuration.BuildSessionFactory();
            }
            return _sessionFactory.OpenSession();
        }
    }
}
