//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IEnum décrit une énumération.
	/// </summary>
	public interface IEnumType : INamedType
	{
		IEnumValue[]	Values				{ get; }			//	triés selon Rank
		
		IEnumValue		this[string name]	{ get; }
		IEnumValue		this[int rank]		{ get; }
		
		bool			IsCustomizable		{ get; }
		bool			IsDefinedAsFlags	{ get; }
	}
}
