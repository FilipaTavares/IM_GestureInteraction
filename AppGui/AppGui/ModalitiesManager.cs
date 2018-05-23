using AppGui.Data;
using AppGui.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace AppGui
{
    class ModalitiesManager
    {

        private ClientCanteen canteen;
        private ClientSAS parking;
        private ClientSAC tickets;
        private ClientNews news;
        private ClientWeather weather;
        private MainWindow window;

        private Tts t;
        private Answers answers;
        private dynamic lastCommand;
        private string lastCommandInputText;

        private static int i = 0;


        private void greathingsCallback()
        {
            t.Speak(answers.getGreathings());
        }

        public ModalitiesManager(MainWindow window)
        {
            canteen = new ClientCanteen(this);
            parking = new ClientSAS(this);
            tickets = new ClientSAC(this);
            news = new ClientNews(this);
            weather = new ClientWeather(this);

            answers = new Answers();
            t = new Tts(greathingsCallback);
            this.window = window;
        }

        public void handleIMcommand(string command)
        {
            dynamic json = JsonConvert.DeserializeObject(command);
            if (((string)json.type.ToString()).Equals("NORMAL"))
            {
                switch ((string)json.confidence.ToString())
                {
                    case "GOOD":
                        lastCommand = json.recognized;
                        handleRecognized(json.recognized);
                        break;
                    case "MEDIUM":
                        lastCommand = json.recognized;
                        lastCommandInputText = (string)json.inputText.ToString();
                        //dizer que nao percebeu
                        t.Speak(answers.getNormalConfidenceTypeNormal(lastCommandInputText), false);
                        break;
                    case "BAD":
                        t.Speak(answers.getLowConfidenceTypeNormal());
                        break;
                }
            }
            else if (((string)json.type.ToString()).Equals("YESNO"))
            {
                switch ((string)json.confidence.ToString())
                {
                    case "GOOD":
                        if (((string)json.recognized[0].ToString()).Equals("YES"))
                        {
                            handleRecognized(lastCommand);
                        }
                        else
                        {
                            t.Speak(answers.getOk(), false);
                        }

                        break;
                    case "BAD":
                        t.Speak(answers.getLowConfidenceTypeYesNo(lastCommandInputText), false);
                        break;

                }
            }
        }

        public void handleRecognized(dynamic recognized)
        {
            //dynamic json = JsonConvert.DeserializeObject(command);

            string[] array = new string[recognized.Count - 1];
            for (int i = 1; i < recognized.Count; i++)          //0 alredy handled
                array[i - 1] = (string)recognized[i].ToString();

            switch ((string)recognized[0].ToString())
            {
                case "CANTEENS":
                    Console.WriteLine("CANTEENS");
                    canteen.request(array);
                    break;

                case "SAS":
                    Console.WriteLine("SAS");
                    parking.request(array);
                    break;

                case "SAC":
                    Console.WriteLine("SAC");
                    tickets.request(array);

                    break;
                case "NEWS":

                    Console.WriteLine("NEWS");
                    news.request(array);

                    break;

                case "WEATHER":
                    Console.WriteLine("WEATHER");
                    weather.request(array);
                    break;
                case "HELP":
                    Console.WriteLine("HELP");
                    manageDialogueHelp();
                    
                    break;

                case "REPEAT":
                    Console.WriteLine("REPEAT");
                    manageDialogueRepeat();
                    break;
            }
        }

        public void displayServiceNotAvailable(string service)
        {
            window.Dispatcher.BeginInvoke((Action)(() =>
            {
                ServiceNotAvailable page = new ServiceNotAvailable(service);
                window.NavigationService.Navigate(page);
            }));
        }

        public void displayWeather(WeatherData weather)
        {
            window.Dispatcher.BeginInvoke((Action)(() =>
            {
                WeatherPage page = new WeatherPage(weather);
                window.NavigationService.Navigate(page);
            }));
        }

        public void close()
        {
            //do some close stuff!!!
            t.close();
        }

        public void displayParks(List<ParkData> parks)
        {
            window.Dispatcher.BeginInvoke((Action)(() =>
            {
                ParksPage page = new ParksPage(parks);
                window.NavigationService.Navigate(page);
            }));

        }

        public void displaySAC(List<TicketData> tickets)
        {
            window.Dispatcher.BeginInvoke((Action)(() =>
            {
                TicketsPage page = new TicketsPage(tickets);
                window.NavigationService.Navigate(page);
            }));

        }

        public void displayHELP()
        {
            window.Dispatcher.BeginInvoke((Action)(() =>
            {
                PresentationPage page = new PresentationPage();
                window.NavigationService.Navigate(page);
            }));

        }


        public void manageDialogueCanteen(List<CanteenData> canteens)
        {

            StringBuilder phrase = new StringBuilder();

            foreach (var canteen in canteens)
            {
                if (canteen.Disabled.Equals("0"))
                {
                    phrase.Append(answers.getCanteenMeals(canteen));
                }
                else
                {
                    phrase.Append(answers.getDisableCanteen(canteen));
                }
                phrase.Append("\n");
            }

            t.Speak(phrase.ToString());
        }

        public void manageDialogueCanteenHelp()
        {
            t.Speak(answers.getCanteensHelp());
        }
        public void manageDialogueCanteenInvalidDate(int day, int month)
        {
            string phrase = answers.getCanteenMealsDayInvalid(day, month);
            t.Speak(phrase);
        }

        public void manageDialogueSAS(ParkData park, string[] args)
        {

            string phrase = "";

            if (park.Found)
            {
                switch (args[1])
                {
                    case ("SUBTYPE1"):
                    case ("SUBTYPE2"):
                        if (park.Livre > 0)
                            phrase = answers.getParkIsFree(park);
                        else
                            phrase = answers.getParkIsNotFree(park);

                        break;
                    case ("SUBTYPE3"):
                        if (park.Livre > 0)
                            phrase = answers.getParkFreeSpots(park);
                        else
                            phrase = answers.getParkNoFreeSpots(park);

                        break;
                }
            }
            else
            {
                phrase = answers.getParkNotFound(park.Nome);
            }


            t.Speak(phrase);
        }

        public void manageDialogueSAS(List<ParkData> park, string[] args)
        {
            string phrase = "";
            Console.WriteLine("ENTER DIALOGUE SAS");
            if (park.Count > 0)
            {
                if (args[0].Equals("TYPE1"))
                    phrase = answers.getAllParksFree(park);
                else //TYPE3
                    phrase = answers.getPraksHelp(park);

            }
            else
            {
                phrase = answers.getParkServiceUnavailable();
            }

            t.Speak(phrase);
        }

        public void manageDialogueSAC(string type, bool available)
        {
            string phrase = "";
            if (type.Equals("service not available"))
            {
                phrase = answers.getTicketsServiceUnavailable();
            }

            else  //help para senhas
            {
                phrase = answers.getSacsHelp(available);
            }

            t.Speak(phrase);

            Console.WriteLine(phrase);
        }

        public void manageDialogueSAC(List<TicketData> tickets)
        {

            string phrase = "";

            bool isEmpty = !tickets.Any();

            if (isEmpty)
            {
                phrase = answers.getTicketsServiceUnavailable();
            }

            else
            {
                phrase = answers.getTicketsInfo(tickets);
            }

            t.Speak(phrase);
            Console.WriteLine(phrase);
        }


        public void manageDialogueSAC(TicketData ticket, string type)
        {

            string phrase = "";

            if (ticket.Enabled)
            {
                Console.WriteLine("ENABLED");

                if (type.Equals("TYPE2"))
                {
                    phrase = answers.getlastTicketNumber(ticket);
                    Console.WriteLine("GET LAST TICKET NUMBER");
                }

                else if (type.Equals("TYPE3"))
                {
                    phrase = answers.getTicketAverageWaitingTime(ticket);
                    Console.WriteLine("GET AVERAGE WAITING TICKET NUMBER");
                }

                else if (type.Equals("TYPE4"))
                {
                    phrase = answers.getTicketPeopleWaiting(ticket);
                    Console.WriteLine("GET PEOPLE WAITING TICKET NUMBER");
                }

                else if (type.Equals("TYPE5"))
                {
                    phrase = answers.getTicketLineA(ticket);
                    Console.WriteLine("GET INFO ABOUT TICKET A TO PAY FEES");
                }

            }

            else
            {
                if (type.Equals("TYPE5"))
                {
                    phrase = answers.getTicketLineAClosed(ticket);
                    Console.WriteLine("TICKET A CLOSED - PROPINAS");
                }

                else
                {
                    phrase = answers.getTicketNotFound(ticket);
                    Console.WriteLine("TICKET NOT FOUND");
                }
            }

            t.Speak(phrase);
            Console.WriteLine(phrase);
        }

        public void manageDialogueNews(List<NewsData> news, string[] args)
        {
            string phrase = "";

            if (news.Count < 0)
            {
                if (args[0].Equals("TYPE3"))
                    phrase = answers.getHelpNews(true);
                else
                    phrase = answers.getNewsServiceUnavailable();
            }
            else
            {
                switch (args[0])
                {
                    case "TYPE1":
                        phrase = answers.getAllNews(news);

                        List<string> lNews = new List<string>();
                        foreach (NewsData nD in news)
                        {
                            lNews.Add(nD.Title);
                        }

                        Console.WriteLine("UPDATE GRAMMAR");
                        //t.addNewsToGrammar(lNews);

                        break;
                    case "TYPE2":
                        phrase = answers.getNewsDescription(news[int.Parse(args[1])]);
                        break;
                    case "TYPE3":
                        phrase = answers.getHelpNews(true);
                        break;
                }
            }


            t.Speak(phrase);
            Console.WriteLine(phrase);
        }

        public void manageDialogueWeather(WeatherData weather, string type)
        {
            string phrase = "";

            if (type.Equals("TYPE1"))
            {
                phrase = answers.getWeatherInDay(weather);
            }

            else
            {
                phrase = answers.getWeatherRain(weather);
            }

            t.Speak(phrase);
            Console.WriteLine(phrase);
        }

        public void manageDialogueWeatherOutOfRangeDay(DateTime date, int days)
        {
            string phrase = answers.getWeatherDayOutOfRange(date, days);
            t.Speak(phrase);
            Console.WriteLine(phrase);

        }

        public void manageDialogueWeatherInvalidDate(int day, int month)
        {
            string phrase = answers.getWeatherDayInvalid(day, month);
            t.Speak(phrase);
            Console.WriteLine(phrase);

        }

        public void manageDialogueWeatherHelp()
        {
            t.Speak(answers.getWeatherHelp());
        }

        public void manageDialogueHelp()
        {
            displayHELP();
            t.Speak(answers.getHelp());
        }

        public void manageDialogueRepeat()
        {
            t.SpeakRepeat();
        }


        public void manageDialogueWeatherConnectionErrors(string error, string description)
        {
            string phrase = "";

            switch (error)
            {
                case "web exception":
                    phrase = answers.getConnectionError(description);
                    break;

                case "warning timeout":
                    phrase = answers.getWarningSlowConnection(description);
                    break;

                case "timeout":
                    phrase = answers.getConnectionTimeoutError(description);
                    break;
            }

            t.Speak(phrase);
        }

    }
}
