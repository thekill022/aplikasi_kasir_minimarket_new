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
    public partial class manajemenproduk : Form
    {
        private string connectionStirng = "Data Source=LAPTOP-5DVR7M3S\\GFB_SERVER;Initial Catalog=KasirMinimarket;Integrated Security=True";

        private void loadDataKategori()
        {
            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Kategori";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void loadDataProduk(string nama)
        {
            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM vw_produk WHERE nama_produk LIKE @produk";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@produk", "%" + nama + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView2.AutoGenerateColumns = true;
                    dataGridView2.DataSource = dt;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EnsureIndexes()
        {
            using (var conn = new SqlConnection(connectionStirng)) {
                conn.Open();
                var indexScript = @"
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_namaproduk' AND object_id = OBJECT_ID('dbo.produk'))
                    BEGIN
                        CREATE NONCLUSTERED INDEX idx_namaproduk ON dbo.produk(nama_produk)
                    END;
                    ";
                using(var cmd = new SqlCommand(indexScript, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //ini lebih ke untuk ngeliat hasil statistiknya aja
        private void AnalyzeQuery(string sqlQuery)
        {
            using (var conn = new SqlConnection(connectionStirng))
            {
                conn.InfoMessage += (s, e) => MessageBox.Show(e.Message, "STATISTICS INFO");
                conn.Open();
                var wrapped = $@"
                SET STATISTICS IO ON;
                SET STATISTICS TIME ON;
                {sqlQuery};
                SET STATISTICS IO OFF;
                SET STATISTICS TIME OFF;";

                using (var cmd = new SqlCommand(wrapped, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void clearForm()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }

        private string namaAdmin;
        private string username;
        public manajemenproduk(string namaAdmin, string username)
        {
            InitializeComponent();
            this.namaAdmin = namaAdmin;
            this.username = username;

            //ini optimasi querynya
            EnsureIndexes();
            loadDataKategori();
            loadDataProduk("");
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                loadDataProduk("");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (textBox5.Text != "" && textBox5.Text != null)
            {
                MessageBox.Show("Data Already Exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (textBox1.Text != null && textBox1.Text.Trim() != "" && textBox2.Text != null && textBox2.Text.Trim() != "" && textBox3.Text != null && textBox3.Text.Trim() != "" && textBox4.Text != null && textBox4.Text.Trim() != "")
            {
                using (SqlConnection conn = new SqlConnection(connectionStirng))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        conn.Open();
                        transaction = conn.BeginTransaction();
                        string query = "add_produk";
                        using (SqlCommand cmd = new SqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction,
                            CommandText = query
                        })
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@nama", textBox4.Text.Trim());
                            cmd.Parameters.AddWithValue("@kategori", textBox1.Text.Trim());
                            cmd.Parameters.AddWithValue("@harga", textBox2.Text.Trim());
                            cmd.Parameters.AddWithValue("@stok", textBox3.Text.Trim());

                            object status = cmd.ExecuteScalar();
                            if (status.ToString() == "Berhasil Menambahkan Data Produk")
                            {
                                transaction.Commit();
                                MessageBox.Show("Data Berhasil Ditambahkan", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearForm();
                                loadDataProduk("");
                            }
                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show("Data Gagal Ditambahkan", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        MessageBox.Show("error : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        clearForm();
                    }
                }
            }
            else
            {
                MessageBox.Show("Input tidak boleh kosong", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text.Trim() != "" && textBox2.Text != null && textBox2.Text.Trim() != "" && textBox3.Text != null && textBox3.Text.Trim() != "" && textBox4.Text != null && textBox4.Text.Trim() != "" && textBox5.Text != null && textBox5.Text.Trim() != "")
            {
                using (SqlConnection conn = new SqlConnection(connectionStirng))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        conn.Open();
                        transaction = conn.BeginTransaction();
                        string query = "update_produk";
                        using (SqlCommand cmd = new SqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction,
                            CommandText = query
                        })
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@nama", textBox4.Text.Trim());
                            cmd.Parameters.AddWithValue("@kategori", textBox1.Text.Trim());
                            cmd.Parameters.AddWithValue("@harga", textBox2.Text.Trim());
                            cmd.Parameters.AddWithValue("@stok", textBox3.Text.Trim());
                            cmd.Parameters.AddWithValue("@id", textBox5.Text.Trim());

                            object status = cmd.ExecuteScalar();
                            if (status.ToString() == "Berhasil Mengupdate Data Produk")
                            {
                                transaction.Commit();
                                MessageBox.Show("Data Berhasil update", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearForm();
                                loadDataProduk("");
                            }
                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show("Data Gagal Diupdate", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        MessageBox.Show("error : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        clearForm();
                    }
                }
            }
            else
            {
                MessageBox.Show("Input tidak boleh kosong", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text.Trim() != "")
            {
                using (SqlConnection conn = new SqlConnection(connectionStirng))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        conn.Open();
                        transaction = conn.BeginTransaction();
                        string query = "delete_produk";
                        using (SqlCommand cmd = new SqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction,
                            CommandText = query
                        })
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id", textBox5.Text);

                            object status = cmd.ExecuteScalar();
                            if (status.ToString() == "Berhasil Menghapus Data Produk")
                            {
                                transaction.Commit();
                                MessageBox.Show("Data Berhasil dihapus", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearForm();
                                loadDataProduk("");
                            }
                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show("Data Gagal Dihapus", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        MessageBox.Show("error : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        clearForm();
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data terlebih dahulu", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (textBox6.Text != null && textBox6.Text.Trim() != "")
            {
                loadDataProduk(textBox6.Text);
                clearForm();
            }
            else
            {
                MessageBox.Show("Input tidak boleh kosong", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox1.Text = row.Cells[1].Value.ToString();
            }
        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                textBox5.Text = row.Cells[0].Value.ToString();
                textBox4.Text = row.Cells[1].Value.ToString();
                textBox1.Text = row.Cells[2].Value.ToString();
                textBox2.Text = Convert.ToDecimal(row.Cells[3].Value).ToString("0");
                textBox3.Text = row.Cells[4].Value.ToString();
            }
        }

        private void label9_Click_1(object sender, EventArgs e)
        {
            adminpage admin = new adminpage(namaAdmin, username, "admin");
            admin.Show();
            this.Close();
        }
    }
}
