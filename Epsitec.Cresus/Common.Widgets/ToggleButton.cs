namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ToggleButton impl�mente un bouton qui peut �tre enclench� et
	/// d�clench� (WidgetState.ActiveOn/Off/Maybe).
	/// </summary>
	public class ToggleButton : Button
	{
		public ToggleButton()
		{
			this.internal_state |= InternalState.AutoToggle;
		}
	}
}
