//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using Win32Api = Epsitec.Common.Widgets.Platform.Win32Api;
	
	/// <summary>
	/// La classe MouseCursor décrit un curseur de souris.
	/// </summary>
	public sealed class MouseCursor : System.IDisposable
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
		
		
		public static MouseCursor FromImage(Drawing.Image cursorImage, int xhot, int yhot)
		{
			Drawing.Image         image      = Drawing.Bitmap.CopyImage (cursorImage);
			System.Drawing.Bitmap bitmap     = image.BitmapImage.NativeBitmap;
			System.IntPtr         orgHandle = bitmap == null ? System.IntPtr.Zero : bitmap.GetHicon ();
			Win32Api.IconInfo     iconInfo  = new Win32Api.IconInfo ();
			
			if (orgHandle == System.IntPtr.Zero)
			{
				throw new System.NullReferenceException ("FromImage cannot derive bitmap handle.");
			}
			
			Win32Api.GetIconInfo (orgHandle, out iconInfo);
			
			if ((iconInfo.BitmapColor == System.IntPtr.Zero) ||
				(iconInfo.BitmapMask == System.IntPtr.Zero))
			{
				throw new System.NullReferenceException ("FromImage got empty IconInfo.");
			}
			
			iconInfo.FlagIcon = 0;
			iconInfo.HotspotX = xhot;
			iconInfo.HotspotY = (int)(image.Height) - yhot;
			
			System.IntPtr               newHandle = Win32Api.CreateIconIndirect (ref iconInfo);
			System.Windows.Forms.Cursor winCursor = new System.Windows.Forms.Cursor (newHandle);

			Win32Api.DeleteObject (iconInfo.BitmapColor);
			Win32Api.DeleteObject (iconInfo.BitmapMask);
			Win32Api.DestroyIcon (orgHandle);
			image.Dispose ();
			
			return new MouseCursor (winCursor, newHandle);
		}
		
		public static MouseCursor FromImage(Drawing.Image cursorImage)
		{
			Drawing.Image         image      = Drawing.Bitmap.CopyImage (cursorImage);
			System.Drawing.Bitmap bitmap     = image.BitmapImage.NativeBitmap;
			System.IntPtr         orgHandle = bitmap == null ? System.IntPtr.Zero : bitmap.GetHicon ();
			Win32Api.IconInfo     iconInfo  = new Win32Api.IconInfo ();
			
			if (orgHandle == System.IntPtr.Zero)
			{
				throw new System.NullReferenceException ("FromImage cannot derive bitmap handle.");
			}
			
			Win32Api.GetIconInfo (orgHandle, out iconInfo);
			
			if ((iconInfo.BitmapColor == System.IntPtr.Zero) ||
				(iconInfo.BitmapMask == System.IntPtr.Zero))
			{
				throw new System.NullReferenceException ("FromImage got empty IconInfo.");
			}
			
			double ox = image.Origin.X;
			double oy = image.Height - image.Origin.Y;
			
			iconInfo.FlagIcon = 0;
			iconInfo.HotspotX = (int)System.Math.Floor(ox+0.5);;
			iconInfo.HotspotY = (int)System.Math.Floor(oy+0.5);;
			
			System.IntPtr               newHandle = Win32Api.CreateIconIndirect (ref iconInfo);
			System.Windows.Forms.Cursor winCursor = new System.Windows.Forms.Cursor (newHandle);

			Win32Api.DeleteObject (iconInfo.BitmapColor);
			Win32Api.DeleteObject (iconInfo.BitmapMask);
			Win32Api.DestroyIcon (orgHandle);
			image.Dispose ();
			
			return new MouseCursor (winCursor, newHandle);
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
		
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.cursor != null)
				{
					this.cursor.Dispose ();
					this.cursor = null;
				}
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
		
		
		public static MouseCursor				Default
		{
			get { return MouseCursor.asArrow; }
		}
		
		public static MouseCursor				AsArrow
		{
			get { return MouseCursor.asArrow; }
		}
		
		public static MouseCursor				AsHand
		{
			get { return MouseCursor.asHand; }
		}
		
		public static MouseCursor				AsIBeam
		{
			get { return MouseCursor.asIBeam; }
		}
		
		public static MouseCursor				AsHSplit
		{
			get { return MouseCursor.asHSplit; }
		}
		
		public static MouseCursor				AsVSplit
		{
			get { return MouseCursor.asVSplit; }
		}
		
		public static MouseCursor				AsCross
		{
			get { return MouseCursor.asCross; }
		}
		
		public static MouseCursor				AsWait
		{
			get { return MouseCursor.asWait; }
		}
		
		public static MouseCursor				AsHelp
		{
			get { return MouseCursor.asHelp; }
		}
		
		public static MouseCursor				AsNo
		{
			get { return MouseCursor.asNo; }
		}
		
		public static MouseCursor				AsNoHMove
		{
			get { return MouseCursor.asNoHMove; }
		}
		
		public static MouseCursor				AsNoVMove
		{
			get { return MouseCursor.asNoVMove; }
		}
		
		public static MouseCursor				AsPanEast
		{
			get { return MouseCursor.asPanEast; }
		}
		
		public static MouseCursor				AsPanNE
		{
			get { return MouseCursor.asPanNe; }
		}
		
		public static MouseCursor				AsPanNorth
		{
			get { return MouseCursor.asPanNorth; }
		}
		
		public static MouseCursor				AsPanNW
		{
			get { return MouseCursor.asPanNw; }
		}
		
		public static MouseCursor				AsPanSE
		{
			get { return MouseCursor.asPanSe; }
		}
		
		public static MouseCursor				AsPanSouth
		{
			get { return MouseCursor.asPanSouth; }
		}
		
		public static MouseCursor				AsPanSW
		{
			get { return MouseCursor.asPanSw; }
		}
		
		public static MouseCursor				AsPanWest
		{
			get { return MouseCursor.asPanWest; }
		}
		
		public static MouseCursor				AsSizeAll
		{
			get { return MouseCursor.asSizeAll; }
		}
		
		public static MouseCursor				AsSizeNESW
		{
			get { return MouseCursor.asSizeNesw; }
		}
		
		public static MouseCursor				AsSizeNS
		{
			get { return MouseCursor.asSizeNs; }
		}
		
		public static MouseCursor				AsSizeNWSE
		{
			get { return MouseCursor.asSizeNwse; }
		}
		
		public static MouseCursor				AsSizeWE
		{
			get { return MouseCursor.asSizeWe; }
		}
		
		
		private System.Windows.Forms.Cursor		cursor;
		private System.IntPtr					handle;
		
		
		private static readonly MouseCursor		asArrow    = new MouseCursor (System.Windows.Forms.Cursors.Arrow);
		private static readonly MouseCursor		asHand     = new MouseCursor (System.Windows.Forms.Cursors.Hand);
		private static readonly MouseCursor		asIBeam    = new MouseCursor (System.Windows.Forms.Cursors.IBeam);
		private static readonly MouseCursor		asHSplit   = new MouseCursor (System.Windows.Forms.Cursors.HSplit);
		private static readonly MouseCursor		asVSplit   = new MouseCursor (System.Windows.Forms.Cursors.VSplit);
		private static readonly MouseCursor		asCross    = new MouseCursor (System.Windows.Forms.Cursors.Cross);
		private static readonly MouseCursor		asWait     = new MouseCursor (System.Windows.Forms.Cursors.WaitCursor);
		private static readonly MouseCursor		asHelp     = new MouseCursor (System.Windows.Forms.Cursors.Help);
		private static readonly MouseCursor		asNo       = new MouseCursor (System.Windows.Forms.Cursors.No);
		private static readonly MouseCursor		asNoHMove  = new MouseCursor (System.Windows.Forms.Cursors.NoMoveHoriz);
		private static readonly MouseCursor		asNoVMove  = new MouseCursor (System.Windows.Forms.Cursors.NoMoveVert);
		private static readonly MouseCursor		asPanEast  = new MouseCursor (System.Windows.Forms.Cursors.PanEast);
		private static readonly MouseCursor		asPanNe    = new MouseCursor (System.Windows.Forms.Cursors.PanNE);
		private static readonly MouseCursor		asPanNorth = new MouseCursor (System.Windows.Forms.Cursors.PanNorth);
		private static readonly MouseCursor		asPanNw    = new MouseCursor (System.Windows.Forms.Cursors.PanNW);
		private static readonly MouseCursor		asPanSe    = new MouseCursor (System.Windows.Forms.Cursors.PanSE);
		private static readonly MouseCursor		asPanSouth = new MouseCursor (System.Windows.Forms.Cursors.PanSouth);
		private static readonly MouseCursor		asPanSw    = new MouseCursor (System.Windows.Forms.Cursors.PanSW);
		private static readonly MouseCursor		asPanWest  = new MouseCursor (System.Windows.Forms.Cursors.PanWest);
		private static readonly MouseCursor		asSizeAll  = new MouseCursor (System.Windows.Forms.Cursors.SizeAll);
		private static readonly MouseCursor		asSizeNesw = new MouseCursor (System.Windows.Forms.Cursors.SizeNESW);
		private static readonly MouseCursor		asSizeNs   = new MouseCursor (System.Windows.Forms.Cursors.SizeNS);
		private static readonly MouseCursor		asSizeNwse = new MouseCursor (System.Windows.Forms.Cursors.SizeNWSE);
		private static readonly MouseCursor		asSizeWe   = new MouseCursor (System.Windows.Forms.Cursors.SizeWE);
	}
}
