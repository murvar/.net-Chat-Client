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
using System.Windows;
using System.ComponentModel;

namespace TDDD49.ViewModel.Tasks
{
    internal class Connections : INotifyPropertyChanged
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

        private Message recievedMessage;
        public Message RecievedMessage
        {
            get { return recievedMessage; }
            set { 
                recievedMessage = value;
                OnPropertyChanged("RecievedMessage");   
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


        public Connections(ViewModelClient vmc)
        {
            this.Vmc = vmc;
            //this.client = new List<TcpClient>();
        }

        public void ConnectTaskMethod(ModelClient modelClient)
        {
            Task.Factory.StartNew(() =>
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

                    var something = new TcpClient(server, port);
                    client.Add(something);

                    Vmc.ShowConnectionStatusMsg = "Connected";

                    // Translate the passed message into ASCII and store it as a Byte array.
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(modelClient.Name);

                    // Get a client stream for reading and writing.
                    //Stream stream = client.GetStream();

                    NetworkStream stream = client[0].GetStream();
                    /**
                    // Send the message to the connected TcpServer.
                    stream.Write(data, 0, data.Length);

                    Debug.WriteLine("Sent: {0}", modelClient.Name);

                    Debug.WriteLine(client[0].Connected);

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

                    }*/

                }
                catch (InvalidOperationException e)
                {
                    Debug.WriteLine("InvalidOperationException", e);
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
                finally
                {
                    ListenForMessage();
                }
            });
        }

        public void ListeningTaskMethod(int port)
        {

            Task.Factory.StartNew( async () =>
            {
                try
                {
                    //Debug.WriteLine("Running listening");
                    // Set the TcpListener on port 13000.
                    IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                    // TcpListener server = new TcpListener(port);
                    Server = new TcpListener(localAddr, port);

                    // Start listening for client requests.
                    Server.Start();


                    // Enter the listening loop.

                    var something = await Server.AcceptTcpClientAsync();

                    client.Add(something);
                    Vmc.PopUpActive = true;
                    /**
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                    Vmc.PopUpActive = true;
                        Vmc.ShowConnectionStatusMsg = "Not Connected";
                    }));
                   */

                    Debug.WriteLine("Connected!");



                }
                catch (SocketException e)
                {
                    Debug.WriteLine("SocketException: {0}", e);
                }
                finally
                {
                    // Stop listening for new clients.
                    // Debug.WriteLine("server stopping");
                    server.Stop();
                }

                //Debug.WriteLine("\nHit enter to continue...");
                //Console.Read();

            });

        }

        public void SendMessage(Message msgObj)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(msgObj.Msg);

            NetworkStream stream = client[0].GetStream();
           
            stream.Write(data, 0, data.Length);

        }

        private void ListenForMessage()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {

                    Byte[] data = new Byte[256];
                    NetworkStream stream = client[0].GetStream();
                    // String to store the response ASCII representation.
                    String responseData = String.Empty;

                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Debug.WriteLine("Will now update REcVMESSAGE");
                    if(responseData != String.Empty)
                    {
                        RecievedMessage = new Message("SENDER", "00:00:00", responseData);
                        stream.Flush();
                    } else
                    {
                        CloseClient();
                        break;
                    }
                }
            });
        }

        public void AcceptConnection()
        {
            Task.Factory.StartNew(() =>
            {

                try
                {
                    Debug.WriteLine("WE are connected");
                } 
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
               
            });

            ListenForMessage();
        }

        public void DisconnectConnection()
        {
            CloseClient();
        }

        public void DenyConnection()
        {
            CloseClient();
        }

        private void CloseClient()
        {
            Vmc.ShowConnectionStatusMsg = "No connection";
            client[0].Close();
            client.RemoveAt(0);
        }


    }
}
