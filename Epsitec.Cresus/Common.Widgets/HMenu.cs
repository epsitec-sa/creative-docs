namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HMenu implémente le menu horizontal (ligne).
	/// </summary>
	public class HMenu : AbstractMenu
	{
		public HMenu() : base(MenuType.Horizontal)
		{
		}
		
		public HMenu(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
