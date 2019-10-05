using System;
using System.Collections.Generic;
using System.IO;
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

namespace wpfClearCodeDirecotries
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<string> _clearDirs = new List<string>(".vs,bin,obj".Split(',').ToList());
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dirPath = txtPath.Text.Trim();
            if (Directory.Exists(dirPath))
            {

                var temp = Directory.GetDirectories(dirPath);
                Stack<string> stack = new Stack<string>(new List<string> { temp[0] });
                while (stack.Count>0)
                {
                    var dir = stack.Pop();
                    if (!Directory.Exists(dir))
                    {
                        continue;
                    }
                    var end = dir.Substring(dir.LastIndexOf('\\')+1);
                    if (_clearDirs.Contains(end))
                    {
                        Directory.Delete(dir, true);
                    }
                    else
                    {
                        var subDirs = Directory.GetDirectories(dir);
                        foreach (var item in subDirs)
                        {
                            stack.Push(item);
                        }
                    }
                }
            }
        }


    }
}
