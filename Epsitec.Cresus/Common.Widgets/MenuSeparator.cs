namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe MenuSeparator est la variante du MenuItem servant
	/// à peindre les séparations.
	/// </summary>
	public class MenuSeparator : MenuItem
	{
		public MenuSeparator()
		{
			this.separator = true;
		}
		
		public MenuSeparator(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
