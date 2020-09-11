using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Azure.Devices.Client;

namespace SignalRChart.Models
{
    public class ChartData
    {
        private readonly static Lazy<ChartData> _instance = new Lazy<ChartData>(() => new ChartData());
        private static DeviceClient s_deviceClient;
        private readonly static string s_connectionString01 = "HostName=TestApp.azure-devices.net;DeviceId=MyDotnetDevice;SharedAccessKey=+ugIncKyBuElCY3TqfH6hlWR6McV1MoZrJ9cfF9C5hA=";

        private ChartData()
        {

        }

        public static ChartData Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private static async Task ReceiveC2dAsync()
        {
            while (true)
            {
                Message receivedMessage = await s_deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                var message = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                var messageArray = message.Replace(']', ',').Replace('[', ',').Replace('"', ',').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var intArray = Array.ConvertAll(messageArray, int.Parse);

                GetClients().updateData(intArray);

                await s_deviceClient.CompleteAsync(receivedMessage);
            }
        }

        public void initData()
        {
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString01, TransportType.Mqtt);
            ReceiveC2dAsync();
        }

        private static dynamic GetClients()
        {
            return GlobalHost.ConnectionManager.GetHubContext<ChartHub>().Clients.All;
        }
    }
}