namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ScreenInfo fournit les informations au sujet d'un écran.
	/// </summary>
	public class ScreenInfo
	{
		public Drawing.Rectangle			Bounds
		{
			get
			{
				int ox = this.screen.Bounds.Left;
				int oy = System.Windows.Forms.SystemInformation.VirtualScreen.Height - this.screen.Bounds.Bottom;
				int dx = this.screen.Bounds.Width;
				int dy = this.screen.Bounds.Height;
				
				return new Drawing.Rectangle (ox, oy, dx, dy);
			}
		}
		
		public Drawing.Rectangle			WorkingArea
		{
			get
			{
				int ox = this.screen.WorkingArea.Left;
				int oy = System.Windows.Forms.SystemInformation.VirtualScreen.Height - this.screen.WorkingArea.Bottom;
				int dx = this.screen.WorkingArea.Width;
				int dy = this.screen.WorkingArea.Height;
				
				return new Drawing.Rectangle (ox, oy, dx, dy);
			}
		}
		
		public bool							IsPrimary
		{
			get { return this.screen.Primary; }
		}
		
		
		public static Drawing.Rectangle		GlobalArea
		{
			get
			{
				int ox = System.Windows.Forms.SystemInformation.VirtualScreen.Left;
				int oy = System.Windows.Forms.SystemInformation.VirtualScreen.Height - System.Windows.Forms.SystemInformation.VirtualScreen.Bottom;
				int dx = System.Windows.Forms.SystemInformation.VirtualScreen.Width;
				int dy = System.Windows.Forms.SystemInformation.VirtualScreen.Height;
				
				return new Drawing.Rectangle (ox, oy, dx, dy);
			}
		}
		
		public static ScreenInfo[]			AllScreens
		{
			get
			{
				System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
				
				int n = screens.Length;
				
				ScreenInfo[] infos = new ScreenInfo[n];
				
				for (int i = 0; i < n; i++)
				{
					infos[i] = new ScreenInfo (screens[i]);
				}
				
				return infos;
			}
		}
		
		
		public static ScreenInfo Find(Drawing.Point point)
		{
			int ox = (int)(point.X + 0.5);
			int oy = System.Windows.Forms.SystemInformation.VirtualScreen.Height - (int)(point.Y + 0.5);
			
			System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint (new System.Drawing.Point (ox, oy));
			return screen == null ? null : new ScreenInfo (screen);
		}
		
		public static ScreenInfo Find(Drawing.Rectangle rect)
		{
			int ox = (int)(rect.Left + 0.5);
			int oy = System.Windows.Forms.SystemInformation.VirtualScreen.Height - (int)(rect.Top + 0.5);
			int dx = (int)(rect.Width + 0.5);
			int dy = (int)(rect.Height + 0.5);
			
			System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromRectangle (new System.Drawing.Rectangle (ox, oy, dx, dy));
			return screen == null ? null : new ScreenInfo (screen);
		}
		
		protected ScreenInfo(System.Windows.Forms.Screen screen)
		{
			this.screen = screen;
		}
		
		
		private System.Windows.Forms.Screen		screen;
	}
}
