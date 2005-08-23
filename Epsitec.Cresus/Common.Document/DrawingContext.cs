using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
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
			this.drawer = new Drawer(this.document);
			this.rootStack = new System.Collections.ArrayList();
			this.masterPageList = new System.Collections.ArrayList();
			this.magnetLayerList = new System.Collections.ArrayList();

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

			this.magnetLineMain   = new MagnetLine(this.document, this, MagnetLine.Type.Main);
			this.magnetLineBegin  = new MagnetLine(this.document, this, MagnetLine.Type.Perp);
			this.magnetLineEnd    = new MagnetLine(this.document, this, MagnetLine.Type.Perp);
			this.magnetLineMiddle = new MagnetLine(this.document, this, MagnetLine.Type.Perp);
			this.magnetLinePerp   = new MagnetLine(this.document, this, MagnetLine.Type.Perp);
			this.magnetLineInter  = new MagnetLine(this.document, this, MagnetLine.Type.Inter);
			this.magnetLineProj   = new MagnetLine(this.document, this, MagnetLine.Type.Proj);
		}

		public Viewer Viewer
		{
			get { return this.viewer; }
		}

		public Drawer Drawer
		{
			get { return this.drawer; }
		}


		#region Zoom
		// Retourne l'origine horizontale minimale.
		public double MinOriginX
		{
			get
			{
				Size size = this.document.Size;
				Size area = this.document.Modifier.SizeArea;
				Size container = this.ContainerSize/this.ScaleX;
				
				double min = -(area.Width-size.Width)/2;

				if ( area.Width < container.Width )  // fenêtre trop grande ?
				{
					min -= (container.Width-area.Width)/2;  // centrer
				}

				return min;
			}
		}

		// Retourne l'origine verticale minimale.
		public double MinOriginY
		{
			get
			{
				Size size = this.document.Size;
				Size area = this.document.Modifier.SizeArea;
				Size container = this.ContainerSize/this.ScaleY;
				
				double min = -(area.Height-size.Height)/2;

				if ( area.Height < container.Height )  // fenêtre trop grande ?
				{
					min -= (container.Height-area.Height)/2;  // centrer
				}

				return min;
			}
		}

		// Retourne l'origine horizontale maximale.
		public double MaxOriginX
		{
			get
			{
				Size area = this.document.Modifier.SizeArea;
				Size container = this.ContainerSize/this.ScaleX;

				if ( area.Width < container.Width )  // fenêtre trop grande ?
				{
					return this.MinOriginX;
				}

				return (area.Width+this.MinOriginX) - container.Width;
			}
		}

		// Retourne l'origine verticale maximale.
		public double MaxOriginY
		{
			get
			{
				Size area = this.document.Modifier.SizeArea;
				Size container = this.ContainerSize/this.ScaleY;

				if ( area.Height < container.Height )  // fenêtre trop grande ?
				{
					return this.MinOriginY;
				}

				return (area.Height+this.MinOriginY) - container.Height;
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

		// Vérifie si on utilise le zoom pleine page centré.
		public bool IsZoomPage
		{
			get
			{
				if ( System.Math.Abs(this.zoom-this.ZoomPage) > 0.00001 )  return false;

				Size cs = this.ContainerSize;
				Size size = this.document.Size;
				Point scale = this.ScaleForZoom(this.zoom);
				double originX = size.Width/2 - (cs.Width/scale.X)/2;
				double originY = size.Height/2 - (cs.Height/scale.Y)/2;

				return ( System.Math.Abs(this.originX+originX) < 0.00001 &&
						 System.Math.Abs(this.originY+originY) < 0.00001 );
			}
		}

		// Vérifie si on utilise le zoom pleine page centré.
		public bool IsZoomPageWidth
		{
			get
			{
				if ( System.Math.Abs(this.zoom-this.ZoomPageWidth) > 0.00001 )  return false;

				Size cs = this.ContainerSize;
				Size size = this.document.Size;
				Point scale = this.ScaleForZoom(this.zoom);
				double originX = size.Width/2 - (cs.Width/scale.X)/2;

				return ( System.Math.Abs(this.originX+originX) < 0.00001 );
			}
		}

		// Retourne le zoom pleine page.
		public double ZoomPage
		{
			get
			{
				if ( this.document.Type == DocumentType.Pictogram )
				{
					return 1.0;
				}
				else
				{
					Size cs = this.ContainerSize;
					if ( cs.Width <= 0.0 || cs.Height <= 0.0 )  return 1.0;
					Size size = this.document.Size;
					double zx = cs.Width/size.Width;
					double zy = cs.Height/size.Height;
					double dpi = this.document.GlobalSettings.ScreenDpi;
					return System.Math.Min(zx, zy)*2.54*(96.0/dpi);
				}
			}
		}

		// Retourne le zoom largeur page.
		public double ZoomPageWidth
		{
			get
			{
				if ( this.document.Type == DocumentType.Pictogram )
				{
					return 1.0;
				}
				else
				{
					Size cs = this.ContainerSize;
					if ( cs.Width <= 0.0 || cs.Height <= 0.0 )  return 1.0;
					Size size = this.document.Size;
					return (cs.Width/size.Width)*2.54;
				}
			}
		}

		// Remet le zoom pleine page et le centre par défaut.
		public void ZoomPageAndCenter()
		{
			this.ZoomAndCenter(this.ZoomPage, this.document.Size.Width/2, this.document.Size.Height/2);
		}

		// Remet le zoom largeur page et le centre par défaut.
		public void ZoomPageWidthAndCenter()
		{
			this.ZoomAndCenter(this.ZoomPageWidth, this.document.Size.Width/2, this.document.Size.Height/2);
		}

		// Remet le zoom 100% et le centre par défaut.
		public void ZoomDefaultAndCenter()
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
			bool changed = false;
			if ( this.zoom != zoom )
			{
				this.zoom = zoom;
				changed = true;
			}

			Size container = this.ContainerSize;

			double originX = (container.Width/this.ScaleX)/2-centerX;
			originX = -System.Math.Max(-originX, this.MinOriginX);
			originX = -System.Math.Min(-originX, this.MaxOriginX);

			double originY = (container.Height/this.ScaleY)/2-centerY;
			originY = -System.Math.Max(-originY, this.MinOriginY);
			originY = -System.Math.Min(-originY, this.MaxOriginY);

			if ( this.originX != originX ||
				 this.originY != originY )
			{
				this.originX = originX;
				this.originY = originY;
				changed = true;
			}

			if ( changed && this.document.Notifier != null )
			{
				this.document.Notifier.NotifyArea(this.viewer);
				this.document.Notifier.NotifyZoomChanged();
				this.document.Notifier.NotifyOriginChanged();
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
			if ( this.document.Type == DocumentType.Pictogram )
			{
				Size size = this.ContainerSize;
				double sx = zoom*size.Width/this.document.Size.Width;
				double sy = zoom*size.Height/this.document.Size.Height;
				double scale = System.Math.Min(sx, sy);
				return new Point(scale, scale);
			}
			else
			{
				double dpi = this.document.GlobalSettings.ScreenDpi;
				double scale = (dpi*zoom) / (25.4*10.0);
				return new Point(scale, scale);
			}
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

		// Mode "comme imprimé".
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

		// Force la longueur d'un vecteur sur la grille magnétique, si nécessaire.
		public void SnapGridVectorLength(ref Point vector)
		{
			double d = Point.Distance(new Point(0,0), vector);
			if ( d == 0 )  return;
			Point pd = new Point(d, 0);
			this.SnapGrid(ref pd);
			double sd = pd.X;
			if ( sd == d )  return;

			vector.X *= sd/d;
			vector.Y *= sd/d;
		}

		// Force un point sur la grille magnétique, si nécessaire.
		public void SnapGrid(ref Point pos)
		{
			this.SnapGrid(ref pos, this.SnapGridOffset, Rectangle.Empty);
		}

		// Force un point sur la grille magnétique, si nécessaire.
		public void SnapGrid(ref Point pos, Rectangle box)
		{
			this.SnapGrid(ref pos, this.SnapGridOffset, box);
		}

		// Force un point sur la grille magnétique, si nécessaire.
		public void SnapGrid(ref Point pos, Point offset, Rectangle box)
		{
			bool snapX, snapY;

			if ( !this.gridActive ^ this.isAlt )
			{
				this.SnapGuides(ref pos, box, out snapX, out snapY);
				return;
			}

			Point guidePos = pos;
			this.SnapGuides(ref guidePos, box, out snapX, out snapY);
			this.SnapGridForce(ref pos, offset);

			if ( snapX )  pos.X = guidePos.X;
			if ( snapY )  pos.Y = guidePos.Y;
		}

		// Force un point sur la grille magnétique, toujours.
		public void SnapGridForce(ref Point pos)
		{
			pos = Point.GridAlign(pos, this.SnapGridOffset, this.gridStep);
		}

		// Force un point sur la grille magnétique, toujours.
		public void SnapGridForce(ref Point pos, Point offset)
		{
			pos = Point.GridAlign(pos, offset, this.gridStep);
		}

		// Retourne l'offset standard pour la grille magnétique.
		protected Point SnapGridOffset
		{
			get
			{
				Point offset = new Point(0.0, 0.0);
				if ( this.document.Type == DocumentType.Pictogram )
				{
					offset = new Point(this.gridStep.X/2, this.gridStep.Y/2);
				}
				return offset-this.gridOffset;
			}
		}
		#endregion


		#region Ruler
		// Affichage des règles graduées.
		public bool RulersShow
		{
			get
			{
				return this.rulersShow;
			}

			set
			{
				if ( this.rulersShow != value )
				{
					this.rulersShow = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyGridChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}
		#endregion


		#region Labels
		// Affichage des noms de objets.
		public bool LabelsShow
		{
			get
			{
				return this.labelsShow;
			}

			set
			{
				if ( this.labelsShow != value )
				{
					this.labelsShow = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyGridChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}
		#endregion


		#region Aggregates
		// Affichage des noms de styles.
		public bool AggregatesShow
		{
			get
			{
				return this.aggregatesShow;
			}

			set
			{
				if ( this.aggregatesShow != value )
				{
					this.aggregatesShow = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyGridChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
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

		// Déplacement avec la souris des repères magnétiques.
		public bool GuidesMouse
		{
			get
			{
				return this.guidesMouse;
			}

			set
			{
				if ( this.guidesMouse != value )
				{
					this.guidesMouse = value;

					if ( this.document.Notifier != null )
					{
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		// Force un point sur un repère magnétique.
		protected void SnapGuides(ref Point pos, Rectangle box, out bool snapX, out bool snapY)
		{
			snapX = false;
			snapY = false;
			if ( !this.guidesActive ^ this.isAlt )  return;

			Objects.Page page = this.document.GetObjects[this.CurrentPage] as Objects.Page;

			if ( page.MasterGuides && this.MasterPageList.Count > 0 )
			{
				foreach ( Objects.Page masterPage in this.MasterPageList )
				{
					this.SnapGuides(masterPage.Guides, ref pos, box, ref snapX, ref snapY);
				}
			}

			this.SnapGuides(page.Guides, ref pos, box, ref snapX, ref snapY);
			this.SnapGuides(this.document.Settings.GuidesListGlobal, ref pos, box, ref snapX, ref snapY);
		}

		// Force un point sur un repère magnétique d'une liste.
		protected void SnapGuides(UndoableList guides, ref Point pos, Rectangle box, ref bool snapX, ref bool snapY)
		{
			if ( snapX && snapY )  return;

			int total = guides.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Guide guide = guides[i] as Settings.Guide;
				double apos = guide.AbsolutePosition;

				if ( guide.IsHorizontal )  // repère horizontal ?
				{
					if ( !snapY && !box.IsEmpty )
					{
						double len = System.Math.Abs(box.Bottom - apos);
						if ( len <= this.GuideMargin )
						{
							pos.Y += apos-box.Bottom;
							snapY = true;
						}
					}

					if ( !snapY && !box.IsEmpty )
					{
						double len = System.Math.Abs(box.Top - apos);
						if ( len <= this.GuideMargin )
						{
							pos.Y += apos-box.Top;
							snapY = true;
						}
					}

					if ( !snapY )
					{
						double len = System.Math.Abs(pos.Y - apos);
						if ( len <= this.GuideMargin )
						{
							pos.Y = apos;
							snapY = true;
						}
					}
				}
				else	// repère vertical ?
				{
					if ( !snapX && !box.IsEmpty )
					{
						double len = System.Math.Abs(box.Left - apos);
						if ( len <= this.GuideMargin )
						{
							pos.X += apos-box.Left;
							snapX = true;
						}
					}

					if ( !snapX && !box.IsEmpty )
					{
						double len = System.Math.Abs(box.Right - apos);
						if ( len <= this.GuideMargin )
						{
							pos.X += apos-box.Right;
							snapX = true;
						}
					}

					if ( !snapX )
					{
						double len = System.Math.Abs(pos.X - apos);
						if ( len <= this.GuideMargin )
						{
							pos.X = apos;
							snapX = true;
						}
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
				return ( this.viewer != null && this.viewer == this.document.Modifier.ActiveViewer );
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

		// Marge magnétique des constructions.
		public double MagnetMargin
		{
			get { return this.magnetMargin/this.ScaleX; }
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

		// Taille de la zone à redessiner d'une poignée.
		public double HandleRedrawSize
		{
			get { return (this.handleSize+1.0)/this.ScaleX; }
		}

		// Marge à ajouter à la bbox lors du dessin, pour résoudre le cas des poignées
		// qui débordent d'un objet avec un trait mince, et du mode Hilite qui augmente
		// l'épaisseur lors du survol de la souris.
		public double SelectMarginSize
		{
			get { return System.Math.Max(this.handleSize+4, this.hiliteSize)/this.ScaleX/2; }
		}


		// Estompe une couleur.
		public void DimmedColor(ref RichColor color)
		{
			if ( this.isDimmed )
			{
				double alpha = color.A;
				double intensity = color.Basic.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.05;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.1, 1.0);  // augmente l'intensité
				color = RichColor.FromBrightness(intensity);
				color.A = alpha*0.2;  // très transparent
			}
		}


		// Couleur lorsqu'un objet est survolé par la souris.
		public Color HiliteOutlineColor
		{
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
				Color color = Color.FromColor(adorner.ColorCaption, 0.8);
				if ( this.previewActive )
				{
					color = Color.FromBrightness(color.GetBrightness());
					color.A *= 0.5;
				}
				return color;
			}
		}

		// Couleur lorsqu'un objet est survolé par la souris.
		public Color HiliteSurfaceColor
		{
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
				Color color = Color.FromColor(adorner.ColorCaption, 0.4);
				if ( this.previewActive )
				{
					color = Color.FromBrightness(color.GetBrightness());
					color.A *= 0.5;
				}
				return color;
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
				this.isShift = value;
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
				if ( this.isCtrl != value )
				{
					this.isCtrl = value;
					this.ConstrainUpdateCtrl();
				}
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


		#region Magnet
		// Action des lignes magnétiques.
		public bool MagnetActive
		{
			get
			{
				return this.magnetActive;
			}

			set
			{
				if ( this.magnetActive != value )
				{
					this.magnetActive = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyMagnetChanged();
						this.document.Notifier.NotifySettingsChanged();
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		// Indique s'il existe des lignes magnétiques activées.
		public bool MagnetActiveAndExist
		{
			get
			{
				return (this.magnetActive && this.magnetLayerList.Count > 0);
			}
		}

		// Annule le point de départ.
		public void MagnetClearStarting()
		{
			this.isMagnetStarting = false;
		}

		// Fixe le point de départ.
		public void MagnetFixStarting(Point pos)
		{
			this.isMagnetStarting = true;
			this.magnetStarting = pos;
		}

		// Retourne une position éventuellement contrainte.
		public bool MagnetSnapPos(ref Point pos)
		{
			if ( !this.magnetActive )  return false;

			if ( this.isCtrl )
			{
				this.MagnetDelStarting();
				return false;
			}

			if ( this.isMagnetStarting && this.magnetLineMain.IsUsed )
			{
				Point proj = this.magnetLineMain.Projection(this.magnetStarting);
				if ( Point.Distance(proj, pos) <= this.MagnetMargin )
				{
					this.magnetLineProj.Initialise(proj, this.magnetStarting, false);
					pos = proj;
					return true;
				}
			}
			this.magnetLineProj.Clear();

			double margin = this.MagnetMargin;

			MagnetLine first  = null;
			MagnetLine second = null;

			if ( this.magnetLineMain.Detect(pos, margin) )
			{
				first = this.magnetLineMain;
			}
			else if ( this.magnetLineMain.Detect(pos, margin*3.0) )
			{
				Point proj = this.magnetLineMain.Projection(pos);
				if ( !Geometry.IsInside(this.magnetLineMain.P1, this.magnetLineMain.P2, proj) )
				{
					first = this.magnetLineMain;
				}
			}

			if ( first == null )
			{
				margin *= 3.0;
			}

			bool perp = false;

			MagnetLine piston = null;
			if ( this.magnetLineBegin.IsUsed  && this.magnetLineBegin.Infinite  )  piston = this.magnetLineBegin;
			if ( this.magnetLineEnd.IsUsed    && this.magnetLineEnd.Infinite    )  piston = this.magnetLineEnd;
			if ( this.magnetLineMiddle.IsUsed && this.magnetLineMiddle.Infinite )  piston = this.magnetLineMiddle;
			if ( this.magnetLinePerp.IsUsed   && this.magnetLinePerp.Infinite   )  piston = this.magnetLinePerp;
			if ( piston != null )
			{
				if ( piston.Detect(pos, margin) )
				{
					perp = true;
					this.magnetLineBegin.Infinite  = (this.magnetLineBegin  == piston);
					this.magnetLineEnd.Infinite    = (this.magnetLineEnd    == piston);
					this.magnetLineMiddle.Infinite = (this.magnetLineMiddle == piston);
					this.magnetLinePerp.Infinite   = (this.magnetLinePerp   == piston);
					if ( first == null )  first  = piston;
					else                  second = piston;
				}
			}

			if ( !perp )
			{
				this.magnetLineBegin.Infinite = false;
				if ( !perp && this.magnetLineBegin.Detect(pos, margin) )
				{
					perp = true;
					this.magnetLineBegin.Infinite = true;
					if ( first == null )  first  = this.magnetLineBegin;
					else                  second = this.magnetLineBegin;
				}

				this.magnetLineEnd.Infinite = false;
				if ( !perp && this.magnetLineEnd.Detect(pos, margin) )
				{
					perp = true;
					this.magnetLineEnd.Infinite = true;
					if ( first == null )  first  = this.magnetLineEnd;
					else                  second = this.magnetLineEnd;
				}

				this.magnetLineMiddle.Infinite = false;
				if ( !perp && this.magnetLineMiddle.Detect(pos, margin) )
				{
					perp = true;
					this.magnetLineMiddle.Infinite = true;
					if ( first == null )  first  = this.magnetLineMiddle;
					else                  second = this.magnetLineMiddle;
				}

				this.magnetLinePerp.Infinite = false;
				if ( !perp && this.magnetLinePerp.Detect(pos, margin) )
				{
					perp = true;
					this.magnetLinePerp.Infinite = true;
					if ( first == null )  first  = this.magnetLinePerp;
					else                  second = this.magnetLinePerp;
				}
			}

			if ( perp )
			{
				this.magnetLineInter.Clear();
			}
			else
			{
				if ( first != null && first.IsMain )
				{
					if ( this.magnetLineInter.Detect(pos, margin) )
					{
						second = this.magnetLineInter;
					}
				}
			}

			if ( first == null )
			{
				Point p1, p2;
				if ( this.document.Modifier.MagnetLayerDetect(pos, new Point(0,0), new Point(0,0), out p1, out p2) )
				{
					this.magnetLineMain.Initialise(p1, p2, true);
					first = this.magnetLineMain;

					// Ajoute le segment au départ.
					Point pp1 = Point.Move(p1, p2, this.MagnetMargin);
					Point pb1 = Transform.RotatePointDeg(p1,  90, pp1);
					Point pb2 = Transform.RotatePointDeg(p1, -90, pp1);
					this.magnetLineBegin.Initialise(pb1, pb2, false);

					// Ajoute le segment à l'arrivée.
					Point pp2 = Point.Move(p2, p1, this.MagnetMargin);
					Point pe1 = Transform.RotatePointDeg(p2,  90, pp2);
					Point pe2 = Transform.RotatePointDeg(p2, -90, pp2);
					this.magnetLineEnd.Initialise(pe1, pe2, false);

					if ( this.isMagnetStarting &&
						 this.magnetLineMain.Detect(this.magnetStarting, 0.001) )
					{
						Point delta = Point.Move(p1, p2, this.MagnetMargin*2.0)-p1;
						Point pi1 = new Point(this.magnetStarting.X-delta.Y, this.magnetStarting.Y+delta.X);
						Point pi2 = new Point(this.magnetStarting.X+delta.Y, this.magnetStarting.Y-delta.X);
						this.magnetLinePerp.Initialise(pi1, pi2, false);

						this.magnetLineMiddle.Clear();
					}
					else
					{
						// Ajoute le segment au milieu.
						Point m = Point.Scale(p1, p2, 0.5);
						Point n = Point.Move(m, p2, this.MagnetMargin);
						Point pm1 = Transform.RotatePointDeg(m,  90, n);
						Point pm2 = Transform.RotatePointDeg(m, -90, n);
						this.magnetLineMiddle.Initialise(pm1, pm2, false);

						this.magnetLinePerp.Clear();
					}
				}
			}
			else
			{
				if ( first.IsMain && second == null )
				{
					Point p1, p2;
					if ( this.document.Modifier.MagnetLayerDetect(pos, this.magnetLineMain.P1, this.magnetLineMain.P2, out p1, out p2) )
					{
						Point[] inter = Geometry.Intersect(first.P1, first.P2, p1, p2);
						if ( inter != null && Geometry.IsInside(p1, p2, inter[0]) )
						{
							this.magnetLineInter.Initialise(p1, p2, false);
							second = this.magnetLineInter;
						}
					}
				}
			}

			if ( first == null )
			{
				this.magnetLineMain.Clear();
				this.magnetLineBegin.Clear();
				this.magnetLineEnd.Clear();
				this.magnetLineMiddle.Clear();
				this.magnetLinePerp.Clear();
				this.magnetLineInter.Clear();
			}
			else
			{
				if ( second == null )
				{
					this.magnetLineInter.Clear();
					pos = first.Projection(pos);
					return true;
				}
				else
				{
					Point[] inter = Geometry.Intersect(first.P1, first.P2, second.P1, second.P2);
					if ( inter != null )
					{
						pos = inter[0];
						return true;
					}
				}
			}

			return false;
		}

		// Enlève le point initial pour les lignes magnétiques.
		public void MagnetDelStarting()
		{
			this.magnetLineMain.Clear();
			this.magnetLineBegin.Clear();
			this.magnetLineEnd.Clear();
			this.magnetLineMiddle.Clear();
			this.magnetLinePerp.Clear();
			this.magnetLineInter.Clear();
			this.magnetLineProj.Clear();
		}

		// Dessine les lignes magnétiques.
		public void DrawMagnet(Graphics graphics, Size size)
		{
			if ( !this.magnetActive )  return;
			if ( this.isCtrl )  return;

			double max = System.Math.Max(size.Width, size.Height);
			this.magnetLineMain.Draw(graphics, max);
			this.magnetLineBegin.Draw(graphics, max);
			this.magnetLineEnd.Draw(graphics, max);
			this.magnetLineMiddle.Draw(graphics, max);
			this.magnetLinePerp.Draw(graphics, max);
			this.magnetLineInter.Draw(graphics, max);
			this.magnetLineProj.Draw(graphics, max);
		}
		#endregion


		#region Constrain
		// Efface toutes les contraintes.
		public void ConstrainFlush()
		{
			this.constrainList.Clear();
		}

		// Ajoute 4 contraintes pour former un rectangle HV.
		// p1 et p2 sont 2 coins opposés quelconques du rectangle.
		public void ConstrainAddRect(Point p1, Point p2)
		{
			Point p3 = new Point(p1.X, p2.Y);
			Point p4 = new Point(p2.X, p1.Y);
			this.ConstrainAddLine(p1, p3);
			this.ConstrainAddLine(p3, p2);
			this.ConstrainAddLine(p2, p4);
			this.ConstrainAddLine(p4, p1);
		}

		// Ajoute des contraintes pour déplacer le sommet d'un rectangle.
		public void ConstrainAddRect(Point corner, Point opp, Point left, Point right)
		{
			this.ConstrainAddLine(corner, opp);
			this.ConstrainAddLine(corner, left);
			this.ConstrainAddLine(corner, right);
		}

		// Ajoute un centre de rotation pour les contraintes, permettant des
		// rotations multiples de 45 degrés.
		public void ConstrainAddCenter(Point pos)
		{
			this.ConstrainAddHV(pos);
			this.ConstrainAddHomo(pos);
		}

		// Ajoute une croix de zoom à 45 degrés pour les contraintes.
		public void ConstrainAddHomo(Point pos)
		{
			this.ConstrainAddLine(pos, new Point(pos.X+1.0, pos.Y+1.0));
			this.ConstrainAddLine(pos, new Point(pos.X+1.0, pos.Y-1.0));
		}

		// Ajoute une contrainte horizontale et verticale (+).
		public void ConstrainAddHV(Point pos)
		{
			this.ConstrainAddHorizontal(pos.Y);
			this.ConstrainAddVertical(pos.X);
		}

		// Ajoute une contrainte horizontale (-).
		public void ConstrainAddHorizontal(double y)
		{
			this.ConstrainAddLine(new Point(0.0, y), new Point(1.0, y));
		}

		// Ajoute une contrainte verticale (|).
		public void ConstrainAddVertical(double x)
		{
			this.ConstrainAddLine(new Point(x, 0.0), new Point(x, 1.0));
		}

		// Ajoute une contrainte quelconque.
		public void ConstrainAddLine(Point p1, Point p2)
		{
			if ( p1 == p2 )  return;

			MagnetLine line = new MagnetLine(this.document, this, MagnetLine.Type.Constrain);
			line.Initialise(p1, p2, true, false);

			foreach ( MagnetLine exist in this.constrainList )
			{
				if ( line.Compare(exist) )  return;
			}

			line.IsVisible = this.isCtrl;
			this.constrainList.Add(line);
		}

		// Ajoute une contrainte de distance (circulaire).
		public void ConstrainAddCircle(Point center, Point ext)
		{
			if ( center == ext )  return;

			MagnetLine line = new MagnetLine(this.document, this, MagnetLine.Type.Circle);
			line.Initialise(center, ext, true, false);

			line.IsVisible = this.isCtrl;
			this.constrainList.Add(line);
		}

		// Retourne une position éventuellement contrainte, d'abord sur une
		// contrainte magnétique, sinon sur la grille.
		public void SnapPos(ref Point pos)
		{
			if ( this.ConstrainSnapPos(ref pos) )  return;
			this.SnapGrid(ref pos);
		}

		// Retourne une position éventuellement contrainte, en fonction du nombre
		// quelconque de contraintes existantes.
		public bool ConstrainSnapPos(ref Point pos)
		{
			if ( !this.isCtrl )
			{
				return this.MagnetSnapPos(ref pos);
			}

			// Met toutes les lignes proches dans une table avec les distances
			// respectives.
			double margin = this.MagnetMargin*2.0;
			int detect = 0;
			MagnetLine[] table = new MagnetLine[10];
			double[] dist = new double[10];
			foreach ( MagnetLine line in this.constrainList )
			{
				double d = line.Distance(pos);
				if ( d <= margin )
				{
					System.Diagnostics.Debug.Assert(detect<10, "Too many magnet constrain.");
					table[detect] = line;
					dist[detect++] = d;
				}
				line.Temp = false;
			}

			bool snap = false;
			if ( detect >= 2 )
			{
				// Trie les lignes détectées, afin d'avoir la plus proche en premier.
				// Bubble sort peu efficace, mais c'est sans grande importance vu
				// le petit nombre de lignes à trier (<10).
				bool more;
				do
				{
					more = false;
					for ( int i=0 ; i<detect-1 ; i++ )
					{
						if ( dist[i] > dist[i+1] )
						{
							double t = dist[i];
							dist[i] = dist[i+1];
							dist[i+1] = t;

							MagnetLine tl = table[i];
							table[i] = table[i+1];
							table[i+1] = tl;

							more = true;
						}
					}
				}
				while ( more );

				// Calcule l'intersection entre les 2 lignes les plus proches.
				Point[] inter = MagnetLine.Intersect(table[0], table[1]);
				if ( inter != null )
				{
					if ( inter.Length == 2 )
					{
						double d1 = Point.Distance(pos, inter[0]);
						double d2 = Point.Distance(pos, inter[1]);
						if ( d2 < d1 )
						{
							inter[0] = inter[1];
						}
					}

					if ( Point.Distance(pos, inter[0]) <= this.MagnetMargin )
					{
						pos = inter[0];
						table[0].Temp = true;
						table[1].Temp = true;
						snap = true;

						// S'il existe plus de 2 lignes faisant partie de
						// l'intersection, on les ajoute ici (pour faire joli).
						for ( int i=2 ; i<detect ; i++ )
						{
							if ( dist[i] <= margin && table[i].Distance(pos) < 0.0001 )
							{
								table[i].Temp = true;
							}
						}
					}
				}
			}

			// Si on n'a pas trouvé d'intersection, ajuste la position sur la
			// ligne la plus proche (projection).
			if ( !snap && detect >= 1 )
			{
				table[0].Snap(ref pos, margin);
				table[0].Temp = true;
				snap = true;
			}

			// Modifie la propriété FlyOver une seule fois, pour éviter de redessiner
			// inutilement des grandes zones.
			foreach ( MagnetLine line in this.constrainList )
			{
				line.FlyOver = line.Temp;
			}

			return snap;
		}

		// Modifie les contraintes suite à la pression de la touche espace.
		public void ConstrainSpacePressed()
		{
			int visible = 0;
			foreach ( MagnetLine line in this.constrainList )
			{
				if ( line.IsVisible )  visible ++;
			}

			if ( visible == this.constrainList.Count )  // toutes les lignes visibles ?
			{
				foreach ( MagnetLine line in this.constrainList )
				{
					line.IsVisible = line.FlyOver;  // ne garde que les lignes actives
				}
			}
			else	// pas toutes les lignes visibles ?
			{
				foreach ( MagnetLine line in this.constrainList )
				{
					line.IsVisible = true;  // remontre toutes les lignes
				}
			}
		}

		// Met à jour les contraintes en fonction de la touche Ctrl.
		protected void ConstrainUpdateCtrl()
		{
			foreach ( MagnetLine line in this.constrainList )
			{
				line.IsVisible = this.isCtrl;
			}
		}

		// Enlève le point initial pour les contraintes.
		public void ConstrainDelStarting()
		{
			foreach ( MagnetLine line in this.constrainList )
			{
				line.Clear();
			}
		}

		// Dessine les contraintes.
		public void DrawConstrain(Graphics graphics, Size size)
		{
			if ( !this.isCtrl )  return;

			double max = System.Math.Max(size.Width, size.Height);
			foreach ( MagnetLine line in this.constrainList )
			{
				line.Draw(graphics, max);
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
			get { return Color.FromARGB(0.3, 1.0, 0.75, 0.0); }  // orange
		}

		// Retourne la couleur pour indiquer un style.
		static public Color ColorStyleBack
		{
			get { return Color.FromARGB(0.15, 1.0, 0.75, 0.0); }  // orange
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
			get { return Color.FromARGB(1.0, 0.0, 0.7, 1.0); }  // bleu
		}

		// Retourne la couleur du curseur pendant l'édition.
		static public Color ColorCursorEdit
		{
			get { return Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur des sélections pendant l'édition.
		static public Color ColorSelectEdit(bool active)
		{
			if ( active )
			{
				return Color.FromARGB(1.0, 1.0, 1.0, 0.0);  // jaune
			}
			else
			{
				return Color.FromBrightness(0.85);  // gris
			}
		}
		#endregion


		#region RootStack
		// Donne toute la pile.
		public System.Collections.ArrayList RootStack
		{
			get { return this.rootStack; }
			set { this.rootStack = value; }
		}

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
			bool ie = this.document.Modifier.OpletQueueEnable;
			this.document.Modifier.OpletQueueEnable = false;
			this.RootStackClear();
			this.RootStackPush(page);
			this.RootStackPush(layer);
			this.UpdateAfterPageChanged();
			this.document.Modifier.OpletQueueEnable = ie;
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
						this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.PageChange);
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
						this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.LayerChange);
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

		// Donne la liste des pages maître à utiliser.
		public System.Collections.ArrayList MasterPageList
		{
			get { return this.masterPageList; }
		}

		// Donne la liste des calques magnétiques à utiliser.
		public System.Collections.ArrayList MagnetLayerList
		{
			get { return this.magnetLayerList; }
		}

		// Met à jour masterPageList et magnetLayerList après un changement de page.
		public void UpdateAfterPageChanged()
		{
			this.document.Modifier.ComputeMasterPageList(this.masterPageList, this.CurrentPage);
			this.document.Modifier.ComputeMagnetLayerList(this.magnetLayerList, this.CurrentPage);
		}

		// Met à jour magnetLayerList après un changement de page.
		public void UpdateAfterLayerChanged()
		{
			this.document.Modifier.ComputeMagnetLayerList(this.magnetLayerList, this.CurrentPage);
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
		protected Drawer						drawer;
		protected Size							containerSize;
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
		protected bool							guidesMouse = true;
		protected bool							magnetActive = true;
		protected bool							rulersShow = true;
		protected bool							labelsShow = false;
		protected bool							aggregatesShow = false;
		protected bool							hideHalfActive = true;
		protected bool							isDimmed = false;
		protected bool							isDrawBoxThin = false;
		protected bool							isDrawBoxGeom = false;
		protected bool							isDrawBoxFull = false;
		protected double						minimalSize = 3;
		protected double						minimalWidth = 5;
		protected double						closeMargin = 10;
		protected double						guideMargin = 8;
		protected double						magnetMargin = 6;
		protected double						hiliteSize = 4;
		protected double						handleSize = 8;
		protected bool							isShift = false;
		protected bool							isCtrl = false;
		protected bool							isAlt = false;
		protected System.Collections.ArrayList	constrainList = new System.Collections.ArrayList();
		protected bool							isMagnetStarting = false;
		protected Point							magnetStarting;
		protected MagnetLine					magnetLineMain;
		protected MagnetLine					magnetLineBegin;
		protected MagnetLine					magnetLineEnd;
		protected MagnetLine					magnetLineMiddle;
		protected MagnetLine					magnetLinePerp;
		protected MagnetLine					magnetLineInter;
		protected MagnetLine					magnetLineProj;
		protected System.Collections.ArrayList	rootStack;
		protected System.Collections.ArrayList	masterPageList;
		protected System.Collections.ArrayList	magnetLayerList;
	}
}
