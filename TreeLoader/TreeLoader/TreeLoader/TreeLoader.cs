using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeLoader
{
    /// <summary>
    /// 基于10万节点（测试 秒加载）前提已经获取所有节点，这些节点耗时不计入本次时间中。
    ///  参考用于实现加载树控件的方式
    /// </summary>
    class TreeLoader
    {
        protected Dictionary<string, TreeNodeModel> _cacheOrgs = new Dictionary<string, TreeNodeModel>();

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


        public void LoadOrgNodes()
        {
            var orgs = GetAllOrgnizationInfo();

            LoadOrgsWay_1(orgs);

            // LoadOrgsWay_2(orgs);//里面有两种方式需要测试

        }

        /// <summary>
        /// 只能逐个添加节点 性能应该还是 O(n)
        /// </summary>
        /// <param name="orgs"></param>
        private void LoadOrgsWay_1(List<NodeInfoFromDB> orgs)
        {
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
                orgNode.ParentNode = orgParentNode;
            }

            //添加到树控件种


        }

        /// <summary>
        /// 批量加载组织节点（一次性添加所有子节点) ，已经按父节点分好组了！！！) 复杂度 O(n)
        /// </summary>
        /// <param name="orgs"></param>
        private void LoadOrgsWay_2(List<NodeInfoFromDB> orgs)
        {
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
                        pNode.Nodes.Add(_cacheOrgs[info.Index]);//如果在异步线程,则需要到UI线程中回调
                        _cacheOrgs[info.Index].ParentNode = pNode;
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

            #region 增加给组织计算数功能
            //var devDicts = devs.GroupBy(g => g.ParentIndex).ToDictionary(g => g.Key, g => g.ToList());

            ////设备添加到组织，并给组织计算直接挂载的设备节点数量
            //foreach (var kv in devDicts)
            //{
            //    if (_cacheOrgs.ContainsKey(kv.Key))
            //    {
            //        var orgNode = _cacheOrgs[kv.Key];
            //        foreach (var dev in kv.Value)
            //        {
            //            var devNode = TransFromNodeInfo(dev);
            //            orgNode.Nodes.Add(devNode);
            //        }
            //        orgNode.DevCount = kv.Value.Count;
            //    }
            //    else
            //    {
            //        //异常冗余数据
            //    }
            //}

            //#region //计算组织下所有设备节点 
            ////way 1 组织的父节点存在，那么该父节点需要增加子节点的设备数据量；，逐层计算

            //var orgs = GetAllOrgnizationInfo();
            //var orgDict = orgs.ToDictionary(g => g.Index, g => g);//利用缓存中已存在数据，不要重新查找

            //var curOrgHasDevList = new List<string>(devDicts.Keys);//最末节点组织集合，有设备子节点的所有组织
            //while (true)
            //{
            //    var orgCountChangeDict = new Dictionary<string,int>();//父组织设备计数变化的集合

            //    foreach(var item in curOrgHasDevList)
            //    {
            //        if(orgDict.ContainsKey(item))
            //        {
            //            var orgInfo = orgDict[item];
            //            if(orgDict.ContainsKey(orgInfo.ParentIndex))
            //            {
            //                _cacheOrgs[orgInfo.ParentIndex].DevCount+=  _cacheOrgs[item].DevCount;
            //                if(!orgCountChangeDict.ContainsKey(orgInfo.ParentIndex))
            //                {
            //                    orgCountChangeDict.Add(orgInfo.ParentIndex, 0);
            //                }
            //            }
            //        }                 
            //    }
            //    if (orgCountChangeDict.Count == 0)
            //    {
            //        break;
            //    }
            //    curOrgHasDevList.AddRange(orgCountChangeDict.Keys);//设置新值
            //}


            //#region way2 组织分为多层（从根开始数，0,1,2...）后，得到matrix，从倒数第二层开始逆序遍历matrix，计算倒数第二层的节点的设备数：根据tempDicts分好的组，获取对应子组织集合，计算当前节点的dev数。依次类推，计算倒数第三层，直到根


            ////
            ////var orgs = GetAllOrgnizationInfo();
            ////var orgDict = orgs.ToDictionary(g => g.Index, g => g);//利用缓存中已存在数据，不要重新查找
            ////var tempDicts = orgs.GroupBy(g => g.ParentIndex).ToDictionary(g => g.Key, g => g.Select(c=>c.Index).ToList());// 

            //////1、计算组织的所属层（节点到根的距离）

            ////List<List<string>> matrix = new List<List<string>>();//按层分好的组织节点的Index ，节省空间
            //////1)获取根列表
            ////List<string> roots = new List<string>();
            ////foreach(var kv in tempDicts)
            ////{
            ////    if(!orgDict.ContainsKey(kv.Key))
            ////    {
            ////        roots.AddRange(kv.Value);
            ////    }
            ////}
            ////matrix.Add(roots);
            ////int orgNum = roots.Count;

            //////2)分层
            ////var orgTotalNum = orgs.Count;
            ////var curRowOrgs = new List<string>(roots);

            ////while (orgNum < orgTotalNum)
            ////{
            ////    var nextRowNodes = new List<string>();
            ////    foreach(var item in curRowOrgs)
            ////    {
            ////        if(tempDicts.ContainsKey(item))//存在子节点
            ////        {
            ////            nextRowNodes.AddRange(tempDicts[item]);
            ////        }
            ////    }
            ////    matrix.Add(nextRowNodes);
            ////    orgNum += nextRowNodes.Count;
            ////    curRowOrgs = new List<string>(nextRowNodes);
            ////}

            //////3、逆序计算所有组织下的设备数
            ////for(int i=matrix.Count-2;i>=0;i--) //倒数第一层的组织是没有子组织的，直接跳过
            ////{
            ////    foreach(var row in matrix)
            ////    {
            ////        foreach(var cell in row)
            ////        {
            ////            var pNode = _cacheOrgs[cell];
            ////            if(tempDicts.ContainsKey(cell))//该组织有子节点
            ////            {
            ////                 foreach(var item in tempDicts[cell])
            ////                {
            ////                    pNode.DevCount+= _cacheOrgs[item].DevCount;
            ////                }
            ////            }
            ////        }
            ////    }
            ////}
            //#endregion
            //#endregion
            #endregion

        }


    }

    }
