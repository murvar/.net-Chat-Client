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
            if (!File.Exists(@"c:\TDDD49STORAGE\conversations.json")){
                File.WriteAllText(@"c:\TDDD49STORAGE\conversations.json", String.Empty);
            }

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

            JArray arrayOfConvos = (JArray)conversations["conversations"];
            JArray aConvo = (JArray)arrayOfConvos.Last;        
            
            aConvo.Add(jsonObj);

            Debug.WriteLine(conversations.ToString());

            File.WriteAllText(@"c:\TDDD49STORAGE\conversations.json", conversations.ToString());

        }
        
        public void InitConversation()
        {
            JArray arrayOfConvos = (JArray)conversations["conversations"];
            arrayOfConvos.Add(new JArray());
        }
       
    }
}
