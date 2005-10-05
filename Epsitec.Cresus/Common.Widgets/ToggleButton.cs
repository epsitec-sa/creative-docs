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
			this.AutoToggle = true;
		}
		
		public ToggleButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
