using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49.Model
{

    public class FileWriter
    {
        private JObject conversations;
        public FileWriter()
        {
            //Skapar folder
            Directory.CreateDirectory(@"c:\TDDD49STORAGE");

            //Skapar fil
            File.WriteAllText(@"c:\TDDD49STORAGE\conversations.json", String.Empty);

            if (File.ReadAllText(@"c:\TDDD49STORAGE\conversations.json") == String.Empty)
                {
                    Debug.WriteLine("created conversations object");
                    conversations = new JObject(
                        new JProperty("conversations", new JArray(new JArray()))
                        );
                }
                else
                {
                    conversations = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"c:\TDDD49STORAGE\conversations.json"));
                }
        }

        public void WriteToFile(JObject jsonObj) 
        {
            //ska ta in jsonobj
            //ska hämta hem nuvarande konversation från lokal fil
            //ska parta konversation till jsonobj
            //ska lägga till jsonobj till hämtad json
            //ska skriva json till lokal fil

            //List<List<Message>> Data = JsonConvert.DeserializeObject<List<List<Message>>>(conversations.First.ToString());
            //JObject oldConversations = JObject.Parse(File.ReadAllText(@"c:\TDDD49STORAGE\conversations.json"));

            JArray arrayOfConvos = (JArray)conversations["conversations"];
            JArray aConvo;
            if ((JArray)arrayOfConvos.Last == null)
            {
                aConvo = new JArray();
                arrayOfConvos.Add(aConvo);
              }
            else
            {
                aConvo = (JArray)arrayOfConvos.Last;
            }
            
            aConvo.Add(jsonObj);

            Debug.WriteLine(conversations.ToString());

            File.WriteAllText(@"c:\TDDD49STORAGE\conversations.json", conversations.ToString());


        }
        /**
        public void InitConversation()
        {
            //funktion ska skapa ny konversation (vid behov)
            if (!conversations.Last.HasValues)
            {
                conversations.AddAfterSelf(new JProperty("conversation", new JArray()));
            }
        }*/
        
    }
}
