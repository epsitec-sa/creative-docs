namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ToggleButton implémente un bouton qui peut être enclenché et
	/// déclenché (WidgetState.ActiveOn/Off/Maybe).
	/// </summary>
	public class ToggleButton : Button
	{
		public ToggleButton()
		{
			this.internalState |= InternalState.AutoToggle;
		}
		
		public ToggleButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
