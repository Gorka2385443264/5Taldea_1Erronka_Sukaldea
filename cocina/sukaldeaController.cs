using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cocina
{
    internal class sukaldeaController
    {
        public void HacerTransaccion(Pedido pedido)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(pedido);
                    transaction.Commit();
                }
            }
        }
        
    }
}
