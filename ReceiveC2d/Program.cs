using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;

namespace ReceiveC2d
{
    class Program
    {
        private static DeviceClient s_deviceClient;
        private readonly static string s_connectionString01 = "HostName=TestApp.azure-devices.net;DeviceId=MyDotnetDevice;SharedAccessKey=+ugIncKyBuElCY3TqfH6hlWR6McV1MoZrJ9cfF9C5hA=";

        private static async void ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await s_deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                var message = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                var messageArray = message.Replace(']', ',').Replace('[', ',').Replace('"', ',').Split(',', StringSplitOptions.RemoveEmptyEntries);
                var intArray = Array.ConvertAll(messageArray, int.Parse);
                for (int i=0; i < intArray.Length; i++)
                {
                    Console.WriteLine(intArray[i]);
                }
                Console.ResetColor();

                await s_deviceClient.CompleteAsync(receivedMessage);
            }
        }

        static void Main(string[] args)
        {
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString01, TransportType.Mqtt);
            ReceiveC2dAsync();
            Console.ReadLine();
        }
    }
}
