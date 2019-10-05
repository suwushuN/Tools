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

        /// <summary>
        /// 栈实现深度遍历方式（类似递归）
        /// 逐个进栈，出栈
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dirPath = txtPath.Text.Trim();
            if (Directory.Exists(dirPath))
            {

                var temp = Directory.GetDirectories(dirPath);
                Stack<string> stack = new Stack<string>(  temp );
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


        /// <summary>
        /// 栈实现广度遍历（从根开始逐层遍历）， 也可用Queue实现,【直接用List也可以，缓存下】
        /// 每次清空栈/队列，然后将所有子项都加入栈/队列
        /// </summary>
        private void LevelByLevel()
        {
            var dirPath = txtPath.Text.Trim();
            if (Directory.Exists(dirPath))
            {
                Queue<string> q = new Queue<string>();
                var temp = Directory.GetDirectories(dirPath);
                Stack<string> stack = new Stack<string>(  temp );
                while (stack.Count > 0)
                {
                    string[] dirs=new string[stack.Count];
                     stack.CopyTo(dirs, 0);
                    foreach(var dir in dirs)
                    {
                        if (!Directory.Exists(dir))
                        {
                            continue;
                        }
                        var end = dir.Substring(dir.LastIndexOf('\\') + 1);
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
}
