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
using System.Threading;

namespace TDDD49.ViewModel.Tasks
{
    internal class ListeningTask
    {
        public ViewModelClient Vmc { get; set; }

        private TcpListener server;
        public TcpListener Server
        {
            set { server = value; }
            get { return server; }
        }

        public static List<TcpClient> client = new List<TcpClient>();
        /**
        public List<TcpClient> Client 
        { 
            set { client = value; } 
            get { return client; }
        }

        */

        public ListeningTask(ViewModelClient vmc)
        {
            this.Vmc = vmc;
            //this.client = new List<TcpClient>();
        }

        public void ListeningTaskMethod(int port)
        {

            Task.Factory.StartNew(() =>
            {
                ListeningMethod(port);

            });

        }

        private async void ListeningMethod(int portListen)
        {

            try
            {
                //Debug.WriteLine("Running listening");
                // Set the TcpListener on port 13000.
                int port = portListen;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                Server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                Server.Start();

                // Enter the listening loop.
                while (true)
                {
                    var something = await Server.AcceptTcpClientAsync();

                    client.Add(something);
                    
                    Debug.WriteLine("Connected!");
                    Vmc.PopUpActive = true;

                }

            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                // Debug.WriteLine("server stopping");
                // server.Stop();
            }

            //Debug.WriteLine("\nHit enter to continue...");
            //Console.Read();
        }

        public void AcceptConnection()
        {

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            // Get a stream object for reading and writing
            if (!client.Any()) {
                throw new Exception("fel");
            }
            NetworkStream stream = client[0].GetStream();
            int i;
            //while Answer {}
            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                //Debug.WriteLine("Received: {0}", data);

                // Process the data sent by the client.
                data = data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
                //Debug.WriteLine("Sent: {0}", data);
            }
            CloseClient();
        }

        public void DenyConnection()
        {
            CloseClient();
        }

        private void CloseClient()
        {
            client[0].Close();
            client.RemoveAt(0);
        }


    }
}
