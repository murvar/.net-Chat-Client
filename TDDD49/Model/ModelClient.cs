namespace TDDD49.Model
{
    public class ModelClient 
    {
        private string? name;
        public string? Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        private string? ip;
        public string? Ip
        {
            get { return ip; }
            set 
            {
                ip = value;
            }
        }

        private string? port;
        public string? Port
        {
            get { return port; }
            set 
            {
                port = value;
            }
        }

        private string? listeningPort;

        public string? ListeningPort
        {
            get { return listeningPort; }
            set
            {
                listeningPort = value;
            }
        }

        public ModelClient()
        {
            this.Name = null;
            this.Ip = null;
            this.Port = null;
            this.ListeningPort = null;
        }
    }
}
