using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe AbstractRuler implémente la classe de base des règles
	/// HRuler et VRuler.
	/// </summary>
	public abstract class AbstractRuler : Widget, Common.Widgets.Helpers.IToolTipHost
	{
		protected AbstractRuler(bool vertical)
		{
			this.isVertical = vertical;
		}

		protected AbstractRuler(Widget embedder, bool vertical) : this(vertical)
		{
			this.SetEmbedder(embedder);
		}

		public Document Document
		{
			get
			{
				return this.document;
			}

			set
			{
				this.document = value;
			}
		}


		public double PPM
		{
			//	Nombre de points/millimètres pour la graduation.
			get
			{
				return this.ppm;
			}

			set
			{
				if ( this.ppm != value )
				{
					this.ppm = value;
					this.Invalidate();
				}
			}
		}

		public double Starting
		{
			//	Début de la graduation, pour permettre la conversion points écran en
			//	points document et inversément.
			get
			{
				return this.starting;
			}

			set
			{
				if ( this.starting != value )
				{
					this.starting = value;
					this.Invalidate();
				}
			}
		}

		public double Ending
		{
			//	Fin de la graduation, pour permettre la conversion points écran en
			//	points document et inversément.
			get
			{
				return this.ending;
			}

			set
			{
				if ( this.ending != value )
				{
					this.ending = value;
					this.Invalidate();
				}
			}
		}

		public double Marker
		{
			//	Position du marqueur.
			get
			{
				return this.marker;
			}

			set
			{
				if ( this.marker != value )
				{
					this.invalidateBox = Rectangle.Empty;
					this.InvalidateBoxMarker();

					this.marker = value;
					
					this.InvalidateBoxMarker();
					this.Invalidate(this.invalidateBox);
				}
			}
		}

		public bool MarkerVisible
		{
			//	Visibilité du marqueur.
			get
			{
				return this.markerVisible;
			}

			set
			{
				if ( this.markerVisible != value )
				{
					this.invalidateBox = Rectangle.Empty;
					this.InvalidateBoxMarker();
					
					this.markerVisible = value;
					
					this.InvalidateBoxMarker();
					this.Invalidate(this.invalidateBox);
				}
			}
		}

		public bool Edited
		{
			//	Edition en cours ?
			get
			{
				return this.edited;
			}

			set
			{
				if ( this.edited != value )
				{
					this.edited = value;
					this.Invalidate();
				}
			}
		}

		public Objects.AbstractText EditObject
		{
			//	Objet en cours d'édition.
			get
			{
				return this.editObject;
			}

			set
			{
				this.editObject = value;
			}
		}

		public virtual void WrappersAttach()
		{
			//	Attache la règle aux wrappers.
		}

		public virtual void WrappersDetach()
		{
			//	Détache la règle des wrappers.
		}

		public double LimitLow
		{
			//	Limite basse, selon la bbox de l'obet édité.
			get
			{
				return this.limitLow;
			}

			set
			{
				if ( this.limitLow != value )
				{
					this.limitLow = value;
					this.Invalidate();
				}
			}
		}

		public double LimitHigh
		{
			//	Limite haute, selon la bbox de l'obet édité.
			get
			{
				return this.limitHigh;
			}

			set
			{
				if ( this.limitHigh != value )
				{
					this.limitHigh = value;
					this.Invalidate();
				}
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if ( !this.IsEnabled )  return;
			
			if ( this.document != null )
			{
				this.document.Modifier.ActiveViewer.DrawingContext.IsAlt = message.IsAltPressed;
			}

			switch ( message.MessageType )
			{
				case MessageType.MouseDown:
					if ( this.edited )
					{
						this.isDragging = true;
						this.draggingHandle = this.DraggingDetect(pos);
						this.DraggingStart(ref this.draggingHandle, pos);
						message.Consumer = this;
						return;
					}
					else if ( this.document != null )
					{
						this.document.Modifier.ActiveViewer.GuideInteractiveStart(!this.isVertical);
					}
					break;

				case MessageType.MouseMove:
					if ( this.edited )
					{
						if ( this.isDragging )
						{
							this.DraggingMove(ref this.draggingHandle, pos);
							message.Consumer = this;
							return;
						}
						else
						{
							this.HiliteHandle = this.DraggingDetect(pos);
						}
					}
					break;

				case MessageType.MouseUp:
					if ( this.edited )
					{
						if ( this.isDragging )
						{
							this.DraggingEnd(ref this.draggingHandle, pos);
							this.draggingHandle = null;
							this.isDragging = false;
							message.Consumer = this;
							return;
						}
					}
					else if ( this.document != null )
					{
						this.document.Modifier.ActiveViewer.GuideInteractiveEnd();
					}
					break;

				case MessageType.MouseLeave:
					if ( this.edited )
					{
						this.HiliteHandle = null;
					}
					break;
			}

			base.ProcessMessage(message, pos);
		}

		protected string HiliteHandle
		{
			//	Poignée mise en évidence.
			get
			{
				return this.hiliteHandle;
			}

			set
			{
				if ( this.hiliteHandle != value )
				{
					this.hiliteHandle = value;
					this.Invalidate();
				}
			}
		}

		protected string DraggingDetect(Point pos)
		{
			//	Détecte la poignée visé par la souris.
			return this.DraggingDetect(pos, null);
		}

		protected virtual string DraggingDetect(Point pos, string exclude)
		{
			//	Détecte la poignée visé par la souris.
			return null;
		}

		protected virtual void DraggingStart(ref string handle, Point pos)
		{
			//	Début du drag d'une poignée.
		}

		protected virtual void DraggingMove(ref string handle, Point pos)
		{
			//	Déplace une poignée.
		}

		protected virtual void DraggingEnd(ref string handle, Point pos)
		{
			//	Fin du drag d'une poignée.
		}


		protected virtual void InvalidateBoxMarker()
		{
			//	Invalide la zone contenant le marqueur.
		}


		protected Color ColorBackground
		{
			//	Donne la couleur pour le fond de la règle.
			get
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				Color color = adorner.ColorWindow;
				if ( this.edited )
				{
					double factor = 0.3+0.6*color.GetBrightness();
					color = Drawing.Color.FromRgb(color.R*factor, color.G*factor, color.B*factor);  // couleur plus foncée
				}
				return color;
			}
		}

		protected Color ColorBackgroundEdited
		{
			//	Donne la couleur pour le fond de la zone éditée de la règle.
			get
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				return adorner.ColorWindow;
			}
		}

		protected Color ColorBackgroundMargins(bool hilite)
		{
			//	Donne la couleur pour le fond des marqueurs des marges.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			if ( hilite )
			{
				return adorner.ColorCaption;
			}
			else
			{
				Color color = adorner.ColorWindow;
				if ( color.GetBrightness() >= 0.9 )  // très clair ?
				{
					return Color.FromBrightness(1);  // blanc
				}
				else
				{
					return Color.FromBrightness(0.9);  // gris très clair
				}
			}
		}

		protected Color ColorBorderMargins
		{
			//	Donne la couleur pour le bord des marqueurs des marges.
			get
			{
				return Color.FromBrightness(0);  // noir
			}
		}


		#region Helpers.IToolTipHost
		public object GetToolTipCaption(Point pos)
		{
			//	Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
			if ( this.edited )  // édition en cours ?
			{
				return this.GetTooltipEditedText(pos);
			}
			else
			{
				return Res.Strings.Action.Text.Ruler.Drag;
			}
		}

		protected virtual string GetTooltipEditedText(Point pos)
		{
			//	Donne le texte du tooltip d'édition en fonction de la position.
			return null;  // pas de tooltip
		}
		#endregion


		protected static readonly double	defaultBreadth = 13;

		protected Document					document = null;
		protected bool						isVertical;
		protected double					ppm = 10.0;
		protected double					starting = 0.0;
		protected double					ending = 100.0;
		protected double					marker = 0.0;
		protected bool						markerVisible = false;
		protected bool						edited = false;
		protected Objects.AbstractText		editObject = null;
		protected double					limitLow = 0.0;
		protected double					limitHigh = 0.0;
		protected Rectangle					invalidateBox;
		protected string					hiliteHandle = null;
		protected bool						isDragging = false;
		protected string					draggingHandle = null;
	}
}
