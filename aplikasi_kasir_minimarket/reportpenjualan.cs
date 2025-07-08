using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aplikasi_kasir_minimarket
{
    public partial class reportpenjualan : Form
    {
        private string nama;
        private string username;
        public reportpenjualan(string nama, string username)
        {
            InitializeComponent();
            this.nama = nama;
            this.username = username;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            adminpage admin = new adminpage(nama, username, "admin");
            admin.Show();
            this.Close();
        }

        private void setupReportViewer()
        {
            connection kn = new connection();
            string connectionStirng = kn.connectionString();
            string query = @"
                SELECT 
                p.nama_produk, 
                t.jumlah, 
                t.subtotal, 
                t.id_transaksi, 
                DATENAME(month, r.tanggal_transaksi) AS NamaBulanTransaksi, 
                YEAR(r.tanggal_transaksi) AS TahunTransaksi
                FROM Detail_Transaksi t 
                JOIN Produk p ON t.id_produk = p.id_produk 
                JOIN Riwayat_Transaksi r ON t.id_transaksi = r.id_transaksi
                ORDER BY r.tanggal_transaksi;";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            ReportDataSource rds = new ReportDataSource("DataSet1", dt);

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            reportViewer1.LocalReport.ReportPath = @"C:\PABD\aplikasi_kasir_minimarket\aplikasi_kasir_minimarket\Report1.rdlc";
            reportViewer1.RefreshReport();

        }

        private void reportViewer1_Load_1(object sender, EventArgs e)
        {
            setupReportViewer();
            this.reportViewer1.RefreshReport();
        }
    }
}
