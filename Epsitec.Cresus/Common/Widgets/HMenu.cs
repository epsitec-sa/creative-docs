//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HMenu implémente le menu horizontal (ligne).
	/// </summary>
	public class HMenu : AbstractMenu
	{
		public HMenu()
		{
			MenuItem.SetZeroDelay (this, true);
			
			Behaviors.MenuBehavior behavior = MenuItem.GetMenuBehavior (this);
			
			if (behavior != null)
			{
				behavior.Attach (this);
			}
		}
		
		public HMenu(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		

		public override MenuOrientation			MenuOrientation
		{
			get
			{
				return MenuOrientation.Horizontal;
			}
		}

		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Behaviors.MenuBehavior behavior = MenuItem.GetMenuBehavior (this);
				
				if (behavior != null)
				{
					behavior.Detach (this);
				}
			}
			
			base.Dispose (disposing);
		}
	}
}
