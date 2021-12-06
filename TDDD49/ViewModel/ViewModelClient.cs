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

namespace TDDD49.ViewModels
{
    public class ViewModelClient : INotifyPropertyChanged
    {

        public ClientFetchCommand ClientFetchCommand { get; set; }
        public ClientListenCommand ClientListenCommand { get; set; }
        public AcceptConnectionCommand AcceptConnectionCommand { get; set; }
        public DenyConnectionCommand DenyConnectionCommand { get; set; }
        public DisconnectConnectionCommand DisconnectConnectionCommand {get; set; }    


        private ListeningTask listeningTask;
        private ConnectTask connectTask;

        private ModelClient modelClient;

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
            get { return informativeConnectBoxMsg;}
            set { informativeConnectBoxMsg = value;}    
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
            this.InformativeConnectBoxActive = false;
            this.PopUpActive = false;
            this.ShowConnectionStatusMsg = "No connection";

            this.listeningTask = new ListeningTask(this);
            this.connectTask = new ConnectTask(this);

            this.ClientFetchCommand = new ClientFetchCommand(this);
            this.ClientListenCommand = new ClientListenCommand(this);
            this.AcceptConnectionCommand = new AcceptConnectionCommand(this);
            this.DenyConnectionCommand = new DenyConnectionCommand(this);
            this.DisconnectConnectionCommand = new DisconnectConnectionCommand(this);



        }

        public void ClientFetchMethod()
        {
            connectTask.ConnectTaskMethod(modelClient);
        }

        public void ClientListenMethod()
        {

            listeningTask.ListeningTaskMethod(ListeningPort);
        }

        public void createConnectionPromptWindow()
        {

        }

        public void AcceptConnectionMethod()
        {
            
            PopUpActive = false;
            ShowConnectionStatusMsg = "Connected";
            listeningTask.AcceptConnection();

        }

        public void DenyConnectionMethod()
        {
            PopUpActive = false;
            listeningTask.DenyConnection();
        }

        public void DisconnectConnectionMethod()
        {
            Debug.WriteLine("Terminating connetion");
            ShowConnectionStatusMsg = "No connection";
            listeningTask.DisconnectConnection();
        }



    }

}


