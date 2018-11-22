using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Autoeac3to
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Cmd 的摘要说明。
        /// </summary>
        public class Cmd
        {
            private Process proc = null;
            /// <summary>
            /// 构造方法
            /// </summary>
            public Cmd()
            {
                proc = new Process();
            }
            /// <summary>
            /// 执行CMD语句
            /// </summary>
            /// <param name="cmd">要执行的CMD命令</param>
            public string RunCmd(string cmd)
            {
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardInput = true; 
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();
                proc.StandardInput.WriteLine(cmd);
                proc.StandardInput.WriteLine("exit");
                string outStr = proc.StandardOutput.ReadToEnd();
                proc.Close();
                return outStr;
            }
            /// <summary>
            /// 打开软件并执行命令
            /// </summary>
            /// <param name="programName">软件路径加名称（.exe文件）</param>
            /// <param name="cmd">要执行的命令</param>
            public void RunProgram(string programName, string cmd)
            {
                Process proc = new Process();
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = programName;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();
                if (cmd.Length != 0)
                {
                    proc.StandardInput.WriteLine(cmd);
                }
                proc.Close();
            }
            /// <summary>
            /// 打开软件
            /// </summary>
            /// <param name="programName">软件路径加名称（.exe文件）</param>
            public void RunProgram(string programName)
            {
                this.RunProgram(programName, "");
            }
        }
        List<string> cmdo = new List<string>();
        private FileStream currentFs;
        private async void Create(string root)
        {
            Button1.IsEnabled = false;
            string eac3topatch1 = eac3topath.Text;
            var files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            pb2.Maximum = files.Length;
            outtxt.Text = "";
            cmdo.Clear();
            await Task.Run(() =>
            {
                List<string> filies = new List<string>();
                for (int i = 0; i < files.Length; i++)
                {
                    var f = files[i];
                    using (var fs = File.OpenRead(f))
                    {
                        currentFs = fs;
                        if (f.Substring(f.LastIndexOf(".") + 1, (f.Length - f.LastIndexOf(".") - 1)) == "mpls")
                        {
                            var line = $"\"{eac3topatch1}\\eac3to\" \"{f}\"";
                            var link = f;
                            string zz1 = @"Chapters";
                            List<string> cmdp = new List<string>();
                            cmdp.Clear();
                            Cmd c = new Cmd();
                            string paragraph = c.RunCmd(line.Trim());
                            string cmdtxt1 = $"{line}";
                            foreach (Match match in Regex.Matches(paragraph, zz1))
                            {
                                string li = link.Substring(0, (link.LastIndexOf(".")));
                                cmdtxt1 = $"{cmdtxt1} 1:\"{li}.txt\"";
                                string cmd1 = c.RunCmd(cmdtxt1.Trim());
                                cmdo.Add(cmd1);
                            }
                        }
                            if (f.Substring(f.LastIndexOf(".") + 1, (f.Length - f.LastIndexOf(".") - 1)) == "m2ts")
                        {
                            var line = $"\"{eac3topatch1}\\eac3to\" \"{f}\"";
                            var link = f;
                            List<string> cmdp = new List<string>();
                            cmdp.Clear();
                            string zz1 = @"[0-9].*?(?=\r)";
                            string zz2 = @".*?(?=:)";
                            Cmd c = new Cmd();
                            string paragraph = c.RunCmd(line.Trim());
                            string cmdtxt1 = $"{line}";
                            foreach (Match match in Regex.Matches(paragraph, zz1))
                            {
                                string relse = match.Value;
                                if (relse.Contains("RAW"))
                                {
                                    string li = link.Substring(0, (link.LastIndexOf(".")));
                                    Match match1 = Regex.Match(relse, zz2);
                                    string test = match1.Value;
                                    var output = $"{test}:\"{li}_{test}.flac\"";
                                    cmdp.Add(output);
                                }
                                if (relse.Contains("DTS"))
                                {
                                    string li = link.Substring(0, (link.LastIndexOf(".")));
                                    Match match1 = Regex.Match(relse, zz2);
                                    string test = match1.Value;
                                    var output = $"{test}:\"{li}_{test}.dtsma\"";
                                    cmdp.Add(output);
                                }
                                if (relse.Contains("TrueHD"))
                                {
                                    string li = link.Substring(0, (link.LastIndexOf(".")));
                                    Match match1 = Regex.Match(relse, zz2);
                                    string test = match1.Value;
                                    var output = $"{test}:\"{li}_{test}.thd\"";
                                    cmdp.Add(output);
                                }
                                else
                                {
                                    if (relse.Contains("AC3"))
                                    {
                                        string li = link.Substring(0, (link.LastIndexOf(".")));
                                        Match match1 = Regex.Match(relse, zz2);
                                        string test = match1.Value;
                                        var output = $"{test}:\"{li}_{test}.ac3\"";
                                        cmdp.Add(output);
                                    }
                                }
                                if (relse.Contains("Subtitle"))
                                {
                                    string li = link.Substring(0, (link.LastIndexOf(".")));
                                    Match match1 = Regex.Match(relse, zz2);
                                    string test = match1.Value;
                                    var output = $"{test}:\"{li}_{test}.sup\"";
                                    cmdp.Add(output);
                                }
                            }
                            foreach (string txt in cmdp)
                            {
                                cmdtxt1 = $"{cmdtxt1} {txt}";
                            }
                            string cmd1 = c.RunCmd(cmdtxt1.Trim());
                            cmdo.Add(cmd1);
                        }
                        Dispatcher.Invoke(() => pb2.Value = i + 1);
                    }
                }
            });
            string outpu = "";
            foreach (string txt in cmdo)
            {
                outpu = $"{outpu}{txt}";
            }
            outtxt.Text = outpu;
            Button1.IsEnabled = true;
            outtxt.IsEnabled = true;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                cmdtxt.Text = foldPath;
            }
            if (cmdtxt.Text != "")
            {
                Create(cmdtxt.Text);
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                eac3topath.Text = foldPath;
                Button1.IsEnabled = true;
            }
        }
    }
}
