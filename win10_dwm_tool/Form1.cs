using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace win10_dwm_tool
{
    public partial class Form1 : Form
    {
        private const string CONFIG_FILE = "win10_dwm_tool.ini";
        private List<string> kill_proc_list = new List<string>();
        private static TextBox st_tb_console;

        public Form1()
        {
            InitializeComponent();
            st_tb_console = tb_console;
            try
            {
                WriteToConsole($"Loading: {CONFIG_FILE}");
                if (!File.Exists(CONFIG_FILE))
                {
                    WriteToConsole($"Not found: {CONFIG_FILE}");
                    return;
                }
                using (StreamReader sr = new StreamReader(CONFIG_FILE))
                {
                    while (sr.Peek() > -1)
                    {
                        string line = sr.ReadLine();
                        if(line.Length > 0) kill_proc_list.Add(line);
                    }
                }
                WriteToConsole("OK");
            }
            catch (Exception ex)
            {
                WriteToConsole($"Failed to load {CONFIG_FILE} file:");
                WriteToConsole(ex.Message);
            }
        }

        private void bt_dwm_on_Click(object sender, EventArgs e)
        {
            ClearConsole();
            RestoreDWM();
        }

        private void bt_dwm_off_Click(object sender, EventArgs e)
        {
            ClearConsole();
            if(!ProcessHandler.ProcessExists("dwm"))
            {
                return;
            }
            ProcessHandler.KillProcess("explorer.exe");
            foreach (string p in kill_proc_list)
                ProcessHandler.KillProcess(p);
            ProcessHandler.SuspendProcess("winlogon");
            ProcessHandler.KillProcess("dwm.exe");
            tmr_restore.Enabled = true;
            if (MessageBox.Show("Press Yes if you can in 5 seconds.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                tmr_restore.Enabled = false;
            }
        }

        private void bt_run_Click(object sender, EventArgs e)
        {
            ClearConsole();
            ProcessHandler.StartProcess(tb_run.Text);
        }

        private void tmr_restore_Tick(object sender, EventArgs e)
        {
            tmr_restore.Enabled = false;
            RestoreDWM();
        }

        public static void RestoreDWM()
        {
            if(ProcessHandler.ProcessExists("dwm"))
            {
                return;
            }
            ProcessHandler.ResumeProcess("winlogon");
            ProcessHandler.StartExplorer();
        }

        public static void ClearConsole()
        {
            st_tb_console.Text = string.Empty;
        }

        public static void WriteToConsole(string text)
        {
            st_tb_console.Text += text + Environment.NewLine;
        }
    }
}