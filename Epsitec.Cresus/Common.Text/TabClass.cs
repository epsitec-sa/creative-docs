//	Copyright � 2005-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'�num�ration TabClass d�finit � quelle classe de tabulateurs un tag
	/// fait r�f�rence.
	/// </summary>
	public enum TabClass
	{
		Unknown				= 0,
		
		Auto				= 1,				//	tab automatique
		Shared				= 2,				//	tab associ� � un style
	}
}
