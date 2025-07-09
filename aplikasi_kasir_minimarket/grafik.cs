using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace aplikasi_kasir_minimarket
{
    public partial class grafik : Form
    {
        connection kn = new connection();
        string connectionString = "";

        private void chartBulanan()
        {
            try
            {
                chart1.Series.Clear();
                chart1.ChartAreas.Clear();
                chart1.Titles.Clear();

                ChartArea chartArea = new ChartArea();
                chartArea.Name = "Pendapatan Bulanan Area";
                chart1.ChartAreas.Add(chartArea);

                Title chartTitle = new Title("Pendapatan Bulanan", Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black);
                chart1.Titles.Add(chartTitle);

                Series series = new Series("Pendapatan Bulanan");
                series.ChartType = SeriesChartType.Column;
                series.Color = Color.Blue;

                string query = "GetPendapatanBulanan";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StartDate", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@EndDate", dateTimePicker2.Value.Date.AddDays(1));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        string bulan = row["Bulan"].ToString();
                        decimal totalPendapatan = Convert.ToDecimal(row["TotalPendapatan"]);
                        series.Points.AddXY(bulan, totalPendapatan);
                    }
                }

                chart1.Series.Add(series);

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error database saat mengambil data bulanan: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chartTahunan()
        {
            try
            {
                chart1.Series.Clear();
                chart1.ChartAreas.Clear();
                chart1.Titles.Clear();

                ChartArea chartArea = new ChartArea();
                chart1.ChartAreas.Add(chartArea);

                Title chartTitle = new Title("Pendapatan Tahunan", Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black);
                chart1.Titles.Add(chartTitle);

                Series series = new Series("Pendapatan Tahunan");
                series.ChartType = SeriesChartType.Column;
                series.Color = Color.Green;

                string query = "GetPendapatanTahunan";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StartDate", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@EndDate", dateTimePicker2.Value.Date.AddDays(1));

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        decimal total = Convert.ToDecimal(reader["TotalPendapatan"]);
                        string label = $"Tahun {dateTimePicker1.Value.Year}";

                        series.Points.AddXY(label, total);
                    }
                    else
                    {
                        series.Points.AddXY($"Tahun {dateTimePicker1.Value.Year}", 0);
                    }
                }

                chart1.Series.Add(series);

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string nama;
        private string username;

        public grafik(string nama, string username)
        {
            InitializeComponent();
            connectionString = kn.connectionString();
            this.nama = nama;
            this.username = username;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            chartBulanan();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chartTahunan();
        }

        private void label2_Click_1(object sender, EventArgs e)
        {
            adminpage admin = new adminpage(nama, username, "admin");
            admin.Show();
            this.Close();
        }
    }
}