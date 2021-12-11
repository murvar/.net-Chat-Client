using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49.Model
{
    public class Conversation
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private List<Message> listOfMessages;

        public List<Message> ListOfMessages
        {
            get { return listOfMessages; }
            set { listOfMessages = value; }
        }


        public Conversation(string name, List<Message> listOfMessages)
        {
            this.Name = name;
            this.ListOfMessages = listOfMessages;
        }
    }
}
