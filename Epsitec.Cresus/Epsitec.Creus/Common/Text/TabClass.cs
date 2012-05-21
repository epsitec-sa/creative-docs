//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
