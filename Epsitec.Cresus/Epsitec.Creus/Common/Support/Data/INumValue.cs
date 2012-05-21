//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

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
		bool						IsValid			{ get; }
		
		event Support.EventHandler	ValueChanged;
	}
}
