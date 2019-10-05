using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WpfMyTreeLoader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// 实现的MyTreeLoader,
    /// 树加载和查询已完整测试OK
    /// </summary>
    public partial class MainWindow : Window
    {

        MyTreeLoader.MyTreeLoader wpf;

        public MainWindow()
        {
            InitializeComponent();

            // wpf = new MyTreeLoader.MyTreeLoader(tv2, MTreeHelper.Instance);
            wpf = new MyTreeLoader.MyTreeLoader(new MyTree(tv1), MTreeHelper.Instance);
            wpf.LoadTree();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            wpf.QueryTree(txt.Text);
        }
    }

    public class INT32ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(int))
            {
                return "";
            }
            var res = (int)value;
            if (res == 0)
            {

            }
            return (int)value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || typeof(bool) == value.GetType())
            {
                return Visibility.Visible;
            }
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
