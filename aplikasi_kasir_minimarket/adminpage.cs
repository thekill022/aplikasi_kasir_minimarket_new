using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aplikasi_kasir_minimarket
{
    public partial class adminpage : Form
    {
        private string namaAdmin;
        private string username;
        public adminpage(string namaAdmin, string username, string roleAdmin)
        {
            InitializeComponent();
            this.namaAdmin = namaAdmin;
            this.username = username;
            label1.Text = namaAdmin;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            loginpage form3 = new loginpage();
            form3.Show();
            this.Hide();
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            manajemenkaryawan karyawan = new manajemenkaryawan(namaAdmin, username);
            karyawan.Show();
            this.Hide();
        }

        private void pictureBox4_Click_1(object sender, EventArgs e)
        {
            manajemenkategori kategori = new manajemenkategori(namaAdmin, username);
            kategori.Show();
            this.Hide();
        }

        private void pictureBox6_Click_1(object sender, EventArgs e)
        {
            grafik Grafik = new grafik(namaAdmin, username);
            Grafik.Show();
            this.Hide();
        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {
            manajemenproduk produk = new manajemenproduk(namaAdmin, username);
            produk.Show();
            this.Hide();
        }

        private void pictureBox5_Click_1(object sender, EventArgs e)
        {
            riwayattransaksi transaksi = new riwayattransaksi(namaAdmin, username);
            transaksi.Show();
            this.Hide();
        }

        private void pictureBox7_Click_1(object sender, EventArgs e)
        {
            reportpenjualan report = new reportpenjualan(namaAdmin, username);
            report.Show();
            this.Hide();
        }

        private void adminpage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
