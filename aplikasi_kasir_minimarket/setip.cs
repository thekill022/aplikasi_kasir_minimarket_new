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
    public partial class setip : Form
    {
        public setip()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != "")
            {

                connection kn = new connection();
                kn.ipAddress = textBox1.Text.Trim();

                if (kn.isValid())
                {  
                   welcomepage form = new welcomepage();
                   form.Show();
                    this.Hide();
                } else
                {
                    MessageBox.Show("Koneksi gagal, silakan periksa alamat IP dan coba lagi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }

            } else
            {
                MessageBox.Show("Harap isi alamat ip dengan valid", "Error", MessageBoxButtons.OK);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            TextBox tb = sender as TextBox;

            if (tb.Text == "" && e.KeyChar == '.')
            {
                e.Handled = true;
            }
        }
    }
}
