//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IEnumValue décrit une valeur dans une énumération.
	/// </summary>
	public interface IEnumValue : INameCaption
	{
		int		Rank	{ get; }
	}
}
