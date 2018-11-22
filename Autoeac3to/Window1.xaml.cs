using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Autoeac3to
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string fileName = @"F:/zmz/eac3to/eac3to.exe";
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = fileName;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = @"X:\[BDMV][アニメ][160722] マクロスΔ 01\BDROM\BDMV\STREAM\00010.m2ts";//参数以空格分隔，如果某个参数为空，可以传入””  
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            textbox.Text = output;
        }
    }
}
