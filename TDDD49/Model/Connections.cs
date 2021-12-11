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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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

        private bool connected;
        public bool Connected
        {
            get { return connected; }
            set {

                connected = value;
                OnPropertyChanged("Connected");

            }
        }

        private String connectedToUser;
        public String ConnectedToUser
        {
            get { return connectedToUser; }
            set
            {

                connectedToUser = value;
                OnPropertyChanged("ConnectedToUser");

            }
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
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    Vmc.InformativeConnectBoxActive = false;

                    String server = modelClient.Ip;
                    int port = modelClient.Port;

                    //IMPLEMENTERA DEFENSIV PROGRAMMERING HÄR

                    var something = new TcpClient(server, port);
                    client.Add(something);
                    HandShake(modelClient.Name);
                    /**
                   Vmc.ShowConnectionStatusMsg = "Connected";

                   // Translate the passed message into ASCII and store it as a Byte array.
                   Byte[] data = System.Text.Encoding.ASCII.GetBytes(modelClient.Name);

                   // Get a client stream for reading and writing.
                   //Stream stream = client.GetStream();

                   NetworkStream stream = client[0].GetStream();
                   
                   // Send the message to the connected TcpServer.
                   stream.Write(data, 0, data.Length);

                   Debug.WriteLine("Sent: {0}", modelClient.Name);

                   Debug.WriteLine(client[0].Connected);

                   //Implementera abnryta connection knapp här

                   // Receive the TcpServer.response.

                   // Buffer to store the response bytes.
                   data = new Byte[256];

                   // String to store the response ASCII representation.
                   String responseData = String.Empty;

                   // Read the first batch of the TcpServer response bytes.
                  
                   Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
                   responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                   Debug.WriteLine("response from init connection is " + responseData);
                   ConnectedToUser = responseData;*/
                    //Debug.WriteLine("Received: {0}", responseData);


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
                    //Vmc.InformativeConnectBoxActive = true;
                    Debug.WriteLine("SocketException: {0}", e);
                }
                finally
                {
                    Connected = true;
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
                    IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                    Server = new TcpListener(localAddr, port);
                    Server.Start();

                    var something = await Server.AcceptTcpClientAsync();

                    client.Add(something);
                    Vmc.PopUpActive = true;

                }
                catch (SocketException e)
                {
                    Debug.WriteLine("SocketException: {0}", e);
                }
                finally
                {
                    server.Stop();
                }


            });

        }

        public void SendMessage(Message msgObj)
        {
            JObject jsonObj =
                new JObject(
                    new JProperty("sender", msgObj.Sender),
                    new JProperty("time", msgObj.Time),
                    new JProperty("msg", msgObj.Msg)
                    );
            Send(jsonObj);
        }

        public void HandShake(String name)
        {
            JObject jsonObj =
                new JObject(
                    new JProperty("handshake", name));
            Send(jsonObj);

        }

        public void Send(JObject dataAsJson)
        {
            String dataToSend = dataAsJson.ToString();

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(dataToSend);

            NetworkStream stream = client[0].GetStream();

            stream.Write(data, 0, data.Length);
        }

        private void ListenForMessage()
        {
            //finns tom konversation? Nej -> skapa ny tom

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
                        Debug.WriteLine(responseData);
                        Debug.WriteLine("Debug 1");
                        //JObject o = JObject.Parse(responseData);
                        JObject o = JObject.Parse(responseData);
                        if (o.ContainsKey("handshake"))
                        {
                            connectedToUser = (string)o["handshake"];
                        } else
                        {
                            Debug.WriteLine("Debug 2");
                            RecievedMessage = new Message((string)o["sender"], (string)o["time"], (string)o["msg"]);
                            Debug.WriteLine("Debug 3");
                        }

                        stream.Flush();
                    } else
                    {
                        App.Current.Dispatcher.Invoke((System.Action)delegate
                        {
                            CloseClient();
                        });
                        
                        break;
                    }
                }
            });
        }

        public void AcceptConnection(String name)
        {
            Task.Factory.StartNew(async () =>
            {

                try
                {
                    HandShake(name);
                     /**
                    Byte[] data = new Byte[256];
                    NetworkStream stream = client[0].GetStream();
                    String responseData = String.Empty;

                    Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Debug.WriteLine("Connected to user " + responseData);
                    ConnectedToUser = responseData;

                    data = System.Text.Encoding.ASCII.GetBytes(name);
                    stream.Write(data, 0, data.Length);
                    Debug.WriteLine("Sent name " + name);
                    Debug.WriteLine("WE are connected");
                     */
                } 
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    Connected = true;
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
            Connected = false;
            client[0].Close();
            client.RemoveAt(0);
        }


    }
}
