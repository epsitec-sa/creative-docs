namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CheckButton r�alise un bouton cochable.
	/// </summary>
	public class CheckButton : AbstractButton
	{
		public CheckButton()
		{
			this.internal_state |= InternalState.AutoToggle;
		}
	}
}
