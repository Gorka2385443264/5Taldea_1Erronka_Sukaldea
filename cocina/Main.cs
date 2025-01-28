using NHibernate.Mapping.ByCode;
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
        sukaldeaController controller = new sukaldeaController();
        private bool isFullScreen = false;
        private FormBorderStyle previousFormBorderStyle;
        private FormWindowState previousWindowState;
        private Timer updateTimer;

        private BindingSource bindingSource = new BindingSource();

        // Colores de la app
        private readonly Color primaryColor = ColorTranslator.FromHtml("#212121");
        private readonly Color secondaryColor = ColorTranslator.FromHtml("#3a3a3a");
        private readonly Color accentColor = ColorTranslator.FromHtml("#ececec");

        public Main()
        {
            InitializeComponent();
            InitializeTimer();
            this.KeyPreview = true;
            this.SizeChanged += new EventHandler(Form_SizeChanged);
            this.Load += new EventHandler(Main_Load);
            this.KeyDown += new KeyEventHandler(Main_KeyDown);

            dataGridView1.CellMouseClick += new DataGridViewCellMouseEventHandler(dataGridView1_CellMouseClick);

            this.BackColor = primaryColor;
            this.ForeColor = accentColor;
        }

        private void InitializeTimer()
        {
            updateTimer = new Timer();
            updateTimer.Interval = 10000; // Intervalo de 10 segundos
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            RefreshDataGrid();
            Console.WriteLine("Tabla actualizada automaticamente");
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Configuración del DataGridView
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Ajustar colores del DataGridView
            dataGridView1.BackgroundColor = primaryColor;
            dataGridView1.DefaultCellStyle.BackColor = primaryColor;
            dataGridView1.DefaultCellStyle.ForeColor = accentColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = secondaryColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = accentColor;

            // Hacer que el color de selección sea transparente
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Transparent;
            dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.Transparent;
            dataGridView1.RowsDefaultCellStyle.SelectionForeColor = Color.Transparent;

            dataGridView1.EnableHeadersVisualStyles = false; // Asegurar que los colores personalizados se apliquen

            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Nº PEDIDO",
                    DataPropertyName = "Mesa",
                    ReadOnly = true
                });

                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "PLATO",
                    DataPropertyName = "NombrePlato",
                    ReadOnly = true
                });

                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "NOTA",
                    DataPropertyName = "Nota",
                    ReadOnly = false
                });

                var preparandocolumn = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "PREPARADO",
                    DataPropertyName = "Preparando",
                    Name = "egoera",
                    ReadOnly = false,
                    CellTemplate = new CustomDataGridViewCheckBoxCell()
                };
                dataGridView1.Columns.Add(preparandocolumn);

                var donecolumn = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "ENTREGADO",
                    DataPropertyName = "Done",
                    Name = "done",
                    ReadOnly = false,
                    CellTemplate = new CustomDataGridViewCheckBoxCell()
                };
                dataGridView1.Columns.Add(donecolumn);

                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "FECHA DE ENTREGA",
                    DataPropertyName = "DoneAt",
                    Name = "done_at",
                    ReadOnly = true
                });
                dataGridView1.Columns["done_at"].Visible = false;
            }

            dataGridView1.DataSource = bindingSource;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;

            RefreshDataGrid();
            Form_SizeChanged(sender, e);
            ToggleFullScreen();
        }


        private void Form_SizeChanged(object sender, EventArgs e)
        {
            int rowHeight = this.Height / 20;
            dataGridView1.RowTemplate.Height = rowHeight;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = rowHeight;
            }

            float fontSize = Math.Max(10, this.Width / 100.0f);
            Font newFont = new Font("Roboto", fontSize);
            dataGridView1.DefaultCellStyle.Font = newFont;
            dataGridView1.DefaultCellStyle.ForeColor = accentColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = newFont;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = accentColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = secondaryColor;
            dataGridView1.RowHeadersDefaultCellStyle.Font = newFont;
            dataGridView1.RowHeadersDefaultCellStyle.ForeColor = accentColor;

            dataGridView1.Refresh();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count && e.ColumnIndex >= 0)
            {
                try
                {
                    if (dataGridView1.Rows[e.RowIndex].DataBoundItem is Pedido pedidoEditado)
                    {
                        int pedidoId = pedidoEditado.Id;
                        var pedido = new Pedido().ObtenerPedidoPorId(pedidoId);

                        if (pedido != null)
                        {
                            int egoeraIndex = dataGridView1.Columns["egoera"]?.Index ?? -1;
                            int doneIndex = dataGridView1.Columns["done"]?.Index ?? -1;

                            if (egoeraIndex != -1 && e.ColumnIndex == egoeraIndex)
                            {
                                pedido.Preparando = Convert.ToBoolean(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
                            }
                            else if (doneIndex != -1 && e.ColumnIndex == doneIndex)
                            {
                                pedido.Done = Convert.ToBoolean(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
                                if (pedido.Done)
                                {
                                    pedido.DoneAt = DateTime.Now;
                                }
                                else
                                {
                                    pedido.DoneAt = null;
                                }
                            }
                            controller.HacerTransaccion(pedido);
                            RefreshDataGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
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

        private void RefreshDataGrid()
        {
            var servicio = new PedidoService();
            var pedidos = servicio.ObtenerPedidosConPlatos();
            bindingSource.DataSource = null;

            bindingSource.DataSource = pedidos;

            dataGridView1.Invoke((MethodInvoker)delegate
            {
                dataGridView1.DataSource = null;
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

        public class CustomDataGridViewCheckBoxCell : DataGridViewCheckBoxCell
        {
            private readonly Color checkBoxFillColor = ColorTranslator.FromHtml("#212121");
            private readonly Color checkBoxTickColor = ColorTranslator.FromHtml("#ececec");

            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

                // Hacer que el checkbox ocupe toda la celda
                Size checkBoxSize = new Size(cellBounds.Width - 1, cellBounds.Height - 1);
                Point checkBoxLocation = new Point(cellBounds.X, cellBounds.Y);

                using (Brush fillBrush = new SolidBrush(checkBoxFillColor))
                {
                    graphics.FillRectangle(fillBrush, new Rectangle(checkBoxLocation, checkBoxSize));
                }

                if (Convert.ToBoolean(value))
                {
                    using (Pen tickPen = new Pen(checkBoxTickColor, 2)) //grosor del tick
                    {
                        PointF[] tickPoints = new PointF[]
                        {
                    new PointF(checkBoxLocation.X + checkBoxSize.Width * 0.2f, checkBoxLocation.Y + checkBoxSize.Height * 0.5f),
                    new PointF(checkBoxLocation.X + checkBoxSize.Width * 0.4f, checkBoxLocation.Y + checkBoxSize.Height * 0.75f),
                    new PointF(checkBoxLocation.X + checkBoxSize.Width * 0.8f, checkBoxLocation.Y + checkBoxSize.Height * 0.3f)
                        };

                        graphics.DrawLines(tickPen, tickPoints);
                    }
                }
            }
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var preparingColumnIndex = dataGridView1.Columns["egoera"]?.Index ?? -1;
                var doneColumnIndex = dataGridView1.Columns["done"]?.Index ?? -1;

                if (e.ColumnIndex == preparingColumnIndex)
                {
                    dataGridView1.BeginEdit(true);
                    var currentValue = Convert.ToBoolean(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
                    dataGridView1[e.ColumnIndex, e.RowIndex].Value = !currentValue;
                    dataGridView1.EndEdit();
                }
                else if (e.ColumnIndex == doneColumnIndex)
                {
                    var isPreparingChecked = Convert.ToBoolean(dataGridView1[preparingColumnIndex, e.RowIndex].Value);
                    if (isPreparingChecked)
                    {
                        dataGridView1.BeginEdit(true);
                        var currentValue = Convert.ToBoolean(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
                        dataGridView1[e.ColumnIndex, e.RowIndex].Value = !currentValue;
                        dataGridView1.EndEdit();
                    }
                    else
                    {
                        MessageBox.Show("Debes activar la casilla 'Preparando' antes de marcar 'Entregado'.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
    }
}
