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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace aplikasi_kasir_minimarket
{
    public partial class riwayattransaksi : Form
    {

        connection kn = new connection();
        string connectionStirng = "";
        private void loadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Riwayat_Transaksi";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Terjadi kesalahan saat memuat data transaksi.\n\n" +
                        "Pesan: " + ex.Message +
                        "\n\nPastikan koneksi database sudah benar dan coba lagi.",
                        "Kesalahan Memuat Data",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    adminpage form = new adminpage(nama, user, "admin");
                    form.Show();
                    this.Hide();
                }

            }
        }

        private void loadDetail(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionStirng))
            {
                try
                {
                    conn.Open();
                    string query = "load_detail";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    dataGridView2.AutoGenerateColumns = true;
                    dataGridView2.DataSource = dt;
                }
                catch (Exception ex) {
                    MessageBox.Show("Kesalahan dalam mengoneksikan ke database. Pastikan terhubung ke jaringan yang sama.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EnsureIndexes()
        {
            using (var conn = new SqlConnection(connectionStirng))
            {
                conn.Open();
                var indexScript = @"
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DetailTransaksi_id_transaksi' AND object_id = OBJECT_ID('dbo.Detail_Transaksi'))
                    BEGIN
                        CREATE NONCLUSTERED INDEX IX_DetailTransaksi_id_transaksi ON dbo.Detail_Transaksi(id_transaksi);
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
        }

        private string nama;
        private string user;
        public riwayattransaksi(string nama, string user)
        {
            InitializeComponent();
            connectionStirng = kn.connectionString();
            this.nama = nama;
            this.user = user;

            EnsureIndexes();
            loadData();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox1.Text = row.Cells[0].Value.ToString();
            }
        }

        private void label3_Click_1(object sender, EventArgs e)
        {
            adminpage admin = new adminpage(nama, user, "admin");
            admin.Show();
            this.Hide();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text.Trim() != "")
            {
                loadDetail(int.Parse(textBox1.Text));
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text.Trim() != "")
            {
                SqlTransaction transaction = null;
                using (SqlConnection conn = new SqlConnection(connectionStirng))
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    using (SqlCommand setContextCmd = new SqlCommand("EXEC sp_set_session_context @key, @value", conn, transaction))
                    {
                        setContextCmd.Parameters.AddWithValue("@key", "nama_admin");
                        setContextCmd.Parameters.AddWithValue("@value", this.nama);
                        setContextCmd.ExecuteNonQuery();
                    }

                    string query = "delete_riwayat";
                    SqlCommand cmd = new SqlCommand
                    {
                        Connection = conn,
                        Transaction = transaction
                    };
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", textBox1.Text);

                    object status = cmd.ExecuteScalar();
                    if (status.ToString() == "Transaksi berhasil dihapus")
                    {
                        transaction.Commit();
                        MessageBox.Show("Data Berhasil hapus", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearForm();
                        loadData();
                        dataGridView2.DataSource = null;
                    }
                    else
                    {
                        transaction.Rollback();
                        MessageBox.Show("Data Gagal Dihapus", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


            }
            else
            {
                MessageBox.Show("Pilih data terlebih dahulu", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void riwayattransaksi_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}