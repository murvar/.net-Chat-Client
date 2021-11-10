using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49.Model
{
    internal class ModelClient : INotifyPropertyChanged
    {
        private string ip;
        public string Ip
        {
            get { return ip; }
            set 
            {
                System.Diagnostics.Debug.WriteLine(value);
                ip = value;
                OnPropertyChanged("Ip");
            }
        }

        private int? port;
        public int? Port
        {
            get { return port; }
            set 
            {
                System.Diagnostics.Debug.WriteLine(value);
                port = value;
                OnPropertyChanged("Port");
            }
        }

        private int? listeningPort;

        public int? ListeningPort
        {
            get { return listeningPort; }
            set
            {
                System.Diagnostics.Debug.WriteLine(value);
                listeningPort = value;
                OnPropertyChanged("ListeningPort");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;


        private void OnPropertyChanged(string propertyName)
        {
            if (propertyName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



    }
}
