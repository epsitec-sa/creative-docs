//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 25/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IEnum décrit une énumération.
	/// </summary>
	public interface IEnum : INameCaption
	{
		IEnumValue[]	Values		{ get; }
	}
}
