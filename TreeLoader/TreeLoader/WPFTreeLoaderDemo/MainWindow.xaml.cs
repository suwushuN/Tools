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

namespace WPFTreeLoaderDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            wpf = new WPFTreeLoader(tv);
        }
        WPFTreeLoader wpf;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            wpf.LoadOrgNodes();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            wpf.LoadOrgNodesAsync();
        }
    }
}
