//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface INamedType sert de base pour INum, IEnum et IString.
	/// </summary>
	public interface INamedType : INameCaption
	{
		System.Type		SystemType		{ get; }
	}
}
