using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aplikasi_kasir_minimarket
{
    internal class connection
    {
        public string ipAddress ="";
        public string connectionString()
        {
            string connectionString = "";

            try
            {
                connectionString = $"Data Source={ipAddress};Initial Catalog=KasirMinimarket;User ID=sa;Password=123";

                return connectionString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return string.Empty;
            }
        }

        public bool isValid()
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString());
                conn.Open();
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Connection failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
