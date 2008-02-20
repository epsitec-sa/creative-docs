//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VMenu impl�mente le menu vertical, utilis� pour tous les
	/// menus et sous-menus (sauf le menu horizontal, �videmment).
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
