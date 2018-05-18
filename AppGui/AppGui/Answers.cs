using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppGui.Data;
using System.Text.RegularExpressions;

namespace AppGui
{
    class Answers
    {
        private Random random;
        private CultureInfo culture;

        public Answers() {
            random = new Random();
            this.culture = new CultureInfo("pt-PT");
        }

        private string[] parkNotFound = new string[] {
            "Lamento informar mas não encontrei nenhum parque de estacionamento com o nome <NOME_PARQUE_ESTACIONAMENTO>",
            "Infelizmente não localizei o parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO>"
        };

        private string[] parkIsFree = new string[] {
            "Sim, o parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO> tem <NUM_LIVRES> lugares livres",
            "Afirmativo, o parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO> ainda tem <NUM_LIVRES> lugares livres",
        };

        private string[] parkIsNotFree = new string[] {
            "Não, o parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO> está completamente ocupado",
            "Estás com azar, o parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO> está completamente ocupado",
            "Que infortunio, parece que o parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO> está ocupado"
        };

        private string[] parkFreeSpots = new string[] {
            "O parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO> tem <NUM_LIVRES> lugares livres",
            "Existem <NUM_LIVRES> lugares livres no parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO>"
        };

        private string[] parkNoFreeSpots = new string[] {
            "O parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO> está completamente ocupado",
            "O parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO> está cheio",
            "Não existem lugares livres no parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO>"
        };

        private string[] parkServiceUnavailable = new string[] {
            "Estranho não consegui encontrar nenhum parque de estacionamento",
            "O serviço de parque de estacionamento não parece estar a funcionar"
        };

        

        private string[] allParksFreeSTART = new string[] {
            "Podes estacionar nos seguintes parques:",
            "Os seguintes parques de estacionamento estão livres:"
        };

        private string[] allParksFree = new string[] {
            "O parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO> tem <NUM_LIVRES> lugares livres",
            "O parque <NOME_PARQUE_ESTACIONAMENTO> tem <NUM_LIVRES> lugares disponiveis",
            "O estacionamento <NOME_PARQUE_ESTACIONAMENTO> apresenta <NUM_LIVRES> lugares livres"
        };
        
        private string[] parksHelpSTART = new string[] {
            "Podes efectuar qualquer questão, acerca da disponibilidade ou lotação, dos parques de estacionamento do campos.\nConheço os seguintes parques de estacionamento:",
            "Consigo-te dizer, a disponibilidade ou lotação, dos seguintes parques de estacionamento ao redor do campos.\n"
        };

        private string[] allParks = new string[] {
            "O parque de estacionamento <NOME_PARQUE_ESTACIONAMENTO>",
            "O parque <NOME_PARQUE_ESTACIONAMENTO>",
            "O estacionamento <NOME_PARQUE_ESTACIONAMENTO>"
        };

        private string[] ticketsDescriptionStart = new string[] {
            "Ok, encontrei as seguintes filas em atendimento:",
            "Estas são as filas que estão a atender:"
          };

        private string[] ticketDescription = new string[] {
            "A fila <NOME_DA_FILA> que trata de assuntos <DESCRIÇÃO> vai no número <NÚMERO_DA_SENHA>. \n"
        };

        private string[] ticketNotFound = new string[] {
            "Lamento informar mas não encontrei nenhuma senha da fila <NOME_DA_FILA> em funcionamento",
            "Infelizmente não encontrei nenhuma fila em atendimento com a descrição <NOME_DA_FILA>",
            "A fila <NOME_DA_FILA> não está aberta"
        };

        private string[] ticketsServiceUnavailable = new string[] {
            "Não encontrei nenhuma fila em atendimento. O serviço deve estar fechado.",
            "Estás com azar, o serviço de atendimento da universidade não está em funcionamento."
        };

        private string[] lastTicketNumber = new string[] {
            "Para a fila <NOME_DA_FILA> a senha vai no número <NÚMERO_DA_SENHA>",
            "A fila de atendimento <NOME_DA_FILA> vai no número <NÚMERO_DA_SENHA>",
            "Neste momento, a última senha atendida da fila <NOME_DA_FILA> tem o número <NÚMERO_DA_SENHA>"
        };

