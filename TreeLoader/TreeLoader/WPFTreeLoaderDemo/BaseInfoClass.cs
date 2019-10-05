using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFTreeLoaderDemo
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
            public ObservableCollection<TreeNodeModel> Nodes { get; set; }

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

        public ImageSource Image { get; set; }
        }

        public class TreeUtil
        {

        /// <summary>
        /// 此处的URI相对位置为程序安装后的文件相对位置，磁盘上必须有该文件，而不是程序集包含该文件【图片的build Action(生成操作)为内容，复制到输出目录可以为copy===》保证程序下有该文件】
        /// 如果使用wpf的Image控件Source那么只需要程序集有即可，生成的程序中没有该文件也是可以正常显示的。【图片的build Action(生成操作)为内容，复制到输出目录可以为非copy】
        /// 用代码访问的图片和在xaml中访问的图片其实不是同一个图片
        /// </summary>
        public static ImageSource ImageRescource = new BitmapImage(new Uri("/images/th.jfif", UriKind.RelativeOrAbsolute));
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
                    Nodes = new ObservableCollection<TreeNodeModel>()
                };
            }
        }
   
}
