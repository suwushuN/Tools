using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTreeLoader
{
    /// <summary>
    /// 测试类
    /// </summary>
    public class TestTreeNodeModel : ITreeNodeModel
    {
        public string IndexVM { get; set; }

        public string NameVM { get; set; }

        public string ParentIndexVM { get; set; }

        /// <summary>
        /// 子节点集合（类型按需定义）
        /// </summary>
        public ICollection<ITreeNodeModel> NodesVM { get; set; }

        public ITreeNodeModel ParentNodeVM { get; set; }

        /// <summary>
        /// 是否为组织
        /// </summary>
        public bool IsOrgVM { get; set; }

        /// <summary>
        /// 节点类型（具体的组织或者设备的类型）
        /// </summary>
        public int NodeTypeVM { get; set; }

        /// <summary>
        /// 设备个数（只有组织节点才会>0)
        /// </summary>
        public int DevCountVM { get; set; }

        public bool IsVisibleVM { get; set; }

    }

    /// <summary>
    /// 测试类
    /// </summary>
    public class TestTranslator : ITreeHelper
    {

        public ITreeNodeModel TransFromNodeInfo(NodeInfoFromDB info)
        {
            throw new NotImplementedException();
        }

        List<NodeInfoFromDB> ITreeHelper.GetAllDevicesInfo()
        {
            var li = System.IO.File.ReadAllLines("").Skip(12631)
              .Select(c =>
              {
                  var nd = c.Split('&');
                  return new NodeInfoFromDB { Index = nd[0], ParentIndex = nd[2], Name = nd[1], IsOrg = false };
              })
              .ToList();
            return li;
        }

        List<NodeInfoFromDB> ITreeHelper.GetAllOrgnizationInfo()
        {
            var li = System.IO.File.ReadAllLines("").Take(12631)
                 .Select(c =>
                 {
                     var nd = c.Split('&');
                     return new NodeInfoFromDB { Index = nd[0], ParentIndex = nd[2], Name = nd[1], IsOrg = true };
                 })
                 .ToList();
            return li;
        }
        public void CreateFile()
        {
            StringBuilder sb = new StringBuilder();
            List<int> liCount = new List<int> { 30, 20, 20, 20 };//每个节点的子节点数量
            //根  zhongguo 中国 1
            NodeInfoFromDB root = new NodeInfoFromDB { ParentIndex = "0", Index = "1", Name = "中国" };

            List<string> liProvinceName = new List<string> { "浙江", "杭州", "萧山", "西兴" };
            List<string> liNodeIndex = new List<string> { "zhejiang", "hangzhou", "xiaoshan", "xixing" };
            List<List<NodeInfoFromDB>> matrix = new List<List<NodeInfoFromDB>>();
            matrix.Add(new List<NodeInfoFromDB> { root });

            //先添加第一层
            List<NodeInfoFromDB> listFirst = new List<NodeInfoFromDB>();//z浙江01到30
            for (int j = 1; j < liCount[0] + 1; ++j)
            {
                var temp = new NodeInfoFromDB { Index = liNodeIndex[0] + j.ToString().PadLeft(2, '0'), Name = liProvinceName[0] + j.ToString().PadLeft(2, '0'), ParentIndex = "1" };
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
                        var temp = new NodeInfoFromDB
                        {
                            Index = item.Index.Replace(liNodeIndex[i - 1], liNodeIndex[i]) + jk.ToString().PadLeft(2, '0'),
                            Name = item.Name.Replace(liProvinceName[i - 1], liProvinceName[i]) + jk.ToString().PadLeft(2, '0'),
                            ParentIndex = item.Index
                        };
                        listTemp.Add(temp);
                    }
                }
                listRow = new List<NodeInfoFromDB>(listTemp);
                matrix.Add(listTemp);
            }



            foreach (var line in matrix)
            {
                foreach (var cell in line)
                {
                    sb.AppendLine(string.Format("{0}&{1}&{2}", cell.Index, cell.Name, cell.ParentIndex));
                }
            }
            System.IO.File.WriteAllText("1.txt", sb.ToString());
        }
        public void UIInvoker(Action act)
        {
            //   Wpf.Dispatcher.Inovke(act);

            // winform.Invoke(new Action(act));
        }
    }
}
