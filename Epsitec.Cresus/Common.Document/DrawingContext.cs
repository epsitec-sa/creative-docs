using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	public enum ConstrainType
	{
		None,			// aucune
		Normal,			// horizontal, vertical et 45 degrés
		Square,			// uniquement 45 degrés
		Line,			// uniquement horizontal et vertical
		Scale,			// mise à l'échelle
		Rotate,			// rotation
	}

	public enum LayerDrawingMode
	{
		ShowInactive,	// affiche normalement tous les calques
		DimmedInactive,	// affiche en estompé les calques inactifs
		HideInactive,	// cache les calques inactifs
	}


	/// <summary>
	/// La classe DrawingContext contient le "device contexte".
	/// </summary>
	public class DrawingContext
	{
		public DrawingContext(Document document, Viewer viewer)
		{
			this.document = document;
			this.viewer = viewer;
			this.rootStack = new System.Collections.ArrayList();

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.gridStep   = new Point(1.0, 1.0);
				this.gridSubdiv = new Point(1.0, 1.0);
				this.gridOffset = new Point(0.0, 0.0);
			}
			else
			{
				this.gridStep   = new Point(50.0, 50.0);  // 5mm
				this.gridSubdiv = new Point(5.0, 5.0);
				this.gridOffset = new Point(0.0, 0.0);
			}
		}

		public Viewer Viewer
		{
			get { return this.viewer; }
		}

		public IAdorner Adorner
		{
			get { return this.adorner; }
			set { this.adorner = value; }
		}

		public GlyphPaintStyle GlyphPaintStyle
		{
			get { return this.glyphPaintStyle; }
			set { this.glyphPaintStyle = value; }
		}

		public Color UniqueColor
		{
			get { return this.uniqueColor; }
			set { this.uniqueColor = value; }
		}


		#region Zoom
		// Retourne l'origine horizontale minimale.
		public double MinOriginX
		{
			get
			{
				Size size = this.document.Size;
				Size area = this.document.Modifier.SizeArea;
				return (size.Width-area.Width)/2;
			}
		}

		// Retourne l'origine verticale minimale.
		public double MinOriginY
		{
			get
			{
				Size size = this.document.Size;
				Size area = this.document.Modifier.SizeArea;
				return (size.Height-area.Height)/2;
			}
		}

		// Retourne l'origine horizontale maximale.
		public double MaxOriginX
		{
			get
			{
				Size size = this.ContainerSize;
				Size area = this.document.Modifier.SizeArea;
				return (area.Width+this.MinOriginX) - size.Width/this.ScaleX;
			}
		}

		// Retourne l'origine verticale maximale.
		public double MaxOriginY
		{
			get
			{
				Size size = this.ContainerSize;
				Size area = this.document.Modifier.SizeArea;
				return (area.Height+this.MinOriginY) - size.Height/this.ScaleY;
			}
		}


		// Spécifie l'origine de la zone visible.
		public void Origin(Point origin)
		{
			this.Origin(origin.X, origin.Y);
		}

		public void Origin(double originX, double originY)
		{
			if ( this.originX != originX ||
				 this.originY != originY )
			{
				this.originX = originX;
				this.originY = originY;

				if ( this.document.Notifier != null )
				{
					this.document.Notifier.NotifyArea(this.viewer);
					this.document.Notifier.NotifyOriginChanged();
				}
			}
		}

		// Retourne le centre de la zone visible.
		public Point Center
		{
			get
			{
				Point center = new Point();
				Size cs = this.ContainerSize;
				center.X = -this.OriginX+(cs.Width/this.ScaleX)/2;
				center.Y = -this.OriginY+(cs.Height/this.ScaleY)/2;
				return center;
			}
		}

		// Vérifie si on utilise le zoom 100% centré.
		public bool IsZoomDefault
		{
			get
			{
				if ( this.zoom != 1.0 )  return false;

				Size cs = this.ContainerSize;
				Size size = this.document.Size;
				Point scale = this.ScaleForZoom(this.zoom);
				double originX = size.Width/2 - (cs.Width/scale.X)/2;
				double originY = size.Height/2 - (cs.Height/scale.Y)/2;

				return ( System.Math.Abs(this.originX+originX) < 0.00001 &&
						 System.Math.Abs(this.originY+originY) < 0.00001 );
			}
		}

		// Remet le zoom et le centre par défaut.
		public void ZoomAndCenter()
		{
			this.ZoomAndCenter(1.0, this.document.Size.Width/2, this.document.Size.Height/2);
		}

		// Spécifie le zoom et le centre de la zone visible.
		public void ZoomAndCenter(double zoom, Point center)
		{
			this.ZoomAndCenter(zoom, center.X, center.Y);
		}

		public void ZoomAndCenter(double zoom, double centerX, double centerY)
		{
			Size cs = this.ContainerSize;
			Point scale = this.ScaleForZoom(zoom);
			double originX = centerX - (cs.Width/scale.X)/2;
			double originY = centerY - (cs.Height/scale.Y)/2;
			this.ZoomAndOrigin(zoom, -originX, -originY);
		}

		protected void ZoomAndOrigin(double zoom, double originX, double originY)
		{
			if ( this.zoom    != zoom    ||
				 this.originX != originX ||
				 this.originY != originY )
			{
				this.zoom    = zoom;
				this.originX = originX;
				this.originY = originY;

				if ( this.document.Notifier != null )
				{
					this.document.Notifier.NotifyArea(this.viewer);
					this.document.Notifier.NotifyZoomChanged();
					this.document.Notifier.NotifyOriginChanged();
				}
			}
		}

		// Zoom courant.
		public double Zoom
		{
			get
			{
				return this.zoom;
			}

			set
			{
				if ( this.zoom != value )
				{
					this.zoom = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyZoomChanged();
					}
				}
			}
		}

		// Origine horizontale de la zone visible.
		public double OriginX
		{
			get
			{
				return this.originX;
			}

			set
			{
				if ( this.originX != value )
				{
					this.originX = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyOriginChanged();
					}
				}
			}
		}

		// Origine verticale de la zone visible.
		public double OriginY
		{
			get
			{
				return this.originY;
			}

			set
			{
				if ( this.originY != value )
				{
					this.originY = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyOriginChanged();
					}
				}
			}
		}
		#endregion

		// Taille du conteneur, qui peut être un viewer ou une taille fixe.
		// Si le viewer existe, il est inutile d'appeler ContainerSize.set
		// Si le viewer n'existe pas, il faut appeler ContainerSize.set
		public Size ContainerSize
		{
			set
			{
				System.Diagnostics.Debug.Assert(this.viewer == null);
				this.containerSize = value;
			}

			get
			{
				if ( this.viewer == null )
				{
					return this.containerSize;
				}
				else
				{
					return this.viewer.Client.Size;
				}
			}
		}

		// Echelles à utiliser pour le dessin pour un zoom donné.
		protected Point ScaleForZoom(double zoom)
		{
			Size size = this.ContainerSize;
			double sx = zoom*size.Width/this.document.Size.Width;
			double sy = zoom*size.Height/this.document.Size.Height;
			double scale = System.Math.Min(sx, sy);
			if ( this.document.Type != DocumentType.Pictogram )
			{
				scale *= 0.9;  // ch'tite marge
			}
			return new Point(scale, scale);
		}

		// Echelles à utiliser pour le dessin.
		public Point Scale
		{
			get
			{
				return this.ScaleForZoom(this.zoom);
			}
		}

		// Echelle horizontale à utiliser pour le dessin.
		public double ScaleX
		{
			get { return this.Scale.X; }
		}

		// Echelle verticale à utiliser pour le dessin.
		public double ScaleY
		{
			get { return this.Scale.Y; }
		}


		// Mode de dessin pour les calques.
		public LayerDrawingMode LayerDrawingMode
		{
			get { return this.layerDrawingMode; }
			set { this.layerDrawingMode = value; }
		}

		// Mode "aperçu avant impression".
		public bool PreviewActive
		{
			get
			{
				return this.previewActive;
			}
			
			set
			{
				if ( this.previewActive != value )
				{
					this.previewActive = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyPreviewChanged();
					}
				}
			}
		}


		#region Grid
		// Action de la grille magnétique.
		public bool GridActive
		{
			get
			{
				return this.gridActive;
			}

			set
			{
				if ( this.gridActive != value )
				{
					this.gridActive = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		// Affichage de la grille magnétique.
		public bool GridShow
		{
			get
			{
				return this.gridShow;
			}

			set
			{
				if ( this.gridShow != value )
				{
					this.gridShow = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		// Pas de la grille magnétique.
		public Point GridStep
		{
			get
			{
				return this.gridStep;
			}

			set
			{
				if ( this.gridStep != value )
				{
					this.gridStep = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		// Subdivisions de la grille magnétique.
		public Point GridSubdiv
		{
			get
			{
				return this.gridSubdiv;
			}

			set
			{
				if ( this.gridSubdiv != value )
				{
					this.gridSubdiv = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		// Décalage de la grille magnétique.
		public Point GridOffset
		{
			get
			{
				return this.gridOffset;
			}

			set
			{
				if ( this.gridOffset != value )
				{
					this.gridOffset = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		// Force un point sur la grille magnétique.
		public void SnapGrid(ref Point pos)
		{
			bool snapX, snapY;

			if ( !this.gridActive ^ this.isAlt )
			{
				this.SnapGuides(ref pos, out snapX, out snapY);
				return;
			}

			Point offset = new Point(0.0, 0.0);
			if ( this.document.Type == DocumentType.Pictogram )
			{
				offset = new Point(this.gridStep.X/2, this.gridStep.Y/2);
			}

			Point guidePos = pos;
			this.SnapGuides(ref guidePos, out snapX, out snapY);

			pos = Point.GridAlign(pos, offset-this.gridOffset, this.gridStep);

			if ( snapX )  pos.X = guidePos.X;
			if ( snapY )  pos.Y = guidePos.Y;
		}

		// Force un point sur la grille magnétique.
		public void SnapGrid(Point origin, ref Point pos)
		{
			if ( !this.gridActive || this.isAlt )  return;
			pos -= origin;
			pos = Point.GridAlign(pos, -this.gridOffset, this.gridStep);
			pos += origin;
		}
		#endregion


		#region Guides
		// Action des repères magnétiques.
		public bool GuidesActive
		{
			get
			{
				return this.guidesActive;
			}

			set
			{
				if ( this.guidesActive != value )
				{
					this.guidesActive = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifySettingsChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		// Affichage des repères magnétiques.
		public bool GuidesShow
		{
			get
			{
				return this.guidesShow;
			}

			set
			{
				if ( this.guidesShow != value )
				{
					this.guidesShow = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifySettingsChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		// Force un point sur un repère magnétique.
		protected void SnapGuides(ref Point pos, out bool snapX, out bool snapY)
		{
			snapX = false;
			snapY = false;
			if ( !this.guidesActive ^ this.isAlt )  return;

			int total = this.document.Settings.GuidesCount;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Guide guide = this.document.Settings.GuidesGet(i);

				if ( guide.IsHorizontal )  // repère horizontal ?
				{
					double len = System.Math.Abs(pos.Y - guide.AbsolutePosition);
					if ( len <= this.GuideMargin )
					{
						pos.Y = guide.AbsolutePosition;
						snapY = true;
					}
				}
				else	// repère vertical ?
				{
					double len = System.Math.Abs(pos.X - guide.AbsolutePosition);
					if ( len <= this.GuideMargin )
					{
						pos.X = guide.AbsolutePosition;
						snapX = true;
					}
				}
			}
		}
		#endregion


		// Mode caché à moitié (estomper).
		public bool HideHalfActive
		{
			get
			{
				return this.hideHalfActive;
			}

			set
			{
				if ( this.hideHalfActive != value )
				{
					this.hideHalfActive = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyHideHalfChanged();
					}
				}
			}
		}


		// Indique si le viewer associé à ce contexte est actif.
		public bool IsActive
		{
			get
			{
				return ( this.viewer == this.document.Modifier.ActiveViewer );
			}
		}

		// Indique si l'icône est estompée.
		public bool IsDimmed
		{
			get
			{
				return this.isDimmed;
			}

			set
			{
				if ( this.isDimmed != value )
				{
					this.isDimmed = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
					}
				}
			}
		}

		#region DrawBox
		// Indique s'il faut afficher les bbox.
		public bool IsDrawBoxThin
		{
			get
			{
				return this.isDrawBoxThin;
			}

			set
			{
				if ( this.isDrawBoxThin != value )
				{
					this.isDrawBoxThin = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyDebugChanged();
					}
				}
			}
		}

		// Indique s'il faut afficher les bbox.
		public bool IsDrawBoxGeom
		{
			get
			{
				return this.isDrawBoxGeom;
			}

			set
			{
				if ( this.isDrawBoxGeom != value )
				{
					this.isDrawBoxGeom = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyDebugChanged();
					}
				}
			}
		}

		// Indique s'il faut afficher les bbox.
		public bool IsDrawBoxFull
		{
			get
			{
				return this.isDrawBoxFull;
			}

			set
			{
				if ( this.isDrawBoxFull != value )
				{
					this.isDrawBoxFull = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyDebugChanged();
					}
				}
			}
		}
		#endregion

		// Taille minimale que doit avoir un objet à sa création.
		public double MinimalSize
		{
			get { return this.minimalSize/this.ScaleX; }
		}

		// Epaisseur minimale d'un objet pour la détection du coutour.
		public double MinimalWidth
		{
			get { return this.minimalWidth/this.ScaleX; }
		}

		// Marge pour fermer un polygone.
		public double CloseMargin
		{
			get { return this.closeMargin/this.ScaleX; }
		}

		// Marge magnétique d'un repère.
		public double GuideMargin
		{
			get { return this.guideMargin/this.ScaleX; }
		}

		// Taille supplémentaire lorsqu'un objet est survolé par la souris.
		public double HiliteSize
		{
			get { return this.hiliteSize/this.ScaleX; }
		}

		// Taille d'une poignée.
		public double HandleSize
		{
			get { return this.handleSize/this.ScaleX; }
		}

		// Marge à ajouter à la bbox lors du dessin, pour résoudre le cas des poignées
		// qui débordent d'un objet avec un trait mince, et du mode Hilite qui augmente
		// l'épaisseur lors du survol de la souris.
		public double SelectMarginSize
		{
			get { return System.Math.Max(this.handleSize+4, this.hiliteSize)/this.ScaleX/2; }
		}

		// Adapte une couleur en fonction de l'état de l'icône.
		public Color AdaptColor(Color color)
		{
			if ( this.modifyColor != null )
			{
				this.modifyColor(ref color);
			}

			if ( this.adorner != null )
			{
				this.adorner.AdaptPictogramColor(ref color, this.glyphPaintStyle, this.uniqueColor);
			}

			if ( this.isDimmed )  // estompé (hors groupe) ?
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.05;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.1, 1.0);  // augmente l'intensité
				color = Color.FromBrightness(intensity);
				color.A = alpha*0.2;  // très transparent
			}

			return color;
		}

		public delegate void ModifyColor(ref Color color);
		public ModifyColor modifyColor;

		// Couleur lorsqu'un objet est survolé par la souris.
		public Color HiliteOutlineColor
		{
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.8);
			}
		}

		// Couleur lorsqu'un objet est survolé par la souris.
		public Color HiliteSurfaceColor
		{
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
				return Color.FromColor(adorner.ColorCaption, 0.4);
			}
		}


		#region SuperShift
		// Indique si la touche Shift est pressée.
		public bool IsShift
		{
			get
			{
				return this.isShift;
			}
			
			set
			{
				if ( this.isShift != value )
				{
					this.isShift = value;

					if ( this.constrainType != ConstrainType.None )
					{
						this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					}
				}
			}
		}

		// Indique si la touche Ctrl est pressée.
		public bool IsCtrl
		{
			get
			{
				return this.isCtrl;
			}
			
			set
			{
				this.isCtrl = value;
			}
		}

		// Indique si la touche Alt est pressée.
		public bool IsAlt
		{
			get
			{
				return this.isAlt; 
			}
			
			set
			{
				this.isAlt = value;
			}
		}
		#endregion

		#region Constrain
		// Fixe le point initial pour les contraintes.
		public void ConstrainFixStarting(Point pos)
		{
			this.constrainStarting = pos;
			this.constrainOrigin = pos;
		}

		// Fixe le point initial pour les contraintes.
		public void ConstrainFixStarting(Point origin, Point pos)
		{
			this.constrainStarting = pos;
			this.constrainOrigin = origin;
		}

		// Fixe le type des contraintes.
		public void ConstrainFixType(ConstrainType type)
		{
			if ( this.constrainType != type )
			{
				this.constrainType = type;

				if ( this.IsShift )
				{
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
				}
			}
		}

		// Retourne une position éventuellement contrainte.
		public void ConstrainSnapPos(ref Point pos)
		{
			if ( this.constrainType == ConstrainType.None || !this.isShift )  return;

			if ( this.constrainType == ConstrainType.Normal ||
				 this.constrainType == ConstrainType.Rotate )
			{
				double angle = Point.ComputeAngleDeg(this.constrainStarting, pos);
				double dist = Point.Distance(pos, this.constrainStarting);
				angle = System.Math.Floor((angle+22.5)/45)*45;
				pos = Transform.RotatePointDeg(this.constrainStarting, angle, this.constrainStarting+new Point(dist,0));
			}

			if ( this.constrainType == ConstrainType.Square )
			{
				double angle = Point.ComputeAngleDeg(this.constrainStarting, pos);
				double dist = Point.Distance(pos, this.constrainStarting);
				angle += 45;
				angle = System.Math.Floor((angle+45)/90)*90;
				angle -= 45;
				pos = Transform.RotatePointDeg(this.constrainStarting, angle, this.constrainStarting+new Point(dist,0));
			}

			if ( this.constrainType == ConstrainType.Line )
			{
				if ( System.Math.Abs(pos.X-this.constrainStarting.X) < System.Math.Abs(pos.Y-this.constrainStarting.Y) )
				{
					pos.X = this.constrainStarting.X;
				}
				else
				{
					pos.Y = this.constrainStarting.Y;
				}
			}

			if ( this.constrainType == ConstrainType.Scale )
			{
				double dist = Point.Distance(this.constrainStarting, pos);
				dist = System.Math.Min(dist/4, 10.0/this.ScaleX);
				Point proj = Point.Projection(this.constrainStarting, this.constrainOrigin, pos);
				if ( Point.Distance(proj, pos) < dist )
				{
					pos = proj;
				}
				else
				{
					if ( System.Math.Abs(pos.X-this.constrainStarting.X) < System.Math.Abs(pos.Y-this.constrainStarting.Y) )
					{
						pos.X = this.constrainStarting.X;
					}
					else
					{
						pos.Y = this.constrainStarting.Y;
					}
				}
			}
		}

		// Enlève le point initial pour les contraintes.
		public void ConstrainDelStarting()
		{
			if ( this.constrainType == ConstrainType.None )  return;
			this.constrainType = ConstrainType.None;

			if ( this.IsShift )
			{
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			}
		}

		// Dessine les contraintes.
		public void DrawConstrain(Graphics graphics, Size size)
		{
			if ( this.constrainType == ConstrainType.None || !this.isShift )  return;

			graphics.LineWidth = 1.0/this.ScaleX;
			Point pos = this.constrainStarting;
			ConstrainType type = this.constrainType;
			double max = System.Math.Max(size.Width, size.Height);

			if ( type == ConstrainType.Normal ||
				 type == ConstrainType.Rotate ||
				 type == ConstrainType.Line   ||
				 type == ConstrainType.Scale  )
			{
				graphics.AddLine(pos.X, -size.Height, pos.X, size.Height);
				graphics.AddLine(-size.Width, pos.Y, size.Width, pos.Y);
				graphics.RenderSolid(DrawingContext.ColorConstrain);
			}

			if ( type == ConstrainType.Normal ||
				 type == ConstrainType.Rotate ||
				 type == ConstrainType.Square )
			{
				Point p1 = Transform.RotatePointDeg(pos, 180.0*0.25, pos+new Point(max,0));
				Point p2 = Transform.RotatePointDeg(pos, 180.0*1.25, pos+new Point(max,0));
				graphics.AddLine(p1, p2);

				p1 = Transform.RotatePointDeg(pos, 180.0*0.75, pos+new Point(max,0));
				p2 = Transform.RotatePointDeg(pos, 180.0*1.75, pos+new Point(max,0));
				graphics.AddLine(p1, p2);

				graphics.RenderSolid(DrawingContext.ColorConstrain);
			}

			if ( this.constrainType == ConstrainType.Scale )
			{
				Point p1 = Point.Move(this.constrainStarting, this.constrainOrigin, max);
				Point p2 = Point.Move(this.constrainOrigin, this.constrainStarting, max);
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(DrawingContext.ColorConstrain);
			}
		}
		#endregion


		#region GetColors
		// Retourne la couleur pour indiquer une sélection multiple.
		static public Color ColorMulti
		{
			get { return Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur pour indiquer une sélection multiple.
		static public Color ColorMultiBack
		{
			get { return Color.FromARGB(0.15, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur pour indiquer un style.
		static public Color ColorStyle
		{
			get { return Color.FromARGB(1.0, 0.0, 0.5, 1.0); }  // bleu
		}

		// Retourne la couleur pour indiquer un style.
		static public Color ColorStyleBack
		{
			get { return Color.FromARGB(0.15, 0.0, 0.5, 1.0); }  // bleu
		}

		// Retourne la couleur du pourtour d'une poignée.
		static public Color ColorHandleOutline
		{
			get { return Color.FromARGB(1.0, 0.0, 0.0, 0.0); }  // noir
		}

		// Retourne la couleur d'une poignée principale.
		static public Color ColorHandleMain
		{
			get { return Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur d'une poignée de début/fin.
		static public Color ColorHandleStart
		{
			get { return Color.FromARGB(1.0, 0.0, 1.0, 0.0); }  // vert
		}

		// Retourne la couleur d'une poignée de propriété.
		static public Color ColorHandleProperty
		{
			get { return Color.FromARGB(1.0, 0.0, 1.0, 1.0); }  // cyan
		}

		// Retourne la couleur d'une poignée de sélection globale.
		static public Color ColorHandleGlobal
		{
			get { return Color.FromARGB(1.0, 1.0, 1.0, 1.0); }  // blanc
		}

		// Retourne la couleur pour dessiner une contrainte.
		static public Color ColorConstrain
		{
			get { return Color.FromARGB(0.5, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur du cadre pendant l'édition.
		static public Color ColorFrameEdit
		{
			get { return Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur des sélections pendant l'édition.
		static public Color ColorSelectEdit
		{
			get { return Color.FromARGB(1.0, 1.0, 1.0, 0.0); }  // jaune
		}
		#endregion


		#region RootStack
		// Vide toute la pile.
		public void RootStackClear()
		{
			this.rootStack.Clear();
		}

		// Indique si la pile est vide.
		public bool RootStackIsEmpty
		{
			get { return (this.rootStack.Count < 2); }  // au moins page.calque ?
		}

		// Indique si on est à la racine (donc dans page.calque).
		public bool RootStackIsBase
		{
			get { return (this.rootStack.Count == 2); }
		}

		// Retourne la profondeur.
		public int RootStackDeep
		{
			get { return this.rootStack.Count; }
		}

		// Ajoute un nouvel élément.
		public void RootStackPush(int index)
		{
			this.InsertOpletRootStack();
			this.rootStack.Add(index);
		}

		// Retire le dernier élément.
		public int RootStackPop()
		{
			if ( this.rootStack.Count == 0 )  return -1;
			this.InsertOpletRootStack();
			int index = (int) this.rootStack[this.rootStack.Count-1];
			this.rootStack.RemoveAt(this.rootStack.Count-1);
			return index;
		}

		// Retourne l'objet racine le plus profond.
		// Il s'agira d'un calque ou d'un groupe.
		public Objects.Abstract RootObject()
		{
			return this.RootObject(1000);
		}

		// Retourne l'objet racine à une profondeur donnée.
		public Objects.Abstract RootObject(int deepMax)
		{
			UndoableList list = this.document.GetObjects;
			Objects.Abstract obj = null;
			int deep = System.Math.Min(this.rootStack.Count, deepMax);
			for ( int i=0 ; i<deep ; i++ )
			{
				System.Diagnostics.Debug.Assert(list != null);
				int index = (int) this.rootStack[i];
				System.Diagnostics.Debug.Assert(index < list.Count);
				obj = list[index] as Objects.Abstract;
				System.Diagnostics.Debug.Assert(obj != null);
				list = obj.Objects;
			}
			return obj;
		}


		// Spécifie une page et un calque.
		public void PageLayer(int page, int layer)
		{
			this.InsertOpletRootStack();
			this.InternalPageLayer(page, layer);
		}

		// Spécifie une page et un calque.
		public void InternalPageLayer(int page, int layer)
		{
			this.document.Modifier.OpletQueueEnable = false;
			this.RootStackClear();
			this.RootStackPush(page);
			this.RootStackPush(layer);
			this.document.Modifier.OpletQueueEnable = true;
		}

		// Retourne le nombre total de pages.
		public int TotalPages()
		{
			return this.document.GetObjects.Count;
		}

		// Retourne le nombre total de calques.
		public int TotalLayers()
		{
			Objects.Abstract page = this.RootObject(1);
			return page.Objects.Count;
		}

		// Page courante.
		public int CurrentPage
		{
			get
			{
				System.Diagnostics.Debug.Assert(!this.RootStackIsEmpty);
				return (int) this.rootStack[0];
			}

			set
			{
				System.Diagnostics.Debug.Assert(!this.RootStackIsEmpty);
				int newPage  = value;
				int newLayer = 0;
				int curPage  = this.CurrentPage;
				int curLayer = this.CurrentLayer;

				if ( newPage != curPage || newLayer != curLayer )
				{
					if ( this.document.Modifier == null )
					{
						this.InternalPageLayer(newPage, newLayer);
					}
					else
					{
						this.document.Modifier.OpletQueueBeginAction();
						this.document.Modifier.InitiateChangingPage();
						this.document.Modifier.TerminateChangingPage(newPage);
						this.document.Modifier.OpletQueueValidateAction();
					}

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
						this.document.Notifier.NotifySelectionChanged();
						this.document.Notifier.NotifyPagesChanged();
						this.document.Notifier.NotifyLayersChanged();
					}
				}
			}
		}

		// Calque courant.
		public int CurrentLayer
		{
			get
			{
				System.Diagnostics.Debug.Assert(!this.RootStackIsEmpty);
				return (int) this.rootStack[1];
			}

			set
			{
				System.Diagnostics.Debug.Assert(!this.RootStackIsEmpty);
				int newLayer = value;
				int curLayer = this.CurrentLayer;

				if ( newLayer != curLayer )
				{
					if ( this.document.Modifier == null )
					{
						this.InternalPageLayer(this.CurrentPage, newLayer);
					}
					else
					{
						this.document.Modifier.OpletQueueBeginAction();
						this.document.Modifier.InitiateChangingLayer();
						this.document.Modifier.TerminateChangingLayer(newLayer);
						this.document.Modifier.OpletQueueValidateAction();
					}

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
						this.document.Notifier.NotifySelectionChanged();
						this.document.Notifier.NotifyLayersChanged();
					}
				}
			}
		}
		#endregion

		#region OpletRootStack
		// Ajoute un oplet pour mémoriser les informations de sélection de l'objet.
		protected void InsertOpletRootStack()
		{
			if ( this.document.Modifier == null )  return;
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletRootStack oplet = new OpletRootStack(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise tout le RootStack.
		protected class OpletRootStack : AbstractOplet
		{
			public OpletRootStack(DrawingContext host)
			{
				this.host = host;

				this.rootStack = new System.Collections.ArrayList();
				int total = this.host.rootStack.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					int index = (int) this.host.rootStack[i];
					this.rootStack.Add(index);
				}
			}

			protected void Swap()
			{
				System.Collections.ArrayList temp = new System.Collections.ArrayList();

				// this.host.rootStack -> temp
				int total = this.host.rootStack.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					int index = (int) this.host.rootStack[i];
					temp.Add(index);
				}

				// this.rootStack -> this.host.rootStack
				this.host.rootStack.Clear();
				total = this.rootStack.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					int index = (int) this.rootStack[i];
					this.host.rootStack.Add(index);
				}

				// temp -> this.rootStack
				this.rootStack.Clear();
				total = temp.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					int index = (int) temp[i];
					this.rootStack.Add(index);
				}

				this.host.document.Modifier.DirtyCounters();
				this.host.document.Notifier.NotifyArea();
				this.host.document.Notifier.NotifyPagesChanged();
				this.host.document.Notifier.NotifyLayersChanged();
				this.host.document.Notifier.NotifySelectionChanged();
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected DrawingContext				host;
			protected System.Collections.ArrayList	rootStack;
		}
		#endregion


		protected Document						document;
		protected Viewer						viewer;
		protected Size							containerSize;
		protected IAdorner						adorner;
		protected GlyphPaintStyle				glyphPaintStyle;
		protected Color							uniqueColor;
		protected double						zoom = 1;
		protected double						originX = 0;
		protected double						originY = 0;
		protected LayerDrawingMode				layerDrawingMode = LayerDrawingMode.DimmedInactive;
		protected bool							previewActive = false;
		protected bool							gridActive = false;
		protected bool							gridShow = false;
		protected Point							gridStep = new Point(1, 1);
		protected Point							gridSubdiv = new Point(1, 1);
		protected Point							gridOffset = new Point(0, 0);
		protected bool							guidesActive = true;
		protected bool							guidesShow = true;
		protected bool							hideHalfActive = true;
		protected bool							isDimmed = false;
		protected bool							isDrawBoxThin = false;
		protected bool							isDrawBoxGeom = false;
		protected bool							isDrawBoxFull = false;
		protected double						minimalSize = 3;
		protected double						minimalWidth = 5;
		protected double						closeMargin = 10;
		protected double						guideMargin = 8;
		protected double						hiliteSize = 6;
		protected double						handleSize = 10;
		protected bool							isShift = false;
		protected bool							isCtrl = false;
		protected bool							isAlt = false;
		protected Point							constrainStarting;
		protected Point							constrainOrigin;
		protected ConstrainType					constrainType;
		protected System.Collections.ArrayList	rootStack;
	}
}
