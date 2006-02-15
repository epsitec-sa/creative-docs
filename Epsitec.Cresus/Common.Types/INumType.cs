//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface INumType décrit un type numérique.
	/// </summary>
	public interface INumType : INamedType
	{
		DecimalRange	Range	{ get; }
	}
}
