using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using mmisharp;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using AppGui.Pages;

namespace AppGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        private MmiCommunication mmiC;

        private ModalitiesManager dManager;

        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            mmiC = new MmiCommunication("localhost", 8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            mmiC.Start();

            dManager = new ModalitiesManager(this);
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            dManager.close();
        }

        private void MmiC_Message(object sender, MmiEventArgs e)
        {
            Console.WriteLine(e.Message);
            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;
          
            dManager.handleIMcommand(com);

        }

        public void NavigateToPageParks(int i)
        {
            if (i == 0)
            {
                ParksPage page = new ParksPage();
                this.NavigationService.Navigate(page);
            }

            if (i == 1)
            {
                CanteensPage page = new CanteensPage();
                this.NavigationService.Navigate(page);
            }

            //MessageBox.Show("todos os parques");
        }

    }
}
