namespace Epsitec.Common.Widgets
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