        // ver plural e singular
        private string[] ticketAverageWaitingTime = new string[] {
            "Na fila <NOME_DA_FILA> o tempo médio de espera é de <TEMPO_ESPERA> minutos e o tempo médio de atendimento é de <TEMPO_ATENDIMENTO> minutos",
            "Na fila de atendimento <NOME_DA_FILA> demora-se cerca de <TEMPO_ESPERA> minutos à espera e <TEMPO_ATENDIMENTO> minutos a ser atendido",
            "Neste momento, na fila <NOME_DA_FILA> espera-se cerca de <TEMPO_ESPERA> minutos e é-se atendido em <TEMPO_ATENDIMENTO> minutos",
            "Vais ter de esperar <TEMPO_ESPERA> minutos na fila <NOME_DA_FILA> para seres atendido em cerca de <TEMPO_ATENDIMENTO> minutos"
        };

        private string[] ticketPeopleWaiting = new string[]
        {
            "Para a fila <NOME_DA_FILA> estão à espera <CLIENTES_EM_ESPERA> pessoas",
            "<CLIENTES_EM_ESPERA> pessoas estão à espera na fila <NOME_DA_FILA>",
            "Neste momento, a fila <NOME_DA_FILA> tem <CLIENTES_EM_ESPERA> à espera"
        };

        // PARA AS PROPINAS

        private string[] ticketLineA = new string[]
        {
            "Para pagares as propinas podes tirar uma senha da fila <NOME_DA_FILA> que trata de assuntos de <DESC>," +
            " e que vai no número <NÚMERO_DA_SENHA>",
            "Podes pagar as propinas com uma senha da fila <NOME_DA_FILA> que trata de assuntos de <DESC>," +
            " e que vai no número <NÚMERO_DA_SENHA>"
        };

        private string[] ticketLineAClosed = new string[]
        {
            "Estás com azar, a fila <NOME_DA_FILA> que trata de assuntos de <DESC> não está aberta",
            "Estás com azar, a fila <NOME_DA_FILA> que trata de assuntos de <DESC> está fechada",
            "A fila <NOME_DA_FILA> que trata de assuntos de <DESC> não está em atendimento"
        };

        private string[] sacsHelp = new string[] {
            "Podes perguntar-me que senhas existem e do que tratam, em que número vai uma determinada fila, qual o tempo de espera de uma qualquer fila e quantas pessoas estão à espera. Neste momento, o serviço está <SERV>",
            "Consigo-te dizer que senhas existem e do que tratam, em que senha vai uma determinada fila, qual o tempo de espera de uma qualquer fila e quantas pessoas estão à espera. Neste momento, o serviço está <SERV>",
            "Relativamente às senhas posso-te dizer quais as senhas existem e o assunto que tratam, em que senha vai uma determinada fila, qual o tempo de espera de uma qualquer fila e quantas pessoas estão à espera. Neste momento, o serviço está <SERV>"
        };

        /*
         * FRASES PARA NEWS 
         */
        private string[] newsServiceUnavailable = new string[] {
            "Estranho não consegui encontrar nenhuma notícia, secalhar existem problemas no servidor.",
            "O serviço de notícias não parece estar a funcionar, não encontrei nenhuma."
        };

        private string[] allNewsSTART = new string[] {
            "Ok, encontrei as seguintes notícias:",
            "Encontrei as seguintes novidades"
        };

        private string[] helpNewsTrue = new string[] {
            "Posso dizer-te as notícias actuais acerca do campos, basta perguntar.\nO serviço de notícias neste momento está disponível",
            "Consigo-te dizer quais são as novidades recentes sobre o campos.\nNeste momento o serviço de notícia encontra-se disponível",
        };

        private string[] helpNewsFalse = new string[] {
            "Posso dizer-te as notícias, actuais acerca do campos.\nO serviço neste momento está indisponível",
            "Consigo-te dizer quais são as novidades recentes sobre o campos.\nNeste momento o serviço encontra-se indisponível",
        };

        private string[] allNews = new string[] {
            "<TITULO_NOTICA> \n\n\n"
        };

        private string[] newsDescription = new string[] {
            "<DESCRICAO>"
        };

        // 13 14 31 0 chuva moderada domingo 15/04/2018 00:00:00
        // return minTemp + " " + maxTemp + " " + windSpeed + " " + humidity + " " + description + " " + dayDescription
        //    + " " + Date.ToString();

        private string[] weatherInDay = new string[]
        {
            "<DIA> a previsão é de <DESC> com temperatura mínima de <MIN> graus e máxima de <MAX>.\nO vento vai soprar a <VEL> quilómetros por hora."
        };

