using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;
namespace AppGui
{
    class SpeachClient
    {
        private NamedPipeClientStream client;
        private StreamWriter writer;
        private Action ttsGreatingsCallback;

        public SpeachClient(Action ttsGreatingsCallback)
        {
         
            client = new NamedPipeClientStream("APPCALLBACK");
            writer = new StreamWriter(client);
            this.ttsGreatingsCallback = ttsGreatingsCallback;

        }

        public void connect() {
            Task.Factory.StartNew(()=> {
                Console.WriteLine("Wait connection");
                client.Connect();
                Console.WriteLine("Connected");
                ttsGreatingsCallback();
            });
            
            
        }

        private void send(string message, Action f) {
            //if (client.IsConnected)
            //{
                try
                {
                    writer.WriteLine(message);
                    writer.Flush();
                }
                catch (Exception e)
                { //Para o caso do server ter levado reboot a meio da conexao
                    Console.WriteLine("Retry connect because of error: " + e.Message);
                
                    Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("Wait connection");
                        client.Connect();
                        Console.WriteLine("Connected");
                        f();
                    });
                }
            //}
            //else { Console.WriteLine("Não estou connectado ao server"); }
        }
        private void send(string message)
        {
            //if (client.IsConnected)
            //{
                try
                {
                    writer.WriteLine(message);
                    writer.Flush();
                }
                catch (Exception e) { Console.WriteLine("ATENçÃO esta execao aconteceu!!!! linha 65 nSpeachClient ver "); }
            //}
            //else { Console.WriteLine("Não estou connectado ao server"); }
        }

        public void sendTtsStop() {
            send("<STOP>", sendTtsStop);
        }

        public void sendTtsStart()
        {
            send("<START>", sendTtsStart);
        }

        public void sendDynamicNews(List<string> news) {
            Console.WriteLine("\nADD DYNAMIC\n");
            send("<DYNAMICADD>"+String.Join("|",news));
        }

        public void close() {
            send("<CLOSE>");
            client.Close();
        }
    }
}
