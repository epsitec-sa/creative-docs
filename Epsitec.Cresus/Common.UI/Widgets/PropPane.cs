//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Ui.Widgets
{
	/// <summary>
	/// La classe PropPane d�finit un panneau pour les propri�t�s.
	/// </summary>
	public class PropPane : Widget
	{
		public PropPane()
		{
			this.CreateWidgets ();
		}
		
		public PropPane(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public bool								AcceptToggle
		{
			get
			{
				return this.accept_toggle;
			}
			set
			{
				if (this.accept_toggle != value)
				{
					this.accept_toggle = value;
					this.toggle_button.SetVisible (this.accept_toggle);
				}
			}
		}
		
		public bool								AcceptExtra
		{
			get
			{
				return this.accept_extra;
			}
			set
			{
				if (this.accept_extra != value)
				{
					this.accept_extra = value;
					this.extra_button.SetEnabled (this.accept_extra);
				}
			}
		}
		
		public int								ViewCount
		{
			get
			{
				return this.views == null ? 0 : this.views.Length;
			}
		}
		
		public override double					DefaultWidth
		{
			get
			{
				return 200;
			}
		}
		
		public override double					DefaultHeight
		{
			get
			{
				return 30;
			}
		}

		
		public override Drawing.Rectangle GetShapeBounds()
		{
			//	Afin de permettre de r�aliser des "grilles" jolies en mettant c�te � c�te (ou
			//	plut�t l'un sous l'autre) divers �l�ments PropPane, il faut que le cadre soit
			//	dessin� en partie en dehors du rectangle englobant :
			
			Drawing.Rectangle rect = base.GetShapeBounds();
			rect.Inflate (1, 1, 1, 0);
			return rect;
		}
		
		
		protected virtual void CreateWidgets()
		{
			this.toggle_button = new GlyphButton (this, GlyphShape.ArrowDown);
			this.extra_button  = new GlyphButton (this, GlyphShape.Dots);
			
			Drawing.Rectangle rect = this.Bounds;
			double dim = PropPane.button_dim;
			
			this.toggle_button.Clicked      += new MessageEventHandler (this.HandleToggleButtonClicked);
			this.toggle_button.TabIndex      = 0;
			this.toggle_button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toggle_button.Bounds        = new Drawing.Rectangle (rect.Left + 1, rect.Top - 8 - dim, dim, dim);
			this.toggle_button.Anchor        = AnchorStyles.TopLeft;
			
			this.extra_button.Clicked       += new MessageEventHandler (this.HandleExtraButtonClicked);
			this.extra_button.TabIndex       = 1000;
			this.extra_button.TabNavigation  = Widget.TabNavigationMode.ActivateOnTab;
			this.extra_button.Bounds         = new Drawing.Rectangle (rect.Right - dim - 1, rect.Top - 8 - dim, dim, dim);
			this.extra_button.Anchor         = AnchorStyles.TopRight;
			
			this.toggle_button.SetVisible (this.accept_toggle);
			this.extra_button.SetEnabled (this.accept_extra);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			this.CreateViews (list, rect);
			
			this.views = new Widget[list.Count];
			list.CopyTo (this.views);
		}
		
		protected virtual void CreateViews(System.Collections.ArrayList list, Drawing.Rectangle bounds)
		{
			Widget base_view = this.CreateViewWidget (bounds);
			
			this.InitialiseBaseView (base_view);
			
			list.Add (base_view);
		}
		
		protected virtual void InitialiseBaseView(Widget base_view)
		{
		}
		
		protected Widget CreateViewWidget(Drawing.Rectangle bounds)
		{
			Widget widget = new Widget (this);
			
			widget.Bounds = bounds;
			widget.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			widget.SetVisible (false);
			
			return widget;
		}
		
		
		private void HandleToggleButtonClicked(object sender, MessageEventArgs e)
		{
		}
		
		private void HandleExtraButtonClicked(object sender, MessageEventArgs e)
		{
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
			
			Drawing.Rectangle rect = this.Client.Bounds;
			
			graphics.AddFilledRectangle (rect);
			
			Drawing.Color color_caption = adorner.ColorCaption;
			Drawing.Color color_back    = Drawing.Color.FromARGB (0.2,
				/**/											  0.5 + color_caption.R * 0.5,
				/**/											  0.5 + color_caption.G * 0.5,
				/**/											  0.5 + color_caption.B * 0.5);
			
			graphics.RenderSolid (color_back);
			
//			if ( this.multi )
//			{
//				Drawing.Rectangle part = rect;
//				part.Width = ex;
//				graphics.AddFilledRectangle(part);
//				graphics.RenderSolid(IconContext.ColorMulti);
//			}

//			if ( this.styleID != 0 || styleDirect )
//			{
//				Drawing.Rectangle part = rect;
//				part.Left = part.Right-ex;
//				graphics.AddFilledRectangle(part);
//				graphics.RenderSolid(IconContext.ColorStyle);
//				part.Left = rect.Left+ex;
//				part.Right = rect.Right-ex;
//				graphics.AddFilledRectangle(part);
//				graphics.RenderSolid(IconContext.ColorStyleBack);
//			}

			rect.Deflate (0.5, 0.5);
			
			graphics.AddLine (rect.Left - 1,                     rect.Bottom - 0.5, rect.Left - 1,                     rect.Top + 0.5);
			graphics.AddLine (rect.Left + PropPane.toggle_width, rect.Bottom - 0.5, rect.Left + PropPane.toggle_width, rect.Top + 0.5);
			graphics.AddLine (rect.Right - PropPane.extra_width, rect.Bottom - 0.5, rect.Right - PropPane.extra_width, rect.Top + 0.5);
			graphics.AddLine (rect.Right + 1,                    rect.Bottom - 0.5, rect.Right + 1,                    rect.Top + 0.5);
			graphics.AddLine (rect.Left - 1.5,                   rect.Top + 1,      rect.Right + 1.5,                  rect.Top + 1);
			graphics.AddLine (rect.Left - 1.5,                   rect.Bottom,       rect.Right + 1.5,                  rect.Bottom);
			graphics.RenderSolid (adorner.ColorBorder);
		}
		
		
		protected const double					button_dim   = 13;
		protected const double					toggle_width = PropPane.button_dim+2;
		protected const double					extra_width  = PropPane.button_dim+2;
		
		protected string						caption;
		
		protected Widget[]						views;
		protected GlyphButton					toggle_button;
		protected GlyphButton					extra_button;
		
		protected bool							accept_toggle = false;
		protected bool							accept_extra  = false;
	}
}
