using AppGui.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppGui
{
    class ClientNews
    {

        private HttpClient client;
        private ModalitiesManager dManager;

        public ClientNews(ModalitiesManager dManager)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://services.sapo.pt/UA/Online/contents_xml");
            client.Timeout = TimeSpan.FromSeconds(6);

            this.dManager = dManager;
        }

        public void request(string[] args) {

            getResponse(args);
        }

        async void getResponse(string[] args)
        {
            try
            {
                Task<byte[]> getResponseTask = client.GetByteArrayAsync("?jsonText=true");

                anotherTask(getResponseTask);

                byte[] response = await getResponseTask;
                handleResponse(Encoding.UTF8.GetString(response), args);

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException is WebException)
                {
                    dManager.manageDialogueWeatherConnectionErrors("web exception", "das senhas académicas");
                }

            }
            catch (TaskCanceledException e)
            {
                dManager.manageDialogueWeatherConnectionErrors("timeout", "das senhas académicas");
            }
        }

        private async void anotherTask(Task<byte[]> getResponseTask)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            if (!getResponseTask.IsCompleted)
            {
                dManager.manageDialogueWeatherConnectionErrors("warning timeout", "das senhas académicas");
            }
        }

        private void handleResponse(string response, string[] args)
        {
            Console.WriteLine("Handle");
            Console.WriteLine(response);
            dynamic json = JsonConvert.DeserializeObject(response);

            List<NewsData> newsList = new List<NewsData>();
            foreach (dynamic item in json.rss.channel.item) {
                NewsData news = new NewsData();
                news.Title = item.title;
                news.Description = Regex.Replace(item.description.ToString(), "<.*?>", String.Empty); 
                news.Date = item.pubDate;

                newsList.Add(news);
            }

            dManager.manageDialogueNews(newsList, args);
            dManager.displayNews(newsList);
        }
    }
}