        private string[] weatherDayInvalid = new string[] // exemplo 30 de fevereiro
        {
            "Desculpa, mas o dia que pediste <DIA> não existe",
            "Desculpa, mas o dia que pediste <DIA> é inválido"
        };

        private string[] weatherDayOutOfRange = new string[]
        {
            "Desculpa, não consigo ver o tempo para o dia <DIA>.\nA previsão é só até <DIAS> dias",
            "Estás com azar, para o dia <DIA> não é possível saber o tempo.\nA previsão só vai até <DIAS> dias"
        };

        private string[] weatherRainTrue = new string[]
        {
            "Sim.\n<DIA> a previsão é de <DESC>.",
            "Sim.\nÉ melhor levares o guarda-chuva.\n<DIA> a previsão é de <DESC>"
        };

        private string[] weatherRainFalse = new string[]
        {
            "Não.\n<DIA> a previsão é de <DESC>.",
            "Não.\nPodes deixar o guarda-chuva em casa.\n<DIA> a previsão é de <DESC>"
        };

        private string[] weatherHelp = new string[] {
            "Podes perguntar-me como vai estar o tempo num determinado dia em Aveiro e se chove. ",
            "Consigo-te dizer em Aveiro se chove num dia bem como uma previsão do tempo mais detalhada para um dia",
            "Relativamente ao tempo em Aveiro posso te dizer como vai estar num dia e também me podes perguntar se vai chover."
        };

        /*
        * Fr
        */

        /*
         * Greathings
         */
        private string[] greathings = new string[]{
            "Olá, sou um assistente virtual. Consigo ajudar-te com senhas académicas, refeições nas cantinas, parques de estacionamento do campos, as últimas notícias acerca da Universidade, e o estado do tempo\nEm caso de dúvidas basta pedir \"ajuda\"\nSe por acaso não conseguires perceber alguma informação, devido ao barulho de fundo, podes pedir para repetir",
            //"MUDAR"
        };

        /*
         * HELP
         */

        private string[] help = new string[]{
            "Podes questionar-me acerca das refeições nas cantinas.\nEstado das filas das senhas académicas.\nLotação dos parques de estacionamento do campos.\n As últimas notícias acerca da Universidade.\n E o estado do tempo\nSe por acaso não conseguires perceber alguma informação, devido ao barulho, de fundo podes pedir para repetir",
        };

        public string[] connectionError = new string[] {
            "Desculpa, mas não consigo contactar o serviço <DESC>. Poderá ser um problema de internet ou então o serviço está em baixo.",
            "Infelizmente, não consigo contactar o serviço <DESC>. Será talvez um problema de internet ou então o serviço está em baixo."
        };

        public string[] warningSlowConnection = new string[] {
            "Desculpa, a conexão ao serviço <DESC> está a demorar um pouco. Assim que tiver a resposta informo-te.",
            "Infelizmente, a conexão ao serviço <DESC> está a demorar um pouco. Assim que puder já te respondo.",
            "A conexão ao serviço <DESC> está um pouco lenta. Não saias daí, assim que conseguir já te respondo."
        };

        public string[] connectionTimeoutError = new string[] {
            "Desculpa, a conexão ao serviço <DESC> demorou muito. Aconselho-te a tentares mais tarde.",
            "Infelizmente, a conexão ao serviço <DESC> demorou muito. Poderás tentar mais tarde.",
            "A conexão ao serviço <DESC> está muito lenta. Aconselho-te a tentares mais tarde."
        };

        public string[] normalConfidenceTypeNormal = new string[] {
            "Desculpa, Percebi <COMMAND>.\n Estou correcto?\n",
            "Percebi <COMMAND>.\n Estou correcto?\n",
            "Disseste <COMMAND>?\n"
        };
        

        public string[] lowConfidenceTypeNormal = new string[] {
            "Estava distraído podes repetir?\n",
            "Não compreendi, podes repetir?\n",
            "Não consegui perceber, podes repetir?\n"
        };

        public string[] lowConfidenceTypeYesNo = new string[] {
            "Não compreendi. Tu disseste <COMMAND>?\n",
            "Não compreendi, <COMMAND>, foi isto que disseste sim ou não?\n",
        };

        //CANTEEN

        private string[] canteenDisable = new string[] {
            "Parece que a cantina do <NOME_CANTINA> está encerrada durante o <REFEIÇÃO> <DIA> ",
            "Infelizmente a cantina do <NOME_CANTINA> está encerrada durante o <REFEIÇÃO> <DIA> "
        };

