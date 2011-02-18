//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IImageProvider donne accès à des images en fonction de
	/// leur nom.
	/// </summary>
	public interface IImageProvider
	{
		Drawing.Image GetImage(string name, ResourceManager resourceManager);
		void ClearImageCache(string name);
	}
}
