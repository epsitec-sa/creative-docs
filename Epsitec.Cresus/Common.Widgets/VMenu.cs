//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VMenu implémente le menu vertical, utilisé pour tous les
	/// menus et sous-menus (sauf le menu horizontal, évidemment).
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
