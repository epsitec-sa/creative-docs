namespace Epsitec.Common.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe ColorSample permet de représenter une couleur rgb.
	/// </summary>
	public class ColorSample : AbstractButton
	{
		public ColorSample()
		{
			this.LinkClear();
		}
		
		public ColorSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		// Lie l'échantillon à une couleur dans une liste.
		public void LinkWithColorsCollection(Drawing.ColorsCollection list, int index)
		{
			this.collectionList = list;
			this.collectionIndex = index;
		}

		// Délie l'échantillon.
		public void LinkClear()
		{
			this.collectionList = null;
			this.collectionIndex = -1;
		}

		// Couleur.
		public Drawing.Color Color
		{
			get
			{
				if ( this.collectionList == null )
				{
					return this.color;
				}
				else
				{
					return this.collectionList[this.collectionIndex];
				}
			}

			set
			{
				if ( this.collectionList == null )
				{
					if ( this.color != value )
					{
						this.color = value;
						this.Invalidate();
					}
				}
				else
				{
					if ( this.collectionList[this.collectionIndex] != value )
					{
						this.collectionList[this.collectionIndex] = value;
						this.Invalidate();
					}
				}
			}
		}


		// Possibilité d'utiliser ce widget comme origine des couleurs.
		public bool PossibleOrigin
		{
			get
			{
				return this.possibleOrigin;
			}

			set
			{
				this.possibleOrigin = value;
			}
		}


		public override Drawing.Rectangle GetShapeBounds()
		{
			if ( this.possibleOrigin )
			{
				return new Drawing.Rectangle(-5, -5, this.Client.Width+10, this.Client.Height+10);
			}
			return this.Client.Bounds;
		}


		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.mouseDown = true;
					this.dragTarget = null;
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						ColorSample cs = this.DragSearchDst(pos);
						if ( cs != this.dragTarget )
						{
							this.DragHilite(this.dragTarget, false);
							this.dragTarget = cs;
							this.DragHilite(this.dragTarget, true);
						}
						message.Consumer = this;
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.DragHilite(this.dragTarget, false);
						if ( this.dragTarget != null )
						{
							if ( message.IsShiftPressed || message.IsCtrlPressed )
							{
								Drawing.Color temp = this.Color;
								this.Color = this.dragTarget.Color;
								this.dragTarget.Color = temp;

								this.OnChanged();
								this.dragTarget.OnChanged();
							}
							else
							{
								this.dragTarget.Color = this.Color;
								this.dragTarget.OnChanged();
							}
						}
						this.mouseDown = false;
						message.Consumer = this;
					}
					break;
			}
		}

		// Cherche un widget ColorSample destinataire du drag & drop.
		protected ColorSample DragSearchDst(Drawing.Point mouse)
		{
			mouse = this.MapClientToScreen(mouse);
			Widget parent = this.RootParent;
			Widget[] widgets = parent.FindAllChildren();
			foreach ( Widget widget in widgets )
			{
				if ( widget.IsVisible && widget is ColorSample && widget != this )
				{
					Drawing.Rectangle rect = widget.MapClientToScreen(widget.Client.Bounds);
					if ( rect.Contains(mouse) )
					{
						return widget as ColorSample;
					}
				}
			}
			return null;
		}

		// Met en évidence le widget ColorSample destinataire du drag & drop.
		protected void DragHilite(ColorSample widget, bool enable)
		{
			if ( widget == null )  return;

			Drawing.Color color = enable ? this.Color : Drawing.Color.Empty;
			if ( widget.dragColor != color )
			{
				widget.dragColor = color;
				widget.Invalidate();
			}
		}


		// Dessine la couleur.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			if ( this.possibleOrigin && this.ActiveState == WidgetState.ActiveYes )
			{
				Drawing.Rectangle r = rect;
				r.Inflate(4);
				graphics.AddFilledRectangle(r);
				graphics.RenderSolid(adorner.ColorCaption);
			}

			if ( this.IsEnabled )
			{
				graphics.AddLine(rect.Left+0.5, rect.Bottom+0.5, rect.Right-0.5, rect.Top-0.5);
				graphics.AddLine(rect.Left+0.5, rect.Top-0.5, rect.Right-0.5, rect.Bottom+0.5);
				graphics.RenderSolid(adorner.ColorBorder);

				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.Color);

				rect.Deflate(0.5, 0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);

				if ( (this.PaintState&WidgetState.Focused) != 0 )
				{
					rect.Deflate(1, 1);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(ColorSample.Opposite(this.Color));
				}

				if ( !this.dragColor.IsEmpty )
				{
					rect.Deflate(1, 1);
					double radius = System.Math.Min(rect.Width, rect.Height)/2;

					graphics.AddLine(rect.Center.X-radius, rect.Center.Y, rect.Center.X+radius, rect.Center.Y);
					graphics.AddLine(rect.Center.X, rect.Center.Y-radius, rect.Center.X, rect.Center.Y+radius);
					graphics.RenderSolid(adorner.ColorBorder);

					graphics.AddFilledCircle(rect.Center, radius);
					graphics.RenderSolid(this.dragColor);

					graphics.AddCircle(rect.Center, radius);
					graphics.RenderSolid(ColorSample.Opposite(this.dragColor));
				}
			}
			else
			{
				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);
			}
		}

		// Calcule la couleur opposée pour le mise en évidence.
		protected static Drawing.Color Opposite(Drawing.Color color)
		{
			double h,s,v;
			color.GetHSV(out h, out s, out v);
			if ( s < 0.2 )  // gris ou presque ?
			{
				IAdorner adorner = Widgets.Adorner.Factory.Active;
				color = adorner.ColorCaption;
			}
			else
			{
				color.R = 1.0-color.R;
				color.G = 1.0-color.G;
				color.B = 1.0-color.B;  // couleur opposée
			}
			color.A = 1.0;
			return color;
		}


		// Génère un événement pour dire que la couleur a changé.
		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}
		
		public event Support.EventHandler		Changed;

		protected Drawing.ColorsCollection		collectionList;
		protected int							collectionIndex;
		protected Drawing.Color					color;
		protected bool							possibleOrigin = false;
		protected Drawing.Color					dragColor = Drawing.Color.Empty;
		protected bool							mouseDown = false;
		protected ColorSample					dragTarget = null;
	}
}
