namespace Epsitec.Common.Widgets
{
	public delegate void PaintFrameCallback(Panel panel, Drawing.Graphics graphics, Drawing.Rectangle frame_outside, Drawing.Rectangle frame_inside);
	
	/// <summary>
	/// La classe Panel permet de gérer des panneaux avec une marge peinte par des
	/// décorateurs externes.
	/// </summary>
	public class Panel : AbstractGroup
	{
		public Panel()
		{
		}
		
		public Panel(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			return Drawing.Rectangle.Inflate (base.GetShapeBounds (), this.frame_margins);
		}
		
		public override Drawing.Rectangle GetClipBounds()
		{
			Drawing.Rectangle parent_clip = this.MapParentToClient (this.parent.GetClipBounds ());
			Drawing.Rectangle client_clip = base.GetClipBounds ();
			
			return Drawing.Rectangle.Intersection (parent_clip, client_clip);
		}
		
		
		public Drawing.Margins			FrameMargins
		{
			//	Les marges du cadre du panel sont utilisées par la classe Scrollable pour
			//	assurer que l'ouverture est toujours calée de telle manière qu'il reste de
			//	la place pour un cadre autour du morceau de panel visible.
			
			get
			{
				return this.frame_margins;
			}
			
			set
			{
				if (this.frame_margins != value)
				{
					this.frame_margins = value;
					this.Invalidate ();
				}
			}
		}
		
		public Drawing.Rectangle		Aperture
		{
			//	L'ouverture, si elle est définie pour un panel, permet de définir quelle
			//	partie est actuellement visible, relativement au système de coordonnées
			//	client du panel.
			
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
		
		protected override void PaintForegroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clip_rect)
		{
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
		}
		
		
		public event PaintFrameCallback	PaintFrameCallback;
		
		
		protected Drawing.Margins		frame_margins;
		protected Drawing.Rectangle		aperture;
	}
}
