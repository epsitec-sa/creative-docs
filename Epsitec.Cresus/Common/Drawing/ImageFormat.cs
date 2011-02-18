//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// L'énumération ImageFormat décrit un format d'image bitmap.
	/// </summary>
	public enum ImageFormat
	{
		Unknown			= -1,
		
		Bmp				= 0,
		Gif				= 1,
		Png				= 2,
		Tiff			= 3,
		
		Jpeg			= 10,
		Exif			= 11,
		
		WindowsIcon		= 20,
		WindowsEmf		= 21,
		WindowsWmf		= 22,
		WindowsPngIcon= 23,
	}
}
