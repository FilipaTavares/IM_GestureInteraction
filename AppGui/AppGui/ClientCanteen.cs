using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Xml.Linq;
using System.IO;
using System.Globalization;
using System.Net;

namespace AppGui
{
    class ClientCanteen
    {

        private HttpClient client;
        private ModalitiesManager dManager;
        private CultureInfo culture;

        public ClientCanteen(ModalitiesManager dManager)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://services.web.ua.pt/sas/ementas?date=week&place=santiago");
            client.Timeout = TimeSpan.FromSeconds(6);
            this.dManager = dManager;
            this.culture = new CultureInfo("en-US");
        }

        public void request(string[] args)
        {
            if (args[0].Equals("TYPE4")) //dont need get info
            {
                dManager.manageDialogueCanteenHelp();
                return;
            }
            getResponse(args);
        }

        async void getResponse(string[] args)
        {
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
                    dManager.manageDialogueWeatherConnectionErrors("web exception", "das cantinas");
                    // meter imagem na cena dos gestos
                }

            }

            catch (TaskCanceledException e)
            {
                dManager.manageDialogueWeatherConnectionErrors("timeout", "das cantinas");
                // meter imagem na cena dos gestos
            }
        }

        private async void anotherTask(Task<string> getResponseTask)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            if (!getResponseTask.IsCompleted)
            {
                dManager.manageDialogueWeatherConnectionErrors("warning timeout", "das cantinas");
                // meter imagem a pensar na cena dos gestos
            }
        }


        private Tuple<DateTime, string> getValidDate_Description(string[] new_args) {

            if (new_args[0].ToString().Equals("SUBTYPE1") || new_args[1].ToString().Equals("today")) return new Tuple<DateTime, string>(DateTime.Today, "hoje");

            else if (new_args[1].ToString().Equals("tomorrow")) return new Tuple<DateTime, string>(DateTime.Today.AddDays(1), "amanhã");

            else if (new_args[1].ToString().Equals("dayOfWeek")) return new Tuple<DateTime, string>(DateTime.Today.AddDays(getNextWeekday(DateTime.Today, int.Parse(new_args[3].ToString()))), new_args[2].ToString());

            else if (new_args[1].ToString().Equals("numberOfDay"))
            {
                int day = int.Parse(new_args[2].ToString());

                DateTime today = DateTime.Today;
                int month = today.Month;
                int year = today.Year;

                if (day > today.Day && day > DateTime.DaysInMonth(year, month) && month != 12)
                    month += 1;

                else if (day < today.Day && month == 12)
                {
                    month = 1;
                    year += 1;
                }

                else if (day < today.Day && month != 12)
                {
                    month += 1;
                }

                Console.WriteLine(month + "/" + day + "/" + year);

                bool parsed = DateTime.TryParse(day + "-" + month + "-" + year, out today);

                if (!parsed) return null;
                else return new Tuple<DateTime, string>(today, "no dia " + day);
            }

            return null;

        }

        private void handleResponse(string response, string[] args)
        {

            foreach (var item in args)
            {
                Console.WriteLine(item.ToString());
            }

            XDocument document = XDocument.Load(new StringReader(response));

            Tuple<DateTime, string> date_descripton;
            string meal = args[1].ToString();

            List<CanteenData> meals = null;

            if (args[0].Equals("TYPE1"))
            {

                date_descripton = getValidDate_Description(args.Where((_, index) => index >= 2).ToArray<string>());

                if (date_descripton == null)
                {
                    dManager.manageDialogueCanteenInvalidDate(date_descripton.Item1.Day, date_descripton.Item1.Month);
                    return;
                }

                DateTime date = date_descripton.Item1; // find replace??
                string dayDescription = date_descripton.Item2; // find replace??

                string format = "ddd, dd MMM yyyy";   // Use this format.
                Console.WriteLine(date.ToString(format, culture)); // Write to console.

                meals = (from r in document.Descendants("menu").Where
                                    (r => r.Attribute("canteen").Value.Equals("Refeitório do Crasto") || r.Attribute("canteen").Value.Equals("Refeitório de Santiago")).Where
                                    (r => r.Attribute("meal").Value.Equals(meal)).Where
                                    (r => int.Parse(r.Attribute("weekdayNr").Value) == (int)date.DayOfWeek).Where
                                    (r => r.Attribute("date").Value.Contains(date.ToString(format, culture)))
                         from d in r.Elements("items")
                             //where !d.IsEmpty // elimina cantinas fechadas - vê se items = <items /> era fixe mas vou tirar
                             // pq se estiver fechado é diferente do que nao dar para ver a data para um dia longe

                         select new CanteenData
                         {
                             Canteen = r.Attribute("canteen").Value,
                             Meal = r.Attribute("meal").Value,
                             Date = r.Attribute("date").Value,
                             Weekday = r.Attribute("weekday").Value,
                             WeekdayNr = int.Parse(r.Attribute("weekdayNr").Value),
                             Disabled = r.Attribute("disabled").Value,
                             Meat = (d.IsEmpty || d.Descendants("item").ElementAt(1).IsEmpty) ? "0" : d.Descendants("item").ElementAt(1).Value,
                             Fish = (d.IsEmpty || d.Descendants("item").ElementAt(2).IsEmpty) ? "0" : d.Descendants("item").ElementAt(2).Value,
                             Diet = (d.IsEmpty || d.Descendants("item").ElementAt(3).IsEmpty) ? "0" : d.Descendants("item").ElementAt(3).Value,
                             Vegetarian = (d.IsEmpty || d.Descendants("item").ElementAt(4).IsEmpty) ? "0" : d.Descendants("item").ElementAt(4).Value,
                             Option = (d.IsEmpty || d.Descendants("item").ElementAt(5).IsEmpty) ? "0" : d.Descendants("item").ElementAt(5).Value
                         }).ToList<CanteenData>();
            }

            else if (args[0].Equals("TYPE2"))
            {
                string canteen = args[2].Equals("Crasto") ? "Refeitório do Crasto" : "Refeitório de Santiago";

                date_descripton = getValidDate_Description(args.Where((_, index) => index >= 3).ToArray<string>());

                if (date_descripton == null)
                {
                    dManager.manageDialogueCanteenInvalidDate(date_descripton.Item1.Day, date_descripton.Item1.Month);
                    return;
                }

                DateTime date = date_descripton.Item1; // find replace??
                string dayDescription = date_descripton.Item2; // find replace??

                string format = "ddd, dd MMM yyyy";   // Use this format.
                Console.WriteLine(date.ToString(format, culture)); // Write to console.

                meals = (from r in document.Descendants("menu").Where
                                    (r => r.Attribute("canteen").Value.Equals(canteen)).Where
                                    (r => r.Attribute("meal").Value.Equals(meal)).Where
                                    (r => int.Parse(r.Attribute("weekdayNr").Value) == (int)date.DayOfWeek).Where
                                    (r => r.Attribute("date").Value.Contains(date.ToString(format, culture)))
                         from d in r.Elements("items")
                             //where !d.IsEmpty // elimina cantinas fechadas - vê se items = <items /> era fixe mas vou tirar
                             // pq se estiver fechado é diferente do que nao dar para ver a data para um dia longe

                         select new CanteenData
                         {
                             Canteen = r.Attribute("canteen").Value,
                             Meal = r.Attribute("meal").Value,
                             Date = r.Attribute("date").Value,
                             Weekday = r.Attribute("weekday").Value,
                             WeekdayNr = int.Parse(r.Attribute("weekdayNr").Value),
                             Disabled = r.Attribute("disabled").Value,
                             Meat = (d.IsEmpty || d.Descendants("item").ElementAt(1).IsEmpty) ? "0" : d.Descendants("item").ElementAt(1).Value,
                             Fish = (d.IsEmpty || d.Descendants("item").ElementAt(2).IsEmpty) ? "0" : d.Descendants("item").ElementAt(2).Value,
                             Diet = (d.IsEmpty || d.Descendants("item").ElementAt(3).IsEmpty) ? "0" : d.Descendants("item").ElementAt(3).Value,
                             Vegetarian = (d.IsEmpty || d.Descendants("item").ElementAt(4).IsEmpty) ? "0" : d.Descendants("item").ElementAt(4).Value,
                             Option = (d.IsEmpty || d.Descendants("item").ElementAt(5).IsEmpty) ? "0" : d.Descendants("item").ElementAt(5).Value
                         }).ToList<CanteenData>(); // é uma lista para ficar + generico ver dialogue manager

                if (meals.Count > 0) meals[0].DayDescription = dayDescription;
                else meals.Add(new CanteenData
                {
                    Disabled = "Encerrada", //vamos considerar encerrado
                    Canteen = canteen,
                    Meal = meal,
                    DayDescription = "no dia " + date.Day + " do " + date.Month
                });


                Console.WriteLine("---------------------------------------------- " + meals[0].Canteen);
                Console.WriteLine("---------------------------------------------- " + meals[0].Meal);
                Console.WriteLine("---------------------------------------------- " + meals[0].Date);
                Console.WriteLine("---------------------------------------------- " + meals[0].Weekday);
                Console.WriteLine("---------------------------------------------- " + meals[0].WeekdayNr);
                Console.WriteLine("---------------------------------------------- " + meals[0].Disabled);
                Console.WriteLine("---------------------------------------------- " + meals[0].Meat);
                Console.WriteLine("---------------------------------------------- " + meals[0].Fish);
                Console.WriteLine("---------------------------------------------- " + meals[0].Diet);
                Console.WriteLine("---------------------------------------------- " + meals[0].Vegetarian);
            }

            else if (args[0].Equals("TYPE3"))
            {
                DateTime date = DateTime.Today;
                string dayDescription = "hoje";

                string format = "ddd, dd MMM yyyy";   // Use this format.
                Console.WriteLine(date.ToString(format, culture)); // Write to console.

                meals = (from r in document.Descendants("menu").Where
                                    (r => r.Attribute("canteen").Value.Equals("Refeitório do Crasto") || r.Attribute("canteen").Value.Equals("Refeitório de Santiago")).Where
                                    (r => r.Attribute("meal").Value.Equals(meal)).Where
                                    (r => int.Parse(r.Attribute("weekdayNr").Value) == (int)date.DayOfWeek).Where
                                    (r => r.Attribute("date").Value.Contains(date.ToString(format, culture)))
                         from d in r.Elements("items")

                         select new CanteenData
                         {
                             Canteen = r.Attribute("canteen").Value,
                             Meal = r.Attribute("meal").Value,
                             Date = r.Attribute("date").Value,
                             Weekday = r.Attribute("weekday").Value,
                             WeekdayNr = int.Parse(r.Attribute("weekdayNr").Value),
                             Disabled = r.Attribute("disabled").Value,
                             Meat = (d.IsEmpty || d.Descendants("item").ElementAt(1).IsEmpty) ? "0" : d.Descendants("item").ElementAt(1).Value,
                             Fish = (d.IsEmpty || d.Descendants("item").ElementAt(2).IsEmpty) ? "0" : d.Descendants("item").ElementAt(2).Value,
                             Diet = (d.IsEmpty || d.Descendants("item").ElementAt(3).IsEmpty) ? "0" : d.Descendants("item").ElementAt(3).Value,
                             Vegetarian = (d.IsEmpty || d.Descendants("item").ElementAt(4).IsEmpty) ? "0" : d.Descendants("item").ElementAt(4).Value,
                             Option = (d.IsEmpty || d.Descendants("item").ElementAt(5).IsEmpty) ? "0" : d.Descendants("item").ElementAt(5).Value,
                             DayDescription = dayDescription
                         }).ToList<CanteenData>();
            }
            dManager.manageDialogueCanteen(meals);
        }

        private int getNextWeekday(DateTime start, int day)
        {
            return (day - (int)DateTime.Today.DayOfWeek + 7) % 7;
        }

    }
}
