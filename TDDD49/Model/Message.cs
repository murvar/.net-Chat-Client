using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49.Model
{
    internal class Message /*: INotifyPropertyChanged*/
    {
        private string sender;
        public string Sender
        {
            get { return sender; }
            set
            {
                sender = value;
            }
        }

        private DateTime time;
        public DateTime Time
        {
            get { return time; }
            set
            {
                time = DateTime.Now;
            }
        }

        private string msg;
        public string Msg
        {
            get { return msg; }
            set
            {
                msg = value;
            }
        }


    }
}
