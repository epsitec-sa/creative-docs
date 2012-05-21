//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		
		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.GetPaintState ();
			
			double iw = (this.IconWidth > 10) ? this.IconWidth+3 : 0;
			adorner.PaintMenuBackground (graphics, rect, state, Direction.Down, Drawing.Rectangle.Empty, iw);
		}
	}
}
