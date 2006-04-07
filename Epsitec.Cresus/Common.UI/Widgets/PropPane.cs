//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Widgets
{
	/// <summary>
	/// La classe PropPane définit un panneau pour les propriétés.
	/// </summary>
	public class PropPane : Widget
	{
		public PropPane()
		{
		}
		
		public PropPane(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public void Attach(Adapters.IAdapter adapter)
		{
			this.controllers = Controllers.ControllerFactory.CreateControllers (adapter);
			this.CreateWidgets ();
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
					this.toggle_button.Visibility = (this.accept_toggle);
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
					this.extra_button.Enable = this.accept_extra;
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
		
		public int								VisibleViewIndex
		{
			get
			{
				return this.visible_view_index;
			}
			set
			{
				if (this.visible_view_index != value)
				{
					this.MakeViewVisible (value);
				}
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
			//	Afin de permettre de réaliser des "grilles" jolies en mettant côte à côte (ou
			//	plutôt l'un sous l'autre) divers éléments PropPane, il faut que le cadre soit
			//	dessiné en partie en dehors du rectangle englobant :
			
			Drawing.Rectangle rect = base.GetShapeBounds();
			rect.Inflate (1, 1, 1, 0);
			return rect;
		}
		
		
		protected virtual void CreateWidgets()
		{
			this.toggle_button = new GlyphButton (this, GlyphShape.ArrowDown);
			this.extra_button  = new GlyphButton (this, GlyphShape.Dots);
			
			double dim = PropPane.button_dim;
			
			this.toggle_button.Clicked      += new MessageEventHandler (this.HandleToggleButtonClicked);
			this.toggle_button.TabIndex      = 0;
			this.toggle_button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.toggle_button.Size          = new Drawing.Size (dim, dim);
			this.toggle_button.Anchor        = AnchorStyles.TopLeft;
			this.toggle_button.Margins = new Drawing.Margins (1, 0, 8, 0);
			
			this.extra_button.Clicked       += new MessageEventHandler (this.HandleExtraButtonClicked);
			this.extra_button.TabIndex       = 1000;
			this.extra_button.TabNavigation  = Widget.TabNavigationMode.ActivateOnTab;
			this.extra_button.Size           = new Drawing.Size (dim, dim);
			this.extra_button.Anchor         = AnchorStyles.TopRight;
			this.extra_button.Margins  = new Drawing.Margins (0, 1, 8, 0);
			
			this.toggle_button.Visibility = (this.accept_toggle);
			this.extra_button.Enable = this.accept_extra;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			Drawing.Margins margins = new Drawing.Margins (PropPane.toggle_width + 5, PropPane.extra_width + 5, 0, 1);
			
			this.CreateViews (list, margins);
			
			this.visible_view_index = -1;
			this.views = new Widget[list.Count];
			list.CopyTo (this.views);
			
			this.MakeViewVisible (0);
			this.AcceptToggle = this.views.Length > 1;
		}
		
		protected virtual void CreateViews(System.Collections.ArrayList list, Drawing.Margins margins)
		{
			string caption = this.controllers[0].Adapter.Binder.Caption;
			
			for (int i = 0; i < this.controllers.Length; i++)
			{
				Widget view = this.CreateViewWidget (margins);
				
				this.controllers[i].CreateUI (view);
				this.controllers[i].Caption = caption;
				
				list.Add (view);
				
				view.TabIndex      = i+1;
				view.Name          = string.Format (System.Globalization.CultureInfo.InvariantCulture, "View_{0}", i);
				view.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			}
		}
		
		protected Widget CreateViewWidget(Drawing.Margins margins)
		{
			Widget widget = new FatWidget (this);
			
			widget.Anchor        = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			widget.Margins = margins;
			widget.Height        = this.DefaultHeight - 1;
			
			widget.Visibility = false;
			
			return widget;
		}
		
		
		protected virtual void MakeViewVisible(int index)
		{
			if (this.visible_view_index == index)
			{
				return;
			}
			
			if (this.visible_view_index >= 0)
			{
				this.views[this.visible_view_index].Visibility = false;
			}
			
			this.visible_view_index = index;
			
			if (this.visible_view_index < 0)
			{
				this.Height = this.DefaultHeight;
			}
			else
			{
				this.views[this.visible_view_index].Visibility = true;
				this.Height = this.views[this.visible_view_index].Height + 1;
			}
			
//-			this.ForceLayout ();
		}
		
		
		private void HandleToggleButtonClicked(object sender, MessageEventArgs e)
		{
			int index = this.visible_view_index + 1;
			
			if (index >= this.views.Length)
			{
				index = 0;
			}
			
			this.MakeViewVisible (index);
		}
		
		private void HandleExtraButtonClicked(object sender, MessageEventArgs e)
		{
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			
			Drawing.Rectangle rect = this.Client.Bounds;
			
			graphics.AddFilledRectangle (rect);
			
			Drawing.Color color_caption = adorner.ColorCaption;
			Drawing.Color color_back    = Drawing.Color.FromAlphaRgb (0.2,
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
		
		
		
		[SuppressBundleSupport]
		internal class FatWidget : Widget
		{
			public FatWidget(Widget embedder)
			{
				this.SetEmbedder (embedder);
			}
			
			public override Epsitec.Common.Drawing.Rectangle GetShapeBounds()
			{
				Drawing.Rectangle bounds = base.GetShapeBounds ();
				bounds.Inflate (3, 3, 3, 3);
				return bounds;
			}
		}
		
		protected const double					button_dim   = 13;
		protected const double					toggle_width = PropPane.button_dim+2;
		protected const double					extra_width  = PropPane.button_dim+2;
		
		protected string						caption;
		
		protected int							visible_view_index;
		protected Widget[]						views;
		protected GlyphButton					toggle_button;
		protected GlyphButton					extra_button;
		
		protected bool							accept_toggle = false;
		protected bool							accept_extra  = false;
		
		protected Controllers.IController[]		controllers;
	}
}
