//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		Localised,						//	ressources localisées
		Customised,						//	ressources personnalisées
		
		All								//	toutes les variantes (pour GetBundleIds)
	}
}
