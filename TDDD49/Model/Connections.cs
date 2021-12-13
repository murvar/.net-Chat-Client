using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TDDD49.Model;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace TDDD49.ViewModel.Tasks
{
    public class Connections : INotifyPropertyChanged
    {
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

        private bool infoNoConnection;
        public bool InfoNoConnection
        {
            get { return infoNoConnection; }
            set
            {
                infoNoConnection = value;
                if (value == true)
                {
                    InfoNoConnection = false;
                    OnPropertyChanged("InfoNoConnection");
                }
            }
        }

        private bool foundConnection;

        public bool FoundConnection
        {
            get { return foundConnection; }
            set { 
                foundConnection = value; 
                OnPropertyChanged("FoundConnection");

            }
        }


        public static List<TcpClient> client = new List<TcpClient>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public Connections()
        {
        }

        public void ConnectTaskMethod(string name, string ip, int port)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var something = new TcpClient(ip, port);
                    client.Add(something);
                    //HandShake(name);
                    Debug.WriteLine("Pritn after handshake");

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
                    //Informative chat box
                    InfoNoConnection = true;

                }
                finally
                {
                    Debug.WriteLine("This runs now");
                    Debug.WriteLine(client.Count.ToString());
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
                    FoundConnection = true;

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
            Debug.WriteLine("This runs now 2");
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {

                    Byte[] data = new Byte[256];
                    NetworkStream stream = client[0].GetStream();
                    String responseData = String.Empty;

                    Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    if(responseData != String.Empty)
                    {
                        JObject o = JObject.Parse(responseData);
                        if (o.ContainsKey("handshake"))
                        {
                            ConnectedToUser = (string)o["handshake"];
                        } 
                        else if (o.ContainsKey("buzz"))
                        {

                            Buzzed = true;
                        }
                        else {
                            RecievedMessage = new Message((string)o["sender"], (string)o["time"], (string)o["msg"]);
                        }

                        stream.Flush();
                    } 
                    else {
                        Debug.WriteLine("This runs now 2");
                        App.Current.Dispatcher.Invoke((System.Action)delegate
                        {
                            Debug.WriteLine("This runs from listening for message");
                            CloseClient();
                        });
                        break;
                    }
                }
            });
        }

        public void AcceptConnection(String name)
        {
            Task.Factory.StartNew(() =>
            {

                try
                {
                    //HandShake(name);
                } 
                catch (Exception e)
                {
                    //Debug.WriteLine(e);
                }
                finally
                {
                    Connected = true;
                    FoundConnection = false;
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
            FoundConnection = false;
            CloseClient();
        }

        private void CloseClient()
        {
            Debug.WriteLine("client should now close");
            
            Connected = false;
            client[0].Close();
            client.RemoveAt(0);
        }


    }
}
