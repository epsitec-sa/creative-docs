namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractButton implémente les fonctions de base communes de tous
	/// les boutons (notamment au niveau de la gestion des événements).
	/// </summary>
	public abstract class AbstractButton : Widget
	{
		public AbstractButton()
		{
			this.internal_state |= InternalState.AcceptTaggedText;
		}
	}
}
