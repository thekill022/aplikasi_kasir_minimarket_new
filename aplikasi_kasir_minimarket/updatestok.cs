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
    public partial class updatestok : Form
    {
        connection kn = new connection();
        string connectionStirng = "";
        private void loadDataProduk(string nama)
        {

            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT nama_produk, stok FROM Produk WHERE nama_produk LIKE @produk";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@produk", "%" + nama + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kesalahan dalam mengoneksikan ke database. Pastikan terhubung ke jaringan yang sama.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    adminpage form = new adminpage(nama, username, "admin");
                    form.Show();
                    this.Hide();
                }
            }
        }

        private void clearForm()
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private string nama;
        private string username;
        public updatestok(string nama, string username)
        {
            InitializeComponent();
            connectionStirng = kn.connectionString();
            this.nama = nama;
            this.username = username;
            loadDataProduk("");
        }

        private void label4_Click_1(object sender, EventArgs e)
        {
            kasirpage kasir = new kasirpage(nama, username, "kasir");
            kasir.Show();
            this.Hide();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                try
                {

                    conn.Open();

                    using (SqlCommand setContextCmd = new SqlCommand("EXEC sp_set_session_context @key, @value", conn))
                    {
                        setContextCmd.Parameters.AddWithValue("@key", "nama_admin");
                        setContextCmd.Parameters.AddWithValue("@value", this.nama);
                        setContextCmd.ExecuteNonQuery();
                    }

                    string query = "UPDATE Produk SET stok = (select stok from Produk where nama_produk=@nama) + @stok WHERE nama_produk=@nama";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nama", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@stok", textBox2.Text.Trim());

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Data Berhasil update", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clearForm();
                            loadDataProduk("");
                        }
                        else
                        {
                            MessageBox.Show("Data Gagal Diupdate", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("error : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    clearForm();
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            loadDataProduk(textBox3.Text);
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox1.Text = row.Cells[0].Value.ToString();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

            TextBox tb = sender as TextBox;

            if (tb.Text == "" && e.KeyChar == '0')
            {
                e.Handled = true;
            }
        }

        private void updatestok_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

    }
}
