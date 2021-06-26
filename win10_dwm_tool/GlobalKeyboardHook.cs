/* File: GlobalKeyboardHook.cs
 * Proj: Common
 * Date: 20.03.2009
 * Desc: GlobalKeyboardHook - Helper class for global (system-wide) keyboard hooks. 
 * Elem: CLASS GlobalKeyboardHook - -"-
 * Auth: © Johannes Nestler 2009 */


using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace KeyHook
{
    #region CLASS GlobalKeyboardHook

    /// <summary>
    /// Helper class for global (system-wide) keyboard hooks.
    /// </summary>        
    public class GlobalKeyboardHook
    {
        #region TYPES

        #region CLASS KeyboardHookStruct

        /// <summary>
        /// Marshalling of the Windows-API KBDLLHOOKSTRUCT structure.#
        /// Contains information about a low-level keyboard input event.
        /// This is named "struct" to be consistent with the Windows API name,
        /// but it must be a class since it is passed as a pointer in SetWindowsHookEx.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        #endregion // CLASS KeyboardHookStruct

        #region DELEGATE HookProc

        /// <summary>
        /// Represents the method called when a hook catches a monitored event.
        protected delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        #endregion // DELEGATE HookProc

        #endregion // TYPES

        #region CONSTANTS

        const int WH_KEYBOARD_LL = 13;
        const int WH_KEYBOARD = 2;

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        const byte VK_SHIFT = 0x10;
        const byte VK_CAPITAL = 0x14;
        const byte VK_NUMLOCK = 0x90;

        const byte VK_LSHIFT = 0xA0;
        const byte VK_RSHIFT = 0xA1;
        const byte VK_LCONTROL = 0xA2;
        const byte VK_RCONTROL = 0x3;
        const byte VK_LALT = 0xA4;
        const byte VK_RALT = 0xA5;

        // const byte LLKHF_ALTDOWN = 0x20; // not used

        #endregion // CONSTANTS

        #region VARIABLES

        /// <summary>
        /// Value indicating if hook is active.
        /// </summary>
        bool m_bHookActive;

        /// <summary>
        /// Stored hook handle returned by SetWindowsHookEx
        /// </summary>
        int m_iHandleToHook;

        /// <summary>
        /// Stored reference to the HookProc delegate (to prevent delegate from beeing collected by GC!)
        /// </summary>
        protected HookProc m_hookproc;

        #endregion // VARIABLES

        #region EVENTS

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when a key is released.
        /// </summary>
        public event KeyEventHandler KeyUp;
        /// <summary>
        /// Occurs when a character key is pressed.
        /// </summary>
        public event KeyPressEventHandler KeyPress;

        #endregion // EVENTS

        #region CONSTRUCTION & DESTRUCTION

        /// <summary>
        /// Dtor.
        /// </summary>
        ~GlobalKeyboardHook()
        {
            Unhook();
        }

        #endregion // CONSTRUCTION & DESTRUCTION

        #region PROPERTIES

        /// <summary>
        /// Gets a value indicating if hook is active.
        /// </summary>
        public bool HookActive
        {
            get { return m_bHookActive; }
        }

        #endregion // PROPERTIES

        #region METHODS

        /// <summary>
        /// Install the global hook. 
        /// </summary>
        /// <returns> True if hook was successful, otherwise false. </returns>
        public bool Hook()
        {
            if (!m_bHookActive)
            {
                m_hookproc = new HookProc(HookCallbackProcedure);

                IntPtr hInstance = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                m_iHandleToHook = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    m_hookproc,
                    hInstance,
                    0);

                if (m_iHandleToHook != 0)
                {
                    m_bHookActive = true;
                }
            }
            return m_bHookActive;
        }

        /// <summary>
        /// Uninstall the global hook.
        /// </summary>
        public void Unhook()
        {
            if (m_bHookActive)
            {
                UnhookWindowsHookEx(m_iHandleToHook);
                m_bHookActive = false;
            }
        }

        /// <summary>
        /// Raises the KeyDown event.
        /// </summary>
        /// <param name="kea"> KeyEventArgs </param>
        protected virtual void OnKeyDown(KeyEventArgs kea)
        {
            if (KeyDown != null)
                KeyDown(this, kea);
        }

        /// <summary>
        /// Raises the KeyUp event.
        /// </summary>
        /// <param name="kea"> KeyEventArgs </param>
        protected virtual void OnKeyUp(KeyEventArgs kea)
        {
            if (KeyUp != null)
                KeyUp(this, kea);
        }

        /// <summary>
        /// Raises the KeyPress event.
        /// </summary>
        /// <param name="kea"> KeyEventArgs </param>
        protected virtual void OnKeyPress(KeyPressEventArgs kpea)
        {
            if (KeyPress != null)
                KeyPress(this, kpea);
        }

        #endregion // METHODS

        #region EVENTHANDLER

        /// <summary>
        /// Called when hook is active and a key was pressed.
        /// </summary>
        int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            bool bHandled = false;

            if (nCode > -1 && (KeyDown != null || KeyUp != null || KeyPress != null))
            {
                // Get keyboard data
                KeyboardHookStruct khs = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                // Get key states
                bool bControl = ((GetKeyState(VK_LCONTROL) & 0x80) != 0) || ((GetKeyState(VK_RCONTROL) & 0x80) != 0);
                bool bShift = ((GetKeyState(VK_LSHIFT) & 0x80) != 0) || ((GetKeyState(VK_RSHIFT) & 0x80) != 0);
                bool bAlt = ((GetKeyState(VK_LALT) & 0x80) != 0) || ((GetKeyState(VK_RALT) & 0x80) != 0);
                bool bCapslock = (GetKeyState(VK_CAPITAL) != 0);

                // Create KeyEventArgs 
                KeyEventArgs kea = new KeyEventArgs((Keys)(khs.vkCode |
                        (bControl ? (int)Keys.Control : 0) |
                        (bShift ? (int)Keys.Shift : 0) |
                        (bAlt ? (int)Keys.Alt : 0)));

                // Raise KeyDown/KeyUp events
                if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
                {
                    OnKeyDown(kea);
                    bHandled = kea.Handled;
                }
                else if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
                {
                    OnKeyUp(kea);
                    bHandled = kea.Handled;
                }

                // Raise KeyPress event
                if (wParam == WM_KEYDOWN && !bHandled && !kea.SuppressKeyPress)
                {
                    byte[] abyKeyState = new byte[256];
                    byte[] abyInBuffer = new byte[2];
                    GetKeyboardState(abyKeyState);

                    if (ToAscii(khs.vkCode, khs.scanCode, abyKeyState, abyInBuffer, khs.flags) == 1)
                    {
                        char chKey = (char)abyInBuffer[0];
                        if ((bCapslock ^ bShift) && Char.IsLetter(chKey))
                            chKey = Char.ToUpper(chKey);
                        KeyPressEventArgs kpea = new KeyPressEventArgs(chKey);
                        OnKeyPress(kpea);
                        bHandled = kea.Handled;
                    }
                }
            }

            if (bHandled)
                return 1;
            else
                return CallNextHookEx(m_iHandleToHook, nCode, wParam, lParam);
        }

        #endregion // EVENTHANDLER

        #region EXTERN

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern short GetKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion // EXTERN
    }

    #endregion // CLASS GlobalKeyboardHook
}