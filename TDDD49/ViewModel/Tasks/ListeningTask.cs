using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDDD49.ViewModels;
using TDDD49.Model;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TDDD49.ViewModel.Tasks
{
    internal class ListeningTask
    {
        public ViewModelClient Vmc { get; set; }
        private CancellationToken cancelToken;
        public CancellationToken CancelToken
        {
            get { return cancelToken; }
            set { cancelToken = value; }
        }

        private CancellationTokenSource tokenSource;

        public CancellationTokenSource TokenSource
        {
            get { return tokenSource; }
            set { tokenSource = value; }
        }


        public ListeningTask(ViewModelClient vmc)
        {
            this.Vmc = vmc;
            this.TokenSource = new CancellationTokenSource();
            this.CancelToken = TokenSource.Token;
            //this.Answer = null;
        }

        public void CancelListeningTask()
        {
            tokenSource.Cancel();
            Debug.WriteLine("CANCLED TASK LISTENING");
        }


        public void ListeningTaskMethod(ViewModelClient viewModelClient)
        {

            Task.Factory.StartNew(() =>
            {
                ListeningMethod(viewModelClient);

            });


        }

        private void ListeningMethod(ViewModelClient viewModelClient)
        {
            Debug.WriteLine("LISTENING");
            TcpListener server = null;
            try
            {
                //Debug.WriteLine("Running listening");
                // Set the TcpListener on port 13000.
                int port = viewModelClient.ListeningPort;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    //Debug.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    if (!server.Pending())
                    {
                        //Debug.WriteLine("No connection!");
                        //Funktinalitet för att kolla om vi har canclat
                        CancelToken.ThrowIfCancellationRequested();

                    }

                    else
                    {
                        TcpClient client = server.AcceptTcpClient();
                        Debug.WriteLine("Connected!");

                        data = null;

                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();

                        //ÖPPNA FÖRNSTRET
                        //VÄNTA PÅ SVAR PÅ FÖNSTER, KOLLA M VARIABEL
                        Vmc.PopUpActive = true;

                        int i;

                        //while Answer {}

                        // Loop to receive all the data sent by the client.
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            //Debug.WriteLine("Received: {0}", data);

                            // Process the data sent by the client.
                            data = data.ToUpper();

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            //Debug.WriteLine("Sent: {0}", data);
                        }

                        // Shutdown and end connection

                        client.Close();
                    }
                }
                    
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            //Debug.WriteLine("\nHit enter to continue...");
            Console.Read();
        }


    }
}
