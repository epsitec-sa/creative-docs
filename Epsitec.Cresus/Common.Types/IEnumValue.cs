//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IEnumValue décrit une valeur dans une énumération.
	/// </summary>
	public interface IEnumValue : INameCaption
	{
		System.Enum Value
		{
			get;
		}
		
		int Rank
		{
			get;
		}
		
		bool IsHidden
		{
			get;
		}
	}
}
