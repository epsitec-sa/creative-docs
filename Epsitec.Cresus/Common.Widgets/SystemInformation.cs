namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe SystemInformation donne des informations sur le système.
	/// </summary>
	public class SystemInformation
	{
		public static double			InitialKeyboardDelay
		{
			get
			{
				try
				{
					using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey (@"Control Panel\Keyboard"))
					{
						switch (System.Int32.Parse ((string) key.GetValue ("KeyboardDelay")))
						{
							case 0:	return 0.250;
							case 1: return 0.500;
							case 2: return 0.750;
							case 3: return 1.000;
						}
					}
				}
				catch
				{
				}
				
				return 0.5;
			}
		}
		
		public static double			KeyboardRepeatPeriod
		{
			get
			{
				try
				{
					using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey (@"Control Panel\Keyboard"))
					{
						int speed = System.Int32.Parse ((string) key.GetValue ("KeyboardSpeed")) + 2;
						return 1.0 / speed;
					}
				}
				catch
				{
				}
				
				return 0.1;
			}
		}
		
		public static int				CursorBlinkDelay
		{
			get
			{
				try
				{
					using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey (@"Control Panel\Desktop"))
					{
						int delay = System.Int32.Parse ((string) key.GetValue ("CursorBlinkRate"));
						return delay;
					}
				}
				catch
				{
				}
				
				return 499;
			}
		}
		
		public static int				MenuShowDelay
		{
			get
			{
				try
				{
					using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey (@"Control Panel\Desktop"))
					{
						int delay = System.Int32.Parse ((string) key.GetValue ("MenuShowDelay"));
						return delay;
					}
				}
				catch
				{
				}
				
				return 199;
			}
		}
		
		
		public static bool				SupportsLayeredWindows
		{
			get
			{
				return System.Windows.Forms.OSFeature.Feature.GetVersionPresent (System.Windows.Forms.OSFeature.LayeredWindows) != null;
			}
		}
		
		
		public static bool				PreferRightAlignedMenus
		{
			get { return System.Windows.Forms.SystemInformation.RightAlignedMenus; }
		}
		
		public static int				DoubleClickDelay
		{
			get { return System.Windows.Forms.SystemInformation.DoubleClickTime; }
		}
		
		public static int				DoubleClickRadius2
		{
			get
			{
				System.Drawing.Size size = System.Windows.Forms.SystemInformation.DoubleClickSize;
				
				int dx = size.Width;
				int dy = size.Height;
				
				return dx*dx + dy*dy;
			}
		}
	}
}
