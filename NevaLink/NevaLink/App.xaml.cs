using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;
namespace NevaLink
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Authorisation());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }

    public class serverCon
    {
        public static int port = 7000;
        public static string address = "192.168.0.105";
    }

    public class ServerApi
    {
        public static string[][] tableFunc(string sql)
        {
            try
            {
                TcpClient client = null;
                //определяем сервер для отправки сообщений
                client = new TcpClient(serverCon.address, serverCon.port);

                string message = sql;
                NetworkStream stream = client.GetStream();

                // отправляем сообщение
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(message);
                writer.Flush();


                BinaryReader reader = new BinaryReader(stream);
                string res = "z|cNo";

                try
                {
                    res = "z|c" + reader.ReadString();
                }
                catch
                {
                    res = "z|cNo";
                }



                if (res != "z|cNo")
                {
                    string[] results = res.Split('^');
                    string[][] returns = new string[results.Length][];

                    for (int i = 0; i < results.Length; i++)
                    {
                        string[] results2 = results[i].Split('|');

                        returns[i] = new string[results2.Length];

                        for (int j = 0; j < results2.Length; j++)
                        {
                            returns[i][j] = results2[j];
                        }
                    }

                    reader.Close();
                    writer.Close();
                    stream.Close();

                    return returns;
                }
                else
                {
                    string[][] returns = new string[1][];
                    returns[0] = new string[1];
                    returns[0][0] = "No";

                    reader.Close();
                    writer.Close();
                    stream.Close();

                    return returns;
                }
            }
            catch
            {
                string[][] returns = new string[1][];
                returns[0] = new string[1];
                returns[0][0] = "No";
                return returns;
            }
        }
    }
    
    public class security
    {
        public static int ID;
        public static string Family;
        public static string Name;
        public static string MiddleName;
        public static string Date;

        public static int rate;
        public static int rate_price;
        public static int home;

        public static string homeDescription;
        public static string rateName;
        public static string rateDescription;

        public static string selRateID;
        public static string selRateName;
        public static string selRateDescription;
        public static string selRatePrice;

        public static string getHash(string text)
        {  
            using (var sha256 = SHA256.Create())
            {
                //получение хеша от полученной строки
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
