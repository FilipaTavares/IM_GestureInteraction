using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GestureModality
{
    class AppServer
    {
        private NamedPipeServerStream server;
        private StreamReader reader;
        private bool isSpeakRunning;

        private MainWindow window;

        public bool IsSpeakRunning
        {
            get
            {
                return isSpeakRunning;
            }

            set
            {
                isSpeakRunning = value;
            }
        }

        public AppServer(MainWindow window) {
            isSpeakRunning = false;
            this.window = window;
        }

       

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
                            case "<FIRST_START>":
                                isSpeakRunning = true;
                                window.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    window.resetDefaultColor();
                                    window.changeColorTiles("HELP",Brushes.Green);
                                }));
                                break;
                            case "<STOP>":
                                isSpeakRunning = false;
                                window.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    window.resetDefaultColor();
                                }));
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
