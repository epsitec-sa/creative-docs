//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Widgets
{
	using IWidgetCollectionHost = Epsitec.Common.Widgets.Helpers.IWidgetCollectionHost;
	using WidgetCollection      = Epsitec.Common.Widgets.Helpers.WidgetCollection;
	
	/// <summary>
	/// La classe TabOrderOverlay crée une surface qui a la même taille que
	/// la fenêtre sous-jacente; cette surface va abriter des pastilles
	/// représentant les numéros d'ordre de tabulation.
	/// </summary>
	public class TabOrderOverlay : Widget
	{
		public TabOrderOverlay()
		{
			this.Name = string.Format ("TabOrderOverlay{0}", TabOrderOverlay.overlay_id++);
			
			this.SetFrozen (true);
		}
		
		public TabOrderOverlay(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Widget							RootWidget
		{
			get
			{
				return this.root_widget;
			}
			set
			{
				if (this.root_widget != value)
				{
					if (this.root_widget != null)
					{
						this.DetachWidget (this.root_widget);
					}
					
					this.root_widget = value;
					
					if (this.root_widget != null)
					{
						this.AttachWidget (this.root_widget);
					}
				}
			}
		}
		
		
		protected virtual void AttachWidget(Widget widget)
		{
			if (this.Parent == null)
			{
				this.Parent = widget.Window.Root;
				this.Bounds = this.Parent.Client.Bounds;
				this.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			}
			else
			{
				System.Diagnostics.Debug.Assert (this.Parent == widget.Window.Root);
			}
		}
		
		protected virtual void DetachWidget(Widget widget)
		{
			this.Parent = null;
		}
		
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	TODO: compléter
			}
			
			base.Dispose (disposing);
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			this.PaintTag (this.root_widget, null, graphics, clip_rect);
		}
		
		protected void PaintTag(Widget widget, string prefix, Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			if (prefix == null)
			{
				//	On n'affiche pas de pastille pour la racine commune de tous les widgets.
				//	C'est forcément toujours l'unique...
				
				prefix = "";
			}
			else
			{
				Drawing.Rectangle bounds = widget.MapClientToRoot (widget.Client.Bounds);
				
				if (! clip_rect.IntersectsWith (bounds))
				{
					return;
				}
				
				TabNavigationMode mode = widget.TabNavigation;
			
				if ((mode & TabNavigationMode.ActivateOnTab) != 0)
				{
					string text = string.Format ("{0}{1}", prefix, widget.TabIndex);
					
					Drawing.Font font  = this.DefaultFont;
					double       size  = this.DefaultFontSize;
					double       below = System.Math.Ceiling (font.Descender * size);
					double       above = System.Math.Ceiling (font.Ascender * size);
					
					double x = bounds.Left + 3;
					double y = bounds.Top - above - 1;
					
					graphics.Color = Drawing.Color.FromRGB (1, 1, 1);
					
					double width = System.Math.Ceiling (graphics.PaintText (x+1, y-1, text, font, size));
					
					Drawing.Rectangle rect = new Drawing.Rectangle (x, y + below, width, above - below);
					
					rect.Inflate (2.5, 2.5, 0.5, 0.5);
					
					graphics.LineWidth = 1.0;
					graphics.Color = Drawing.Color.FromARGB (0.8, 1, 1, 1);
					graphics.PaintSurface (Drawing.Path.FromRectangle (rect));
					
					graphics.Color = Drawing.Color.FromRGB (0, 0, 0.6);
					graphics.PaintOutline (Drawing.Path.FromRectangle (rect));
					graphics.PaintText (x, y, text, font, size);
				}
			
				if ((mode & TabNavigationMode.ForwardToChildren) != 0)
				{
					//	Les enfants de ce widget peuvent être atteints par une pression sur TAB.
					//	Il faut donc refléter cela au moyen d'un préfixe incluant le ID du widget
					//	actuel :
					
					prefix = string.Format ("{0}{1}.", prefix, widget.TabIndex);
				}
				else
				{
					prefix = "X.";
				}
			}
			
			if (widget.HasChildren)
			{
				foreach (Widget child in widget.Children)
				{
					this.PaintTag (child, prefix, graphics, clip_rect);
				}
			}
		}
		
		
		protected Widget						root_widget;
		
		static long								overlay_id;
	}
}
