using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDDD49.Model;
using TDDD49.ViewModel.Commands;
using TDDD49.ViewModel.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TDDD49.ViewModels
{
    public class ViewModelClient : INotifyPropertyChanged
    {

        public ClientFetchCommand ClientFetchCommand { get; set; }
        //public ListeningCommand ListeningCommand { get; set; }
        private ListeningTask listeningTask;
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
            modelClient.ListeningPort = 5001;
            this.ClientFetchCommand = new ClientFetchCommand(this);
            //this.ListeningCommand = new ListeningCommand(this);
            this.listeningTask = new ListeningTask(this);
            this.listeningTask.ListeningTaskMethod(modelClient);

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


    }

}


