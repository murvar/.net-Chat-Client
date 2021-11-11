using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDDD49.Model;
using TDDD49.ViewModel.Commands;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TDDD49.ViewModels
{
    public class ViewModelClient : INotifyPropertyChanged
    {

        public ClientFetchCommand ClientFetchCommand { get; set; }
        public ListeningCommand ListeningCommand { get; set; }
        private ModelClient modelClient;

        public string Name
        {
            get { return modelClient.Name; }
            set
            {
                System.Diagnostics.Debug.WriteLine(value);
                modelClient.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Ip
        {
            get { return modelClient.Ip; }
            set
            {
                System.Diagnostics.Debug.WriteLine(value);
                modelClient.Ip = value;
                OnPropertyChanged("Ip");
            }
        }

        public int Port
        {
            get { return modelClient.Port; }
            set
            {
                System.Diagnostics.Debug.WriteLine(value);
                modelClient.Port = value;
                OnPropertyChanged("Port");
            }
        }

        public int ListeningPort
        {
            get { return modelClient.ListeningPort; }
            set
            {
                modelClient.ListeningPort = value;
                System.Diagnostics.Debug.WriteLine(value);
                //listeningPort = value;
                OnPropertyChanged("ListeningPort");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ViewModelClient()
        {
            modelClient = new ModelClient();
            modelClient.Ip = "127.0.0.1";
            modelClient.Port = 5001;
            modelClient.Name = "Robin";
            this.ClientFetchCommand = new ClientFetchCommand(this);
            this.ListeningCommand = new ListeningCommand(this);


        }

        public void ClientFetchMethod()
        {
            try
            {
                //Debug.WriteLine("hellooooooooooooooo");

                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.
                String server = modelClient.Ip;
                int port = modelClient.Port;

                //IMPLEMENTERA DEFENSIV PROGRAMMERING HÄR

                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(modelClient.Name);

                // Get a client stream for reading and writing.
                //Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                Debug.WriteLine("Sent: {0}", modelClient.Name);

                Debug.WriteLine(client.Connected);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Debug.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Debug.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException: {0}", e);
            }

            Debug.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        public void ListeningMethod()
        {
            Debug.WriteLine("Running listening");
            TcpListener server = null;
            try
            {
                Debug.WriteLine("Running listening");
                // Set the TcpListener on port 13000.
                int port = modelClient.ListeningPort;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Debug.Write("Waiting for a connection... ");
                    
                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Debug.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Debug.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Debug.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Debug.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }

}


