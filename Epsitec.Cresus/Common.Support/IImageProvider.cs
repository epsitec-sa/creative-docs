//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IImageProvider donne accès à des images en fonction de
	/// leur nom.
	/// </summary>
	public interface IImageProvider
	{
		Drawing.Image GetImage(string name, ResourceManager resource_manager);
		void ClearImageCache(string name);
	}
}