        private string[] canteenMeals = new string[] {
            "<DIA> para o <REFEIÇÃO>, na cantina do <NOME_CANTINA> podes comer <CARNE>\n<PEIXE>\n<OPÇÃO>\n<DIETA>\n<VEGETARIANO>.",
            "<DIA> a cantina do <NOME_CANTINA> está a servir para o <REFEIÇÃO>, <CARNE>\n<PEIXE>\n<OPÇÃO>\n<DIETA>\n<VEGETARIANO>."
        };

        private string[] canteenNotFound = new string[] {
            "Não existem registos de refeições da cantina do <NOME_CANTINA> durante o <MEAL_TIME> do dia <DIA> de <MES>\n ",
            "A cantina do <NOME_CANTINA> não está a servir <MEAL_TIME> do dia <DIA> de <MES>\n",
        };

        private string[] canteenMealsDayInvalid = new string[] // exemplo 30 de fevereiro
        {
            "Desculpa, mas o dia que pediste <DIA> não existe",
            "Desculpa, mas o dia que pediste <DIA> é inválido"
        };

        private string[] canteensHelp = new string[] {
            "Podes efectuar qualquer questão, acerca do almoço ou jantar nas cantinas do Crasto ou San Tiago, para qualquer dia.",
            "Consigo-te dizer, o almoço ou jantar nas cantinas do Crasto ou San Tiago, para qualquer dia."
        };

        private string[] ok = new string[] {
            "Ok.",
            "Está bem.",
            "O capa"
        };

        public string getOk() { return ok[random.Next(0, ok.Length)]; }

        public string getHelp() { return help[random.Next(0, help.Length)]; }

        public string getGreathings() { return greathings[random.Next(0, greathings.Length)]; }

        public string getDisableCanteen(CanteenData canteen) {return canteenDisable[random.Next(0, canteenDisable.Length)].Replace("<NOME_CANTINA>", canteen.Canteen).Replace("<REFEIÇÃO>", canteen.Meal).Replace("<DIA>", canteen.DayDescription); }

        public string getCanteenMeals(CanteenData canteen)
        {
            StringBuilder sb = new StringBuilder(canteenMeals[random.Next(0, canteenMeals.Length)]);

            sb.Replace("<DIA>", canteen.DayDescription);
            sb.Replace("<REFEIÇÃO>", canteen.Meal);
            sb.Replace("<NOME_CANTINA>", canteen.Canteen);
            sb.Replace("<CARNE>", !canteen.Meat.Equals("0") ? canteen.Meat : "");
            sb.Replace("<PEIXE>", !canteen.Fish.Equals("0") ? canteen.Fish : "");
            sb.Replace("<OPÇÃO>", !canteen.Option.Equals("0") ? canteen.Option : "");
            sb.Replace("<DIETA>", !canteen.Diet.Equals("0") ? canteen.Diet : "");
            sb.Replace("<VEGETARIANO>", !canteen.Vegetarian.Equals("0") ? canteen.Vegetarian : "");

            return sb.ToString();

        }

        public string getCanteensHelp() { return canteensHelp[random.Next(0, canteensHelp.Length)]; }

        public string getCanteenMealsDayInvalid(int day, int month) { return canteenMealsDayInvalid[random.Next(0, canteenMealsDayInvalid.Length)].Replace("<DIA>", day.ToString() + " de " + culture.DateTimeFormat.GetMonthName(month)); }

        public string getParkNotFound(string parkName){return parkNotFound[random.Next(0, parkNotFound.Length)].Replace("<NOME_PARQUE_ESTACIONAMENTO>", parkName);}

        public string getParkIsFree(ParkData park) {return parkIsFree[random.Next(0, parkIsFree.Length)].Replace("<NOME_PARQUE_ESTACIONAMENTO>", park.Nome).Replace("<NUM_LIVRES>", park.Livre.ToString());}

        public string getParkIsNotFree(ParkData park) {return parkIsNotFree[random.Next(0, parkIsNotFree.Length)].Replace("<NOME_PARQUE_ESTACIONAMENTO>", park.Nome);}

        public string getParkFreeSpots(ParkData park){return parkFreeSpots[random.Next(0, parkFreeSpots.Length)].Replace("<NOME_PARQUE_ESTACIONAMENTO>", park.Nome).Replace("<NUM_LIVRES>", park.Livre.ToString()); }

        public string getParkNoFreeSpots(ParkData park) { return parkNoFreeSpots[random.Next(0, parkNoFreeSpots.Length)].Replace("<NOME_PARQUE_ESTACIONAMENTO>", park.Nome); }

