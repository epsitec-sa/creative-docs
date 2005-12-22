namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe SystemInformation donne des informations sur le système.
	/// </summary>
	public class SystemInformation
	{
		public enum Animation
		{
			None,
			Roll,
			Fade
		}
		
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
		
		public static double			CursorBlinkDelay
		{
			get
			{
				try
				{
					using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey (@"Control Panel\Desktop"))
					{
						int delay = System.Int32.Parse ((string) key.GetValue ("CursorBlinkRate"));
						return delay / 1000.0;
					}
				}
				catch
				{
				}
				
				return 0.499;
			}
		}
		
		
		public static double			MenuShowDelay
		{
			get
			{
				try
				{
					using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey (@"Control Panel\Desktop"))
					{
						int delay = System.Int32.Parse ((string) key.GetValue ("MenuShowDelay"));
						return delay / 1000.0;
					}
				}
				catch
				{
				}
				
				return 0.199;
			}
		}
		
		public static Animation			MenuAnimation
		{
			get
			{
				if (SystemInformation.IsMenuAnimationEnabled)
				{
					return SystemInformation.IsMenuFadingEnabled ? Animation.Fade : Animation.Roll;
				}
				
				return Animation.None;
			}
		}
		
		public static double			MenuAnimationRollTime
		{
			get { return 0.250; }
		}
		
		public static double			MenuAnimationFadeInTime
		{
			get { return 0.200; }
		}
		
		public static double			MenuAnimationFadeOutTime
		{
			get { return 0.200; }
		}
		
		
		public static double			ToolTipShowDelay
		{
			get { return 1.0; }
		}
		
		public static double			ToolTipAutoCloseDelay
		{
			get { return 5.0; }
		}
		
		
		public static bool				IsMenuAnimationEnabled
		{
			get { return (SystemInformation.UserPreferenceMask[0] & 0x02) != 0; }
		}
		
		public static bool				IsComboAnimationEnabled
		{
			get { return (SystemInformation.UserPreferenceMask[0] & 0x04) != 0; }
		}
		
		public static bool				IsSmoothScrollEnabled
		{
			get { return (SystemInformation.UserPreferenceMask[0] & 0x08) != 0; }
		}
		
		public static bool				IsMetaUnderlineEnabled
		{
			get { return (SystemInformation.UserPreferenceMask[0] & 0x20) != 0; }
		}
		
		public static bool				IsMenuFadingEnabled
		{
			get { return (SystemInformation.UserPreferenceMask[1] & 0x02) != 0; }
		}
		
		public static bool				IsMenuSelectionFadingEnabled
		{
			get { return (SystemInformation.UserPreferenceMask[1] & 0x04) != 0; }
		}
		
		public static bool				IsMenuShadowEnabled
		{
			get { return (SystemInformation.UserPreferenceMask[2] & 0x04) != 0; }
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
		
		public static double			DoubleClickDelay
		{
			get
			{
				return System.Windows.Forms.SystemInformation.DoubleClickTime / 1000.0;
			}
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
		
		
		internal static int[]			UserPreferenceMask
		{
			get
			{
				try
				{
					using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey (@"Control Panel\Desktop"))
					{
						System.Array data = key.GetValue ("UserPreferencesMask") as System.Array;
						int[] copy = new int[data.Length];
						data.CopyTo (copy, 0);
						return copy;
					}
				}
				catch
				{
				}
				
				return new int[] { 0xBE, 0x28, 0x06, 0x80 };
			}
		}
	}
}
