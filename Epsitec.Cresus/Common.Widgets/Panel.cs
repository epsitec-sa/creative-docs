//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets
{
	public delegate void PaintFrameCallback(Panel panel, Drawing.Graphics graphics, Drawing.Rectangle frame_outside, Drawing.Rectangle frame_inside);
	
	/// <summary>
	/// La classe Panel est un widget qui permet de grouper d'autres widgets
	/// tout en limitant la surface affichée à une ouverture (aperture).
	/// </summary>
	public class Panel : AbstractGroup
	{
		public Panel()
		{
			this.aperture = Drawing.Rectangle.Infinite;
		}
		
		public Panel(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Drawing.Rectangle			Aperture
		{
			//	L'ouverture, si elle est définie pour un panel, permet de définir quelle
			//	partie est actuellement visible, relativement au système de coordonnées
			//	client du panel.
			
			//	Pour indiquer que le panel complet est visible, il faut assigner à Aperture
			//	la valeur Drawing.Rectangle.Infinite.
			
			get
			{
				return this.aperture;
			}
			
			set
			{
				if (this.aperture != value)
				{
					this.aperture = value;
					this.Invalidate ();
				}
			}
		}
		
		public Drawing.Margins				FrameMargins
		{
			//	Les marges du cadre permettent de restreindre la surface utilisable pour
			//	le layout du contenu du panel.
			
			get
			{
				return this.frame_margins;
			}
			
			set
			{
				if (this.frame_margins != value)
				{
					this.frame_margins = value;
					this.OnSurfaceSizeChanged ();
				}
			}
		}
		
		
		public Drawing.Size					DesiredSize
		{
			get
			{
				return this.desired_size;
			}
			set
			{
				if (this.desired_size != value)
				{
					this.desired_size = value;
					this.OnSurfaceSizeChanged ();
				}
			}
		}
		
		public double						DesiredWidth
		{
			get { return this.DesiredSize.Width; }
		}
		
		public double						DesiredHeight
		{
			get { return this.DesiredSize.Height; }
		}
		
		public Drawing.Size					SurfaceSize
		{
			get
			{
				return this.desired_size + this.frame_margins;
			}
		}
		
		public double						SurfaceWidth
		{
			get { return this.SurfaceSize.Width; }
		}
		
		public double						SurfaceHeight
		{
			get { return this.SurfaceSize.Height; }
		}
		
		
		public override Drawing.Rectangle GetClipBounds()
		{
			if (this.aperture == Drawing.Rectangle.Infinite)
			{
				//	Si aucune ouverture n'a été définie, elle a une taille infinie et on
				//	peut donc utiliser la région de clipping retournée par Widget.
				
				return base.GetClipBounds ();
			}
			else
			{
				//	Une ouverture limite la surface visible; il faut donc réaliser
				//	l'intersection de la région complète avec l'ouverture.
				
				return Drawing.Rectangle.Intersection (base.GetClipBounds (), this.aperture);
			}
		}
		
		
		protected override void PaintForegroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clip_rect)
		{
#if false
			if ((this.aperture.IsValid) &&
				(this.PaintFrameCallback != null))
			{
				//	Une ouverture a été définie, il faut donc donner l'occasion à des
				//	éventuels décorateurs de peindre dans le cadre.
				
				Drawing.Rectangle frame_inside  = this.aperture;
				Drawing.Rectangle frame_outside = Drawing.Rectangle.Inflate (frame_inside, this.frame_margins);
				
				Drawing.Rectangle old_clip = graphics.SaveClippingRectangle ();
				Drawing.Rectangle new_clip = this.MapClientToRoot (frame_outside);
				
				base.PaintForegroundImplementation (graphics, clip_rect);
				
				graphics.RestoreClippingRectangle (new_clip);
				this.PaintFrameCallback (this, graphics, frame_outside, frame_inside);
				graphics.RestoreClippingRectangle (old_clip);
			}
			else
			{
				base.PaintForegroundImplementation (graphics, clip_rect);
			}
#else
			base.PaintForegroundImplementation (graphics, clip_rect);
			
			graphics.AddRectangle (20, 20, 3, 3);
			graphics.RenderSolid (Drawing.Color.FromRGB (0.4, 0, 0));
#endif
		}
		
		
		
		protected virtual void OnSurfaceSizeChanged()
		{
			if (this.SurfaceSizeChanged != null)
			{
				this.SurfaceSizeChanged (this);
			}
		}
		
		
		public event PaintFrameCallback		PaintFrameCallback;
		public event EventHandler			SurfaceSizeChanged;
		
		
		protected Drawing.Margins			frame_margins;
		protected Drawing.Rectangle			aperture;
		protected Drawing.Size				desired_size;
	}
}
