namespace Epsitec.Common.Widgets.Platform
{
    /// <summary>
    /// La classe WindowList permet de construire la liste (ordrée) des fenêtres
    /// visibles.
    /// </summary>
    public class WindowList
    {
        // ********************************************************************
        // TODO bl-net8-cross maybedelete
        // - implement WindowList (stub)
        // ********************************************************************
        private WindowList() { }

        static System.Collections.ArrayList windows = new System.Collections.ArrayList();

        internal static void Insert(Platform.PlatformWindow window)
        {
            WindowList.windows.Add(window);
        }

        internal static void Remove(Platform.PlatformWindow window)
        {
            WindowList.windows.Remove(window);
        }

        public static Epsitec.Common.Widgets.Window[] GetVisibleWindows()
        {
            /*
            System.Collections.ArrayList list = new System.Collections.ArrayList();

            if (WindowList.windows.Count > 0)
            {
                System.Windows.Forms.Form form = WindowList.windows[0] as System.Windows.Forms.Form;
                System.IntPtr handle = Win32Api.GetWindow(form.Handle, Win32Const.GW_HWNDFIRST);

                if (handle != System.IntPtr.Zero)
                {
                    int myPid = 0;

                    Win32Api.GetWindowThreadProcessId(form.Handle, out myPid);

                    for (int i = 0; i < 1000; i++)
                    {
                        int pid;
                        int thread;

                        handle = Win32Api.GetWindow(handle, Win32Const.GW_HWNDNEXT);
                        thread = Win32Api.GetWindowThreadProcessId(handle, out pid);

                        bool visible = Win32Api.IsWindowVisible(handle);
                        int style = Win32Api.GetWindowLong(handle, -16); //	GWL_STYLE

                        if (handle == System.IntPtr.Zero)
                            break;
                        if (thread == 0)
                            break;

                        if ((pid == myPid) && (visible))
                        {
                            foreach (Platform.PlatformWindow platformWindow in WindowList.windows)
                            {
                                if (platformWindow.Handle == handle)
                                {
                                    if (
                                        platformWindow.WindowState
                                        != System.Windows.Forms.FormWindowState.Minimized
                                    )
                                    {
                                        list.Add(platformWindow.HostingWidgetWindow);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            Epsitec.Common.Widgets.PlatformWindow[] array = new Epsitec.Common.Widgets.PlatformWindow[list.Count];
            list.CopyTo(array);

            return array;
            */
            throw new System.NotImplementedException();
            return null;
        }
    }
}
