using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Forms;


namespace aplikasi_kasir_minimarket
{
    public partial class grafik : Form
    {
        string connectionString = "Data Source=LAPTOP-5DVR7M3S\\GFB_SERVER;Initial Catalog=KasirMinimarket;Integrated Security=True";
        private void chartBulanan()
        {
            chart1.Series.Clear();
            chart1.Titles.Clear(); 
            chart1.ChartAreas.Clear();
            ChartArea chartArea = new ChartArea("Pendapatan Bulanan Area");
            chart1.ChartAreas.Add(chartArea.ToString());

            Title chartTitle = new Title("Pendapatan Bulanan", Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black);
            chart1.Titles.Add(chartTitle.ToString());
            Series series = new Series("Pendapatan Harian");
            series.ChartType = SeriesChartType.Bar;
            series.Color = System.Drawing.Color.Blue;

            string query = "GetPendapatanBulanan";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pastikan dateTimePicker1 dan dateTimePicker2 ada di desainer form
                    cmd.Parameters.AddWithValue("@StartDate", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@EndDate", dateTimePicker2.Value.Date.AddDays(1));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        string tanggal = row["Tanggal"].ToString();
                        decimal totalPendapatan = Convert.ToDecimal(row["TotalPendapatan"]);
                        series.Points.AddXY(tanggal, totalPendapatan);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error database saat mengambil data bulanan: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            chart1.Series.Add(series.ToString());
        }

        private string nama;
        private string username;
        public grafik(string nama, string username)
        {
            InitializeComponent();
            this.nama = nama;
            this.username = username;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            chartBulanan();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {
            adminpage admin = new adminpage(nama, username, "admin");
            admin.Show();
            this.Close();
        }
    }
}
