using MPManagement.ViewModels;
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

namespace MPManagement.Views
{
    public partial class SolderingPasteInOut : ContentControl
    {
        public SolderingPasteInOut()
        {
            InitializeComponent();
            this.Unloaded += UnloadedContentControl;
        }

        private void UnloadedContentControl(object sender, RoutedEventArgs e) => (this.DataContext as SolderingPasteInOutVM).Dispose();

    }
}
