using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTreeLoaderDemo
{
    /// <summary>
    /// 本例参考TreeLoader实现
    ///******************* winform的树控件可以在异步中添加创建节点，而不会引发跨线程异常*************************
    /// </summary>
    class WinformTreeLoader
    {
        public WinformTreeLoader(TreeView tv)
        {
            _tv = tv;
        }

        private TreeView _tv;
        protected Dictionary<string, TreeNode> _cacheOrgs = new Dictionary<string, TreeNode>();

        //查询到的数据可以进行缓存
        public List<NodeInfoFromDB> GetAllDevicesInfo()
        {
            return new List<NodeInfoFromDB>();
        }
        //查询到的数据可以进行缓存
        public List<NodeInfoFromDB> GetAllOrgnizationInfo()
        {
            List<int> liCount = new List<int> { 30, 20, 40, 40 };//每个节点的子节点数量
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
            var allOrgs = new List<NodeInfoFromDB>();
            matrix.ForEach(c => allOrgs.AddRange(c));
            return allOrgs;

        }

        public TreeNode TransFromNodeInfo(NodeInfoFromDB info)
        {
            if (info == null)
            {
                return null;
            }
            return new TreeNode
            {
                //  Index = info.Index,
                Text = info.Name,
                //   ParentIndex = info.ParentIndex,
                //   Nodes = new List<TreeNode>()
            };
        }
        public void LoadTree()
        {
            LoadOrgNodes();
            LoadDeviceNodes();
        }

        public void LoadOrgNodes()
        {
            var orgs = GetAllOrgnizationInfo();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            LoadOrgsWay_1(orgs);
            sw.Stop();
            var dd = sw.Elapsed.TotalMilliseconds;
            // LoadOrgsWay_2(orgs);//里面有两种方式需要测试

        }

        /// <summary>
        /// 先加根节点会不会好一点，目前是10万个1.2秒左右
        /// </summary>
        /// <param name="orgs"></param>
        private void LoadOrgsWay_1(List<NodeInfoFromDB> orgs)
        {
            List<TreeNode> roots = new List<TreeNode>();

            var orgDicts = orgs.ToDictionary(c => c.Index, c => c);
            foreach (var org in orgs)
            {
                //获取org对应节点
                TreeNode orgNode = GetOrCreateCacheOrgsNode(org);

                //获取父节点
                //1、判断当前节点是否为根节点（根节点的父节点组织信息是不存在的

                if (!orgDicts.ContainsKey(org.ParentIndex))//org为根节点
                {
                    roots.Add(orgNode);//记录根节点。
                                       //可以在此处直接添加到树控件中
                    continue;
                }

                //2、创建父节点
                TreeNode orgParentNode = GetOrCreateCacheOrgsNode(orgDicts[org.ParentIndex]);

                //3、添加到父节点
                orgParentNode.Nodes.Add(orgNode);
                
            }

            //添加到树控件种
            _tv.Nodes.AddRange(roots.ToArray());

        }

        /// <summary>
        /// 批量加载组织节点（一次性添加所有子节点) ，已经按父节点分好组了！！！) 复杂度 O(n)
        /// </summary>
        /// <param name="orgs"></param>
        private void LoadOrgsWay_2(List<NodeInfoFromDB> orgs)
        {
            //获取根节点列表  

            List<TreeNode> roots = new List<TreeNode>();

            var tempDicts = orgs.GroupBy(g => g.ParentIndex).ToDictionary(g => g.Key, g => g.ToList());//
            #region way 1
            //创建树 Way 1 先一次性创建所有节点，然后按照分组构建组织树
            foreach (var item in orgs)
            {
                var nd = TransFromNodeInfo(item);
                _cacheOrgs.Add(item.Index, nd);
            }

            foreach (var item in tempDicts)
            {
                if (_cacheOrgs.ContainsKey(item.Key))///Keys中的根节点的父节点是不存在的，判断Key对应节点是否存在
                {
                    var pNode = _cacheOrgs[item.Key];
                    foreach (var info in item.Value)
                    {
                        pNode.Nodes.Add(_cacheOrgs[info.Index]);
                    }
                }
            }
            #endregion

            #region way 2  当way 1慢时(可能超过5秒)，启用
            ////创建树 Way 2 遍历分组，同时构建组织树，通用性好（数据量超级高时，适当修改后，也不会卡UI------>先加根，然后再加其他）
            //var orgDicts = orgs.ToDictionary(g => g.Index, g => g);

            //List<string> rootIndexes = new List<string>();

            //foreach (var item in tempDicts)//Keys中的根节点的父节点是不存在的。
            //{
            //    if (!orgDicts.ContainsKey(item.Key)) //Key为根的父节点,直接pass
            //    {
            //        rootIndexes.AddRange(item.Value.Select(c => c.Index));
            //        continue;
            //    }

            //    //Key对应的组织节点
            //    TreeNodeModel orgParentNode = GetOrCreateCacheOrgsNode(orgDicts[item.Key]);

            //    foreach (var info in item.Value)//Value中的子节点，此处可以改成批量添加***************
            //    {
            //        TreeNodeModel node = GetOrCreateCacheOrgsNode(info);
            //        orgParentNode.Nodes.Add(node);//如果在异步线程,则需要到UI线程中回调
            //        node.ParentNode = orgParentNode;
            //    }
            //}
            #endregion

            //将根节点添加到树控件中。

        }


        /// <summary>
        /// 从缓存中获取树节点，不存在则创建，
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        private TreeNode GetOrCreateCacheOrgsNode(NodeInfoFromDB org)
        {
            TreeNode model;
            if (!_cacheOrgs.TryGetValue(org.Index, out model))
            {
                model = TransFromNodeInfo(org);
                _cacheOrgs.Add(org.Index, model);
            }
            return model;
        }

        public void LoadDeviceNodes()
        {
            var devs = GetAllDevicesInfo();
            foreach (var dev in devs)
            {
                var devNode = TransFromNodeInfo(dev);
                if (_cacheOrgs.ContainsKey(dev.ParentIndex))
                {
                    _cacheOrgs[dev.ParentIndex].Nodes.Add(devNode);
                }
                else
                {
                    //异常冗余数据
                }
            }
        }
    }
}
