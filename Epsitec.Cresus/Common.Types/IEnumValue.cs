//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
