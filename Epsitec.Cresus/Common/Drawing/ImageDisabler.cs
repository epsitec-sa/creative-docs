//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Platform
{
	/// <summary>
	/// The ImageDisabler class provides access to Windows image converter,
	/// which produces a disabled representation of a given icon.
	/// </summary>
	public sealed class ImageDisabler
	{
		private ImageDisabler()
		{
		}
		
		
		public static void Paint(System.Drawing.Bitmap srcBitmap, System.Drawing.Bitmap dstBitmap, System.Drawing.Color color)
		{
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dstBitmap))
			{
				System.Windows.Forms.ControlPaint.DrawImageDisabled (graphics, srcBitmap, 0, 0, color);
			}
		}
	}
}
