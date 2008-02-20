//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

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
		bool						IsValid			{ get; }
		
		event Support.EventHandler	ValueChanged;
	}
}
