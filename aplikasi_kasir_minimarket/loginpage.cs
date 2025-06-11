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

        private string connectionStirng = "Data Source=LAPTOP-5DVR7M3S\\GFB_SERVER;Initial Catalog=KasirMinimarket;Integrated Security=True";
        public loginpage()
        {
            InitializeComponent();
        }

        private void label5_Click_1(object sender, EventArgs e)
        {
            welcomepage form1 = new welcomepage();
            form1.Show();
            this.Close();
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

                                        string getData = "SELECT nama FROM UserMinimart WHERE username=@username AND password=@password";

                                        MessageBox.Show("Login Berhasil : Welcome mas/mbak", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        if (myRole.ToString() == "admin")
                                        {
                                            using (SqlCommand cmdData = new SqlCommand(getData, conn))
                                            {
                                                cmdData.Parameters.AddWithValue("@username", textBox1.Text.Trim());
                                                cmdData.Parameters.AddWithValue("@password", textBox2.Text.Trim());

                                                SqlDataReader reader = cmdData.ExecuteReader();

                                                if (reader.Read())
                                                {
                                                    string namaUser = reader["nama"].ToString();
                                                    admin = new adminpage(namaUser, textBox1.Text.Trim(), "admin");
                                                    admin.Show();
                                                    this.Close();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            using (SqlCommand cmdData = new SqlCommand(getData, conn))
                                            {
                                                cmdData.Parameters.AddWithValue("@username", textBox1.Text.Trim());
                                                cmdData.Parameters.AddWithValue("@password", textBox2.Text.Trim());

                                                SqlDataReader reader = cmdData.ExecuteReader();

                                                if (reader.Read())
                                                {
                                                    string namaUser = reader["nama"].ToString();
                                                    kasir = new kasirpage(namaUser, textBox1.Text.Trim(), "kasir");
                                                    kasir.Show();
                                                    this.Close();
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
                        MessageBox.Show("Error : " + ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Username dan Password tidak boleh kosong", "Value Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}