//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		
		public override MenuOrientation			MenuOrientation
		{
			get
			{
				return MenuOrientation.Vertical;
			}
		}
		
		
		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clip_rect)
		{
			base.PaintBackgroundImplementation (graphics, clip_rect);
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.PaintState;
			
			double iw = (this.IconWidth > 10) ? this.IconWidth+3 : 0;
			adorner.PaintMenuBackground (graphics, rect, state, Direction.Down, Drawing.Rectangle.Empty, iw);
		}
	}
}
