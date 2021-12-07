using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49.Model
{
    public class Message : INotifyPropertyChanged
    {

        private string sender;
        public string Sender
        {
            get { return sender; }
            set
            {
                sender = value;
                OnPropertyChanged("Sender");
            }
        }

        private String time;
        public String Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }

        private string msg;

        public string Msg
        {
            get { return msg; }
            set
            {
                msg = value;
                OnPropertyChanged("Msg");

            }
        }

        public Message(String sender, String time, String msg)
        {
            this.Sender = sender;
            this.Time = time;
            this.Msg = msg;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public JObject msgToJson()
        {
            return new JObject(
                    new JProperty("sender", Sender),
                    new JProperty("time", Time),
                    new JProperty("msg", Msg)
                    );
        }


    }
}
