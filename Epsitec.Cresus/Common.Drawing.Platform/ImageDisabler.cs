namespace Epsitec.Common.Drawing.Platform
{
	/// <summary>
	/// Summary description for ImageDisabler.
	/// </summary>
	public class ImageDisabler
	{
		private ImageDisabler()
		{
		}
		
		public static void Paint(System.Drawing.Bitmap src_bitmap, System.Drawing.Bitmap dst_bitmap, System.Drawing.Color color)
		{
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (dst_bitmap))
			{
				System.Windows.Forms.ControlPaint.DrawImageDisabled (graphics, src_bitmap, 0, 0, color);
			}
		}
	}
}
