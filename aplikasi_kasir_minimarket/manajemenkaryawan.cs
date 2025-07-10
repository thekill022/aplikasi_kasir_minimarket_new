using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace aplikasi_kasir_minimarket
{
    public partial class manajemenkaryawan : Form
    {
        connection kn = new connection();
        string connectionStirng = "";
        private string namaAdmin;
        private string username;
        private bool IsValidNama(string nama)
        {
            // hanya huruf A-Z atau a-z dan spasi
            return System.Text.RegularExpressions.Regex.IsMatch(nama, @"^[a-zA-Z\s]+$");
        }

        private bool IsValidUsername(string username)
        {
            // hanya huruf dan angka
            return System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9]+$");
        }

        private void clearForm()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void loadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                try
                {

                    string query = "SELECT * FROM vw_karyawan";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;

                }
                catch (Exception ex)
                {


                    MessageBox.Show(
            "Terjadi masalah saat memproses data\n." +
            "Silakan coba lagi atau hubungi admin jika masalah terus berlanjut.",
            "Kesalahan Sistem",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
            );
                    adminpage form = new adminpage(namaAdmin, username, "admin");
                    form.Show();
                    this.Hide();
                }

            }
        }

        private void EnsureIndexes()
        {
            using (var conn = new SqlConnection(connectionStirng))
            {
                conn.Open();
                var indexScript = @"
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_namakaryawan' AND object_id = OBJECT_ID('dbo.UserMinimart'))
                    BEGIN
                        CREATE NONCLUSTERED INDEX idx_namakaryawan ON dbo.UserMinimart(nama);
                    END;
                    ";
                using (var cmd = new SqlCommand(indexScript, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public manajemenkaryawan(string namaAdmin, string username)
        {
            InitializeComponent();
            connectionStirng = kn.connectionString();
            this.namaAdmin = namaAdmin;
            this.username = username;

            EnsureIndexes();
            loadData();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label6_Click_1(object sender, EventArgs e)
        {
            adminpage admin = new adminpage(namaAdmin, username, "admin");
            admin.Show();
            this.Hide();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                SqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    string session_query = "sp_set_session_context";

                    SqlCommand cmd = new SqlCommand
                    {
                        Connection = conn,
                        Transaction = transaction
                    };
                    cmd.CommandText = session_query;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@key", "nama_admin");
                    cmd.Parameters.AddWithValue("@value", namaAdmin);

                    cmd.ExecuteNonQuery();

                    if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text))
                    {
                        MessageBox.Show("Semua field wajib diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!IsValidNama(textBox1.Text))
                    {
                        MessageBox.Show("Nama hanya boleh mengandung huruf dan spasi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!IsValidUsername(textBox2.Text))
                    {
                        MessageBox.Show("Username hanya boleh mengandung huruf dan angka tanpa spasi atau simbol.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    {
                        using (SqlCommand cmdadd = new SqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction,
                            CommandText = "add_karyawan"
                        })
                        {
                            cmdadd.CommandType = CommandType.StoredProcedure;

                            cmdadd.Parameters.AddWithValue("@nama", textBox1.Text.Trim());
                            cmdadd.Parameters.AddWithValue("@username", textBox2.Text.Trim());
                            cmdadd.Parameters.AddWithValue("@password", textBox3.Text.Trim());
                            cmdadd.Parameters.AddWithValue("@role", "kasir");

                            object status = cmdadd.ExecuteScalar();
                            if (status.ToString() == "Berhasil Menambahkan Data")
                            {
                                transaction.Commit();
                                MessageBox.Show(status.ToString(), "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearForm();
                                loadData();
                            }
                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show(status.ToString(), "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                clearForm();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    MessageBox.Show(
                        "Terjadi masalah saat memproses data.\n" +
                        "Silakan coba lagi atau hubungi admin jika masalah terus berlanjut.",
                        "Kesalahan Sistem",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    clearForm();
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
    string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Nama dan Username wajib diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidNama(textBox1.Text))
            {
                MessageBox.Show("Nama hanya boleh mengandung huruf dan spasi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidUsername(textBox2.Text))
            {
                MessageBox.Show("Username hanya boleh mengandung huruf dan angka tanpa spasi atau simbol.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            {
                using (SqlConnection conn = new SqlConnection(connectionStirng))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        conn.Open();
                        transaction = conn.BeginTransaction();
                        string session_query = "sp_set_session_context";

                        SqlCommand cmd = new SqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction
                        };
                        cmd.CommandText = session_query;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@key", "nama_admin");
                        cmd.Parameters.AddWithValue("@value", namaAdmin);

                        cmd.ExecuteNonQuery();

                        string query = "update_karyawan";
                        using (SqlCommand cmdupdate = new SqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction,
                            CommandText = query
                        })
                        {
                            cmdupdate.CommandType = CommandType.StoredProcedure;
                            cmdupdate.Parameters.AddWithValue("@nama", textBox1.Text.Trim());
                            cmdupdate.Parameters.AddWithValue("@username", textBox2.Text.Trim());
                            cmdupdate.Parameters.AddWithValue("@password", textBox3.Text.Trim());
                            cmdupdate.Parameters.AddWithValue("@id", textBox4.Text.Trim());

                            object status = cmdupdate.ExecuteScalar();
                            if (status.ToString() == "Berhasil Mengubah Data")
                            {
                                transaction.Commit();
                                MessageBox.Show(status.ToString(), "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearForm();
                                loadData();
                            }
                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show(status.ToString(), "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                clearForm();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        MessageBox.Show(
                            "Terjadi kesalahan saat mengubah data karyawan.\nPastikan data sudah benar dan coba lagi." +
                            "Silakan coba lagi atau hubungi admin jika masalah terus berlanjut.",
                            "Kesalahan Sistem",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );

                        clearForm();
                    }

                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Apakah yakin ingin menonaktifkan akun?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
            {
                if (textBox4.Text != null && textBox4.Text.Trim() != "")
                {
                    using (SqlConnection conn = new SqlConnection(connectionStirng))
                    {
                        SqlTransaction transaction = null;
                        try
                        {
                            conn.Open();
                            transaction = conn.BeginTransaction();
                            string session_query = "sp_set_session_context";

                            SqlCommand cmd = new SqlCommand
                            {
                                Connection = conn,
                                Transaction = transaction
                            };

                            cmd.CommandText = session_query;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@key", "nama_admin");
                            cmd.Parameters.AddWithValue("@value", namaAdmin);

                            cmd.ExecuteNonQuery();

                            string query = "nonaktifkan_karyawan";
                            using (SqlCommand cmdnonaktif = new SqlCommand
                            {
                                Connection = conn,
                                Transaction = transaction,
                                CommandText = query
                            })
                            {
                                cmdnonaktif.CommandType = CommandType.StoredProcedure;
                                cmdnonaktif.Parameters.AddWithValue("@id", textBox4.Text.Trim());

                                object status = cmdnonaktif.ExecuteScalar();
                                if (status.ToString() == "Akun Berhasil Dinonaktifkan")
                                {
                                    transaction.Commit();
                                    MessageBox.Show(status.ToString(), "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    clearForm();
                                    loadData();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    MessageBox.Show(status.ToString(), "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    clearForm();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction?.Rollback();
                            MessageBox.Show(
                                "Gagal menonaktifkan akun.\nPastikan akun yang dipilih benar." +
                                "Silakan coba lagi atau hubungi admin jika masalah terus berlanjut.",
                                "Kesalahan Sistem",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                        

                            clearForm();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Pilih akun yang ingin dinonaktifkan terlebih dahulu", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Apakah yakin ingin mengaktifkan akun?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
            {
                if (textBox4.Text != null && textBox4.Text.Trim() != "")
                {
                    using (SqlConnection conn = new SqlConnection(connectionStirng))
                    {
                        SqlTransaction transaction = null;
                        try
                        {
                            conn.Open();
                            transaction = conn.BeginTransaction();
                            string session_query = "sp_set_session_context";

                            SqlCommand cmd = new SqlCommand
                            {
                                Connection = conn,
                                Transaction = transaction
                            };
                            cmd.CommandText = session_query;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@key", "nama_admin");
                            cmd.Parameters.AddWithValue("@value", namaAdmin);

                            cmd.ExecuteNonQuery();

                            string query = "aktifkan_karyawan";
                            using (SqlCommand cmdaktif = new SqlCommand
                            {
                                Connection = conn,
                                Transaction = transaction,
                                CommandText = query
                            })
                            {
                                cmdaktif.CommandType = CommandType.StoredProcedure;
                                cmdaktif.Parameters.AddWithValue("@id", textBox4.Text.Trim());

                                object status = cmdaktif.ExecuteScalar();
                                if (status.ToString() == "Akun Berhasil Diaktifkan")
                                {
                                    transaction.Commit();
                                    MessageBox.Show(status.ToString(), "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    clearForm();
                                    loadData();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    MessageBox.Show(status.ToString(), "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    clearForm();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction?.Rollback();
                            MessageBox.Show(
                                "Gagal mengaktifkan akun.\nSilakan coba lagi nanti." +
                                "Silakan coba lagi atau hubungi admin jika masalah terus berlanjut.",
                                "Kesalahan Sistem",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );

                            clearForm();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Pilih akun  yang ingin diaktifkan terlebih dahulu", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox4.Text = row.Cells[0].Value.ToString();
                textBox1.Text = row.Cells[1].Value.ToString();
                textBox2.Text = row.Cells[2].Value.ToString();
            }
        }

        private void manajemenkaryawan_Load(object sender, EventArgs e)
        {

        }

        private void manajemenkaryawan_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
