using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace Client
{
    class Program
    {
        private static ServiceClient serviceClient;
        static string targetDevice = "MyDotnetDevice";
        private readonly static string s_connectionString01 = "HostName=TestApp.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=GxHOnQobv+UEYL+l44JWcAuq6QtFUxdocB6qIpVPX60=";
        private readonly static int _updateInterval = 5000; //ms        
        private static Timer _timer;
        static ConcurrentQueue<string> data = new ConcurrentQueue<string>();

        public static void StartClient()
        {
            byte[] bytes = new byte[1024];
            string IP = "192.168.8.101";
            int PORT = 17880;
            string message;
            string returndata = "temp";
            int byteCount;
            byte[] sendData;

            try
            {
                IPAddress ipAddress = IPAddress.Parse(IP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);
                Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(remoteEP);

                _timer = new Timer(callback: TimerCallBack, state: null, dueTime: _updateInterval, period: _updateInterval);

                while (!returndata.Equals("close"))
                {
                    if (sender.Poll(500000, SelectMode.SelectWrite))
                    {
                        message = "Hi server ..";
                        byteCount = Encoding.ASCII.GetByteCount(message);
                        sendData = new byte[byteCount];
                        sendData = Encoding.ASCII.GetBytes(message);
                        sender.Send(sendData);

                        int bytesRead = sender.Receive(bytes);
                        returndata = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                        data.Enqueue(returndata);
                        Console.WriteLine("Server message: " + returndata);

                        Thread.Sleep(500);

                    }
                }

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                Console.WriteLine("Connection closed ..");
                Console.ReadLine();

            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: ", e);
            }
        }

        private static async void TimerCallBack(object state)
        {   
            if(!data.IsEmpty)
                await SendDeviceToCloudMessageAsync();
        }

        private static async Task SendDeviceToCloudMessageAsync()
        {
            try
            {
                string messageString = "";

                messageString = JsonConvert.SerializeObject(data);

                data.Clear();

                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await serviceClient.SendAsync(targetDevice, message);

                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void Main(string[] args)
        {
            serviceClient = ServiceClient.CreateFromConnectionString(s_connectionString01);
            StartClient();
            Console.ReadLine();
        }
    }
}
