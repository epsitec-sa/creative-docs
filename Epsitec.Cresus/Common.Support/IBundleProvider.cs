//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	using System.Globalization;
	using System.Collections;
	
	/// <summary>
	/// L'interface IBundleProvider permet à du code externe de
	/// fournir des bundles tout prêts à la classe Resources quand
	/// celle-ci doit en charger.
	/// </summary>
	public interface IBundleProvider
	{
		ResourceBundle GetBundle(ResourceManager resource_manager, IResourceProvider provider, string id, ResourceLevel level, CultureInfo culture, int recursion);
	}
}
