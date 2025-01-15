using System.Collections.Generic;
using System.Linq;
using NHibernate;

namespace cocina
{
    public class PedidoService
    {
        public IList<Pedido> ObtenerPedidosConPlatos()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var query = session.CreateQuery("SELECT p FROM Pedido p LEFT JOIN FETCH p.Plato WHERE done_at = NULL");
                var pedidos = query.List<Pedido>();
                return pedidos;
            }
        }
    }

}
