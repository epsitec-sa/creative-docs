//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using Win32Api = Epsitec.Common.Widgets.Platform.Win32Api;
	
	/// <summary>
	/// La classe MouseCursor décrit un curseur de souris.
	/// </summary>
	public class MouseCursor : System.IDisposable
	{
		private MouseCursor(System.Windows.Forms.Cursor cursor)
		{
			this.cursor = cursor;
		}
		
		private MouseCursor(System.Windows.Forms.Cursor cursor, System.IntPtr handle)
		{
			this.cursor = cursor;
			this.handle = handle;
		}
		
		~MouseCursor()
		{
			this.Dispose (false);
		}
		
		
		public static MouseCursor FromImage(Drawing.Image image, int xhot, int yhot)
		{
			System.Drawing.Bitmap bitmap     = image.BitmapImage.NativeBitmap;
			System.IntPtr         org_handle = bitmap == null ? System.IntPtr.Zero : bitmap.GetHicon ();
			Win32Api.IconInfo     icon_info  = new Win32Api.IconInfo ();
			
			if (org_handle == System.IntPtr.Zero)
			{
				throw new System.NullReferenceException ("FromImage cannot derive bitmap handle.");
			}
			
			Win32Api.GetIconInfo (org_handle, out icon_info);
			
			if ((icon_info.BitmapColor == System.IntPtr.Zero) ||
				(icon_info.BitmapMask == System.IntPtr.Zero))
			{
				throw new System.NullReferenceException ("FromImage got empty IconInfo.");
			}
			
			icon_info.FlagIcon = 0;
			icon_info.HotspotX = xhot;
			icon_info.HotspotY = (int)(image.Height) - yhot;
			
			System.IntPtr               new_handle = Win32Api.CreateIconIndirect (ref icon_info);
			System.Windows.Forms.Cursor win_cursor = new System.Windows.Forms.Cursor (new_handle);
			
			Win32Api.DestroyIcon (org_handle);
			
			return new MouseCursor (win_cursor, new_handle);
		}
		
		public static MouseCursor FromImage(Drawing.Image image)
		{
			System.Drawing.Bitmap bitmap     = image.BitmapImage.NativeBitmap;
			System.IntPtr         org_handle = bitmap == null ? System.IntPtr.Zero : bitmap.GetHicon ();
			Win32Api.IconInfo     icon_info  = new Win32Api.IconInfo ();
			
			if (org_handle == System.IntPtr.Zero)
			{
				throw new System.NullReferenceException ("FromImage cannot derive bitmap handle.");
			}
			
			Win32Api.GetIconInfo (org_handle, out icon_info);
			
			if ((icon_info.BitmapColor == System.IntPtr.Zero) ||
				(icon_info.BitmapMask == System.IntPtr.Zero))
			{
				throw new System.NullReferenceException ("FromImage got empty IconInfo.");
			}
			
			double ox = image.Origin.X;
			double oy = image.Height - image.Origin.Y;
			
			icon_info.FlagIcon = 0;
			icon_info.HotspotX = (int)System.Math.Floor(ox+0.5);;
			icon_info.HotspotY = (int)System.Math.Floor(oy+0.5);;
			
			System.IntPtr               new_handle = Win32Api.CreateIconIndirect (ref icon_info);
			System.Windows.Forms.Cursor win_cursor = new System.Windows.Forms.Cursor (new_handle);
			
			Win32Api.DestroyIcon (org_handle);
			
			return new MouseCursor (win_cursor, new_handle);
		}
		
		
		public static void Hide()
		{
			System.Windows.Forms.Cursor.Hide ();
		}
		
		public static void Show()
		{
			System.Windows.Forms.Cursor.Show ();
		}
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.cursor = null;
			}
			
			if (this.handle != System.IntPtr.Zero)
			{
				Win32Api.DestroyIcon (this.handle);
				this.handle = System.IntPtr.Zero;
			}
		}

		
		internal System.Windows.Forms.Cursor GetPlatformCursor()
		{
			return this.cursor;
		}
		
		
		public static MouseCursor			Default
		{
			get { return MouseCursor.AsArrow; }
		}
		
		public static MouseCursor			AsArrow
		{
			get { return MouseCursor.as_arrow; }
		}
		
		public static MouseCursor			AsHand
		{
			get { return MouseCursor.as_hand; }
		}
		
		public static MouseCursor			AsIBeam
		{
			get { return MouseCursor.as_I_beam; }
		}
		
		public static MouseCursor			AsHSplit
		{
			get { return MouseCursor.as_h_split; }
		}
		
		public static MouseCursor			AsVSplit
		{
			get { return MouseCursor.as_v_split; }
		}
		
		public static MouseCursor			AsCross
		{
			get { return MouseCursor.as_cross; }
		}
		
		public static MouseCursor			AsSizeAll
		{
			get { return MouseCursor.as_size_all; }
		}
		
		public static MouseCursor			AsWait
		{
			get { return MouseCursor.as_wait; }
		}
		
		
		private System.Windows.Forms.Cursor	cursor;
		private System.IntPtr				handle;
		
		
		private static MouseCursor			as_arrow   = new MouseCursor (System.Windows.Forms.Cursors.Arrow);
		private static MouseCursor			as_hand    = new MouseCursor (System.Windows.Forms.Cursors.Hand);
		private static MouseCursor			as_I_beam  = new MouseCursor (System.Windows.Forms.Cursors.IBeam);
		private static MouseCursor			as_h_split = new MouseCursor (System.Windows.Forms.Cursors.HSplit);
		private static MouseCursor			as_v_split = new MouseCursor (System.Windows.Forms.Cursors.VSplit);
		private static MouseCursor			as_cross    = new MouseCursor (System.Windows.Forms.Cursors.Cross);
		private static MouseCursor			as_size_all = new MouseCursor (System.Windows.Forms.Cursors.SizeAll);
		private static MouseCursor			as_wait    = new MouseCursor (System.Windows.Forms.Cursors.WaitCursor);
	}
}
