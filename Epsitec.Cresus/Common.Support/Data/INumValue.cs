//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/03/2004

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface INumValue définit les propriétés de base nécessaires à un
	/// widget qui exporte une valeur numérique.
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