        public string getAllParksFree(List<ParkData> park)
        {
            StringBuilder sb = new StringBuilder(allParksFreeSTART[random.Next(0, allParksFreeSTART.Length)]);
            sb.Append(".\n");
            //TODO SORT PARK FOR FREE SPACE
            foreach (var p in park)
            {
                if (p.Livre <= 0) continue;
                sb.Append(allParksFree[random.Next(0, allParksFree.Length)].Replace("<NOME_PARQUE_ESTACIONAMENTO>", p.Nome).Replace("<NUM_LIVRES>", p.Livre.ToString()));
                sb.Append(".\n");//n sei se o speak tem em conta pontuação
            }
            return sb.ToString();
        }

        public string getPraksHelp(List<ParkData> park)
        {
            StringBuilder sb = new StringBuilder(parksHelpSTART[random.Next(0, parksHelpSTART.Length)]);
            sb.Append(".\n");
            foreach (var p in park)
            {
                if (p.Livre <= 0) continue;
                sb.Append(allParks[random.Next(0, allParks.Length)].Replace("<NOME_PARQUE_ESTACIONAMENTO>", p.Nome));
                sb.Append(".\n");
            }
            return sb.ToString();
        }
        

        public string getParkServiceUnavailable() { return parkServiceUnavailable[random.Next(0, parkServiceUnavailable.Length)]; }

        public string getTicketsInfo(List<TicketData> tickets) { 
            StringBuilder sb = new StringBuilder(ticketsDescriptionStart[0]);
            sb.Append("\n");

            foreach(TicketData ticket in tickets)
            {
                sb.Append(ticketDescription[random.Next(0, ticketDescription.Length)]
                    .Replace("<NOME_DA_FILA>", ticket.Letter).Replace("<DESCRIÇÃO>", ticket.Description)
                    .Replace("<NÚMERO_DA_SENHA>", ticket.Latest.ToString()));
            }
            
            return sb.ToString();
        }

        public string getTicketNotFound(TicketData ticket) { return ticketNotFound[random.Next(0, ticketNotFound.Length)].Replace("<NOME_DA_FILA>", ticket.Description);}

        public string getTicketsServiceUnavailable() { return ticketsServiceUnavailable[random.Next(0, ticketsServiceUnavailable.Length)]; }

        public string getlastTicketNumber(TicketData ticket) { return lastTicketNumber[random.Next(0, lastTicketNumber.Length)].Replace("<NOME_DA_FILA>", ticket.Description).Replace("<NÚMERO_DA_SENHA>", ticket.Latest.ToString()); }
        
        public string getTicketAverageWaitingTime(TicketData ticket) { return ticketAverageWaitingTime[random.Next(0, ticketAverageWaitingTime.Length)].Replace("<NOME_DA_FILA>", ticket.Description).Replace("<TEMPO_ESPERA>", ticket.AverageWaitingTime.ToString()).Replace("<TEMPO_ATENDIMENTO>", ticket.AverageAtendingTime.ToString()); }
        
        public string getTicketPeopleWaiting(TicketData ticket) { return ticketPeopleWaiting[random.Next(0, ticketPeopleWaiting.Length)].Replace("<NOME_DA_FILA>", ticket.Description).Replace("<CLIENTES_EM_ESPERA>", ticket.ClientsWaiting.ToString()); }

        public string getNewsServiceUnavailable() { return newsServiceUnavailable[random.Next(0, newsServiceUnavailable.Length)]; }

        public string getAllNews(List<NewsData> news)
        {
            StringBuilder sb = new StringBuilder(allNewsSTART[random.Next(0, allNewsSTART.Length)]);
            sb.Append(".\n");
            //TODO SORT PARK FOR FREE SPACE
            foreach (var p in news)
            {
                sb.Append(allNews[random.Next(0, allNews.Length)].Replace("<TITULO_NOTICA>", p.Title));
            }
            return sb.ToString();
        }

        public string getNewsDescription(NewsData newsData) { return newsDescription[random.Next(0, newsDescription.Length)].Replace("<DESCRICAO>", Regex.Replace(newsData.Description, "<.*?>", String.Empty)); ; }

        public string getHelpNews(bool v){ return v?helpNewsTrue[random.Next(0, helpNewsTrue.Length)]: helpNewsFalse[random.Next(0, helpNewsFalse.Length)];}

