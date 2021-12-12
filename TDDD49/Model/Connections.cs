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

        private Message recievedMessage;
        public Message RecievedMessage
        {
            get { return recievedMessage; }
            set { 
                recievedMessage = value;
                OnPropertyChanged("RecievedMessage");   
            }
        }

        private bool buzzed;

        public bool Buzzed
        {
            get { return buzzed; }
            set { 
                buzzed = value;
                if(value == true)
                {
                    Buzzed = false;
                    OnPropertyChanged("Buzzed");
                }
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

        public void ConnectTaskMethod(string name, string ip, int port)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var something = new TcpClient(ip, port);
                    client.Add(something);
                    HandShake(name);

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

        public void SendBuzz()
        {
            JObject jsonObj = new JObject(new JProperty("buzz", "you've been buzzeeedddd"));
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
                            ConnectedToUser = (string)o["handshake"];
                        } 
                        else if (o.ContainsKey("buzz"))
                        {

                            Buzzed = true;
                        }
                        else
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
