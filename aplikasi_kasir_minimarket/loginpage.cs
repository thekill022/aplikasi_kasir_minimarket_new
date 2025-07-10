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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace aplikasi_kasir_minimarket
{
    public partial class loginpage : Form
    {
        adminpage admin;
        kasirpage kasir;

        connection kn = new connection();
        string connectionStirng = "";
        public loginpage()
        {
            InitializeComponent();
            connectionStirng = kn.connectionString();
        }

        private void label5_Click_1(object sender, EventArgs e)
        {
            welcomepage form1 = new welcomepage();
            form1.Show();
            this.Hide();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text.Trim() != "" && textBox2.Text != null && textBox2.Text.Trim() != "")
            {
                using (SqlConnection conn = new SqlConnection(connectionStirng))
                {
                    try
                    {
                        conn.Open();
                        string query = "EXEC ps_login @username, @password";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@username", textBox1.Text.Trim());
                            cmd.Parameters.AddWithValue("@password", textBox2.Text.Trim());

                            object result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                string getRole = "EXEC ps_role @username, @password";
                                if (result.ToString() == "Berhasil")
                                {
                                    using (SqlCommand role = new SqlCommand(getRole, conn))
                                    {
                                        role.Parameters.AddWithValue("@username", textBox1.Text.Trim());
                                        role.Parameters.AddWithValue("@password", textBox2.Text.Trim());

                                        object myRole = role.ExecuteScalar();

                                        string getData = "get_name";

                                        MessageBox.Show("Login Berhasil : Welcome mas/mbak", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        if (myRole.ToString() == "admin")
                                        {
                                            using (SqlCommand cmdData = new SqlCommand(getData, conn))
                                            {
                                                cmdData.CommandType = CommandType.StoredProcedure;
                                                cmdData.Parameters.AddWithValue("@username", textBox1.Text.Trim());
                                                cmdData.Parameters.AddWithValue("@password", textBox2.Text.Trim());

                                                SqlDataReader reader = cmdData.ExecuteReader();

                                                if (reader.Read())
                                                {
                                                    string namaUser = reader["nama"].ToString();
                                                    admin = new adminpage(namaUser, textBox1.Text.Trim(), "admin");
                                                    admin.Show();
                                                    this.Hide();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            using (SqlCommand cmdData = new SqlCommand(getData, conn))
                                            {
                                                cmdData.CommandType = CommandType.StoredProcedure;
                                                cmdData.Parameters.AddWithValue("@username", textBox1.Text.Trim());
                                                cmdData.Parameters.AddWithValue("@password", textBox2.Text.Trim());

                                                SqlDataReader reader = cmdData.ExecuteReader();

                                                if (reader.Read())
                                                {
                                                    string namaUser = reader.GetString(0);
                                                    kasir = new kasirpage(namaUser, textBox1.Text.Trim(), "kasir");
                                                    kasir.Show();
                                                    this.Hide();
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Login Gagal : Username atau Password salah", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Kegagalan melakukan koneksi ke database. Pastikan menggunakan jaringan yang sama.", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Username dan Password tidak boleh kosong", "Value Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void loginpage_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}