using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Media;

namespace win10_dwm_tool
{
    public partial class Form1 : Form
    {
        //Config files
        private const string CONFIG_FILE = "win10_dwm_tool.ini";
        private const string CONFIG_FILE_HK = "win10_dwm_tool_hk.ini";

        //Sound files (optional)
        private const string SNDFILE_DWM_ON = "dwm_on.wav";
        private const string SNDFILE_DWM_OFF = "dwm_off.wav";

        //Process list, loaded from config
        private List<string> kill_proc_list = new List<string>();

        //Hotkey
        private KeyHook.GlobalKeyboardHook gkh;
        private Keys hk_dwm_on, hkmod_dwm_on;
        private Keys hk_dwm_off, hkmod_dwm_off;

        //Sounds
        SoundPlayer snd_dwm_on, snd_dwm_off;

        //Console textbox
        private static TextBox st_tb_console;

        public Form1()
        {
            InitializeComponent();
            st_tb_console = tb_console;
            //Process kill list
            LoadConfig(CONFIG_FILE, sr =>
            {
                while (sr.Peek() > -1)
                {
                    string line = sr.ReadLine();
                    if (line.Length > 0) kill_proc_list.Add(line);
                }
            });
            //Hotkeys
            LoadConfig(CONFIG_FILE_HK, sr =>
            {
                if (sr.Peek() > -1) ParseHotkey(sr.ReadLine(), out hk_dwm_on, out hkmod_dwm_on);
                if (sr.Peek() > -1) ParseHotkey(sr.ReadLine(), out hk_dwm_off, out hkmod_dwm_off);
                gkh = new KeyHook.GlobalKeyboardHook();
                gkh.KeyUp += Gkh_KeyUp;
                gkh.Hook();
            });
            //Sounds
            snd_dwm_on = LoadAudio(SNDFILE_DWM_ON);
            snd_dwm_off = LoadAudio(SNDFILE_DWM_OFF);
        }

        //Event handlers
        private void Gkh_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == hk_dwm_on && (hkmod_dwm_on == Keys.None || e.Modifiers == hkmod_dwm_on))
            {
                ClearConsole();
                RestoreDWM();
                snd_dwm_on?.Play();
            }
            else if (e.KeyCode == hk_dwm_off && (hkmod_dwm_off == Keys.None || e.Modifiers == hkmod_dwm_off))
            {
                ClearConsole();
                DisableDWM();
                snd_dwm_off?.Play();
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
            DisableDWM();
            tmr_restore.Enabled = true;
            if (MessageBox.Show("Press Yes if you can in 5 seconds.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                tmr_restore.Enabled = false;
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

        //Functions
        private void LoadConfig(string fname, Action<StreamReader> hndl)
        {
            try
            {
                WriteToConsole($"Loading: {fname}");
                if (!File.Exists(fname))
                {
                    WriteToConsole($"Not found: {fname}");
                    return;
                }
                using (StreamReader sr = new StreamReader(fname)) hndl(sr);
                WriteToConsole("OK");
            }
            catch (Exception ex)
            {
                WriteToConsole($"Failed to load {fname} file:");
                WriteToConsole(ex.Message);
            }
        }

        private SoundPlayer LoadAudio(string fname)
        {
            try
            {
                WriteToConsole($"Loading: {fname}");
                if (!File.Exists(fname)) WriteToConsole($"Not found: {fname}");
                else
                {
                    WriteToConsole("OK");
                    return new SoundPlayer(fname);
                }
            }
            catch(Exception ex)
            {
                WriteToConsole($"Failed to load {fname} file:");
                WriteToConsole(ex.Message);
            }
            return null;
        }

        private void ParseHotkey(string input, out Keys key, out Keys modkey)
        {
            modkey = default(Keys);
            string[] tmp = input.Split('+');
            int len = tmp.Length - 1;
            if (len > 0)
            {
                for (int i = 0; i < len; i++)
                    modkey |= (Keys)Enum.Parse(typeof(Keys), tmp[i], true);
            }
            key = (Keys)Enum.Parse(typeof(Keys), tmp[len], true);
        }

        private void DisableDWM()
        {
            if (!ProcessHandler.ProcessExists("dwm")) return;
            ProcessHandler.KillProcess("explorer.exe");
            foreach (string p in kill_proc_list)
                ProcessHandler.KillProcess(p);
            ProcessHandler.SuspendProcess("winlogon");
            ProcessHandler.KillProcess("dwm.exe");
        }

        public static void RestoreDWM()
        {
            if (ProcessHandler.ProcessExists("dwm")) return;
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