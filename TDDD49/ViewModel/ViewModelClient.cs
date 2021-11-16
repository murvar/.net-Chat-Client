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
        public AcceptConnectionCommand AcceptConnectionCommand { get; set; }
        public DenyConnectionCommand DenyConnectionCommand { get; set; }

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

            this.listeningTask = new ListeningTask(this);
            this.connectTask = new ConnectTask(this);

            this.ClientFetchCommand = new ClientFetchCommand(this);
            this.AcceptConnectionCommand = new AcceptConnectionCommand(this);
            this.DenyConnectionCommand = new DenyConnectionCommand(this);

            this.listeningTask.ListeningTaskMethod(this);

        }

        public void ClientFetchMethod()
        {
            connectTask.ConnectTaskMethod(modelClient);
        }

        public void createConnectionPromptWindow()
        {

        }

        public void AcceptConnectionMethod()
        {

        }

        public void DenyConnectionMethod()
        {

        }


    }

}


