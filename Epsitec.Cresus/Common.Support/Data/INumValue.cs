//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/03/2004

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface INumValue d�finit les propri�t�s de base n�cessaires � un
	/// widget qui exporte une valeur num�rique.
	/// </summary>
	public interface INumValue
	{
		decimal						Value			{ get; set; }
		decimal						MinValue		{ get; set; }
		decimal						MaxValue		{ get; set; }
		decimal						Resolution		{ get; set; }
		decimal						Range			{ get; }
		
		event Support.EventHandler	ValueChanged;
	}
}
