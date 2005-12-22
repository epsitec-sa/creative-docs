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
