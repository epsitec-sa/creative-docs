using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe AbstractRuler implémente la classe de base des règles
	/// HRuler et VRuler.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractRuler : Widget
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


		// Nombre de points/millimètres pour la graduation.
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

		// Début de la graduation.
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

		// Fin de la graduation.
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

		// Visibilité du marqueur.
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

		// Objet en cours d'édition.
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

		// Limite basse.
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

		// Limite haute.
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
						int rank = this.MoveDetect(pos);
						if ( rank != -1 )
						{
							this.isDragging = true;
							this.draggingRank = rank;
							this.MoveBeginning(this.draggingRank, pos);
							message.Consumer = this;
							return;
						}
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
							this.MoveDragging(this.draggingRank, pos);
							message.Consumer = this;
							return;
						}
						else
						{
							this.HiliteRank = this.MoveDetect(pos);
						}
					}
					break;

				case MessageType.MouseUp:
					if ( this.edited )
					{
						if ( this.isDragging )
						{
							this.MoveEnding(this.draggingRank, pos);
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
						this.HiliteRank = -1;
					}
					break;
			}

			base.ProcessMessage(message, pos);
		}

		protected int HiliteRank
		{
			get
			{
				return this.hiliteRank;
			}

			set
			{
				if ( this.hiliteRank != value )
				{
					this.hiliteRank = value;
					this.Invalidate();
				}
			}
		}

		protected virtual int MoveDetect(Point pos)
		{
			return -1;
		}

		protected virtual void MoveBeginning(int rank, Point pos)
		{
		}

		protected virtual void MoveDragging(int rank, Point pos)
		{
		}

		protected virtual void MoveEnding(int rank, Point pos)
		{
		}


		protected virtual void InvalidateBoxMarker()
		{
		}


		// Donne la couleur pour le fond de la règle.
		protected Color ColorBackground
		{
			get
			{
				IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
				Color color = adorner.ColorWindow;
				if ( this.edited && color.GetBrightness() >= 0.9 )  // très clair ?
				{
					color.R *= 0.85;  // couleur plus foncée
					color.G *= 0.85;
					color.B *= 0.85;
				}
				return color;
			}
		}

		// Donne la couleur pour le fond de la zone éditée de la règle.
		protected Color ColorBackgroundEdited
		{
			get
			{
				return Color.FromBrightness(1);  // blanc
			}
		}

		// Donne la couleur pour le fond des marqueurs des marges.
		protected Color ColorBackgroundMargins(bool hilite)
		{
			if ( hilite )
			{
				IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
				return adorner.ColorCaption;
			}
			else
			{
				return Color.FromBrightness(0.9);  // gris très clair
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
		protected int						hiliteRank = -1;
		protected bool						isDragging = false;
		protected int						draggingRank = -1;
	}
}
