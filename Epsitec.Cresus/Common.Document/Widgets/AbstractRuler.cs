using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe AbstractRuler impl�mente la classe de base des r�gles
	/// HRuler et VRuler.
	/// </summary>
	[Support.SuppressBundleSupport]
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


		// Nombre de points/millim�tres pour la graduation.
		public double PPM
		{
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

		// D�but de la graduation, pour permettre la conversion points �cran en
		// points document et invers�ment.
		public double Starting
		{
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

		// Fin de la graduation, pour permettre la conversion points �cran en
		// points document et invers�ment.
		public double Ending
		{
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

		// Position du marqueur.
		public double Marker
		{
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

		// Visibilit� du marqueur.
		public bool MarkerVisible
		{
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

		// Edition en cours ?
		public bool Edited
		{
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

		// Objet en cours d'�dition.
		public Objects.Abstract EditObject
		{
			get
			{
				return this.editObject;
			}

			set
			{
				this.editObject = value;
			}
		}

		// Limite basse, selon la bbox de l'obet �dit�.
		public double LimitLow
		{
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

		// Limite haute, selon la bbox de l'obet �dit�.
		public double LimitHigh
		{
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

			switch ( message.Type )
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
							this.DraggingMove(this.draggingHandle, pos);
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
							this.DraggingEnd(this.draggingHandle, pos);
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
						this.HiliteHandle = -1;
					}
					break;
			}

			base.ProcessMessage(message, pos);
		}

		// Poign�e mise en �vidence.
		protected int HiliteHandle
		{
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

		// D�tecte la poign�e vis� par la souris.
		protected int DraggingDetect(Point pos)
		{
			return this.DraggingDetect(pos, -1);
		}

		// D�tecte la poign�e vis� par la souris.
		protected virtual int DraggingDetect(Point pos, int exclude)
		{
			return -1;
		}

		// D�but du drag d'une poign�e.
		protected virtual void DraggingStart(ref int handle, Point pos)
		{
		}

		// D�place une poign�e.
		protected virtual void DraggingMove(int handle, Point pos)
		{
		}

		// Fin du drag d'une poign�e.
		protected virtual void DraggingEnd(int handle, Point pos)
		{
		}


		// Invalide la zone contenant le marqueur.
		protected virtual void InvalidateBoxMarker()
		{
		}


		// Donne la couleur pour le fond de la r�gle.
		protected Color ColorBackground
		{
			get
			{
				IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
				Color color = adorner.ColorWindow;
				if ( this.edited )
				{
					double factor = 0.3+0.6*color.GetBrightness();
					color.R *= factor;  // couleur plus fonc�e
					color.G *= factor;
					color.B *= factor;
				}
				return color;
			}
		}

		// Donne la couleur pour le fond de la zone �dit�e de la r�gle.
		protected Color ColorBackgroundEdited
		{
			get
			{
				IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
				return adorner.ColorWindow;
			}
		}

		// Donne la couleur pour le fond des marqueurs des marges.
		protected Color ColorBackgroundMargins(bool hilite)
		{
			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
			if ( hilite )
			{
				return adorner.ColorCaption;
			}
			else
			{
				Color color = adorner.ColorWindow;
				if ( color.GetBrightness() >= 0.9 )  // tr�s clair ?
				{
					return Color.FromBrightness(1);  // blanc
				}
				else
				{
					return Color.FromBrightness(0.9);  // gris tr�s clair
				}
			}
		}

		// Donne la couleur pour le bord des marqueurs des marges.
		protected Color ColorBorderMargins
		{
			get
			{
				return Color.FromBrightness(0);  // noir
			}
		}


		#region Helpers.IToolTipHost
		// Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
		public object GetToolTipCaption(Point pos)
		{
			if ( this.edited )  // �dition en cours ?
			{
				return this.GetTooltipEditedText(pos);
			}
			else
			{
				return Res.Strings.Action.Text.Ruler.Drag;
			}
		}

		// Donne le texte du tooltip d'�dition en fonction de la position.
		protected virtual string GetTooltipEditedText(Point pos)
		{
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
		protected Objects.Abstract			editObject = null;
		protected double					limitLow = 0.0;
		protected double					limitHigh = 0.0;
		protected Rectangle					invalidateBox;
		protected int						hiliteHandle = -1;
		protected bool						isDragging = false;
		protected int						draggingHandle = -1;
	}
}
