namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractButton impl�mente les fonctions de base communes de tous
	/// les boutons (notamment au niveau de la gestion des �v�nements).
	/// </summary>
	public abstract class AbstractButton : Widget
	{
		public AbstractButton()
		{
			this.internal_state |= InternalState.AcceptTaggedText;
		}
	}
}
