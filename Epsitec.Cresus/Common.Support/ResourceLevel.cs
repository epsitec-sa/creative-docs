//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'énumération ResourceLevel décrit à quel niveau il faut accéder
	/// aux ressources.
	/// </summary>
	public enum ResourceLevel
	{
		None,							//	pas d'accès
		Merged,							//	fusion (accès standard)
		
		Default,						//	ressources par défaut
		Localized,						//	ressources localisées
		Customized,						//	ressources personnalisées
		
		All								//	toutes les variantes (pour GetBundleIds)
	}
}
