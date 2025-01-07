using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace cocina
{
    class Program
    {
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());

            /*var servicio = new PedidoService();
            var pedidos = servicio.ObtenerPedidos();

            foreach (var pedido in pedidos)
            {
                Console.WriteLine($"Id: {pedido.Id}, Mesa: {pedido.Mesa}, Plato: {pedido.Plato}, Nota: {pedido.Nota}");
            }*/

        }
    }
}
