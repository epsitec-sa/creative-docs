//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		Localized,						//	ressources localis�es
		Customized,						//	ressources personnalis�es
		
		All								//	toutes les variantes (pour GetBundleIds)
	}
}
