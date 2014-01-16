using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices; //USED TO CALL THE DLL IMPORTS
using System.Diagnostics;

//Class wasnt done by me
//FULL CREDITS TO 
//http://www.blizzhackers.cc/viewtopic.php?t=396398

namespace TAOS
{
    //constructor loads all the dll's
    class KeyboardHook
    {
        private static class API
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(
                int idHook,
                HookDel lpfn,
                IntPtr hMod,
                uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(
                IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(
                IntPtr hhk,
                int nCode,
                IntPtr
                wParam,
                IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(
                string lpModuleName);

        }
        public enum VK
        {
            //Keycodes recieved from this website:
            //http://delphi.about.com/od/objectpascalide/l/blvkc.htm

            //I've commented out the keys that I've never heard of--feel free to uncomment them if you wish

            VK_LBUTTON = 0X01, //Left mouse
            VK_RBUTTON = 0X02, //Right mouse
            //VK_CANCEL       = 0X03,
            VK_MBUTTON = 0X04,
            VK_BACK = 0X08, //Backspace
            VK_TAB = 0X09,
            //VK_CLEAR        = 0X0C,
            VK_RETURN = 0X0D, //Enter
            VK_SHIFT = 0X10,
            VK_CONTROL = 0X11, //CTRL
            //VK_MENU         = 0X12,
            VK_PAUSE = 0X13,
            VK_CAPITAL = 0X14, //Caps-Lock
            VK_ESCAPE = 0X1B,
            VK_SPACE = 0X20,
            VK_PRIOR = 0X21, //Page-Up
            VK_NEXT = 0X22, //Page-Down
            VK_END = 0X23,
            VK_HOME = 0X24,
            VK_LEFT = 0X25,
            VK_UP = 0X26,
            VK_RIGHT = 0X27,
            VK_DOWN = 0X28,
            //VK_SELECT       = 0X29,
            //VK_PRINT        = 0X2A,
            //VK_EXECUTE      = 0X2B,
            VK_SNAPSHOT = 0X2C, //Print Screen
            VK_INSERT = 0X2D,
            VK_DELETE = 0X2E,
            //VK_HELP         = 0X2F,

            VK_0 = 0X30,
            VK_1 = 0X31,
            VK_2 = 0X32,
            VK_3 = 0X33,
            VK_4 = 0X34,
            VK_5 = 0X35,
            VK_6 = 0X36,
            VK_7 = 0X37,
            VK_8 = 0X38,
            VK_9 = 0X39,

            VK_A = 0X41,
            VK_B = 0X42,
            VK_C = 0X43,
            VK_D = 0X44,
            VK_E = 0X45,
            VK_F = 0X46,
            VK_G = 0X47,
            VK_H = 0X48,
            VK_I = 0X49,
            VK_J = 0X4A,
            VK_K = 0X4B,
            VK_L = 0X4C,
            VK_M = 0X4D,
            VK_N = 0X4E,
            VK_O = 0X4F,
            VK_P = 0X50,
            VK_Q = 0X51,
            VK_R = 0X52,
            VK_S = 0X53,
            VK_T = 0X54,
            VK_U = 0X55,
            VK_V = 0X56,
            VK_W = 0X57,
            VK_X = 0X58,
            VK_Y = 0X59,
            VK_Z = 0X5A,

            VK_NUMPAD0 = 0X60,
            VK_NUMPAD1 = 0X61,
            VK_NUMPAD2 = 0X62,
            VK_NUMPAD3 = 0X63,
            VK_NUMPAD4 = 0X64,
            VK_NUMPAD5 = 0X65,
            VK_NUMPAD6 = 0X66,
            VK_NUMPAD7 = 0X67,
            VK_NUMPAD8 = 0X68,
            VK_NUMPAD9 = 0X69,


            VK_ADD = 0X6B, // + key
            VK_SEPERATOR = 0X6C, // | (shift + backslash)
            VK_SUBTRACT = 0X6D, // -
            VK_DECIMAL = 0X6E, // .
            VK_DIVIDE = 0X6F, // /

            VK_F1 = 0X70,
            VK_F2 = 0X71,
            VK_F3 = 0X72,
            VK_F4 = 0X73,
            VK_F5 = 0X74,
            VK_F6 = 0X75,
            VK_F7 = 0X76,
            VK_F8 = 0X77,
            VK_F9 = 0X78,
            VK_F10 = 0X79,
            VK_F11 = 0X7A,
            VK_F12 = 0X7B, //I only went up to F12, because honestly, who the hell has 24 F buttons?
            //and for the 8 people in the world who do, I think they can live without using them

            VK_NUMLOCK = 0X90,
            VK_SCROLL = 0X91, //Scroll-Lock
            VK_LSHIFT = 0XA0,
            VK_RSHIFT = 0XA1,
            VK_LCONTROL = 0XA2,
            VK_RCONTROL = 0XA3,
        } //keycodes


        public delegate IntPtr HookDel(
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        public delegate void KeyHandler(
            IntPtr wParam,
            IntPtr lParam);

        private static IntPtr hhk = IntPtr.Zero;
        private static HookDel hd;
        private static KeyHandler kh;

        public static void CreateHook(KeyHandler _kh)
        {
            System.Diagnostics.Process _this = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.ProcessModule mod = _this.MainModule;
            hd = HookFunc;
            kh = _kh;

            hhk = API.SetWindowsHookEx(13, hd, API.GetModuleHandle(mod.ModuleName), 0);
            //13 is the parameter specifying that we're gonna do a low-level keyboard hook

            //MessageBox.Show(Marshal.GetLastWin32Error().ToString()); //for debugging
            //Note that this could be a Console.WriteLine(), as well. I just happened
            //to be debugging this in a Windows Application
            //to get the errors, in VS 2005+ (possibly before) do Tools -> Error Lookup
        }

        public static bool DestroyHook()
        {
            //to be called when we're done with the hook
            return API.UnhookWindowsHookEx(hhk);
        }

        private static IntPtr HookFunc(
            int nCode,
            IntPtr wParam,
            IntPtr lParam)
        {
            int iwParam = wParam.ToInt32();
            if (nCode >= 0 &&
                (iwParam == 0x100 ||
                iwParam == 0x104)) //0x100 = WM_KEYDOWN, 0x104 = WM_SYSKEYDOWN
                kh(wParam, lParam);

            return API.CallNextHookEx(hhk, nCode, wParam, lParam);
        }

        public static string checkMatchKey(int key)
        {
            string temp;
            VK vk = (VK)key;
            switch (vk)
            {
                case VK.VK_F1: temp = "<-F1->";
                    break;
                case VK.VK_F2: temp = "<-F2->";
                    break;
                case VK.VK_F3: temp = "<-F3->";
                    break;
                case VK.VK_F4: temp = "<-F4->";
                    break;
                case VK.VK_F5: temp = "<-F5->";
                    break;
                case VK.VK_F6: temp = "<-F6->";
                    break;
                case VK.VK_F7: temp = "<-F7->";
                    break;
                case VK.VK_F8: temp = "<-F8->";
                    break;
                case VK.VK_F9: temp = "<-F9->";
                    break;
                case VK.VK_F10: temp = "<-F10->";
                    break;
                case VK.VK_F11: temp = "<-F11->";
                    break;
                case VK.VK_F12: temp = "<-F12->";
                    break;
                case VK.VK_NUMLOCK: temp = "<-numlock->";
                    break;
                case VK.VK_SCROLL: temp = "<-scroll>";
                    break;
                case VK.VK_LSHIFT: temp = "<-left shift->";
                    break;
                case VK.VK_RSHIFT: temp = "<-right shift->";
                    break;
                case VK.VK_LCONTROL: temp = "<-left control->";
                    break;
                case VK.VK_RCONTROL: temp = "<-right control->";
                    break;
                case VK.VK_SEPERATOR: temp = "|";
                    break;
                case VK.VK_SUBTRACT: temp = "-";
                    break;
                case VK.VK_ADD: temp = "+";
                    break;
                case VK.VK_DECIMAL: temp = ".";
                    break;
                case VK.VK_DIVIDE: temp = "/";
                    break;
                case VK.VK_NUMPAD0: temp = "0";
                    break;
                case VK.VK_NUMPAD1: temp = "1";
                    break;
                case VK.VK_NUMPAD2: temp = "2";
                    break;
                case VK.VK_NUMPAD3: temp = "3";
                    break;
                case VK.VK_NUMPAD4: temp = "4";
                    break;
                case VK.VK_NUMPAD5: temp = "5";
                    break;
                case VK.VK_NUMPAD6: temp = "6";
                    break;
                case VK.VK_NUMPAD7: temp = "7";
                    break;
                case VK.VK_NUMPAD8: temp = "8";
                    break;
                case VK.VK_NUMPAD9: temp = "9";
                    break;
                case VK.VK_Q: temp = "q";
                    break;
                case VK.VK_W: temp = "w";
                    break;
                case VK.VK_E: temp = "e";
                    break;
                case VK.VK_R: temp = "r";
                    break;
                case VK.VK_T: temp = "t";
                    break;
                case VK.VK_Y: temp = "y";
                    break;
                case VK.VK_U: temp = "u";
                    break;
                case VK.VK_I: temp = "i";
                    break;
                case VK.VK_O: temp = "o";
                    break;
                case VK.VK_P: temp = "p";
                    break;
                case VK.VK_A: temp = "a";
                    break;
                case VK.VK_S: temp = "s";
                    break;
                case VK.VK_D: temp = "d";
                    break;
                case VK.VK_F: temp = "f";
                    break;
                case VK.VK_G: temp = "g";
                    break;
                case VK.VK_H: temp = "h";
                    break;
                case VK.VK_J: temp = "j";
                    break;
                case VK.VK_K: temp = "k";
                    break;
                case VK.VK_L: temp = "l";
                    break;
                case VK.VK_Z: temp = "z";
                    break;
                case VK.VK_X: temp = "x";
                    break;
                case VK.VK_C: temp = "c";
                    break;
                case VK.VK_V: temp = "v";
                    break;
                case VK.VK_B: temp = "b";
                    break;
                case VK.VK_N: temp = "n";
                    break;
                case VK.VK_M: temp = "m";
                    break;
                case VK.VK_0: temp = "0";
                    break;
                case VK.VK_1: temp = "1";
                    break;
                case VK.VK_2: temp = "2";
                    break;
                case VK.VK_3: temp = "3";
                    break;
                case VK.VK_4: temp = "4";
                    break;
                case VK.VK_5: temp = "5";
                    break;
                case VK.VK_6: temp = "6";
                    break;
                case VK.VK_7: temp = "7";
                    break;
                case VK.VK_8: temp = "8";
                    break;
                case VK.VK_9: temp = "9";
                    break;
                case VK.VK_SNAPSHOT: temp = "<-print screen->";
                    break;
                case VK.VK_INSERT: temp = "<-insert->";
                    break;
                case VK.VK_DELETE: temp = "<-delete->";
                    break;
                case VK.VK_BACK: temp = "<-backspace->";
                    break;
                case VK.VK_TAB: temp = "<-tab->";
                    break;
                case VK.VK_RETURN: temp = "<-enter->";
                    break;
                case VK.VK_PAUSE: temp = "<-pause->";
                    break;
                case VK.VK_CAPITAL: temp = "<-caps lock->";
                    break;
                case VK.VK_ESCAPE: temp = "<-esc->";
                    break;
                case VK.VK_SPACE: temp = " "; //was <-space->
                    break;
                case VK.VK_PRIOR: temp = "<-page up->";
                    break;
                case VK.VK_NEXT: temp = "<-page down->";
                    break;
                case VK.VK_END: temp = "<-end->";
                    break;
                case VK.VK_HOME: temp = "<-home->";
                    break;
                case VK.VK_LEFT: temp = "<-arrow left->";
                    break;
                case VK.VK_UP: temp = "<-arrow up->";
                    break;
                case VK.VK_RIGHT: temp = "<-arrow right->";
                    break;
                case VK.VK_DOWN: temp = "<-arrow down->";
                    break;
                default: temp = "no"; break;
            }
            return temp;
        }

    }
}
