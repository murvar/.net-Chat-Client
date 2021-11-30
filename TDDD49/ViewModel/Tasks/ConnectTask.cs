using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDDD49.ViewModels;
using TDDD49.Model;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace TDDD49.ViewModel.Tasks
{
    internal class ConnectTask
    {
        public ViewModelClient Vmc { get; set; }

        public ConnectTask(ViewModelClient vmc)
        {
            this.Vmc = vmc;
        }

        public void ConnectTaskMethod(ModelClient modelClient)
        {
            Task.Factory.StartNew(() =>
            {
                Debug.WriteLine(" ConnectTask has now started!");
                ConnectMethod(modelClient);
            });
        }
        private void ConnectMethod(ModelClient modelClient)
        {
            try
            {
                Vmc.InformativeConnectBoxActive = false;
                //göm fönser med felmeddelande
                //Debug.WriteLine("hellooooooooooooooo");

                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.
                String server = modelClient.Ip;
                int port = modelClient.Port;

                //IMPLEMENTERA DEFENSIV PROGRAMMERING HÄR

                TcpClient client = new TcpClient(server, port);
                Vmc.ShowConnectionStatusMsg = "Connected";

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(modelClient.Name);

                // Get a client stream for reading and writing.
                //Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                Debug.WriteLine("Sent: {0}", modelClient.Name);

                Debug.WriteLine(client.Connected);

                while (true)
                {
                    

                    //Implementera abnryta connection knapp här

                    // Receive the TcpServer.response.

                    // Buffer to store the response bytes.
                    data = new Byte[256];

                    // String to store the response ASCII representation.
                    String responseData = String.Empty;

                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    //Debug.WriteLine("Received: {0}", responseData);

                }
                stream.Close();
                client.Close();

            }
            catch (ArgumentNullException e)
            {
                Debug.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                //visa fönser med felmeddelande
                Vmc.InformativeConnectBoxActive = true;
                Debug.WriteLine("SocketException: {0}", e);
            }

            Debug.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}
