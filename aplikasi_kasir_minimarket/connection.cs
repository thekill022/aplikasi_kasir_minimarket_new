using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplikasi_kasir_minimarket
{
    internal class connection
    {
        public string connectionString()
        {
            string connectionString = "";

            try
            {
                string localIP = getLocalIp();
                connectionString = $"Data Source={localIP};Initial Catalog=KasirMinimarket;Integrated Security=True";

                return connectionString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return string.Empty;
            }
        }

        public static string getLocalIp()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Tidak ada IP ditemukan");
        }
    }
}
