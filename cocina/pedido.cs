using System;
using System.Collections.Generic;

namespace cocina
{
    public class Platera
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; }
        public virtual string Deskribapena { get; set; }
        public virtual string Mota { get; set; }
        public virtual string Platera_mota { get; set; }
        public virtual double Prezioa { get; set; }
        public virtual bool Menu { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual int UpdatedBy { get; set; }
        public virtual DateTime? DeletedAt { get; set; }
        public virtual int? DeletedBy { get; set; }
    }

    public class Pedido
    {
        public virtual int Id { get; set; }
        public virtual int Langile { get; set; }
        public virtual int Mesa { get; set; }
        public virtual string Nota { get; set; }
        public virtual bool Preparando { get; set; }
        public virtual bool Done { get; set; }
        public virtual DateTime? DoneAt { get; set; }
        public virtual DateTime? EskaeraOrdua { get; set; }
        public virtual Platera Plato { get; set; }
        public virtual string Egoera { get; set; }

    // Propiedad auxiliar para obtener el nombre del plato
    public virtual string NombrePlato => Plato?.Izena ?? "No tiene plato";

        public virtual void CrearPedido(Pedido nuevoPedido)
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Save(nuevoPedido);
                transaction.Commit();
            }
        }

        public virtual Pedido ObtenerPedidoPorId(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.Get<Pedido>(id);
            }
        }

        public virtual void ActualizarPedido(Pedido pedido)
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Update(pedido);
                transaction.Commit();
            }
        }


        public virtual void EliminarPedido(int id)
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
