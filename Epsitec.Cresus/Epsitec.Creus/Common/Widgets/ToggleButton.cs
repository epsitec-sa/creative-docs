namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ToggleButton impl�mente un bouton qui peut �tre enclench� et
	/// d�clench� (ActiveState.On/Off/Maybe).
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
