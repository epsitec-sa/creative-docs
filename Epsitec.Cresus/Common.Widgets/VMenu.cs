namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VMenu impl�mente le menu vertical (liste).
	/// </summary>
	public class VMenu : AbstractMenu
	{
		public VMenu() : base(MenuType.Vertical)
		{
		}
		
		public VMenu(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
