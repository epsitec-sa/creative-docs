//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface INum d�crit un type num�rique.
	/// </summary>
	public interface INum : INamedType
	{
		DecimalRange	Range	{ get; }
	}
}
