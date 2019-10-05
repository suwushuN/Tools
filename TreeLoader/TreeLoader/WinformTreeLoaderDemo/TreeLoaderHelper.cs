using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTreeLoaderDemo
{
    /// <summary>
    /// 从数据源获取的节点信息  NodeInfoFromDB
    /// </summary>
    public class NodeInfoFromDB
    {
        public string Index { get; set; }

        public string Name { get; set; }

        public string ParentIndex { get; set; }

        public bool IsOrg { get; set; }

    }

    public class TreeUtil
    {
        //查询到的数据可以进行缓存
        public List<NodeInfoFromDB> GetAllDevicesInfo()
        {
            return new List<NodeInfoFromDB>();
        }
        //查询到的数据可以进行缓存
        public List<NodeInfoFromDB> GetAllOrgnizationInfo()
        {
            return new List<NodeInfoFromDB>();
        }

        public TreeNode TransFromNodeInfo(NodeInfoFromDB info)
        {
            if (info == null)
            {
                return null;
            }
            return new TreeNode
            {
                // Index = info.Index,
                Name = info.Name,
                //  ParentIndex = info.ParentIndex,
                //    Nodes = new List<TreeNode>()
            };
        }
    }
}
