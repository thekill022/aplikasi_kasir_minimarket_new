using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace aplikasi_kasir_minimarket
{

    public partial class welcomepage : Form
    {
        public welcomepage()
        {
            InitializeComponent();

            connection kn = new connection();

            if (kn.isValid())
            {
                MessageBox.Show("Koneksi berhasil", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Koneksi gagal, silakan hubungkan ke jaringan yang sama terlebih dahulu.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            loginpage form3 = new loginpage();
            form3.Show();
            this.Hide();
        }
    }
}

