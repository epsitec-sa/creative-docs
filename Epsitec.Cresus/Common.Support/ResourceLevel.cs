//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 08/10/2003

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'�num�ration ResourceLevel d�crit � quel niveau il faut acc�der
	/// aux ressources.
	/// </summary>
	public enum ResourceLevel
	{
		None,							//	pas d'acc�s
		Merged,							//	fusion (acc�s standard)
		
		Default,						//	ressources par d�faut
		Localised,						//	ressources localis�es
		Customised						//	ressources personnalis�es
	}
}
