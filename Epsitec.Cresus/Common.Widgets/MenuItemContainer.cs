//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe MenuItemContainer permet d'incorporer dans une case de menu
	/// des widgets divers et variés.
	/// </summary>
	public class MenuItemContainer : MenuItem
	{
		public MenuItemContainer()
		{
		}
		
		public MenuItemContainer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		

		
		
		public override Drawing.Size GetBestFitSize()
		{
			return this.RealMinSize;
		}
		
		protected override void OnIconSizeChanged()
		{
			//	Met à jour le positionnement des éléments internes en fonction
			//	de la place disponible :
			
			double width = this.IconWidth;
			
			Drawing.Margins padding = this.DockPadding;
			
			if (padding.Left != width)
			{
				padding.Left = width;
				
				this.DockPadding = padding;
			}
			
			base.OnIconSizeChanged ();
		}
	}
}
