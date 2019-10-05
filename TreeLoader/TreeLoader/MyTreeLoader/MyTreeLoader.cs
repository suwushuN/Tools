using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 接口实现
/// </summary>
namespace MyTreeLoader
{
    /// <summary>
    /// 实现接口ITranslatorNodeInfoToModel，ITreeNodeModel，IUITreeControl    /// 
    /// 异步UI实现
    /// </summary>
    public class MyTreeLoader : ITreeLoader
    {
        public MyTreeLoader(IUITreeControl treeView, ITreeHelper trans)
        {
            _treeView = treeView;
            _trans = trans;
        }
        IUITreeControl _treeView;
        ITreeHelper _trans;
        /// <summary>
        /// 缓存所有组织信息
        /// </summary>
        private Dictionary<string, ITreeNodeModel> _cacheOrgs = new Dictionary<string, ITreeNodeModel>();

        private Dictionary<string, ITreeNodeModel> _cacheDeves = new Dictionary<string, ITreeNodeModel>();

        private List<ITreeNodeModel> _roots = new List<ITreeNodeModel>();

        public bool IsLoaded;


        /// <summary>
        /// 加载树控件
        /// </summary>
        public void LoadTree()
        {
            _cacheDeves.Clear();
            _roots.Clear();
            _cacheOrgs.Clear();
            IsLoaded = false;
            Task.Run(() =>
            {
                LoadOrgNodes();
                LoadDeviceNodes();
                //添加到树控件中            
                _treeView.UIInvoker(() =>
                {//主要是为了wpf
                    _treeView.LoadRoots(_roots);
                });
                IsLoaded = true;
            });
        }

        public void QueryTree(string searchString)
        {
            var pIndexes = new Dictionary<string, byte>();//缓存需要显示的设备对应父节点组织的Index
            foreach (var item in _cacheDeves)
            {
                if (item.Value.NameVM.Contains(searchString))
                {
                    item.Value.IsVisibleVM = true;
                    if (!pIndexes.ContainsKey(item.Value.ParentIndexVM))
                        pIndexes.Add(item.Value.ParentIndexVM, 1);
                }
                else
                {
                    item.Value.IsVisibleVM = false;
                }
            }
            //先设置组织全部隐藏
            foreach (var item in _cacheOrgs)
            {
                item.Value.IsVisibleVM = false;
            }
            var curPIndexes = new Dictionary<string, byte>(pIndexes);//当前需显示组织节点集合
            while (true)
            {
                var tempIndexes = new Dictionary<string, byte>();//上一级需显示组织节点集合
                foreach (var item in curPIndexes)
                {
                    ITreeNodeModel model;
                    if (_cacheOrgs.TryGetValue(item.Key, out model))
                    {
                        if (!tempIndexes.ContainsKey(model.ParentIndexVM))
                            tempIndexes.Add(model.ParentIndexVM, 1);
                        model.IsVisibleVM = false;
                    }
                }
                if (tempIndexes.Count == 0)
                {
                    break;
                }
                curPIndexes = new Dictionary<string, byte>(tempIndexes);
            }
        }

        private void LoadOrgNodes()
        {
            var orgs = _trans.GetAllOrgnizationInfo();
            IList<ITreeNodeModel> roots = new List<ITreeNodeModel>();


            var orgDicts = orgs.ToDictionary(c => c.Index, c => c);
            foreach (var org in orgs)
            {
                //获取org对应节点
                ITreeNodeModel orgNode = GetOrCreateCacheOrgsNode(org);

                //获取父节点
                //1、判断当前节点是否为根节点（根节点的父节点组织信息是不存在的

                if (!orgDicts.ContainsKey(org.ParentIndex))//org为根节点
                {
                    roots.Add(orgNode);//记录根节点。
                    _roots.Add(orgNode);
                    continue;
                }

                //2、创建父节点
                ITreeNodeModel orgParentNode = GetOrCreateCacheOrgsNode(orgDicts[org.ParentIndex]);

                //3、添加到父节点
                orgParentNode.NodesVM.Add(orgNode);
                orgNode.ParentNodeVM = orgParentNode;
            }

        }
        private void LoadDeviceNodes()
        {
            var devs = _trans.GetAllDevicesInfo();
            var devDicts = devs.GroupBy(g => g.ParentIndex).ToDictionary(g => g.Key, g => g.ToList());

            //设备添加到组织，并给组织计算直接挂载的设备节点数量
            foreach (var kv in devDicts)
            {
                if (_cacheOrgs.ContainsKey(kv.Key))
                {
                    ITreeNodeModel orgNode = _cacheOrgs[kv.Key];
                    foreach (var dev in kv.Value)
                    {
                        ITreeNodeModel devNode = _trans.TransFromNodeInfo(dev);
                        orgNode.NodesVM.Add(devNode);
                        _cacheDeves.Add(dev.Index, devNode);
                    }
                    orgNode.DevCountVM = kv.Value.Count;
                }
                else
                {
                    //异常冗余数据
                }
            }

            //way 1 组织的父节点存在，那么该父节点需要增加子节点的设备数据量；，逐层计算

            var orgs = _trans.GetAllOrgnizationInfo();
            var orgDict = orgs.ToDictionary(g => g.Index, g => g);//利用缓存中已存在数据，不要重新查找

            var curOrgHasDevList = new List<string>(devDicts.Keys);//最末节点组织集合，有设备子节点的所有组织
            while (true)
            {
                var orgCountChangeDict = new Dictionary<string, int>();//父组织设备计数变化的集合

                foreach (var item in curOrgHasDevList)
                {
                    if (orgDict.ContainsKey(item))
                    {
                        var orgInfo = orgDict[item];
                        if (orgDict.ContainsKey(orgInfo.ParentIndex))
                        {
                            _cacheOrgs[orgInfo.ParentIndex].DevCountVM += _cacheOrgs[item].DevCountVM;
                            if (!orgCountChangeDict.ContainsKey(orgInfo.ParentIndex))
                            {
                                orgCountChangeDict.Add(orgInfo.ParentIndex, 0);
                            }
                        }
                    }
                }
                if (orgCountChangeDict.Count == 0)
                {
                    break;
                }
                curOrgHasDevList = new List<string>(orgCountChangeDict.Keys);//设置新值
            }

        }

        /// <summary>
        /// 根据信息获取活创建对应的Model节点信息
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        private ITreeNodeModel GetOrCreateCacheOrgsNode(NodeInfoFromDB org)
        {
            ITreeNodeModel model;
            if (!_cacheOrgs.TryGetValue(org.Index, out model))
            {
                model = _trans.TransFromNodeInfo(org);
                _cacheOrgs.Add(org.Index, model);
            }
            return model;

        }

    }
}
