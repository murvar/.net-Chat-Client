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
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace TDDD49.ViewModels
{
    public class ViewModelClient : INotifyPropertyChanged
    {

        public ClientFetchCommand ClientFetchCommand { get; set; }
        public ClientListenCommand ClientListenCommand { get; set; }
        public AcceptConnectionCommand AcceptConnectionCommand { get; set; }
        public DenyConnectionCommand DenyConnectionCommand { get; set; }
        public DisconnectConnectionCommand DisconnectConnectionCommand { get; set; }
        public SendMessageCommand SendMessageCommand { get; set; }

        public ShowOldConversationCommand ShowOldConversationCommand { get; set; }

        private Connections connections;

        private ModelClient modelClient;

        private FileWriter fileWriter;

        private bool popUpActive;
        public bool PopUpActive
        {
            get { return popUpActive; }
            set {
                popUpActive = value;
                OnPropertyChanged("PopUpActive");
            }
        }

        private bool informativeConnectBoxActive;
        public bool InformativeConnectBoxActive
        {
            get { return informativeConnectBoxActive; }
            set
            {
                informativeConnectBoxActive = value;
                OnPropertyChanged("InformativeConnectBoxActive");
            }
        }

        private String informativeConnectBoxMsg;
        public String InformativeConnectBoxMsg
        {
            get { return informativeConnectBoxMsg; }
            set { informativeConnectBoxMsg = value; }
        }

        private String showConnectionStatusMsg;
        public String ShowConnectionStatusMsg
        {
            get { return showConnectionStatusMsg; }
            set
            {
                showConnectionStatusMsg = value;
                OnPropertyChanged("ShowConnectionStatusMsg");
            }
        }



        public string Name
        {
            get { return modelClient.Name; }
            set
            {
                System.Diagnostics.Debug.WriteLine(value);
                if (connections != null) {
                    if (!connections.Connected)
                    {
                        modelClient.Name = value;
                        OnPropertyChanged("Name");
                    }
                }

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

        private string search;

        public string Search
        {
            get { return search; }
            set { 
                search = value;
                OnPropertyChanged("Search");
            }
        }


        private String msgTxt;
        public String MsgTxt
        {
            get { return msgTxt; }
            set {
                msgTxt = value;
                OnPropertyChanged("MsgTxt");
            }
        }


        private ObservableCollection<Message> messageList;
        public ObservableCollection<Message> MessageList { get; set; }
        private ObservableCollection<Conversation> convoHistory;
        public ObservableCollection<Conversation> ConvoHistory { 
            get { return convoHistory; } 
            set { 
                convoHistory = value;
                OnPropertyChanged();
            }
        }



        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if(propertyName == "Search")
            {
                FilterSearch();
            }
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
            this.InformativeConnectBoxActive = false;
            this.PopUpActive = false;
            this.ShowConnectionStatusMsg = "No connection";

            this.connections = new Connections(this);
            this.connections.PropertyChanged += connections_PropertyChanged;

            this.MessageList = new ObservableCollection<Message>();

            
            this.fileWriter = new FileWriter();

            this.ConvoHistory = new ObservableCollection<Conversation>(fileWriter.GetHistory());
  


            this.ClientFetchCommand = new ClientFetchCommand(this);
            this.ClientListenCommand = new ClientListenCommand(this);
            this.AcceptConnectionCommand = new AcceptConnectionCommand(this);
            this.DenyConnectionCommand = new DenyConnectionCommand(this);
            this.DisconnectConnectionCommand = new DisconnectConnectionCommand(this);
            this.SendMessageCommand = new SendMessageCommand(this);
            this.ShowOldConversationCommand = new ShowOldConversationCommand(this);



        }

        private void connections_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RecievedMessage")
            {
                AddMessage();
            }
            if(e.PropertyName == "Connected")
            {
                CheckIfClearListChat(connections.Connected);
                UpdateConnectedStatus(connections.Connected);
            }
            if(e.PropertyName == "ConnectedToUser")
            {
                InitConvo(connections.ConnectedToUser);
            }
        }

        public void ClientFetchMethod()
        {
            connections.ConnectTaskMethod(modelClient);
 
        }

        public void ClientListenMethod()
        {
            connections.ListeningTaskMethod(ListeningPort);
        }

        public void AcceptConnectionMethod()
        {
            
            PopUpActive = false;
            connections.AcceptConnection(modelClient.Name);
 
        }

        public void DenyConnectionMethod()
        {
            PopUpActive = false;
            connections.DenyConnection();
        }

        public void DisconnectConnectionMethod()
        {
            connections.DisconnectConnection();
        }

        public void SendMessageMethod()
        {
            Message msg = new Message(Name, DateTime.Now.ToString(), MsgTxt);
            WriteMessageLocal(msg);
            connections.SendMessage(msg);
            MsgTxt = "";
        }

        private void AddMessage()
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                WriteMessageLocal(connections.RecievedMessage);
            });

        }

        private void WriteMessageLocal(Message msg)
        {
            MessageList.Add(msg);
            fileWriter.WriteToFile(msg.msgToJson());

        }

        private void CheckIfClearListChat(bool connected)
        {
            if (!connected)
            {
                MessageList.Clear();
            } 

        }

        private void UpdateConnectedStatus(bool connected)
        {
            Debug.WriteLine("Now running updating connected");
            if(connected)
            {
                ShowConnectionStatusMsg = "Connected";
            } 
            else
            {
                this.ConvoHistory.Insert(0, fileWriter.GetLatestConvo());
                ShowConnectionStatusMsg = "No connection";
            }
        }

        private void InitConvo(string name)
        {
            fileWriter.InitConversation(name);
        }

        public void ShowOldConversationMethod(List<Message> aList)
        {
            if (!connections.Connected)
            {
                Debug.WriteLine("Should update chat right now");
                MessageList.Clear();
                aList.ToList().ForEach(a => MessageList.Add(a)); ;
            }
        }

        private void FilterSearch()
        {
            Debug.WriteLine("Enter filter search");
            //funktionalitet för att visa convoHistory
            //ConvoHistory = new ObservableCollection<Conversation>(fileWriter.GetHistory());

            var myRegex = new Regex("^" + Search);
            //IEnumerable<Conversation> conversations = fileWriter.GetHistory()

            IEnumerable <Conversation> conversations = from conversation in fileWriter.GetHistory()
                                                      where myRegex.IsMatch(conversation.Name)
                                                      select conversation;

            Debug.WriteLine(conversations.ToList().ToString());
            
            ConvoHistory = new ObservableCollection<Conversation>(conversations);

            //ConvoHistory.Clear();
            //conversations.ToList().ForEach(conversation => ConvoHistory.Add(conversation)); 
            //CollectionViewSource.GetDefaultView(ConvoHistory).Refresh();
        }
    }
}


