using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeLoader
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

    public class TreeNodeModel
    {
        public string Index { get; set; }

        public string Name { get; set; }

        public string ParentIndex { get; set; }

        /// <summary>
        /// 子节点集合（类型按需定义）
        /// </summary>
        public List<TreeNodeModel> Nodes { get; set; }

        public TreeNodeModel ParentNode { get; set; }

        /// <summary>
        /// 是否为组织
        /// </summary>
        public bool IsOrg { get; set; }

        /// <summary>
        /// 节点类型（具体的组织或者设备的类型）
        /// </summary>
        public int NodeType { get; set; }

        /// <summary>
        /// 设备个数（只有组织节点才会>0)
        /// </summary>
        public int DevCount { get; set; }
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

        public TreeNodeModel TransFromNodeInfo(NodeInfoFromDB info)
        {
            if (info == null)
            {
                return null;
            }
            return new TreeNodeModel
            {
                Index = info.Index,
                Name = info.Name,
                ParentIndex = info.ParentIndex,
                Nodes = new List<TreeNodeModel>()
            };
        }
           }
}
