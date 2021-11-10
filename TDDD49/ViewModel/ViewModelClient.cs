using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDDD49.Model;

namespace TDDD49.ViewModels
{
    public class ViewModelClient : INotifyPropertyChanged
    {

        private ModelClient modelClient;
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

        public int? Port
        {
            get { return modelClient.Port; }
            set
            {
                System.Diagnostics.Debug.WriteLine(value);
                modelClient.Port = value;
                OnPropertyChanged("Port");
            }
        }

        public int? ListeningPort
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
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));  
            }
        }

        public ViewModelClient()
        {
            modelClient = new ModelClient();
            
        
        }


    
    }
}


