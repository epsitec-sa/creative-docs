//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'énumération TabClass définit à quelle classe de tabulateurs un tag
	/// fait référence.
	/// </summary>
	public enum TabClass
	{
		Unknown				= 0,
		
		Auto				= 1,				//	tab automatique
		Shared				= 2,				//	tab associé à un style
	}
}
