
using System.Runtime.InteropServices;

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// Summary description for Win32Caps.
	/// </summary>
	public class Win32Caps
	{
		private Win32Caps()
		{
		}
		
		static short HORZSIZE			= 4;		//	Horizontal size in millimeters
		static short VERTSIZE			= 6;		//	Vertical size in millimeters
		static short HORZRES			= 8;		//	Horizontal width in pixels
		static short VERTRES			= 10;		//	Vertical height in pixels
		static short PHYSICALOFFSETX	= 112;		//	Physical Printable Area x margin
		static short PHYSICALOFFSETY	= 113;		//	Physical Printable Area y margin
		
		[DllImport("gdi32.dll")] static extern short GetDeviceCaps(System.IntPtr hDC, short func);
		
		/// <summary>
		/// Return the device 'hard' margins for the passed in
		/// Device Context handle. Return data in 1/100ths inch
		/// </summary>
		/// <param name="hDC">Input handle</param>
		/// <param name="left">output left margin in 1/100ths inch</param>
		/// <param name="top">output top margin in 1/100ths inch</param>
		/// <param name="right">output right margin in 1/100ths inch</param>
		/// <param name="bottom">output bottom margin in 1/100ths inch</param>
		public static void GetHardMargins(System.IntPtr hDC, ref float left, ref float top, ref float right, ref float bottom)
		{
			float offx = System.Convert.ToSingle (GetDeviceCaps (hDC, PHYSICALOFFSETX));
			float offy = System.Convert.ToSingle (GetDeviceCaps (hDC, PHYSICALOFFSETY));
			float resx = System.Convert.ToSingle (GetDeviceCaps (hDC, HORZRES));
			float resy = System.Convert.ToSingle (GetDeviceCaps (hDC, VERTRES));
			float hsz  = System.Convert.ToSingle (GetDeviceCaps (hDC, HORZSIZE))/25.4f; // screen width in inches
			float vsz  = System.Convert.ToSingle (GetDeviceCaps (hDC, VERTSIZE))/25.4f; // screen height in inches
			float ppix = resx/hsz;
			float ppiy = resy/vsz;
			
			left   = (offx/ppix) * 100.0f;
			top    = (offy/ppix) * 100.0f;
			bottom = top + (vsz * 100.0f);
			right  = left + (hsz * 100.0f);
		}
	}
}
