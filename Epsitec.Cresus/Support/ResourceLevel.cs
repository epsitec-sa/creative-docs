//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 08/10/2003

namespace Epsitec.Cresus.Support
{
	/// <summary>
	/// L'énumération ResourceLevel décrit à quel niveau il faut accéder
	/// aux ressources.
	/// </summary>
	public enum ResourceLevel
	{
		None,							//	pas d'accès
		
		Default,						//	ressources par défaut
		Localised,						//	ressources localisées
		Customised						//	ressources personnalisées
	}
}
