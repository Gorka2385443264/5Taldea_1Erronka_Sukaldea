using System.Collections.Generic;
using System.Linq;
using NHibernate;

namespace cocina
{
    public class PedidoService
    {
        public List<Pedido> ObtenerPedidos()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.Query<Pedido>().ToList();
            }
        }
    }
}
