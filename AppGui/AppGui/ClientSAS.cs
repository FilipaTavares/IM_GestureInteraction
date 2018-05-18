using AppGui.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppGui
{
    class ClientSAS
    {
        private HttpClient client;
        private ModalitiesManager dManager;
        private HashSet<string> invalidParks;
        private Dictionary<string,string> mapping;

        public ClientSAS(ModalitiesManager dManager)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://services.web.ua.pt/parques/parques");
            client.Timeout = TimeSpan.FromSeconds(6);

            this.dManager = dManager;
            this.invalidParks = new HashSet<string>();
            invalidParks.Add("ZTC");
            invalidParks.Add("ESTGA");
            invalidParks.Add("ISCAA Funcionarios");// TODO ADICIONAR ESTE

            mapping = new Dictionary<string, string>();
            mapping.Add("Residencias","das Residências");
            mapping.Add("Biblioteca", "da Biblioteca");
            mapping.Add("Subterraneo", "Subterrâneo");
            mapping.Add("Ceramica", "de Cerâmica");
            mapping.Add("Linguas", "de Línguas");
            mapping.Add("Incubadora", "da Incubadora");
            mapping.Add("ISCAA Publico", "do Isca");
        }

        public void request(string[] args)
        {
            getResponse(args);
        }

        async void getResponse(string[] args) {
            try
            {
                Task<string> getResponseTask = client.GetStringAsync("");

                anotherTask(getResponseTask);

                string response = await getResponseTask;
                handleResponse(response, args);

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException is WebException)
                {
                    dManager.manageDialogueWeatherConnectionErrors("web exception", "dos parques de estacionamento");
                }

            }
            catch (TaskCanceledException e)
            {
                dManager.manageDialogueWeatherConnectionErrors("timeout", "dos parques de estacionamento");
            }
        }

        private async void anotherTask(Task<string> getResponseTask)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            if (!getResponseTask.IsCompleted)
            {
                dManager.manageDialogueWeatherConnectionErrors("warning timeout", "dos parques de estacionamento");
            }
        }

        private void handleResponse(string response, string[] args)
        {

            dynamic json = JsonConvert.DeserializeObject(response);

            //Console.WriteLine(json.ToString());
            switch (args[0]) {

                case "TYPE1":
                case "TYPE3":
                    dManager.manageDialogueSAS(getAllPark(json), args);
                    dManager.displayParks(getAllPark(json));
                    break;
                case "TYPE2":
                    dManager.manageDialogueSAS(getPark(json, args[2]), args);
                    break;

            }

        }

        private ParkData getPark(dynamic json,string parkName)
        {
            for (int i=1; i<json.Count;i++) {//0 is timestamp
                if (json[i].Nome.ToString().Equals(parkName)) {
                    ParkData park = new ParkData(mapping[parkName], true);
                    park.Capacidade = int.Parse(json[i].Capacidade.ToString());
                    park.Ocupado = int.Parse(json[i].Ocupado.ToString());
                    park.Livre = int.Parse(json[i].Livre.ToString());
                    return park;
                }
            }

            return new ParkData(parkName,false);
        }

        private List<ParkData> getAllPark(dynamic json)
        {
            List<ParkData> list = new List<ParkData>();
            for (int i = 1; i < json.Count; i++)
            {//0 is timestamp
                if (!invalidParks.Contains(json[i].Nome.ToString())) { 
                    ParkData park = new ParkData(mapping[json[i].Nome.ToString()],true);
                    park.Capacidade = int.Parse(json[i].Capacidade.ToString());
                    park.Ocupado = int.Parse(json[i].Ocupado.ToString());
                    park.Livre = int.Parse(json[i].Livre.ToString());
                    list.Add(park);
                }
                
            }

            return list;
        }
    }
}
