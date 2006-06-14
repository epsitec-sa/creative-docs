//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IStringType décrit un type texte.
	/// </summary>
	public interface IStringType : INamedType
	{
		int		MinimumLength	{ get; }
		int		MaximumLength	{ get; }
	}
}
