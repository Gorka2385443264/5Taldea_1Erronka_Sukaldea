using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cocina
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            dataGridView1.Dock = DockStyle.Fill; // Hace que el DataGridView ocupe toda la pestaña
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Obtén los datos usando el servicio de pedidos
            var servicio = new PedidoService();
            var pedidos = servicio.ObtenerPedidosConPlatos();

            foreach (var pedido in pedidos)
            {
                Console.WriteLine($"Pedido ID: {pedido.Id}, Plato: {pedido.Plato?.Izena ?? "No tiene plato"}");
            }


            // Configura el DataGridView
            dataGridView1.AutoGenerateColumns = false;

            // Agrega columnas manualmente
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Número Pedido",
                DataPropertyName = "Id",
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Plato",
                DataPropertyName = "NombrePlato",  // Ahora usa la propiedad auxiliar
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nota",
                DataPropertyName = "Nota",
                ReadOnly = false
            });
            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Preparando",
                DataPropertyName = "Preparando",  // Suponiendo que 'Preparando' es un booleano
                Name = "egoera",
                ReadOnly = false
            });
            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Entregado",
                DataPropertyName = "Done",
                Name = "done",
                ReadOnly = false
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Fecha Entregado",
                DataPropertyName = "DoneAt",
                Name = "done_at",
                ReadOnly = true
            });

            dataGridView1.DataSource = pedidos;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.Refresh();
            
        }

        private BindingSource bindingSource = new BindingSource();
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dataGridView1.Rows[e.RowIndex].DataBoundItem is Pedido pedidoEditado)
                {
                    if (pedidoEditado != null)
                    {
                        int pedidoId = pedidoEditado.Id;
                        var pedido = new Pedido().ObtenerPedidoPorId(pedidoId);

                        if (pedido != null)
                        {
                            if (e.ColumnIndex == dataGridView1.Columns["egoera"].Index)
                            {
                                pedido.Preparando = true;
                                RefreshDataGrid();
                            }
                            else if (e.ColumnIndex == dataGridView1.Columns["done"].Index)
                            {
                                pedido.Done = true;
                                pedido.DoneAt = DateTime.Now;
                                RefreshDataGrid();
                            }

                            using (var session = NHibernateHelper.OpenSession())
                            {
                                using (var transaction = session.BeginTransaction())
                                {
                                    session.Update(pedido);
                                    transaction.Commit();
                                }
                            }
                            RefreshDataGrid();
                        }
                    }
                }
            }
        }

        private void RefreshDataGrid()
        {
            var servicio = new PedidoService();
            var pedidos = servicio.ObtenerPedidosConPlatos();

            bindingSource.DataSource = pedidos;

            dataGridView1.DataSource = bindingSource;

            dataGridView1.Refresh();
        }
    }
}
