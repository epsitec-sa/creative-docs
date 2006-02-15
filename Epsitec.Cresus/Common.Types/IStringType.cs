//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IStringType d�crit un type texte.
	/// </summary>
	public interface IStringType : INamedType
	{
		int		Length	{ get; }
	}
}
