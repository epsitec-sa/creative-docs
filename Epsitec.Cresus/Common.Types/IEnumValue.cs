//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 25/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IEnumValue d�crit une valeur dans une �num�ration.
	/// </summary>
	public interface IEnumValue : INameCaption
	{
		int		Rank	{ get; }
	}
}
