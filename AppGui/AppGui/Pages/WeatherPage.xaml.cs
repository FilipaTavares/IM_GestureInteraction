using AppGui.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppGui.Pages
{
    /// <summary>
    /// Interaction logic for WeatherPage.xaml
    /// </summary>
    public partial class WeatherPage : Page
    {
        public WeatherPage(WeatherData weather)
        {
            InitializeComponent();
            this.description.Text = weather.Description;
            this.mintemp.Text = weather.MinTemp.ToString() + " graus";
            this.maxtemp.Text = weather.MaxTemp.ToString() + " graus";
            this.wind.Text = weather.WindSpeed.ToString() + " km/h";
            this.humidity.Text = weather.Humidity.ToString() + " %";
        }
    }
}
