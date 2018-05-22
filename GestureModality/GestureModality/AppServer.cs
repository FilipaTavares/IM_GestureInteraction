using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureModality
{
    class AppServer
    {
        private NamedPipeServerStream server;
        private StreamReader reader;
        private bool isSpeakRunning;

        public bool IsSpeakRunning
        {
            get
            {
                return isSpeakRunning;
            }
        }

        public AppServer() { isSpeakRunning = false; }

       

        public void run()
        {
            Task.Factory.StartNew(() => {
            connectionLoop: while (true)
                {

                    server = new NamedPipeServerStream("APPCALLBACK");
                    reader = new StreamReader(server);
                    server.WaitForConnection();
                    Console.WriteLine("NOVA CONEXAO");


                    while (true)
                    {

                        var line = reader.ReadLine();
                        Console.WriteLine("RECEBI " + line);

                        switch (line)
                        {
                            case "<START>":
                                isSpeakRunning = true;

                                break;
                            case "<STOP>":
                                isSpeakRunning = false;

                                break;

                            case null:
                            case "<CLOSE>":
                                server.Close();
                                
                                goto connectionLoop;

                        }


                    }
                }

            });

        }
    }
}