        public string getWeatherInDay(WeatherData weather)
        {
            if (weather.DayDescription.Equals("hoje") || weather.DayDescription.Equals("amanhã") || weather.DayDescription.Contains("no dia"))
            {
                return weatherInDay[random.Next(0, weatherInDay.Length)].Replace("<DIA>", weather.DayDescription).Replace("<DESC>", weather.Description).Replace("<MIN>", weather.MinTemp.ToString()).
                    Replace("<MAX>", weather.MaxTemp.ToString()).Replace("<VEL>", weather.WindSpeed.ToString());
            }

            else
            {
                return weatherInDay[random.Next(0, weatherInDay.Length)].Replace("<DIA>", weather.DayDescription + ", no dia " + weather.Date.Day).Replace("<DESC>", weather.Description).Replace("<MIN>", weather.MinTemp.ToString()).
                    Replace("<MAX>", weather.MaxTemp.ToString()).Replace("<VEL>", weather.WindSpeed.ToString());
            }

        }

        public string getWeatherDayOutOfRange(DateTime date, int days) { return weatherDayOutOfRange[random.Next(0, weatherDayOutOfRange.Length)].Replace("<DIA>", date.Day + " de " + culture.DateTimeFormat.GetMonthName(date.Month)).Replace("<DIAS>", days.ToString()); }

        public string getWeatherDayInvalid(int day, int month) { return weatherDayInvalid[random.Next(0, weatherDayInvalid.Length)].Replace("<DIA>", day.ToString() + " de " + culture.DateTimeFormat.GetMonthName(month)); }

        public string getWeatherRain(WeatherData weather) {
            if (weather.Description.Contains("chuva"))
            {
                return getWeatherRainAnswer(weather, weatherRainTrue);
            }

            else
            {
                return getWeatherRainAnswer(weather, weatherRainFalse);
            }
        }
        public string getWeatherHelp()
        {
            return weatherHelp[random.Next(0, weatherHelp.Length)];
        }

        private string getWeatherRainAnswer(WeatherData weather, string[] rainArray)
        {
            if (weather.DayDescription.Equals("hoje") || weather.DayDescription.Equals("amanhã") || weather.DayDescription.Contains("no dia"))
            {
                return rainArray[random.Next(0, rainArray.Length)].Replace("<DIA>", weather.DayDescription).Replace("<DESC>", weather.Description);
            }

            else
            {
                return rainArray[random.Next(0, rainArray.Length)].Replace("<DIA>", weather.DayDescription + ", no dia " + weather.Date.Day).Replace("<DESC>", weather.Description);
            }
        }

        public string getTicketLineA(TicketData ticket)
        {
            return ticketLineA[random.Next(0, ticketLineA.Length)].Replace("<NOME_DA_FILA>", ticket.Letter)
                .Replace("<DESC>", ticket.Description).Replace("<NÚMERO_DA_SENHA>", ticket.Latest.ToString());
        }

        public string getTicketLineAClosed(TicketData ticket)
        {
            return ticketLineAClosed[random.Next(0, ticketLineAClosed.Length)].Replace("<NOME_DA_FILA>", ticket.Letter)
                .Replace("<DESC>", ticket.Description);
        }

        public string getSacsHelp (bool available) {
            StringBuilder sb = new StringBuilder(sacsHelp[random.Next(0, sacsHelp.Length)]);
            sb.Replace("<SERV>", available ? "aberto" : "fechado");
            return sb.ToString();

        }

        public string getConnectionError(string description)
        {
            return connectionError[random.Next(0, connectionError.Length)].Replace("<DESC>", description);
        }

        public string getWarningSlowConnection(string description)
        {
            return warningSlowConnection[random.Next(0, warningSlowConnection.Length)].Replace("<DESC>", description);
        }

        public string getConnectionTimeoutError(string description)
        {
            return connectionTimeoutError[random.Next(0, connectionTimeoutError.Length)].Replace("<DESC>", description);
        }

        public string getNormalConfidenceTypeNormal(string command){return normalConfidenceTypeNormal[random.Next(0, normalConfidenceTypeNormal.Length)].Replace("<COMMAND>", command);}

        public string getLowConfidenceTypeNormal() { return lowConfidenceTypeNormal[random.Next(0, lowConfidenceTypeNormal.Length)]; }

        public string getLowConfidenceTypeYesNo(string command) { return lowConfidenceTypeYesNo[random.Next(0, lowConfidenceTypeYesNo.Length)].Replace("<COMMAND>", command); }
    }
}
