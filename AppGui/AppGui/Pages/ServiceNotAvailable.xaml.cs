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
    /// Interaction logic for ServiceNotAvailable.xaml
    /// </summary>
    public partial class ServiceNotAvailable : Page
    {
        public ServiceNotAvailable(string service)
        {
            InitializeComponent();
            this.textBlock.Text = service;
        }
    }
}
