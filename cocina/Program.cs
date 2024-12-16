using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cocina
{
    class Program
    {
        static void Main(string[] args)
        {
            var servicio = new PedidoService();
            var pedidos = servicio.ObtenerPedidos();

            foreach (var pedido in pedidos)
            {
                Console.WriteLine($"Id: {pedido.Id}, Mesa: {pedido.Mesa}, Plato: {pedido.Plato}, Nota: {pedido.Nota}");
            }
        }
    }
}
