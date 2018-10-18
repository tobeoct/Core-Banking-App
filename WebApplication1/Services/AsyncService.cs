using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication1.Services
{
    public class AsyncService
    {
        private IPAddress ipAddress;
        private int port;

        public AsyncService(string ipAddress,int port)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

        }

        public async void Run()
        {
            TcpListener listener = new TcpListener(this.ipAddress, this.port);
            listener.Start();
          
            while (true)
            {
                try
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    Task t = Process(tcpClient);
                    await t;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private async Task Process(TcpClient tcpClient)
        {
            //            while (true)
            //            {
            //                byte[] receivedBuffer = new byte[2000000];
            //                NetworkStream stream = client.GetStream();
            //                stream.Read(receivedBuffer, 0, receivedBuffer.Length);
            //                StringBuilder msg = new StringBuilder();
            //                //                    string msg = Encoding.ASCII.GetString(receivedBuffer, 0, receivedBuffer.Length);
            //                foreach (byte b in receivedBuffer)
            //                {
            //                    if (b.Equals(59))
            //                    {
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        msg.Append(Convert.ToChar(b).ToString());
            //                    }
            //                }
            //
            //                Console.WriteLine("THE MESSAGE \n" + msg.ToString());
            //                Console.WriteLine("THE LENGTH \n" + msg.Length);
            //            }
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                StreamReader reader = new StreamReader(networkStream);
                StreamWriter writer = new StreamWriter(networkStream);
                writer.AutoFlush = true;
                while (true)
                {
                    string request = await reader.ReadLineAsync();
                    if (request != null)
                    {
//                        Debug.WriteLine("Received service request: " + request);
//                        string response = Response(request);
                        Debug.WriteLine("Computed response is: " + request + "\n");
                        await writer.WriteLineAsync(request);
                    }
                    else
                        break; // Client closed connection
                }
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (tcpClient.Connected)
                    tcpClient.Close();
            }
        }

        private static string Response(string request)
        {
            string[] pairs = request.Split('&');
            string methodName = pairs[0].Split('=')[1];
            string valueString = pairs[1].Split('=')[1];
            string[] values = valueString.Split(' ');
            double[] vals = new double[values.Length];
            for (int i = 0; i < values.Length; ++i)
                vals[i] = double.Parse(values[i]);
            string response = "";
            if (methodName == "average") response += Average(vals);
            else if (methodName == "minimum") response += Minimum(vals);
            else response += "BAD methodName: " + methodName;
            int delay = ((int)vals[0]) * 1000; // Dummy delay
            System.Threading.Thread.Sleep(delay);
            return response;

        }
        private static double Average(double[] vals)
        {
            double sum = 0.0;
            for (int i = 0; i < vals.Length; ++i)
                sum += vals[i];
            return sum / vals.Length;
        }
        private static double Minimum(double[] vals)
        {
            double min = vals[0]; ;
            for (int i = 0; i < vals.Length; ++i)
                if (vals[i] < min) min = vals[i];
            return min;
        }

    }
}