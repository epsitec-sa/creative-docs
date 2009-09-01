//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType.Platform
{
	/// <summary>
	/// La classe Neutral encapsule les dépendances système de manière à ce que
	/// l'appelant ne voie pas ce qui se passe derrière les rideaux.
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
		
		public static byte[] LoadFontData(object systemDescription)
		{
			return Platform.Win32.LoadFontData (systemDescription);
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
		
		
		public static int GetFontWeight(object systemDescription)
		{
			return Platform.Win32.GetFontWeight (systemDescription);
		}
		
		public static int GetFontItalic(object systemDescription)
		{
			return Platform.Win32.GetFontItalic (systemDescription);
		}
		
		
		public static Platform.IFontHandle GetFontHandle(object systemDescription, int size)
		{
			return Platform.Win32.GetFontHandle (systemDescription, size);
		}
		
		
		public static bool FillFontWidths(Platform.IFontHandle font, int glyph, int count, int[] widths, int[] lsb, int[] rsb)
		{
			return Platform.Win32.FillFontWidths (font, glyph, count, widths, lsb, rsb);
		}
		
		public static bool FillFontHeights(Platform.IFontHandle font, out int height, out int ascent, out int descent, out int internalLeading, out int externalLeading)
		{
			return Platform.Win32.FillFontHeights (font, out height, out ascent, out descent, out internalLeading, out externalLeading);
		}
	}
}
