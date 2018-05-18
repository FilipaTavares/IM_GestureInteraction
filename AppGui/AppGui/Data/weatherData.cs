using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGui.Data
{
    class WeatherData
    {

        private int minTemp;
        private int maxTemp;
        private int windSpeed;
        private int humidity;
        private string description;

        private string dayDescription;   // hoje, amanhã, na segunda, dia 23
        private DateTime date;

        public int MinTemp
        {
            get
            {
                return minTemp;
            }

            set
            {
                minTemp = value;
            }
        }

        public int MaxTemp
        {
            get
            {
                return maxTemp;
            }

            set
            {
                maxTemp = value;
            }
        }

        public int WindSpeed
        {
            get
            {
                return windSpeed;
            }

            set
            {
                windSpeed = value;
            }
        }

        public int Humidity
        {
            get
            {
                return humidity;
            }

            set
            {
                humidity = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public string DayDescription
        {
            get
            {
                return dayDescription;
            }

            set
            {
                dayDescription = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return date;
            }

            set
            {
                date = value;
            }
        }

        public override string ToString()
        {
            return MinTemp + " " + MaxTemp + " " + WindSpeed + " " + Humidity + " " + Description + " " + DayDescription
                + " " + Date.ToString();
        }

    }
}
