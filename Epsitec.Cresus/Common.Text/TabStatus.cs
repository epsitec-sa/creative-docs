//	Copyright � 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'�num�ration TabStatus d�finit comment/si un tabulateur est d�fini et
	/// r�f�renc� dans le texte.
	/// </summary>
	public enum TabStatus
	{
		None				= 0,
		
		Definition			= 1,				//	tab d�fini mais pas utilis�
		Live				= 2,				//	tab d�fini et utilis�
		Zombie				= 3					//	tab pas d�fini mais utilis�
	}
}
