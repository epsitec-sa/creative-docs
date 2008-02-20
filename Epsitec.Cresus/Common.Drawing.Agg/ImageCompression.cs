//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// L'énumération ImageCompression décrit un format de compression utilisable
	/// pour les images TIFF uniquement (pour l'instant).
	/// </summary>
	public enum ImageCompression
	{
		None,
		Lzw,
		FaxGroup3,
		FaxGroup4,
		Rle
	}
}
