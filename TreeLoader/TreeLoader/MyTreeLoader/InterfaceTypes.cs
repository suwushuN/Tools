using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTreeLoader
{

    public interface ITreeLoader
    {

        void LoadTree();

        void QueryTree(string searchString);


    }


    public interface IUITreeControl
    {
        void LoadRoots(IList<ITreeNodeModel> arr);

        void UIInvoker(Action act);
    }


    public interface ITreeNodeModel
    {
        string IndexVM { get; set; }

        string NameVM { get; set; }

        string ParentIndexVM { get; set; }

        /// <summary>
        /// 子节点集合（类型按需定义）
        /// </summary>
        ICollection<ITreeNodeModel> NodesVM { get; set; }

        ITreeNodeModel ParentNodeVM { get; set; }

        /// <summary>
        /// 是否为组织
        /// </summary>
        bool IsOrgVM { get; set; }

        /// <summary>
        /// 节点类型（具体的组织或者设备的类型）
        /// </summary>
        int NodeTypeVM { get; set; }

        /// <summary>
        /// 设备个数（只有组织节点才会>0)
        /// </summary>
        int DevCountVM { get; set; }

        bool IsVisibleVM { get; set; }

    }

    /// <summary>
    /// 工具接口
    /// </summary>
    public interface ITreeHelper
    {
        ITreeNodeModel TransFromNodeInfo(NodeInfoFromDB info);

        List<NodeInfoFromDB> GetAllDevicesInfo();

        List<NodeInfoFromDB> GetAllOrgnizationInfo();


    }
}
