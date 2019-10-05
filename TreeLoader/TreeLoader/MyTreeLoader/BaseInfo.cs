using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTreeLoader
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
}
