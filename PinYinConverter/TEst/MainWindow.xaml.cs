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

namespace TEst
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();          
             ReadPinYin.ReadFiles();
       
        }

        private void TestHanZiPinYinFullSpell()
        {
            var res1 = HanZiPinYin.ChineseCharToPinYinHelper.GetFullLetterSpell("单于单于");
        }

        private void TestHanZiPinYinGetFullSpell()
        {
            var res2 = HanZiPinYin.ChineseCharToPinYinHelper.GetFullSpell("单于单于");
        }

        private void TestPinYinConverterGetFullSpellAndFirstLetter()
        {
             var res=  PinYinConverter.PinYinConverter.GetFullSpellAndFirstLetter("单于单于单于单于单于单于单于单于单于单于单于");
        }

    }
}
