//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 25/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IEnum d�crit une �num�ration.
	/// </summary>
	public interface IEnum : INamedType
	{
		IEnumValue[]	Values				{ get; }			//	tri�s selon Rank
		
		IEnumValue		this[string name]	{ get; }
		IEnumValue		this[int rank]		{ get; }
		
		bool			IsCustomizable		{ get; }
	}
}
