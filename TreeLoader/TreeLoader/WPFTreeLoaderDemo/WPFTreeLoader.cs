using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFTreeLoaderDemo
{
    /// <summary>
    /// 1、100万级别的同步加载【LoadOrgsWay】
    /// 2、100万级别的异步加载【LoadOrgNodesAsync】中的3中方式
    ///    2.1  根节点在UI线程添加，其他都在异步线程中进行，耗时2秒
    ///    2.2 一个节点切换一次在10万级别还可以接受，100万太慢了
    ///    2.3批量添加节点，通用型 100万4秒
    /// </summary>
    class WPFTreeLoader
    {

        public WPFTreeLoader(TreeView tv)
        {
            _tv = tv;
        }

        private TreeView _tv;
        protected Dictionary<string, TreeNodeModel> _cacheOrgs = new Dictionary<string, TreeNodeModel>();

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

        /// <summary>
        /// 根据信息，创建不同节点（组织还是设备）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
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
                Nodes = new ObservableCollection<TreeNodeModel>(),
                //Image= new BitmapImage(new Uri(@"th.jfif")),//bad performance
                Image = TreeUtil.ImageRescource//use static Image will imporve performance
            };
        }


        public double LoadOrgNodes()
        {
            _cacheOrgs.Clear();
            var orgs = GetAllOrgnizationInfo();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            LoadOrgsSync(orgs);
            sw.Stop();
            var dd = sw.Elapsed.TotalMilliseconds;
            return dd;
        }

        /// <summary>
        /// 100万以下节点可以用此方法，根节点最后添加，2秒内完成
        /// </summary>
        /// <returns></returns>
        public double LoadOrgNodesAsync()
        {
            _cacheOrgs.Clear();
            var orgs = GetAllOrgnizationInfo();
            var time = 0.0;
            var tt = TreeUtil.ImageRescource;
            Task.Run(() =>
           {
               System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
               sw.Start();
               LoadOrgsWayBatchAsync(orgs);//任意多也可以，不卡且有数据显示
                //  LoadOrgsWayOneByOneAsync(orgs);//10万级别性能可以，100万很慢
                //  LoadOrgsWayOnlyRootAsync(orgs);//100万可以，以上应该会慢，构建树耗时太长，本身数据量大的原因
                sw.Stop();
               var dd = sw.Elapsed.TotalMilliseconds;
               time = dd;
           });
            return time;
        }

        /// <summary>
        /// 10万级别UI线程直接加载方式
        //目前是100万个1.5秒左右
        /// </summary>
        /// <param name="orgs"></param>
        private void LoadOrgsSync(List<NodeInfoFromDB> orgs)
        {
            var dd = TreeUtil.ImageRescource;
            List<TreeNodeModel> roots = new List<TreeNodeModel>();
            var orgDicts = orgs.ToDictionary(c => c.Index, c => c);
            foreach (var org in orgs)
            {
                //获取org对应节点
                TreeNodeModel orgNode = GetOrCreateCacheOrgsNode(org);

                //获取父节点
                //1、判断当前节点是否为根节点（根节点的父节点组织信息是不存在的

                if (!orgDicts.ContainsKey(org.ParentIndex))//org为根节点
                {
                    roots.Add(orgNode);//记录根节点。
                                       //可以在此处直接添加到树控件中
                    continue;
                }

                //2、创建父节点
                TreeNodeModel orgParentNode = GetOrCreateCacheOrgsNode(orgDicts[org.ParentIndex]);

                //3、添加到父节点
                orgParentNode.Nodes.Add(orgNode);
            }

            //添加到树控件种
            _tv.ItemsSource = new ObservableCollection<TreeNodeModel>(roots);

        }

        /// <summary>
        /// 先加载数据，然后绑定到树，数据量少是可用的(100万级别以内)
        /// </summary>
        /// <param name="orgs"></param>
        private void LoadOrgsWayOnlyRootAsync(List<NodeInfoFromDB> orgs)
        {
            _tv.Dispatcher.Invoke(() => { var pp = TreeUtil.ImageRescource; });//初始化图片
            //获取根节点列表 

            List<TreeNodeModel> roots = new List<TreeNodeModel>();
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
                        _cacheOrgs[info.Index].ParentNode = pNode;
                    }
                }
                else
                {
                    foreach (var rt in tempDicts[item.Key])
                    {
                        roots.Add(_cacheOrgs[rt.Index]);
                    }
                }
            }
            // 添加到树
            _tv.Dispatcher.Invoke(() =>
            {
                _tv.ItemsSource = new ObservableCollection<TreeNodeModel>(roots);
            });
            #endregion

        }


        /// <summary>
        /// 10万以内，速度可以，100万的时候很慢（50秒以上）
        /// </summary>
        /// <param name="orgs"></param>
        private void LoadOrgsWayOneByOneAsync(List<NodeInfoFromDB> orgs)
        {
            //获取根节点列表  
            List<TreeNodeModel> roots = new List<TreeNodeModel>();
            var tempDicts = orgs.GroupBy(g => g.ParentIndex).ToDictionary(g => g.Key, g => g.ToList());//
            var orgDicts = orgs.ToDictionary(g => g.Index, g => g);
            foreach (var item in tempDicts.Keys)
            {
                if (!orgDicts.ContainsKey(item))
                {
                    foreach (var rt in tempDicts[item])
                    {
                        roots.Add(GetOrCreateCacheOrgsNode(rt));
                    }
                }
            }
            //添加到根
            _tv.ItemsSource = new ObservableCollection<TreeNodeModel>(roots);
            _tv.Dispatcher.Invoke(() =>
            {
                //tempDicts的Values中的所有集合对应了所有的组织信息
                foreach (var item in tempDicts)//Keys中的根节点的父节点是不存在的。
                {
                    if (!orgDicts.ContainsKey(item.Key)) //Key为根的父节点,直接pass
                    {
                        continue;
                    }

                    //Key对应的组织节点
                    TreeNodeModel orgParentNode = GetOrCreateCacheOrgsNode(orgDicts[item.Key]);

                    var tempOrgs = new List<TreeNodeModel>();
                    foreach (var info in item.Value)//Value中的子节点，此处可以改成批量添加***************
                    {
                        TreeNodeModel node = GetOrCreateCacheOrgsNode(info);
                        tempOrgs.ForEach(c => orgParentNode.Nodes.Add(c));
                        node.ParentNode = orgParentNode;
                    }
                }
            });

        }

        /// <summary>
        /// 批量加载组织节点（一次性添加所有子节点) ，已经按父节点分好组了！！！) 复杂度 O(n)
        /// </summary>
        /// <param name="orgs"></param>
        private void LoadOrgsWayBatchAsync(List<NodeInfoFromDB> orgs)
        {
            //获取根节点列表  
            List<TreeNodeModel> roots = new List<TreeNodeModel>();
            var tempDicts = orgs.GroupBy(g => g.ParentIndex).ToDictionary(g => g.Key, g => g.ToList());//

            var orgDicts = orgs.ToDictionary(g => g.Index, g => g);
            foreach (var item in tempDicts.Keys)
            {
                if (!orgDicts.ContainsKey(item))
                {
                    foreach (var rt in tempDicts[item])
                    {
                        roots.Add(GetOrCreateCacheOrgsNode(rt));
                    }
                }
            }
            //添加到根
            _tv.ItemsSource = new ObservableCollection<TreeNodeModel>(roots);
            Task.Run(() =>
           {
                //tempDicts的Values中的所有集合对应了所有的组织信息
                foreach (var item in tempDicts)//Keys中的根节点的父节点是不存在的。
                {
                   if (!orgDicts.ContainsKey(item.Key)) //Key为根的父节点,直接pass
                    {
                       continue;
                   }

                    //Key对应的组织节点
                    TreeNodeModel orgParentNode = GetOrCreateCacheOrgsNode(orgDicts[item.Key]);

                   var tempOrgs = new List<TreeNodeModel>();
                   foreach (var info in item.Value)//Value中的子节点，此处可以改成批量添加***************
                    {
                       TreeNodeModel node = GetOrCreateCacheOrgsNode(info);
                       tempOrgs.Add(node);
                       node.ParentNode = orgParentNode;
                   }
                   _tv.Dispatcher.Invoke(() =>
                   {
                       tempOrgs.ForEach(c => orgParentNode.Nodes.Add(c));
                   });
               }
           });
        }


        /// <summary>
        /// 从缓存中获取树节点，不存在则创建，
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        private TreeNodeModel GetOrCreateCacheOrgsNode(NodeInfoFromDB org)
        {
            TreeNodeModel model;
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
