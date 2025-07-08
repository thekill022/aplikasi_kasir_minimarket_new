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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace aplikasi_kasir_minimarket
{
    public partial class manajemenkategori : Form
    {
        connection kn = new connection();
        string connectionStirng = "";

        private void loadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                try
                {

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

        private void EnsureIndexes()
        {
            using (var conn = new SqlConnection(connectionStirng))
            {
                conn.Open();
                var indexScript = @"
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_namakategori' AND object_id = OBJECT_ID('dbo.Kategori'))
                    BEGIN
                        CREATE NONCLUSTERED INDEX idx_namakategori ON dbo.Kategori(nama_kategori);
                    END;
                    ";
                using (var cmd = new SqlCommand(indexScript, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void clearForm()
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
            }
        }

        private string namaAdmin;
        private string username;
        public manajemenkategori(string namaAdmin, string username)
        {
            InitializeComponent();
            connectionStirng = kn.connectionString();
            this.namaAdmin = namaAdmin;
            this.username= username;

            EnsureIndexes();
            loadData();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                MessageBox.Show("Data Already Exist", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (textBox2.Text != null && textBox2.Text.Trim() != "")
            {
                using (SqlConnection conn = new SqlConnection(connectionStirng))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        conn.Open();
                        transaction = conn.BeginTransaction();
                        string query = "add_kategori";

                        using (SqlCommand cmd = new SqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction,
                            CommandText = query
                        })
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@nama", textBox2.Text.Trim());

                            object status = cmd.ExecuteScalar();
                            if (status.ToString() == "Berhasil Menambahkan Data")
                            {
                                transaction.Commit();
                                MessageBox.Show("Data Berhasil Ditambahkan", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearForm();
                                loadData();
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
            if (textBox1.Text != null && textBox1.Text.Trim() != "" && textBox2.Text != null && textBox2.Text.Trim() != "")
            {
                using (SqlConnection conn = new SqlConnection(connectionStirng))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        conn.Open();
                        transaction = conn.BeginTransaction();
                        string query = "update_kategori";
                        using (SqlCommand cmd = new SqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction,
                            CommandText = query
                        })
                        {

                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@nama", textBox2.Text);
                            cmd.Parameters.AddWithValue("@id", textBox1.Text);

                            object status = cmd.ExecuteScalar();
                            if (status.ToString() == "Berhasil Mengubah Data")
                            {
                                transaction.Commit();
                                MessageBox.Show("Data Berhasil update", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearForm();
                                loadData();
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
                        string query = "delete_kategori";
                        using (SqlCommand cmd = new SqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction,
                            CommandText = query
                        })
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id", textBox1.Text);

                            object status = cmd.ExecuteScalar();
                            if (status.ToString() == "Berhasil Menghapus Data")
                            {
                                transaction.Commit();
                                MessageBox.Show("Data Berhasil hapus", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearForm();
                                loadData();
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

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
            }
        }

        private void label4_Click_1(object sender, EventArgs e)
        {
            adminpage admin = new adminpage(namaAdmin, username, "admin");
            admin.Show();
            this.Close();
        }
    }
}
