namespace Epsitec.Common.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe ColorSample permet de représenter une couleur rgb.
	/// </summary>
	public class ColorSample : AbstractButton, Helpers.IDragBehaviorHost
	{
		public ColorSample()
		{
			this.dragBehavior = new Helpers.DragBehavior(this, true, true);
			this.DetachColorCollection();
		}
		
		public ColorSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public Drawing.RichColor				Color
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


		public bool								PossibleSource
		{
			// Possibilité d'utiliser ce widget comme origine des couleurs.
			
			get
			{
				return this.possibleSource;
			}

			set
			{
				this.possibleSource = value;
			}
		}

		public double							MarginSource
		{
			// Marge lorsque l'origine des couleurs est affichée.
			
			get
			{
				return this.marginSource;
			}

			set
			{
				this.marginSource = value;
			}
		}

		
		// Lie l'échantillon à une couleur dans une liste.
		public void AttachColorCollection(Drawing.ColorCollection list, int index)
		{
			this.collectionList = list;
			this.collectionIndex = index;
		}

		// Délie l'échantillon.
		public void DetachColorCollection()
		{
			this.collectionList = null;
			this.collectionIndex = -1;
		}


		#region IDragBehaviorHost Members
		public Drawing.Point					DragLocation
		{
			get
			{
				// Pas utile ici:
				return new Drawing.Point(0, 0);
			}
		}

		
		public bool OnDragBegin(Drawing.Point cursor)
		{
			// Crée un échantillon utilisable pour l'opération de drag & drop (il
			// va représenter visuellement l'échantillon de couleur). On le place
			// dans un DragWindow et hop.
			
			ColorSample widget = new ColorSample();
			
			widget.Color = this.Color;
			
			// Signale à l'échantillon qui est la cause du drag. On aurait très
			// bien pu ajouter une variable à ColorSample, mais ça paraît du
			// gaspillage de mémoire d'avoir cette variable inutilisée pour
			// tous les ColorSample. Alors on utilise une "propriété" :
			
			widget.SetProperty("DragHost", this);
			
			this.dragTarget = null;
			this.dragOrigin = this.MapClientToScreen(new Drawing.Point(-5, -5));
			this.dragWindow = new DragWindow();
			this.dragWindow.Alpha = 1.0;
			this.dragWindow.DefineWidget(widget, new Drawing.Size(11, 11), Drawing.Margins.Zero);
			this.dragWindow.WindowLocation = this.dragOrigin + cursor;
			this.dragWindow.Owner = this.Window;
			this.dragWindow.FocusedWidget = widget;
			this.dragWindow.Show();
			
			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			this.dragWindow.WindowLocation = this.dragOrigin + e.Offset;
			
			ColorSample cs = this.SearchDropTarget(e.ToPoint);
			if ( cs != this.dragTarget )
			{
				this.DragHilite(this.dragTarget, this, false);
				this.dragTarget = cs;
				this.DragHilite(this.dragTarget, this, true);
				this.UpdateSwapping();
			}
		}

		public void OnDragEnd()
		{
			this.DragHilite(this, this.dragTarget, false);
			this.DragHilite(this.dragTarget, this, false);
			
			if ( this.dragTarget != null )
			{
				if ( this.dragTarget != this )
				{
					if ( Message.State.IsShiftPressed || Message.State.IsCtrlPressed )
					{
						Drawing.RichColor temp = this.Color;
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
				
				this.dragWindow.Hide();
				this.dragWindow.Dispose();
				this.dragWindow = null;
			}
			else
			{
				this.dragWindow.DissolveAndDisposeWindow();
				this.dragWindow = null;
			}
		}
		#endregion
		
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			if ( this.possibleSource )
			{
				return new Drawing.Rectangle(-5, -5, this.Client.Width+10, this.Client.Height+10);
			}
			return this.Client.Bounds;
		}

		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			ColorSample dragHost = this.GetProperty("DragHost") as ColorSample;
			
			// Est-ce que l'événement clavier est reçu dans un échantillon en
			// cours de drag dans un DragWindow ? C'est possible, car le focus
			// clavier change quand on montre le DragWindow.
			
			if ( dragHost != null && message.IsKeyType )
			{
				// Signalons l'événement clavier à l'auteur du drag :
				
				dragHost.ProcessMessage(message, pos);
				return;
			}
			
			switch ( message.Type )
			{
				case MessageType.KeyDown:
				case MessageType.KeyUp:
					if ( message.KeyCode == KeyCode.ShiftKey   ||
						 message.KeyCode == KeyCode.ControlKey )
					{
						if ( this.dragWindow != null )
						{
							this.UpdateSwapping();
							message.Consumer = this;
							return;
						}
					}
					break;
			}
			
			if ( !this.dragBehavior.ProcessMessage(message, pos) )
			{
				base.ProcessMessage(message, pos);
			}
		}

		// Mise à jour après un changement de mode swap d'un drag & drop.
		protected void UpdateSwapping()
		{
			if ( this.dragTarget != null )
			{
				bool swap = Message.State.IsShiftPressed || Message.State.IsCtrlPressed;
				this.DragHilite(this, this.dragTarget, swap);
			}
			else
			{
				this.dragColor = Drawing.RichColor.Empty;
				this.Invalidate();
			}
		}
		
		// Cherche un widget ColorSample destinataire du drag & drop.
		protected ColorSample SearchDropTarget(Drawing.Point mouse)
		{
			mouse = this.MapClientToRoot(mouse);
			Widget root   = this.Window.Root;
			Widget widget = root.FindChild(mouse, Widget.ChildFindMode.SkipHidden | Widget.ChildFindMode.Deep | Widget.ChildFindMode.SkipDisabled);
			return widget as ColorSample;
		}

		// Met en évidence le widget ColorSample destinataire du drag & drop.
		protected void DragHilite(ColorSample dst, ColorSample src, bool enable)
		{
			if ( dst == null || src == null )  return;

			Drawing.RichColor color = enable ? src.Color : Drawing.RichColor.Empty;
			if ( dst.dragColor != color )
			{
				dst.dragColor = color;
				dst.Invalidate();
			}
		}


		// Dessine la couleur.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			if ( this.possibleSource && this.ActiveState == WidgetState.ActiveYes )
			{
				Drawing.Rectangle r = rect;
				r.Inflate(this.marginSource);
				graphics.AddFilledRectangle(r);
				graphics.RenderSolid(adorner.ColorCaption);
			}

			if ( this.IsEnabled )
			{
				graphics.AddLine(rect.Left+0.5, rect.Bottom+0.5, rect.Right-0.5, rect.Top-0.5);
				graphics.AddLine(rect.Left+0.5, rect.Top-0.5, rect.Right-0.5, rect.Bottom+0.5);
				graphics.RenderSolid(adorner.ColorBorder);

				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.Color.Basic);

				rect.Deflate(0.5, 0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);

				if ( (this.PaintState&WidgetState.Focused) != 0 )
				{
					rect.Deflate(1, 1);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(ColorSample.GetOpposite(this.Color.Basic));
				}

				if ( !this.dragColor.IsEmpty )
				{
					rect.Deflate(1, 1);
					double radius = System.Math.Min(rect.Width, rect.Height)/2;

					graphics.AddLine(rect.Center.X-radius, rect.Center.Y, rect.Center.X+radius, rect.Center.Y);
					graphics.AddLine(rect.Center.X, rect.Center.Y-radius, rect.Center.X, rect.Center.Y+radius);
					graphics.RenderSolid(adorner.ColorBorder);

					graphics.AddFilledCircle(rect.Center, radius);
					graphics.RenderSolid(this.dragColor.Basic);

					graphics.AddCircle(rect.Center, radius);
					graphics.RenderSolid(ColorSample.GetOpposite(this.dragColor.Basic));
				}
			}
			else
			{
				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);
			}
		}

		// Calcule la couleur opposée pour la mise en évidence.
		protected static Drawing.Color GetOpposite(Drawing.Color color)
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

		
		private Helpers.DragBehavior			dragBehavior;
		private DragWindow						dragWindow;
		private Drawing.Point					dragOrigin;
		private ColorSample						dragTarget;
		private Drawing.RichColor				dragColor = Drawing.RichColor.Empty;
		
		protected Drawing.ColorCollection		collectionList;
		protected int							collectionIndex;
		protected Drawing.RichColor				color;
		protected bool							possibleSource = false;
		protected double						marginSource = 4;
	}
}
