//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ScreenInfo fournit les informations au sujet d'un écran.
	/// </summary>
	public class ScreenInfo
	{
		public static double				PrimaryHeight
		{
			get
			{
				return System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
			}
		}
		
		
		/// <summary>
		/// Retourne le rectangle correspondant à l'écran par rapport à la surface
		/// de travail globale.
		/// </summary>
		public Drawing.Rectangle			Bounds
		{
			get
			{
				int ox = this.screen.Bounds.Left;
				int oy = (int) ScreenInfo.PrimaryHeight - this.screen.Bounds.Bottom;
				int dx = this.screen.Bounds.Width;
				int dy = this.screen.Bounds.Height;
				
				return new Drawing.Rectangle (ox, oy, dx, dy);
			}
		}
		
		/// <summary>
		/// Retourne le rectangle correspondant à la surface de travail sur l'écran
		/// par rapport à la surface de travail globale. L'espace pris par la barre
		/// des tâches est automatiquement enlevé.
		/// </summary>
		public Drawing.Rectangle			WorkingArea
		{
			get
			{
				int ox = this.screen.WorkingArea.Left;
				int oy = (int) ScreenInfo.PrimaryHeight - this.screen.WorkingArea.Bottom;
				int dx = this.screen.WorkingArea.Width;
				int dy = this.screen.WorkingArea.Height;
				
				return new Drawing.Rectangle (ox, oy, dx, dy);
			}
		}
		
		/// <summary>
		/// Indique s'il s'agit de l'écran principal (celui où il y a la barre des
		/// tâches).
		/// </summary>
		public bool							IsPrimary
		{
			get { return this.screen.Primary; }
		}
		
		public string						DeviceName
		{
			get
			{
				string name = this.screen.DeviceName;
				int pos = name.IndexOf ('\0');
				
				if (pos >= 0)
				{
					name = name.Substring (0, pos);
				}
				
				return name;
			}
		}
		
		
		/// <summary>
		/// Retourne le rectangle correspondant à la surface de travail globale. Cette
		/// surface peut avoir une origine négative...
		/// </summary>
		public static Drawing.Rectangle		GlobalArea
		{
			get
			{
				int ox = System.Windows.Forms.SystemInformation.VirtualScreen.Left;
				int oy = (int) ScreenInfo.PrimaryHeight - System.Windows.Forms.SystemInformation.VirtualScreen.Bottom;
				int dx = System.Windows.Forms.SystemInformation.VirtualScreen.Width;
				int dy = System.Windows.Forms.SystemInformation.VirtualScreen.Height;
				
				return new Drawing.Rectangle (ox, oy, dx, dy);
			}
		}
		
		/// <summary>
		/// Construit la table de tous les écrans disponibles.
		/// </summary>
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
		
		
		/// <summary>
		/// Trouve l'écran qui se trouve au point indiqué.
		/// </summary>
		/// <param name="point">position absolue</param>
		/// <returns>écran trouvé</returns>
		public static ScreenInfo Find(Drawing.Point point)
		{
			int ox = (int)(point.X + 0.5);
			int oy = (int)(ScreenInfo.PrimaryHeight) - (int)(point.Y + 0.5);
			
			System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint (new System.Drawing.Point (ox, oy));
			return screen == null ? null : new ScreenInfo (screen);
		}
		
		/// <summary>
		/// Trouve l'écran qui est le plus recouvert par le rectangle indiqué.
		/// </summary>
		/// <param name="rect">rectangle à tester</param>
		/// <returns>écran trouvé</returns>
		public static ScreenInfo Find(Drawing.Rectangle rect)
		{
			int ox = (int)(rect.Left + 0.5);
			int oy = (int)(ScreenInfo.PrimaryHeight) - (int)(rect.Top + 0.5);
			int dx = (int)(rect.Width + 0.5);
			int dy = (int)(rect.Height + 0.5);
			
			System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromRectangle (new System.Drawing.Rectangle (ox, oy, dx, dy));
			return screen == null ? null : new ScreenInfo (screen);
		}

		/// <summary>
		/// Fits the specified rectangle into the working area. This can be used
		/// to make a window fully visible.
		/// </summary>
		/// <param name="rect">The rectangle to fit.</param>
		/// <returns>The adjusted rectangle.</returns>
		public static Drawing.Rectangle FitIntoWorkingArea(Drawing.Rectangle rect)
		{
			ScreenInfo si = ScreenInfo.Find (rect.Center);
			Drawing.Rectangle area = si.WorkingArea;

			rect.Width  = System.Math.Min (rect.Width, area.Width);
			rect.Height = System.Math.Min (rect.Height, area.Height);

			if (rect.Left < area.Left)		// dépasse à gauche ?
			{
				rect.Offset (area.Left-rect.Left, 0);
			}

			if (rect.Right > area.Right)	// dépasse à droite ?
			{
				rect.Offset (area.Right-rect.Right, 0);
			}

			if (rect.Bottom < area.Bottom)	// dépasse en bas ?
			{
				rect.Offset (0, area.Bottom-rect.Bottom);
			}

			if (rect.Top > area.Top)		// dépasse en haut ?
			{
				rect.Offset (0, area.Top-rect.Top);
			}

			return rect;
		}
		
		
		protected ScreenInfo(System.Windows.Forms.Screen screen)
		{
			this.screen = screen;
		}
		
		
		private System.Windows.Forms.Screen		screen;
	}
}
