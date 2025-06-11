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
    public partial class kasirpage : Form
    {
        private string nama;
        private string username;
        public kasirpage(string nama, string username, string role)
        {
            InitializeComponent();
            this.nama = nama;
            this.username = username;
            label1.Text = nama;
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            aplikasikasir kasir = new aplikasikasir(nama, username);
            kasir.Show();
            this.Close();
        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {
            updatestok update = new updatestok(nama, username);
            update.Show();
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            loginpage form3 = new loginpage();
            form3.Show();
            this.Close();
        }
    }
}
