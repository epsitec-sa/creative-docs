//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IString décrit un type texte.
	/// </summary>
	public interface IString : INamedType
	{
		int		Length	{ get; }
	}
}
