using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 内存管理C_sharp
{
    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //转换器
        public class SimpleConverter : IMultiValueConverter
        {
            object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                string first = (string)values[0];
                //bool second = (bool)values[1];
                if (first != "") return true;
                else return false;
            }

            object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        //操作存放容器
        ObservableCollection<RAM> Op = new ObservableCollection<RAM>() {
            new RAM{ ID = "作业A", Size = 130, Opera = "申请"},
            new RAM{ ID = "作业B", Size = 60, Opera = "申请"},
            new RAM{ ID = "作业C", Size = 100, Opera = "申请"},
            new RAM{ ID = "作业B", Size = 60, Opera = "释放"},
            new RAM{ ID = "作业D", Size = 200, Opera = "申请"},
            new RAM{ ID = "作业C", Size = 100, Opera = "释放"},
            new RAM{ ID = "作业A", Size = 130, Opera = "释放"},
            new RAM{ ID = "作业E", Size = 140, Opera = "申请"},
            new RAM{ ID = "作业F", Size = 60, Opera = "申请"},
            new RAM{ ID = "作业G", Size = 50, Opera = "申请"},
            new RAM{ ID = "作业H", Size = 60, Opera = "申请"},
        };
        ObservableCollection<RAM> RealRam = new ObservableCollection<RAM>();
        
        //真正处理容器
        public static List<RAM> ITEM = new List<RAM>();
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            DataGrid table = this.OperaTable as DataGrid;
            table.ItemsSource = Op;
            SetBinding();
        }

        //绑定初始化按钮
        public void SetBinding()
        {
            //准备基础绑定
            Binding b1 = new Binding("Text") { Source = this.Totalsize };
            //Binding b2 = new Binding("IsChecked") { Source = this.dymatic_super };

            //准备MultiBinding
            MultiBinding mb = new MultiBinding() { Mode = BindingMode.OneWay };
            mb.Bindings.Add(b1);
            //mb.Bindings.Add(b2);
            mb.Converter = new SimpleConverter();
            //绑定
            this.initial.SetBinding(Button.IsEnabledProperty, mb);
        }
        //初始化按钮
        private void Initial_Click(object sender, RoutedEventArgs e)
        {
            //判断
            String totalsize = this.Totalsize.Text;
            if(!IsNumeric(totalsize))
            {
                MessageBox.Show("只能为纯数字");
                this.Totalsize.Text = "";
                return;
            }
            RAM new_Ram = new RAM
            {
                ID = "空闲分区",
                Size = int.Parse(totalsize),
                Status = Status.free
            };

            RealRam.Clear();
            RealRam.Add(new_Ram);
            for(int i = 0; i < Op.Count(); i++)
            {
                Op[i].Status = Status.Wait;
            }
            DataGrid table = this.RAMTable as DataGrid;
            table.ItemsSource = RealRam;
            DataGrid Optable = this.OperaTable as DataGrid;
            Optable.ItemsSource = null;
            Optable.ItemsSource = Op;
        }
        //退出按钮
        private void Quit_Click(object sender, RoutedEventArgs e) => System.Environment.Exit(0);
        //回收按钮
        private void Recycle_Click(object sender, RoutedEventArgs e)
        {

            String fid = this.Fid.Text;
            String fsize = this.Fsize.Text;
            //判断是否合法
            if (!IsNumeric(fsize))
            {
                MessageBox.Show("必须为纯数字");
                this.Fsize.Text = "";
                return;
            }                 
            RAM new_ram = new RAM
            {
                ID = fid,
                Size = int.Parse(fsize),
                Opera = "释放",
                Status = Status.Wait
            };
            Op.Add(new_ram);
        }
        //申请按钮
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            string rid = this.Rid.Text;
            string rsize = this.Rsize.Text;
            if (!IsNumeric(rsize))
            {
                MessageBox.Show("必须为纯数字");
                this.Fsize.Text = "";
                return;
            }
            RAM new_ram = new RAM
            {
                ID = rid,
                Size = int.Parse(rsize),
                Opera = "申请",
                Status = Status.Wait
            };
            Op.Add(new_ram);
        }
        //执行按钮
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //准备处理容器
            ITEM.Clear();
            for(int i = 0; i < RealRam.Count(); i++)
            {
                ITEM.Add(RealRam[i]);
            }
            //内存未开辟判定
            if(RealRam.Count() == 0)
            {
                MessageBox.Show("还未初始化内存空间");
                return;
            }
            RAM r = new RAM();
            //得到处理实例
            for(int i = 0; i < Op.Count(); i++)
            {
                if (Op[i].Status == Status.Wait)
                {
                    r = Op[i];
                    break;
                }
            }
            //特殊判定
            if (r.Opera == "释放" && !IsExist(r.ID,ITEM))
            {
                MessageBox.Show("ID不存在");
                r.Status = Status.Failed;
                return;
            }
            //申请操作
            if(r.Opera == "申请")
            {
                if (this.ff.IsChecked == true) r.Status = RAM.First_fit(ITEM, r.ID, r.Size);
                else if (this.nf.IsChecked == true) r.Status = RAM.Next_fit(ITEM, r.ID, r.Size);
                else if (this.bf.IsChecked == true) r.Status = RAM.Best_fit(ITEM, r.ID, r.Size);
                else if (this.wf.IsChecked == true) r.Status = RAM.Worst_fit(ITEM, r.ID, r.Size);
            }
            //回收操作
            if (r.Opera == "释放")
                r.Status = RAM.Recycle(ITEM, r.ID, r.Size);
            //重新放回元素
            RealRam.Clear();
            for(int i = 0; i < ITEM.Count(); i++)
            {
                RealRam.Add(ITEM[i]);
            }
            DataGrid table = this.OperaTable as DataGrid;
            table.ItemsSource = null;
            table.ItemsSource = Op;
        }
        //判断是否为纯数字
        bool IsNumeric(string str) //接收一个string类型的参数,保存到str里
        {
            if (str == null || str.Length == 0)    //验证这个参数是否为空
                return false;                           //是，就返回False
            ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例
            byte[] bytestr = ascii.GetBytes(str);         //把string类型的参数保存到数组里

            foreach (byte c in bytestr)                   //遍历这个数组里的内容
            {
                if (c < 48 || c > 57)                          //判断是否为数字
                {
                    return false;                              //不是，就返回False
                }
            }
            return true;                                        //是，就返回True
        }
        //判断ID是否存在
        public static bool IsExist(string str,List<RAM> RealRam)
        {
            int n = RealRam.Count();
            for (int i = 0; i < n; i++)
            {
                if (str == RealRam[i].ID)
                    return true;
            }
            return false;
        }
    }
}
