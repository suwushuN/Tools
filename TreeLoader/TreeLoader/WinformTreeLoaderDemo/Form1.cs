using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
namespace WinformTreeLoaderDemo
{
    /// <summary>
    /// 参考TreeLoader的实现方式实现加载
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                var node = new TreeNode { Text = "dssf" };
                var rt = new TreeNode { Text = "rt1" };
                this.treeView1.Nodes.Add(rt);
                this.treeView1.Nodes.Add(new TreeNode { Text = "rgsdg" });
                rt.Nodes.Add(node);
            });
           // return;
            CreateFile();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var loader = new WinformTreeLoader(treeView1);
            loader.LoadTree();
        }
        public void CreateFile()
        {
            StringBuilder sb = new StringBuilder();
            List<int> liCount = new List<int> { 30, 20, 20, 80 };//每个节点的子节点数量
            //根  zhongguo 中国 1
            NodeInfoFromDB root = new NodeInfoFromDB { ParentIndex = "0", Index = "1", Name = "中国" };
            
            List<string> liProvinceName = new List<string> { "浙江", "杭州", "萧山", "西兴" };
            List<string> liNodeIndex = new List<string> { "zhejiang", "hangzhou", "xiaoshan", "xixing" };
            List<List<NodeInfoFromDB>> matrix = new List<List<NodeInfoFromDB>>();
            matrix.Add(new List<NodeInfoFromDB> { root });

            //先添加第一层
            List<NodeInfoFromDB> listFirst = new List<NodeInfoFromDB>();//z浙江01到30
            for (int j = 1; j < liCount[0]+1; ++j)
            {
                var temp = new NodeInfoFromDB { Index = liNodeIndex[0] + j.ToString().PadLeft(2,'0'), Name = liProvinceName[0] + j.ToString().PadLeft(2, '0'), ParentIndex = "1" };
                listFirst.Add(temp);
            }
            matrix.Add(listFirst);

            List<NodeInfoFromDB> listRow = new List<NodeInfoFromDB>(listFirst);//当前行
            for (int i = 1; i < liProvinceName.Count; ++i)
            {
                List<NodeInfoFromDB> listTemp = new List<NodeInfoFromDB>();//下一行
                foreach (var item in listRow)
                {
                    for (int jk = 1; jk < liCount[i] + 1; ++jk)
                    {
                        var temp = new NodeInfoFromDB { Index = item.Index.Replace(liNodeIndex[i - 1], liNodeIndex[i]) + jk.ToString().PadLeft(2, '0'),
                            Name = item.Name.Replace(liProvinceName[i - 1], liProvinceName[i]) + jk.ToString().PadLeft(2, '0'),
                            ParentIndex = item.Index };
                        listTemp.Add(temp);
                    }
                }
                listRow = new List<NodeInfoFromDB>(listTemp);
                matrix.Add(listTemp);
            }



            foreach(var line in matrix)
            {
                foreach(var cell in line)
                {
                    sb.AppendLine(string.Format("{0}&{1}&{2}",cell.Index,cell.Name,cell.ParentIndex));
                }
            }
            System.IO.File.WriteAllText("1.txt",sb.ToString());
        }

    }
}
