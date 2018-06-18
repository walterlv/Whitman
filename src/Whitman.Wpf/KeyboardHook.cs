using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Walterlv.Whitman
{
    internal class KeyboardHook
    {
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int SetWindowsHookEx(int idHook, KeyboardHook.HookProc lpfn, IntPtr hInstance,
            int threadId);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey,
            int fuState);

        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern short GetKeyState(int vKey);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray)] [In]
            KeyboardHook.INPUT[] pInputs, int cbSize);

        public event EventHandler<Keys> KeyUpEvent;

        public event Action<int> OnSpaced;

        public event Action OnBacked;

        public event Action<int> OnPaged;

        public void Start()
        {
            bool flag = KeyboardHook.hKeyboardHook == 0;
            if (flag)
            {
                this.KeyboardHookProcedure = new KeyboardHook.HookProc(this.KeyboardHookProc);
                KeyboardHook.hKeyboardHook = KeyboardHook.SetWindowsHookEx(13, this.KeyboardHookProcedure,
                    KeyboardHook.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                bool flag2 = KeyboardHook.hKeyboardHook == 0;
                if (flag2)
                {
                    this.Stop();
                    throw new Exception("安装键盘钩子失败");
                }
            }
        }

        public void Stop()
        {
            bool flag = true;
            bool flag2 = KeyboardHook.hKeyboardHook != 0;
            if (flag2)
            {
                flag = KeyboardHook.UnhookWindowsHookEx(KeyboardHook.hKeyboardHook);
                KeyboardHook.hKeyboardHook = 0;
            }

            bool flag3 = !flag;
            if (flag3)
            {
                throw new Exception("卸载钩子失败！");
            }
        }

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public void Send(string msg)
        {
            bool flag = !string.IsNullOrEmpty(msg);
            if (flag)
            {
                this.DoqffzpqkTcgchx = msg;
            }
        }

        private string DoqffzpqkTcgchx { get; set; }

        public bool IsStarted { get; set; }

        public event EventHandler Ctrl;

        public event EventHandler CtrlShift;

        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            bool flag = nCode >= 0;
            if (flag)
            {
                bool flag2 = wParam == 257 && !string.IsNullOrEmpty(this.DoqffzpqkTcgchx) && !this._hiqzgypkKpnc;
                if (flag2)
                {
                    KeyboardHook.KeyboardHookStruct keyboardHookStruct =
                        (KeyboardHook.KeyboardHookStruct) Marshal.PtrToStructure(lParam,
                            typeof(KeyboardHook.KeyboardHookStruct));
                    Keys vkCode = (Keys) keyboardHookStruct.vkCode;
                    ModifierKeys modifierKeys = Keyboard.Modifiers;
                    bool flag3 = (vkCode & Keys.Control) != Keys.None || vkCode == Keys.LControlKey;
                    if (flag3)
                    {
                        modifierKeys &= ~ModifierKeys.Control;
                    }

                    bool flag4 = (vkCode & Keys.ShiftKey) != Keys.None || vkCode == Keys.LShiftKey;
                    if (flag4)
                    {
                        modifierKeys &= ~ModifierKeys.Shift;
                    }

                    bool flag5 = modifierKeys == ModifierKeys.None && !this._hiqzgypkKpnc;
                    if (flag5)
                    {
                        this._hiqzgypkKpnc = true;
                        Task.Run(async delegate()
                        {
                            await Task.Delay(100);
                            this.Send();
                            this._hiqzgypkKpnc = false;
                        });
                    }
                }

                bool flag6 = wParam == 256;
                if (flag6)
                {
                    KeyboardHook.KeyboardHookStruct keyboardHookStruct2 =
                        (KeyboardHook.KeyboardHookStruct) Marshal.PtrToStructure(lParam,
                            typeof(KeyboardHook.KeyboardHookStruct));
                    Keys vkCode2 = (Keys) keyboardHookStruct2.vkCode;
                    bool flag7 = vkCode2 == Keys.Oemcomma;
                    if (flag7)
                    {
                        bool flag8 = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                        if (flag8)
                        {
                            bool flag9 = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
                            if (flag9)
                            {
                                EventHandler kpxgbvyDjcpugw = this.CtrlShift;
                                if (kpxgbvyDjcpugw != null)
                                {
                                    kpxgbvyDjcpugw(this, null);
                                }

                                return 1;
                            }

                            EventHandler dzkchqaTbzapmd = this.Ctrl;
                            if (dzkchqaTbzapmd != null)
                            {
                                dzkchqaTbzapmd(this, null);
                            }

                            return 1;
                        }
                    }

                    return 0;
                }
            }

            return KeyboardHook.CallNextHookEx(KeyboardHook.hKeyboardHook, nCode, wParam, lParam);
        }

        private void Send()
        {
            SendKeys.SendWait(this.DoqffzpqkTcgchx);
            this.DoqffzpqkTcgchx = null;
        }

        private static int hKeyboardHook = 0;

        public const int WH_KEYBOARD_LL = 13;

        private KeyboardHook.HookProc KeyboardHookProcedure;

        private const int WM_KEYDOWN = 256;

        private const int WM_KEYUP = 257;

        private const int WM_SYSKEYDOWN = 260;

        private const int WM_SYSKEYUP = 261;

        private bool _hiqzgypkKpnc;

        // (Invoke) Token: 0x0600003A RID: 58
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode;

            public int scanCode;

            public int flags;

            public int time;

            public int dwExtraInfo;
        }

        public struct INPUT
        {
            public InputType dwType;

            public KeyboardHook.KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct KEYBDINPUT
        {
            public short wVk;

            public short wScan;

            public KeyboardHook.KEYEVENTF dwFlags;

            public int time;

            public IntPtr dwExtraInfo;
        }

        public enum KEYEVENTF
        {
            EXTENDEDKEY = 1,
            KEYUP,
            UNICODE = 4,
            SCANCODE = 8
        }
    }
}
