using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using TDDD49.Model;
using TDDD49.ViewModel.Commands;
using TDDD49.ViewModel.Tasks;
using System.Collections.ObjectModel;
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
        public BuzzCommand BuzzCommand { get; set; }

        private Connections connections;
        private Connections Connections
        {
            get { return connections; }
            set { connections = value; }
        }

        private ModelClient modelClient;
        private  ModelClient ModelClient
        {
            get { return modelClient; }
            set { modelClient = value; }
        }

        private FileWriter fileWriter;
        private FileWriter FileWriter
        {
            get { return fileWriter; }
            set { fileWriter = value; }
        }

        private bool showPanel;
        public bool ShowPanel
        {
            get { return showPanel; }
            set { 
                showPanel = value;
                OnPropertyChanged("ShowPanel");
            }
        }

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
                modelClient.Ip = value;
                OnPropertyChanged("Ip");
            }
        }

        public String Port
        {
            get { return modelClient.Port; }
            set
            {
                modelClient.Port = value;
                OnPropertyChanged("Port");
            }
        }

        public string ListeningPort
        {
            get { return modelClient.ListeningPort; }
            set
            {
                modelClient.ListeningPort = value;
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
        public ObservableCollection<Message> MessageList
        {
            get { return messageList; }
            set
            {
                messageList = value;
                OnPropertyChanged();
            }
        }
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
            modelClient.Port = "5555";
            modelClient.Name = "Robin";
            modelClient.ListeningPort = "5001";
            this.InformativeConnectBoxActive = false;
            this.PopUpActive = false;
            this.ShowConnectionStatusMsg = "No connection";
            this.ShowPanel = false;

            this.Connections = new Connections();
            this.Connections.PropertyChanged += connections_PropertyChanged;

            this.MessageList = new ObservableCollection<Message>();

            this.FileWriter = new FileWriter();

            this.ConvoHistory = new ObservableCollection<Conversation>(FileWriter.GetHistory());

            this.ClientFetchCommand = new ClientFetchCommand(this);
            this.ClientListenCommand = new ClientListenCommand(this);
            this.AcceptConnectionCommand = new AcceptConnectionCommand(this);
            this.DenyConnectionCommand = new DenyConnectionCommand(this);
            this.DisconnectConnectionCommand = new DisconnectConnectionCommand(this);
            this.SendMessageCommand = new SendMessageCommand(this);
            this.ShowOldConversationCommand = new ShowOldConversationCommand(this);
            this.BuzzCommand = new BuzzCommand(this);



        }

        private void connections_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RecievedMessage")
            {
                AddMessage();
            }
            if(e.PropertyName == "Connected")
            {
                CheckIfClearListChat(Connections.Connected);
                UpdateConnectedStatus(Connections.Connected);
                ShowPanel = Connections.Connected;
            }
            if(e.PropertyName == "Buzzed")
            {
                Buzz();
            }
            if(e.PropertyName == "ConnectedToUser")
            {
                InitConvo(Connections.ConnectedToUser);
            }
            if(e.PropertyName == "FoundConnection")
            {
                PopUpActive = Connections.FoundConnection;
            }
        }

        public void ClientFetchMethod()
        {
            if (Name != null && Name != string.Empty &&
                Ip != null && Ip != string.Empty &&
                Port.ToString() != null && Port.ToString() != string.Empty)
            {
                try
                {
                    connections.ConnectTaskMethod(Name, Ip, Int32.Parse(Port));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                
            }
        }

        public void ClientListenMethod()
        {
            if (Name != null && Name != string.Empty &&
                ListeningPort != null && ListeningPort != string.Empty)
            {
                try
                {
                    connections.ListeningTaskMethod(Int32.Parse(ListeningPort));
                   
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                
            }
        }

        public void AcceptConnectionMethod()
        {
            
            PopUpActive = false;
            connections.AcceptConnection(Name);
 
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
            if (MsgTxt != null && MsgTxt != string.Empty && connections.Connected)
            {
                Message msg = new Message(Name, DateTime.Now.ToString(), MsgTxt);
                WriteMessageLocal(msg);
                connections.SendMessage(msg);
                MsgTxt = "";
            }
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
            Debug.WriteLine("In update status with connected as " + connected);
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
                MessageList.Clear();
                aList.ToList().ForEach(a => MessageList.Add(a)); ;
            }
        }

        private void FilterSearch()
        {
            var myRegex = new Regex("^" + Search);
            IEnumerable <Conversation> conversations = from conversation in fileWriter.GetHistory()
                                                      where myRegex.IsMatch(conversation.Name)
                                                      select conversation;
            
            ConvoHistory = new ObservableCollection<Conversation>(conversations);
        }

        public void BuzzMethod()
        {
            connections.SendBuzz();
        }

        private void Buzz()
        {
            Console.Beep(1500, 100);
        }
    }
}


