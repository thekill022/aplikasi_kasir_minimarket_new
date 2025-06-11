using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System;


namespace aplikasi_kasir_minimarket
{

    public partial class welcomepage : Form
    {
        public welcomepage()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            loginpage form3 = new loginpage();
            form3.Show();
            this.Hide();
        }
    }
}

