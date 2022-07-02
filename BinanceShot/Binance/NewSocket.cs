using BinanceShot.ConnectDB;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BinanceShot.Binance
{
    public static class NewSocket
    {
        public static Socket Add(string name)
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "clients");
            string json = File.ReadAllText(path + "\\" + name);
            Client client = JsonConvert.DeserializeObject<Client>(json);
            if (ConnectTrial.Check(client.ClientName))
            {
                return new Socket(client.ApiKey, client.SecretKey);
            }
            return null;
        }
    }
}
