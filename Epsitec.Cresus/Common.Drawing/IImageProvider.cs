namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// L'interface IImageProvider donne acc�s � des images en fonction de
	/// leur nom.
	/// </summary>
	public interface IImageProvider
	{
		Image GetImage(string name);
	}
}
