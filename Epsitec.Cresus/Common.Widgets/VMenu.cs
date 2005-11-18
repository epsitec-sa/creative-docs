namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VMenu implémente le menu vertical (liste).
	/// </summary>
	public class VMenu : AbstractMenu
	{
		public VMenu() : base ()
		{
		}
		
		public VMenu(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override MenuType				MenuType
		{
			get
			{
				return MenuType.Vertical;
			}
		}

	}
}
