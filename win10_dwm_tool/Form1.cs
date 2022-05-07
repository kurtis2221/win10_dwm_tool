using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Media;
using System.Security.AccessControl;
using System.Security.Principal;

namespace win10_dwm_tool
{
    public partial class Form1 : Form
    {
        //Environment vars
        internal readonly static string ENV_WINDOWS = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        internal readonly static string ENV_SYSTEM32 = Environment.GetFolderPath(Environment.SpecialFolder.System);

        //Config files
        private const string CONFIG_FILE = "win10_dwm_tool.ini";
        private const string CONFIG_FILE_HK = "win10_dwm_tool_hk.ini";
        private const string CONFIG_FILE_WIN11 = "win11_dwm_tool.ini";

        //Sound files (optional)
        private const string SNDFILE_DWM_ON = "dwm_on.wav";
        private const string SNDFILE_DWM_OFF = "dwm_off.wav";

        //Windows 10
        private const string PROC_DWM = "dwm";
        private const string PROC_WINLOGON = "winlogon";
        //
        private const string FILE_DWM = "dwm.exe";
        internal const string FILE_EXPLORER = "explorer.exe";
        internal const string FILE_CMD = "cmd.exe";

        //Windows 11
        private const string FILE_DWMINIT = "dwminit.dll";
        private const string FILE_WINDOWS_UI_LOGON = "Windows.UI.Logon.dll";

        //TrustedInstaller user
        private const string USER_TRUSTEDINSTALLER = "NT SERVICE\\TrustedInstaller";

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
        private static CheckBox st_ch_win11_mouse;
        private static CheckBox st_ch_win11_login;

        public Form1()
        {
            InitializeComponent();
            st_tb_console = tb_console;
            st_ch_win11_mouse = ch_win11_mouse;
            st_ch_win11_login = ch_win11_login;
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
            //Windows 11
            LoadConfig(CONFIG_FILE_WIN11, sr =>
            {
                if (sr.Peek() > -1) ch_win11_mouse.Checked = sr.ReadLine() != "0";
                if (sr.Peek() > -1) ch_win11_login.Checked = sr.ReadLine() != "0";
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
            catch (Exception ex)
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
            if (!ProcessHandler.ProcessExists(PROC_DWM)) return;
            //Windows 11
            if (st_ch_win11_mouse.Checked) RenameSystemFile(true, FILE_DWMINIT);
            if (st_ch_win11_login.Checked) RenameSystemFile(true, FILE_WINDOWS_UI_LOGON);
            //
            ProcessHandler.KillProcess(FILE_EXPLORER);
            foreach (string p in kill_proc_list)
                ProcessHandler.KillProcess(p);
            ProcessHandler.SuspendProcess(PROC_WINLOGON);
            ProcessHandler.KillProcess(FILE_DWM);
        }

        public static void RestoreDWM()
        {
            if (ProcessHandler.ProcessExists(PROC_DWM)) return;
            //Windows 11
            if (st_ch_win11_mouse.Checked) RenameSystemFile(false, FILE_DWMINIT);
            if (st_ch_win11_login.Checked) RenameSystemFile(false, FILE_WINDOWS_UI_LOGON);
            //
            ProcessHandler.ResumeProcess(PROC_WINLOGON);
            ProcessHandler.StartExplorer();
        }

        private static void RenameSystemFile(bool input, string fname)
        {
            try
            {
                //Check if rename is needed
                string source = Path.Combine(ENV_SYSTEM32, (input ? "" : "_") + fname);
                if (!File.Exists(source)) return;
                string target = Path.Combine(ENV_SYSTEM32, (input ? "_" : "") + fname);
                string cmd = Path.Combine(ENV_SYSTEM32, FILE_CMD);
                WriteToConsole($"Moving: {fname}");
                //Rename with Task Scheduler
                TaskScheduler.RunTask(cmd, $"/C move \"{source}\" \"{target}\"", USER_TRUSTEDINSTALLER);
                WriteToConsole("OK");
            }
            catch (Exception ex)
            {
                WriteToConsole($"Failed to move file: {fname}");
                WriteToConsole(ex.Message);
            }
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