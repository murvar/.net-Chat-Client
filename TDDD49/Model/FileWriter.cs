using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49.Model
{
    public class FileWriter
    {
        public FileWriter()
        {

        }

        public void WriteToFile(JObject jsonObj) 
        {
            File.WriteAllText(@"c:\TDDD49STORAGE\conversations.json", jsonObj.ToString());

            using (StreamWriter file = File.CreateText(@"c:\TDDD49STORAGE\conversations.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                jsonObj.WriteTo(writer);
            }

        }

    }
}
