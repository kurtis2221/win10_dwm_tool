using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace win10_dwm_tool
{
    public class ProcessHandler
    {
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        public static void StartProcess(string procname)
        {
            try
            {
                Form1.WriteToConsole($"Start: {procname}");
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = procname;
                psi.UseShellExecute = true;
                Process.Start(psi);
                Form1.WriteToConsole("OK");
            }
            catch (Exception ex)
            {
                Form1.WriteToConsole($"Failed: {ex.Message}");
            }
        }

        public static void StartExplorer()
        {
            try
            {
                string procname = Form1.FILE_EXPLORER;
                Form1.WriteToConsole($"Start: {procname}");
                //It only worked with this on explorer.exe
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = Path.Combine(Form1.ENV_WINDOWS, procname);
                psi.UseShellExecute = true;
                Process.Start(psi);
                Form1.WriteToConsole("OK");
            }
            catch (Exception ex)
            {
                Form1.WriteToConsole($"Failed: {ex.Message}");
            }
        }

        public static void KillProcess(string procname)
        {
            try
            {
                Form1.WriteToConsole($"Kill: {procname}");
                //It only worked with this on explorer.exe
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd";
                psi.UseShellExecute = true;
                psi.Arguments = $"/C taskkill /F /IM {procname}";
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi);
                Form1.WriteToConsole("OK");
            }
            catch (Exception ex)
            {
                Form1.WriteToConsole($"Failed: {ex.Message}");
            }
        }

        public static bool ProcessExists(string procname)
        {
            try
            {
                Form1.WriteToConsole($"Check: {procname}");
                return Process.GetProcessesByName(procname).Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public static void SuspendProcess(string procname)
        {
            try
            {
                Form1.WriteToConsole($"Suspend: {procname}");
                Process process = Process.GetProcessesByName(procname)[0];
                foreach (ProcessThread pT in process.Threads)
                {
                    IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                    if (pOpenThread == IntPtr.Zero)
                    {
                        continue;
                    }
                    SuspendThread(pOpenThread);
                    CloseHandle(pOpenThread);
                }
                Form1.WriteToConsole("OK");
            }
            catch (Exception ex)
            {
                Form1.WriteToConsole($"Failed: {ex.Message}");
            }
        }

        public static void ResumeProcess(string procname)
        {
            try
            {
                Form1.WriteToConsole($"Resume: {procname}");
                Process process = Process.GetProcessesByName(procname)[0];
                foreach (ProcessThread pT in process.Threads)
                {
                    IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                    if (pOpenThread == IntPtr.Zero)
                    {
                        continue;
                    }
                    var suspendCount = 0;
                    do
                    {
                        suspendCount = ResumeThread(pOpenThread);
                    }
                    while (suspendCount > 0);
                    CloseHandle(pOpenThread);
                }
                Form1.WriteToConsole("OK");
            }
            catch (Exception ex)
            {
                Form1.WriteToConsole($"Failed: {ex.Message}");
            }
        }
    }
}