//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType.Tests
{
	/// <summary>
	/// Summary description for CheckTables.
	/// </summary>
	public sealed class CheckTables
	{
		public static void RunTests()
		{
//			Platform.Win32.LoadFontDataDrawing ();
			
			string font = "Arial";
			byte[] data = Platform.Win32.LoadFontData (font);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Loaded font {0}: length {1}", font, data.Length));
			
			TableDirectory td = new TableDirectory (data, 0);
			
			for (int i = 0; i < (int) td.NumTables; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Table {0}, offset={1}, length={2}.", td.GetEntry (i).Tag, td.GetEntry (i).Length, td.GetEntry (i).Offset));
			}
		}
	}
}
