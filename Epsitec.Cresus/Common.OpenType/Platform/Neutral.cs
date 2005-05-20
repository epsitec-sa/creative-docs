//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType.Platform
{
	/// <summary>
	/// Summary description for Neutral.
	/// </summary>
	public sealed class Neutral
	{
		private Neutral()
		{
		}
		
		
		public static byte[] LoadFontData(string family, string style)
		{
			return Platform.Win32.LoadFontData (family, style);
		}
		
		public static byte[] LoadFontData(object system_description)
		{
			return Platform.Win32.LoadFontData (system_description);
		}
		
		public static byte[] LoadFontDataNameTable(string family, string style)
		{
			return Platform.Win32.LoadFontDataNameTable (family, style);
		}
		
		
		public static string[] GetFontFamilies()
		{
			return Platform.Win32.GetFontFamilies ();
		}
		
		public static string[] GetFontStyles(string family)
		{
			return Platform.Win32.GetFontStyles (family);
		}
		
		
		public static object GetFontSystemDescription(string family, string style)
		{
			return Platform.Win32.GetFontSystemDescription (family, style);
		}
		
		
		public static int GetFontWeight(object system_description)
		{
			return Platform.Win32.GetFontWeight (system_description);
		}
		
		public static int GetFontItalic(object system_description)
		{
			return Platform.Win32.GetFontItalic (system_description);
		}
		
		
		public static Platform.IFontHandle GetFontHandle(object system_description, int size)
		{
			return Platform.Win32.GetFontHandle (system_description, size);
		}
		
		
		public static bool FillFontWidths(Platform.IFontHandle font, int glyph, int count, int[] widths, int[] lsb, int[] rsb)
		{
			return Platform.Win32.FillFontWidths (font, glyph, count, widths, lsb, rsb);
		}
		
		public static bool FillFontHeights(Platform.IFontHandle font, out int height, out int ascent, out int descent, out int internal_leading, out int external_leading)
		{
			return Platform.Win32.FillFontHeights (font, out height, out ascent, out descent, out internal_leading, out external_leading);
		}
	}
}
