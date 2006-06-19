//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface INamedType sert de base pour INumType, IEnumType et
	/// IStringType.
	/// </summary>
	public interface INamedType : ICaption
	{
		System.Type		SystemType		{ get; }
	}
}
