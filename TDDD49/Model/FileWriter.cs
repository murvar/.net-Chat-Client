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
            Directory.CreateDirectory(@"c:\TDDD49STORAGE");

            File.WriteAllText(@"c:\TDDD49STORAGE\conversations.json", String.Empty);

            if (File.ReadAllText(@"c:\TDDD49STORAGE\conversations.json") == String.Empty)
                {
                    conversations = new JObject(
                        new JProperty("conversations", new JArray())
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
           
            List<List<Message>> Data = JsonConvert.DeserializeObject<List<List<Message>>>(conversations.First.ToString());
       
            Data[-1].Add(new Message((string)jsonObj["sender"], (string)jsonObj["time"], (string)jsonObj["msg"]));  //nytt meddelande tillagt i senaste konversationen

            Debug.WriteLine(Data[-1][0].Msg);

            /**File.WriteAllText(@"c:\TDDD49STORAGE\conversations.json", jsonObj.ToString());

            using (StreamWriter file = File.CreateText(@"c:\TDDD49STORAGE\conversations.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                jsonObj.WriteTo(writer);
            }*/

        }

        public void InitConversation()
        {
            //funktion ska skapa ny konversation (vid behov)
            if (!conversations.Last.HasValues)
            {
                conversations.AddAfterSelf(new JProperty("conversation", new JArray()));
            }
        }

    }
}
