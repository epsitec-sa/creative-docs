//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 25/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface INum d�crit un type num�rique.
	/// </summary>
	public interface INum : INameCaption
	{
		DecimalRange	Range	{ get; }
	}
}
