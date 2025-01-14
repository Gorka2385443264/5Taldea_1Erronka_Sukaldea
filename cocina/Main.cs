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
        private bool isFullScreen = false;
        private FormBorderStyle previousFormBorderStyle;
        private FormWindowState previousWindowState;

        private BindingSource bindingSource = new BindingSource();

        // Definimos los colores personalizados
        private readonly Color primaryColor = ColorTranslator.FromHtml("#091725");
        private readonly Color secondaryColor = ColorTranslator.FromHtml("#BA450D");
        private readonly Color accentColor = ColorTranslator.FromHtml("#E89E47");

        public Main()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.SizeChanged += new EventHandler(Form_SizeChanged);
            this.Load += new EventHandler(Main_Load);
            this.KeyDown += new KeyEventHandler(Main_KeyDown);

            // Aplicar colores personalizados al formulario
            this.BackColor = primaryColor;
            this.ForeColor = accentColor;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Configuración del DataGridView
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;

            // Configuración de pantalla completa
            ToggleFullScreen();

            // Configuración del DataGridView (continuación)
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Ajustar colores del DataGridView
            dataGridView1.BackgroundColor = primaryColor;
            dataGridView1.DefaultCellStyle.BackColor = primaryColor;
            dataGridView1.DefaultCellStyle.ForeColor = accentColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = secondaryColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = accentColor;

            // Configurar columnas del DataGridView
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Número Pedido",
                DataPropertyName = "Id",
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Plato",
                DataPropertyName = "NombrePlato",
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nota",
                DataPropertyName = "Nota",
                ReadOnly = false
            });

            var preparandocolumn = new DataGridViewCheckBoxColumn
            {
                HeaderText = "Preparando",
                DataPropertyName = "Preparando",
                Name = "egoera",
                ReadOnly = false,
                CellTemplate = new CustomDataGridViewCheckBoxCell()
            };
            dataGridView1.Columns.Add(preparandocolumn);

            var donecolumn = new DataGridViewCheckBoxColumn
            {
                HeaderText = "Entregado",
                DataPropertyName = "Done",
                Name = "done",
                ReadOnly = false,
                CellTemplate = new CustomDataGridViewCheckBoxCell()
            };
            dataGridView1.Columns.Add(donecolumn);

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Fecha Entregado",
                DataPropertyName = "DoneAt",
                Name = "done_at",
                ReadOnly = true
            });

            dataGridView1.DataSource = bindingSource;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;

            RefreshDataGrid();
            Form_SizeChanged(sender, e);
        }

        private void Form_SizeChanged(object sender, EventArgs e)
        {
            // Ajustar la altura de las filas en función del tamaño del formulario
            int rowHeight = this.Height / 20;
            dataGridView1.RowTemplate.Height = rowHeight;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = rowHeight;
            }

            // Ajustar el tamaño de la fuente en función del tamaño del formulario
            float fontSize = Math.Max(10, this.Width / 100.0f);
            Font newFont = new Font("Arial", fontSize);
            dataGridView1.DefaultCellStyle.Font = newFont;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = newFont;
            dataGridView1.RowHeadersDefaultCellStyle.Font = newFont;

            dataGridView1.Refresh();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dataGridView1.Rows[e.RowIndex].DataBoundItem is Pedido pedidoEditado)
                {
                    int pedidoId = pedidoEditado.Id;
                    var pedido = new Pedido().ObtenerPedidoPorId(pedidoId);

                    if (pedido != null)
                    {
                        if (e.ColumnIndex == dataGridView1.Columns["egoera"].Index)
                        {
                            pedido.Preparando = !pedido.Preparando;
                        }
                        else if (e.ColumnIndex == dataGridView1.Columns["done"].Index)
                        {
                            pedido.Done = !pedido.Done;
                            if (pedido.Done)
                            {
                                pedido.DoneAt = DateTime.Now;
                            }
                            else
                            {
                                pedido.DoneAt = null;
                            }
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

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        public class CustomDataGridViewCheckBoxCell : DataGridViewCheckBoxCell
        {
            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

                Size checkBoxSize = new Size(cellBounds.Height - 4, cellBounds.Height - 4);
                Point checkBoxLocation = new Point(cellBounds.X + (cellBounds.Width - checkBoxSize.Width) / 2, cellBounds.Y + (cellBounds.Height - checkBoxSize.Height) / 2);

                ControlPaint.DrawCheckBox(graphics, new Rectangle(checkBoxLocation, checkBoxSize), GetCheckBoxState(value));
            }

            private ButtonState GetCheckBoxState(object value)
            {
                if (value == null || value == DBNull.Value)
                    return ButtonState.Normal;

                bool isChecked = Convert.ToBoolean(value);
                return isChecked ? ButtonState.Checked : ButtonState.Normal;
            }
        }

        private void RefreshDataGrid()
        {
            var servicio = new PedidoService();
            var pedidos = servicio.ObtenerPedidosConPlatos();

            bindingSource.DataSource = pedidos;

            dataGridView1.Invoke((MethodInvoker)delegate
            {
                dataGridView1.DataSource = bindingSource;
                dataGridView1.Refresh();
            });
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                ToggleFullScreen();
            }
        }

        private void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                previousFormBorderStyle = this.FormBorderStyle;
                previousWindowState = this.WindowState;

                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
                isFullScreen = true;
            }
            else
            {
                this.FormBorderStyle = previousFormBorderStyle;
                this.WindowState = previousWindowState;
                this.TopMost = false;
                isFullScreen = false;
            }
        }
    }
}
