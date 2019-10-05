using MyTreeLoader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfMyTreeLoader
{
    /// <summary>
    /// MyTreeLoader中的实现接口
    /// </summary>

    class MyTree : IUITreeControl
    {
        TreeView _tv;
        public MyTree(TreeView tv)
        {
            _tv = tv;

        }
        public void LoadRoots(IList<ITreeNodeModel> arr)
        {
            _tv.ItemsSource = arr;
        }

        public void UIInvoker(Action act)
        {
            _tv.Dispatcher.Invoke(() =>
            {
                act.Invoke();
            });
        }

    }

    class CTreeView : TreeView, IUITreeControl
    {
        public void LoadRoots(IList<ITreeNodeModel> arr)
        {
            this.ItemsSource = arr;
        }
        public void UIInvoker(Action act)
        {
            this.Dispatcher.Invoke(() =>
            {
                act.Invoke();
            });
        }
    }

    /// <summary>
    /// 树控件的帮助类
    /// </summary>
    public class MTreeHelper : ITreeHelper
    {
        public static MTreeHelper Instance = new MTreeHelper();

        List<NodeInfoFromDB> AllOrgnizationInfos;

        List<NodeInfoFromDB> AllDeviceInfos;

        public List<NodeInfoFromDB> GetAllDevicesInfo()
        {
            if (AllDeviceInfos == null)
            {
                AllDeviceInfos = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "97万.txt", Encoding.UTF8).Skip(12631)
                           .Select(c =>
                           {
                               var nd = c.Split('&');
                               return new NodeInfoFromDB { Index = nd[0], ParentIndex = nd[2], Name = nd[1], IsOrg = false };
                           })
                           .ToList();
            }

            return AllDeviceInfos;
        }

        public List<NodeInfoFromDB> GetAllOrgnizationInfo()
        {
            if (AllOrgnizationInfos == null)
            {
                AllOrgnizationInfos = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "测试节点.txt", Encoding.UTF8).Take(12631)
                            .Select(c =>
                            {
                                var nd = c.Split('&');
                                return new NodeInfoFromDB { Index = nd[0], ParentIndex = nd[2], Name = nd[1], IsOrg = true, };
                            })
                            .ToList();
            }
            return AllOrgnizationInfos;
        }

        public ITreeNodeModel TransFromNodeInfo(NodeInfoFromDB info)
        {
            return new TreeNodeVM
            {
                IndexVM = info.Index,
                NameVM = info.Name,
                ParentIndexVM = info.ParentIndex,
                IsOrgVM = info.IsOrg,
                NodesVM = info.IsOrg ? new ObservableCollection<ITreeNodeModel>() : null
            };
        }
    }

    /// <summary>
    /// 树绑定对象，需要手动增加PropertyChanged实现数据的动态绑定！！！！！！！
    /// </summary>

    public class TreeNodeVM : ITreeNodeModel, INotifyPropertyChanged
    {
        private string _IndexVM;
        public string IndexVM
        {
            get { return _IndexVM; }
            set
            {
                _IndexVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IndexVM)));
            }
        }
        private string _NameVM;
        public string NameVM
        {
            get
            {
                return _NameVM;
            }
            set
            {
                _NameVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NameVM)));
            }
        }
        private string _ParentIndexVM;
        public string ParentIndexVM
        {
            get
            {
                return _ParentIndexVM;
            }
            set
            {
                _ParentIndexVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NameVM)));
            }
        }

        private ITreeNodeModel _ParentNodeVM;
        public ITreeNodeModel ParentNodeVM
        {
            get { return _ParentNodeVM; }
            set
            {
                _ParentNodeVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParentNodeVM)));
            }
        }



        private ICollection<ITreeNodeModel> _NodesVM;
        public ICollection<ITreeNodeModel> NodesVM
        {
            get { return _NodesVM; }
            set
            {
                _NodesVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NodesVM)));
            }
        }


        private bool _IsOrgVM;
        public bool IsOrgVM
        {
            get { return _IsOrgVM; }
            set
            {
                _IsOrgVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOrgVM)));
            }
        }



        private int _NodeTypeVM;
        public int NodeTypeVM
        {
            get { return _NodeTypeVM; }
            set
            {
                _NodeTypeVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NodeTypeVM)));
            }
        }
              private int _DevCountVM;
        public int DevCountVM
        {
            get { return _DevCountVM; }
            set
            {
                _DevCountVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DevCountVM)));
            }
        }



        private bool _IsVisibleVM;
        public bool IsVisibleVM
        {
            get { return _IsVisibleVM; }
            set
            {
                _IsVisibleVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisibleVM)));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }



}
