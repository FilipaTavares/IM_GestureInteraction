﻿using AppGui.Data;
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
    class ClientSAC
    {
        private HttpClient client;
        private ModalitiesManager dManager;


        public ClientSAC(ModalitiesManager dManager)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://services.web.ua.pt/sac/senhas");
            client.Timeout = TimeSpan.FromSeconds(6);
            this.dManager = dManager;
        }

        public void request(string[] args)
        {
            getResponse(args);
        }

        async void getResponse(string[] args)
        {
            try
            {
                Task<string> getResponseTask = client.GetStringAsync("?format=json");

                anotherTask(getResponseTask);

                string response = await getResponseTask;
                handleResponse(response, args);

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

        private async void anotherTask(Task<string> getResponseTask)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            if (!getResponseTask.IsCompleted)
            {
                dManager.manageDialogueWeatherConnectionErrors("warning timeout", "das senhas académicas");
            }
        }

        private void handleResponse(string response, string[] args)
        {
            dynamic json = JsonConvert.DeserializeObject(response);

            bool serviceAvailable = isServiceAvailable(json);

            if (args.Length == 1 && args[0].ToString().Equals("TYPE6"))
            {
                dManager.manageDialogueSAC(args[0].ToString(), serviceAvailable);
            }
            
            else if (serviceAvailable)
            {
                if (args.Length == 1 && args[0].ToString().Equals("TYPE1"))
                {
                    dManager.manageDialogueSAC(getAllTicketsInfo(json));
                    dManager.displaySAC(getAllTicketsInfo(json));
                }

                else if (args.Length == 1 && args[0].ToString().Equals("TYPE5"))
                {
                    dManager.manageDialogueSAC(getTicket(json, "A", "licenciatura e mestrado"), args[0]);
                }

                else
                {
                    Console.WriteLine("ENTROU");
                    dManager.manageDialogueSAC(getTicket(json, args[1], args[2]), args[0]);
                }

            }

            else
            {
                dManager.manageDialogueSAC("service not available", false);

                List<TicketData> l = new List<TicketData>();

                TicketData data = new TicketData("A", "DESCRIÇÃO FIXE", true);
                data.Latest = 100;
                data.AverageAtendingTime = 150;
                data.AverageWaitingTime = 200;
                data.ClientsWaiting = 20;

                l.Add(data);


                data = new TicketData("C", "DESCRIÇÃO FIXE", true);
                data.Latest = 100;
                data.AverageAtendingTime = 150;
                data.AverageWaitingTime = 200;
                data.ClientsWaiting = 20;

                l.Add(data);

                data = new TicketData("D", "DESCRIÇÃO FIXE", true);
                data.Latest = 100;
                data.AverageAtendingTime = 150;
                data.AverageWaitingTime = 200;
                data.ClientsWaiting = 20;

                l.Add(data);

                data = new TicketData("F", "DESCRIÇÃO FIXE", true);
                data.Latest = 100;
                data.AverageAtendingTime = 150;
                data.AverageWaitingTime = 200;
                data.ClientsWaiting = 20;

                l.Add(data);

                data = new TicketData("G", "DESCRIÇÃO FIXE", true);
                data.Latest = 100;
                data.AverageAtendingTime = 150;
                data.AverageWaitingTime = 200;
                data.ClientsWaiting = 20;

                l.Add(data);

                data = new TicketData("H", "DESCRIÇÃO FIXE", true);
                data.Latest = 100;
                data.AverageAtendingTime = 150;
                data.AverageWaitingTime = 200;
                data.ClientsWaiting = 20;

                l.Add(data);

                data = new TicketData("X", "DESCRIÇÃO FIXE", true);
                data.Latest = 100;
                data.AverageAtendingTime = 150;
                data.AverageWaitingTime = 200;
                data.ClientsWaiting = 20;

                l.Add(data);

                dManager.displaySAC(l);
            }
        }

        private bool isServiceAvailable(dynamic json)
        {
            if (json.items["@attributes"].count == 0)
                return false;

            return true;
        }

        private TicketData getTicket(dynamic json, string letter, string description)
        {
            string s = json.items.item[0]["@attributes"].enabled;
           
            for (int i = 0; i < json.items.item.Count; i++)
            {
                if (int.Parse(json.items.item[i]["@attributes"].enabled.ToString()) == 1 && json.items.item[i].letter.ToString().Equals(letter))
                {
                    Console.WriteLine("entrou");
                    TicketData ticket = new TicketData(letter, description, true);
                    ticket.Latest = int.Parse(json.items.item[i].latest.ToString());

                    double atendingTime = Double.Parse(json.items.item[i].ast.ToString()) / 60.0;
                    double waitingTime = Double.Parse(json.items.item[i].awt.ToString()) / 60.0;

                    ticket.AverageAtendingTime = (int) Math.Round(atendingTime, MidpointRounding.AwayFromZero);
                    ticket.AverageWaitingTime = (int) Math.Round(waitingTime, MidpointRounding.AwayFromZero);
                    ticket.ClientsWaiting = int.Parse(json.items.item[i].wc.ToString());
                    return ticket;
                }
            }

            return new TicketData(letter, description, false);
        }

        private List<TicketData> getAllTicketsInfo(dynamic json)
        {
            List<TicketData> list = new List<TicketData>();

            for (int i = 0; i < json.items.item.Count; i++)
            {

                if (int.Parse(json.items.item[i]["@attributes"].enabled.ToString()) == 1)
                {
                    string letter = json.items.item[i].letter;
                    string description = cleanTicketDescriptionName(json.items.item[i].desc.ToString());
                    TicketData ticket = new TicketData(letter, description, true);

                    ticket.Latest = int.Parse(json.items.item[i].latest.ToString());
                    double averageAtendingTime = (double)int.Parse(json.items.item[i].ast.ToString()) / 60;
                    ticket.AverageAtendingTime = (int) Math.Round(averageAtendingTime, MidpointRounding.AwayFromZero);
                    double averageWaitingTime = (double)int.Parse(json.items.item[i].awt.ToString()) / 60;
                    ticket.AverageWaitingTime = (int)Math.Round(averageWaitingTime, MidpointRounding.AwayFromZero);
                    ticket.ClientsWaiting = int.Parse(json.items.item[i].wc.ToString());

                    list.Add(ticket);
                }
            }

            return list;
        }

        private string cleanTicketDescriptionName(string desc)
        {
            Console.WriteLine(desc);

            if (desc.Equals("Lic. (1º ciclo), Mestrado (2º ciclo)"))
                return "de licenciatura e mestrado";

            else if (desc.Equals("Atendimento Prioritário"))
                return "de atendimento prioritário";

            else if (desc.Equals("Doutoramentos, Agregações"))
                return "de doutoramentos e agregações";

            else if (desc.Equals("Exchange / Intercâmbio (Incoming)"))
                return "de intercâmbio";

            else if (desc.Equals("Estágios Internacionais"))
                return "de estágios internacionais";

            else if (desc.Equals("Mobilidade Erasmus (Alunos UA)"))
                return "de erasmus";

            else
            {
                return "de inserção profissional";
            }

        }
    }
}