using System;

namespace cocina
{
    public class Pedido
    {
        public int Id { get; set; }
        public int Mesa { get; set; }
        public string Plato { get; set; }
        public string Nota { get; set; }
        public bool Preparando { get; set; }
        public DateTime? Entregado { get; set; }

        public void CrearPedido(Pedido nuevoPedido)
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Save(nuevoPedido);
                transaction.Commit();
            }
        }

        public Pedido ObtenerPedidoPorId(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.Get<Pedido>(id);
            }
        }

        public void ActualizarPedido(Pedido pedido)
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Update(pedido);
                transaction.Commit();
            }
        }

        public void EliminarPedido(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var pedido = session.Get<Pedido>(id);
                if (pedido != null)
                {
                    session.Delete(pedido);
                    transaction.Commit();
                }
            }
        }

    }
}
