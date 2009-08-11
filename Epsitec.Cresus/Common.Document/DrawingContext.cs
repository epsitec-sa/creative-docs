using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Document
{
	public enum LayerDrawingMode
	{
		ShowInactive,	// affiche normalement tous les calques
		DimmedInactive,	// affiche en estompé les calques inactifs
		HideInactive,	// cache les calques inactifs
	}

	public enum ConstrainAngle
	{
		None,			// aucune
		Quarter,		// 45°
		Sixth,			// 30°, 60°
		Eight,			// 22.5°, 45°, 67.5°
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
			this.rootStack = new List<int> ();
			this.constrainAngle = ConstrainAngle.None;
			this.constrainList = new List<MagnetLine> ();
			this.masterPageList = new List<Objects.Page> ();
			this.magnetLayerList = new List<Objects.Layer> ();

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.gridStep   = new Point(1.0, 1.0);
				this.gridSubdiv = new Point(1.0, 1.0);
				this.gridOffset = new Point(0.0, 0.0);

				this.textGridStep   = 1.0;
				this.textGridSubdiv = 1.0;
				this.textGridOffset = 0.0;
			}
			else
			{
				if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
				{
					this.gridStep   = new Point(50.0, 50.0);  // 5mm
					this.gridSubdiv = new Point(5.0, 5.0);
					this.gridOffset = new Point(0.0, 0.0);

					this.textGridStep   = 100.0;  // 10mm
					this.textGridSubdiv = 1.0;
					this.textGridOffset = 0.0;
				}
				else
				{
					this.gridStep   = new Point(50.8, 50.8);  // 0.2in
					this.gridSubdiv = new Point(5.0, 5.0);
					this.gridOffset = new Point(0.0, 0.0);

					this.textGridStep   = 127.0;  // 0.5in
					this.textGridSubdiv = 1.0;
					this.textGridOffset = 0.0;
				}
			}

			this.magnetLineMain   = new MagnetLine(this.document, this, MagnetLine.Type.Main);
			this.magnetLineBegin  = new MagnetLine(this.document, this, MagnetLine.Type.Perp);
			this.magnetLineEnd    = new MagnetLine(this.document, this, MagnetLine.Type.Perp);
			this.magnetLineMiddle = new MagnetLine(this.document, this, MagnetLine.Type.Perp);
			this.magnetLinePerp   = new MagnetLine(this.document, this, MagnetLine.Type.Perp);
			this.magnetLineInter  = new MagnetLine(this.document, this, MagnetLine.Type.Inter);
			this.magnetLineProj   = new MagnetLine(this.document, this, MagnetLine.Type.Proj);

			this.imageNameFilters = new string[2];
			this.imageNameFilters[0] = "Blackman";
			this.imageNameFilters[1] = "Bicubic";
		}

		public Viewer Viewer
		{
			get { return this.viewer; }
		}

		public Drawer Drawer
		{
			get { return this.drawer; }
		}


		public Size PageSize
		{
			//	Taille de la page courante du document.
			//	Il ne faut pas utiliser Document.PageSize dans DrawingContext, car les icônes
			//	n'ont pas de Modifier !
			get
			{
				Size size = this.document.DocumentSize;

				if ( !this.RootStackIsEmpty )
				{
					int pageNumber = this.CurrentPage;
					Objects.Page page = this.document.DocumentObjects[pageNumber] as Objects.Page;

					if ( page != null )
					{
						if ( page.PageSize.Width  != 0 )  size.Width  = page.PageSize.Width;
						if ( page.PageSize.Height != 0 )  size.Height = page.PageSize.Height;
					}
				}

				return size;
			}
		}


		#region Zoom
		public double MinOriginX
		{
			//	Retourne l'origine horizontale minimale.
			get
			{
				Size size = this.PageSize;
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

		public double MinOriginY
		{
			//	Retourne l'origine verticale minimale.
			get
			{
				Size size = this.PageSize;
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

		public double MaxOriginX
		{
			//	Retourne l'origine horizontale maximale.
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

		public double MaxOriginY
		{
			//	Retourne l'origine verticale maximale.
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


		public void SetOrigin(Point origin)
		{
			//	Spécifie l'origine de la zone visible.
			this.SetOrigin(origin.X, origin.Y);
		}

		public void SetOrigin(double originX, double originY)
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

		public Point Center
		{
			//	Retourne le centre de la zone visible.
			get
			{
				Point center = new Point();
				Size cs = this.ContainerSize;
				center.X = -this.OriginX+(cs.Width/this.ScaleX)/2;
				center.Y = -this.OriginY+(cs.Height/this.ScaleY)/2;
				return center;
			}
		}

		public bool IsZoomDefault
		{
			//	Vérifie si on utilise le zoom 100% centré.
			get
			{
#if false
				if ( this.zoom != 1.0 )  return false;

				Size cs = this.ContainerSize;
				Size size = this.document.Size;
				Point scale = this.ScaleForZoom(this.zoom);
				double originX = size.Width/2 - (cs.Width/scale.X)/2;
				double originY = size.Height/2 - (cs.Height/scale.Y)/2;

				return ( System.Math.Abs(this.originX+originX) < 0.00001 &&
						 System.Math.Abs(this.originY+originY) < 0.00001 );
#else
				return ( this.zoom == 1.0 );
#endif
			}
		}

		public bool IsZoomPage
		{
			//	Vérifie si on utilise le zoom pleine page centré.
			get
			{
				if ( System.Math.Abs(this.zoom-this.ZoomPage) > 0.00001 )  return false;

				Size cs = this.ContainerSize;
				Size size = this.PageSize;
				Point scale = this.ScaleForZoom(this.zoom);
				double originX = size.Width/2 - (cs.Width/scale.X)/2;
				double originY = size.Height/2 - (cs.Height/scale.Y)/2;

				return ( System.Math.Abs(this.originX+originX) < 0.00001 &&
						 System.Math.Abs(this.originY+originY) < 0.00001 );
			}
		}

		public bool IsZoomPageWidth
		{
			//	Vérifie si on utilise le zoom pleine page centré.
			get
			{
				if ( System.Math.Abs(this.zoom-this.ZoomPageWidth) > 0.00001 )  return false;

				Size cs = this.ContainerSize;
				Size size = this.PageSize;
				Point scale = this.ScaleForZoom(this.zoom);
				double originX = size.Width/2 - (cs.Width/scale.X)/2;

				return ( System.Math.Abs(this.originX+originX) < 0.00001 );
			}
		}

		public double ZoomPage
		{
			//	Retourne le zoom pleine page.
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
					Size size = this.PageSize;
					double zx = cs.Width/size.Width;
					double zy = cs.Height/size.Height;
					double dpi = this.document.GlobalSettings.ScreenDpi;
					return System.Math.Min(zx, zy)*2.54*100/dpi*this.fitScale;
				}
			}
		}

		public double ZoomPageWidth
		{
			//	Retourne le zoom largeur page.
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
					Size size = this.PageSize;
					double zx = cs.Width/size.Width;
					double dpi = this.document.GlobalSettings.ScreenDpi;
					return zx*2.54*100/dpi*this.fitScale;
				}
			}
		}

		public void ZoomPageAndCenter()
		{
			//	Remet le zoom pleine page et le centre par défaut.
			this.ZoomAndCenter(this.ZoomPage, this.PageSize.Width/2, this.PageSize.Height/2, false);
		}

		public void ZoomPageWidthAndCenter()
		{
			//	Remet le zoom largeur page et le centre par défaut.
			this.ZoomAndCenter(this.ZoomPageWidth, this.PageSize.Width/2, this.PageSize.Height/2, false);
		}

		public void ZoomDefaultAndCenter()
		{
			//	Remet le zoom 100% et le centre par défaut.
			this.ZoomAndCenter(1.0, this.PageSize.Width/2, this.PageSize.Height/2, false);
		}

		public void ZoomAndCenter(double zoom, Point center, bool motionless)
		{
			//	Spécifie le zoom et le centre de la zone visible.
			this.ZoomAndCenter(zoom, center.X, center.Y, motionless);
		}

		protected void ZoomAndCenter(double zoom, double centerX, double centerY, bool motionless)
		{
			//	Avec le mode motionless = true, on cherche à conserver la position visée dans l'écran.
			//	Avec le mode motionless = false, on cherche à centrer la position visée.
			Point iPos = this.viewer.InternalToScreen(new Point(centerX, centerY));

			bool changed = false;
			if ( this.zoom != zoom )
			{
				this.zoom = zoom;
				changed = true;
			}

			double originX, originY;

			if (motionless)
			{
				Point iOffset = this.viewer.InternalToScreen(new Point(centerX, centerY))-iPos;
				iOffset.X /= this.ScaleX;
				iOffset.Y /= this.ScaleY;

				originX = this.originX-iOffset.X;
				originY = this.originY-iOffset.Y;
			}
			else
			{
				Size container = this.ContainerSize;

				originX = (container.Width/this.ScaleX)/2-centerX;
				originX = -System.Math.Max(-originX, this.MinOriginX);
				originX = -System.Math.Min(-originX, this.MaxOriginX);

				originY = (container.Height/this.ScaleY)/2-centerY;
				originY = -System.Math.Max(-originY, this.MinOriginY);
				originY = -System.Math.Min(-originY, this.MaxOriginY);
			}

			if ( this.originX != originX ||
				 this.originY != originY )
			{
				this.originX = originX;
				this.originY = originY;
				changed = true;
			}

			if (changed && this.document.Notifier != null)
			{
				this.document.Notifier.NotifyArea(this.viewer);
				this.document.Notifier.NotifyZoomChanged();
				this.document.Notifier.NotifyOriginChanged();
			}
			//?System.Diagnostics.Debug.WriteLine(string.Format("z={0} cx={1} cy={2} ox={3} oy={4}", zoom, centerX, centerY, this.originX, this.originY));
		}

		public double Zoom
		{
			//	Zoom courant.
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

		public double OriginX
		{
			//	Origine horizontale de la zone visible.
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

		public double OriginY
		{
			//	Origine verticale de la zone visible.
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

		public double FitScale
		{
			//	Ajustement pour laisser de l'espace en cas de zoom pleine page ou
			//	pleine largeur; 1.0 = pas d'espace autour de la page.
			get
			{
				return this.fitScale;
			}
			set
			{
				this.fitScale = value;
			}
		}
		#endregion

		public Size ContainerSize
		{
			//	Taille du conteneur, qui peut être un viewer ou une taille fixe.
			//	Si le viewer existe, il est inutile d'appeler ContainerSize.set
			//	Si le viewer n'existe pas, il faut appeler ContainerSize.set
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

		protected Point ScaleForZoom(double zoom)
		{
			//	Echelles à utiliser pour le dessin pour un zoom donné.
			if ( this.document.Type == DocumentType.Pictogram )
			{
				Size size = this.ContainerSize;
				double sx = zoom*size.Width/this.PageSize.Width;
				double sy = zoom*size.Height/this.PageSize.Height;
				double scale = System.Math.Min(sx, sy);
				return new Point(scale, scale);
			}
			else
			{
				double dpi = this.document.GlobalSettings.ScreenDpi;
				double scale = (dpi*zoom) / (2.54*100);
				return new Point(scale, scale);
			}
		}

		public Point Scale
		{
			//	Echelles à utiliser pour le dessin.
			get
			{
				return this.ScaleForZoom(this.zoom);
			}
		}

		public double ScaleX
		{
			//	Echelle horizontale à utiliser pour le dessin.
			get { return this.Scale.X; }
		}

		public double ScaleY
		{
			//	Echelle verticale à utiliser pour le dessin.
			get { return this.Scale.Y; }
		}


		public LayerDrawingMode LayerDrawingMode
		{
			//	Mode de dessin pour les calques.
			get { return this.layerDrawingMode; }
			set { this.layerDrawingMode = value; }
		}

		public bool PreviewActive
		{
			//	Mode "comme imprimé".
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

		public System.Predicate<DrawImageFilterInfo> DrawImageFilter
		{
			get;
			set;
		}

		public struct DrawImageFilterInfo
		{
			public DrawImageFilterInfo(Objects.Abstract obj, string arg)
			{
				this.obj = obj;
				this.arg = arg;
			}

			public Objects.Abstract Object
			{
				get
				{
					return this.obj;
				}
			}

			public string Argument
			{
				get
				{
					return this.arg;
				}
			}

			readonly Objects.Abstract obj;
			readonly string arg;
		}

		public bool FillEmptyPlaceholders
		{
			get
			{
				return this.fillEmptyPlaceholders;
			}
			set
			{
				if (this.fillEmptyPlaceholders != value)
				{
					this.fillEmptyPlaceholders = value;
					
					if (this.document.Notifier != null)
					{
						this.document.Notifier.NotifyArea (this.viewer);
						this.document.Notifier.NotifyPreviewChanged ();
					}
				}
			}
		}


		#region Grid
		public bool GridActive
		{
			//	Action de la grille magnétique.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public bool GridShow
		{
			//	Affichage de la grille magnétique.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public Point GridStep
		{
			//	Pas de la grille magnétique.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public Point GridSubdiv
		{
			//	Subdivisions de la grille magnétique.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public Point GridOffset
		{
			//	Décalage de la grille magnétique.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public void SnapGridVectorLength(ref Point vector)
		{
			//	Force la longueur d'un vecteur sur la grille magnétique, si nécessaire.
			double d = Point.Distance(new Point(0,0), vector);
			if ( d == 0 )  return;
			Point pd = new Point(d, 0);
			this.SnapGrid(ref pd);
			double sd = pd.X;
			if ( sd == d )  return;

			vector.X *= sd/d;
			vector.Y *= sd/d;
		}

		public void SnapGrid(ref Point pos)
		{
			//	Force un point sur la grille magnétique, si nécessaire.
			this.SnapGrid(ref pos, this.SnapGridOffset, Rectangle.Empty);
		}

		public void SnapGrid(ref Point pos, Rectangle box)
		{
			//	Force un point sur la grille magnétique, si nécessaire.
			this.SnapGrid(ref pos, this.SnapGridOffset, box);
		}

		public void SnapGrid(ref Point pos, Point offset, Rectangle box)
		{
			//	Force un point sur la grille magnétique, si nécessaire.
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

		public void SnapGridForce(ref Point pos)
		{
			//	Force un point sur la grille magnétique, toujours.
			pos = Point.GridAlign(pos, this.SnapGridOffset, this.gridStep);
		}

		public void SnapGridForce(ref Point pos, Point offset)
		{
			//	Force un point sur la grille magnétique, toujours.
			pos = Point.GridAlign(pos, offset, this.gridStep);
		}

		protected Point SnapGridOffset
		{
			//	Retourne l'offset standard pour la grille magnétique.
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


		#region TextGrid
		public bool TextGridShow
		{
			//	Affichage de la grille magnétique pour le texte.
			get
			{
				return this.textGridShow;
			}

			set
			{
				if ( this.textGridShow != value )
				{
					this.textGridShow = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public double TextGridStep
		{
			//	Pas de la grille magnétique pour le texte.
			get
			{
				return this.textGridStep;
			}

			set
			{
				if ( this.textGridStep != value )
				{
					this.textGridStep = value;
					this.UpdateAllTextForTextGrid();

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea();
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public double TextGridSubdiv
		{
			//	Subdivisions de la grille magnétique pour le texte.
			get
			{
				return this.textGridSubdiv;
			}

			set
			{
				if ( this.textGridSubdiv != value )
				{
					this.textGridSubdiv = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea(this.viewer);
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public double TextGridOffset
		{
			//	Décalage de la grille magnétique pour le texte.
			get
			{
				return this.textGridOffset;
			}

			set
			{
				if ( this.textGridOffset != value )
				{
					this.textGridOffset = value;
					this.UpdateAllTextForTextGrid();

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea();
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		protected void UpdateAllTextForTextGrid()
		{
			//	Met à jour tous les pavés du document lorsque les lignes magnétiques ont changé.
			foreach ( TextFlow flow in this.document.TextFlows )
			{
				foreach ( Objects.AbstractText obj in flow.Chain )
				{
					Text.ITextFrame frame = obj.TextFrame as Text.ITextFrame;
					if ( frame != null )
					{
						obj.UpdateTextGrid(true);
					}
				}
			}
		}

		public bool TextShowControlCharacters
		{
			//	Affichage des caractères de contrôle pour le texte.
			get
			{
				return this.textShowControlCharacters;
			}

			set
			{
				if ( this.textShowControlCharacters != value )
				{
					this.textShowControlCharacters = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyArea();
						this.document.Notifier.NotifyGridChanged();
						this.document.Notifier.NotifySettingsChanged();

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public bool TextFontFilter
		{
			//	Affichage réduit des caractères (seulement les caractères rapides).
			get
			{
				return this.textFontFilter;
			}

			set
			{
				if ( this.textFontFilter != value )
				{
					this.textFontFilter = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyFontsSettingsChanged();

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public bool TextFontSampleAbc
		{
			//	Echantillons "Abc" à la place de "AaBbYyZz".
			get
			{
				return this.textFontSampleAbc;
			}

			set
			{
				if ( this.textFontSampleAbc != value )
				{
					this.textFontSampleAbc = value;

					if ( this.document.Notifier != null )
					{
						this.document.Notifier.NotifyFontsSettingsChanged();

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public double TextFontSampleHeight
		{
			//	Taille des échantillons de caractères pour FontSelector.
			get
			{
				return this.textFontSampleHeight;
			}

			set
			{
				if ( this.textFontSampleHeight != value )
				{
					this.textFontSampleHeight = value;

					if (this.document.Notifier != null && this.viewer != null && !this.viewer.IsMiniature)
					{
						this.document.SetDirtySerialize(CacheBitmapChanging.None);
					}
				}
			}
		}
		#endregion


		#region Ruler
		public bool RulersShow
		{
			//	Affichage des règles graduées.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}
		#endregion


		#region Labels
		public bool LabelsShow
		{
			//	Affichage des noms de objets.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}
		#endregion


		#region Aggregates
		public bool AggregatesShow
		{
			//	Affichage des noms de styles.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}
		#endregion


		#region Guides
		public bool GuidesActive
		{
			//	Action des repères magnétiques.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public bool GuidesShow
		{
			//	Affichage des repères magnétiques.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public bool GuidesMouse
		{
			//	Déplacement avec la souris des repères magnétiques.
			get
			{
				return this.guidesMouse;
			}

			set
			{
				if ( this.guidesMouse != value )
				{
					this.guidesMouse = value;

					if (this.document.Notifier != null && this.viewer != null && !this.viewer.IsMiniature)
					{
						this.document.SetDirtySerialize(CacheBitmapChanging.None);
					}
				}
			}
		}

		protected void SnapGuides(ref Point pos, Rectangle box, out bool snapX, out bool snapY)
		{
			//	Force un point sur un repère magnétique.
			snapX = false;
			snapY = false;
			if ( !this.guidesActive ^ this.isAlt )  return;

			Objects.Page page = this.document.DocumentObjects[this.CurrentPage] as Objects.Page;

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

		protected void SnapGuides(UndoableList guides, ref Point pos, Rectangle box, ref bool snapX, ref bool snapY)
		{
			//	Force un point sur un repère magnétique d'une liste.
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


		public bool HideHalfActive
		{
			//	Mode caché à moitié (estomper).
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

		public bool VisibleHandles
		{
			//	Montre les poignées quand un objet est sélectionné
			get
			{
				return this.visibleHandles;
			}
			set
			{
				this.visibleHandles = value;
			}
		}

		public bool IsActive
		{
			//	Indique si le viewer associé à ce contexte est actif.
			get
			{
				return ( this.viewer != null && this.viewer == this.document.Modifier.ActiveViewer );
			}
		}

		public bool IsDimmed
		{
			//	Indique si l'icône est estompée.
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

		public bool IsBitmap
		{
			//	Indique si on dessine dans un bitmap.
			get
			{
				return this.isBitmap;
			}

			set
			{
				this.isBitmap = value;
			}
		}

		#region DrawBox
		public bool IsDrawBoxThin
		{
			//	Indique s'il faut afficher les bbox.
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

		public bool IsDrawBoxGeom
		{
			//	Indique s'il faut afficher les bbox.
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

		public bool IsDrawBoxFull
		{
			//	Indique s'il faut afficher les bbox.
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

		public double MinimalSize
		{
			//	Taille minimale que doit avoir un objet à sa création.
			get
			{
				return DrawingContext.minimalSize/this.ScaleX;
			}
		}

		public double MinimalWidth
		{
			//	Epaisseur minimale d'un objet pour la détection du coutour.
			get
			{
				return DrawingContext.minimalWidth/this.ScaleX;
			}
		}

		public double CloseMargin
		{
			//	Marge pour fermer un polygone.
			get
			{
				return DrawingContext.closeMargin/this.ScaleX;
			}
		}

		public double GuideMargin
		{
			//	Marge magnétique d'un repère.
			get
			{
				return DrawingContext.guideMargin/this.ScaleX;
			}
		}

		public double MagnetMargin
		{
			//	Marge magnétique des constructions.
			get
			{
				return DrawingContext.magnetMargin/this.ScaleX;
			}
		}

		public double HiliteSize
		{
			//	Taille supplémentaire lorsqu'un objet est survolé par la souris.
			get
			{
				return this.hiliteSize/this.ScaleX;
			}
			set
			{
				this.hiliteSize = value*this.ScaleX;
			}
		}

		public double HandleSize
		{
			//	Taille d'une poignée.
			get
			{
				return DrawingContext.handleSize/this.ScaleX;
			}
		}

		public double HandleRedrawSize
		{
			//	Taille de la zone à redessiner d'une poignée.
			get
			{
				return (DrawingContext.handleSize+1.0)/this.ScaleX;
			}
		}

		public double SelectMarginSize
		{
			//	Marge à ajouter à la bbox lors du dessin, pour résoudre le cas des poignées
			//	qui débordent d'un objet avec un trait mince, et du mode Hilite qui augmente
			//	l'épaisseur lors du survol de la souris.
			get
			{
				return System.Math.Max (DrawingContext.handleSize+4, this.hiliteSize)/this.ScaleX/2;
			}
		}


		public RichColor DimmedColor(RichColor color)
		{
			//	Estompe une couleur.
			if (this.isDimmed)
			{
				double alpha = color.A;
				double intensity = color.Basic.GetBrightness ();
				intensity = 0.5+(intensity-0.5)*0.05;  // diminue le contraste
				intensity = System.Math.Min (intensity+0.1, 1.0);  // augmente l'intensité
				color = RichColor.FromAGray (alpha*0.2, intensity);  // très transparent
			}
			return color;
		}


		public Color HiliteOutlineColor
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				Color color = Color.FromColor(adorner.ColorCaption, 0.8);
				if ( this.previewActive )
				{
					color = Color.FromBrightness(color.GetBrightness());
					color = Color.FromAlphaColor(color.A*0.5, color);
				}
				return color;
			}
		}

		public Color HiliteSurfaceColor
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				Color color = Color.FromColor(adorner.ColorCaption, 0.4);
				if ( this.previewActive )
				{
					color = Color.FromBrightness(color.GetBrightness());
					color = Color.FromAlphaColor(color.A*0.5, color);
				}
				return color;
			}
		}


		#region SuperShift
		public bool IsShift
		{
			//	Indique si la touche Shift est pressée.
			get
			{
				return this.isShift;
			}
			
			set
			{
				this.isShift = value;
			}
		}

		public bool IsCtrl
		{
			//	Indique si la touche Ctrl est pressée.
			get
			{
				return this.isCtrl;
			}
			
			set
			{
				this.isCtrl = value;
			}
		}

		public bool IsAlt
		{
			//	Indique si la touche Alt est pressée.
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
		public bool MagnetActive
		{
			//	Action des lignes magnétiques.
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

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public bool MagnetActiveAndExist
		{
			//	Indique s'il existe des lignes magnétiques activées.
			get
			{
				return (this.magnetActive && this.magnetLayerList.Count > 0);
			}
		}

		public void MagnetClearStarting()
		{
			//	Annule le point de départ.
			this.isMagnetStarting = false;
		}

		public void MagnetFixStarting(Point pos)
		{
			//	Fixe le point de départ.
			this.isMagnetStarting = true;
			this.magnetStarting = pos;
		}

		public bool MagnetSnapPos(ref Point pos)
		{
			//	Retourne une position éventuellement contrainte.
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
					this.magnetLineProj.Initialize(proj, this.magnetStarting, false);
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
					this.magnetLineMain.Initialize(p1, p2, true);
					first = this.magnetLineMain;

					//	Ajoute le segment au départ.
					Point pp1 = Point.Move(p1, p2, this.MagnetMargin);
					Point pb1 = Transform.RotatePointDeg(p1,  90, pp1);
					Point pb2 = Transform.RotatePointDeg(p1, -90, pp1);
					this.magnetLineBegin.Initialize(pb1, pb2, false);

					//	Ajoute le segment à l'arrivée.
					Point pp2 = Point.Move(p2, p1, this.MagnetMargin);
					Point pe1 = Transform.RotatePointDeg(p2,  90, pp2);
					Point pe2 = Transform.RotatePointDeg(p2, -90, pp2);
					this.magnetLineEnd.Initialize(pe1, pe2, false);

					if ( this.isMagnetStarting &&
						 this.magnetLineMain.Detect(this.magnetStarting, 0.001) )
					{
						Point delta = Point.Move(p1, p2, this.MagnetMargin*2.0)-p1;
						Point pi1 = new Point(this.magnetStarting.X-delta.Y, this.magnetStarting.Y+delta.X);
						Point pi2 = new Point(this.magnetStarting.X+delta.Y, this.magnetStarting.Y-delta.X);
						this.magnetLinePerp.Initialize(pi1, pi2, false);

						this.magnetLineMiddle.Clear();
					}
					else
					{
						//	Ajoute le segment au milieu.
						Point m = Point.Scale(p1, p2, 0.5);
						Point n = Point.Move(m, p2, this.MagnetMargin);
						Point pm1 = Transform.RotatePointDeg(m,  90, n);
						Point pm2 = Transform.RotatePointDeg(m, -90, n);
						this.magnetLineMiddle.Initialize(pm1, pm2, false);

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
							this.magnetLineInter.Initialize(p1, p2, false);
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

		public void MagnetDelStarting()
		{
			//	Enlève le point initial pour les lignes magnétiques.
			this.magnetLineMain.Clear();
			this.magnetLineBegin.Clear();
			this.magnetLineEnd.Clear();
			this.magnetLineMiddle.Clear();
			this.magnetLinePerp.Clear();
			this.magnetLineInter.Clear();
			this.magnetLineProj.Clear();
		}

		public void DrawMagnet(Graphics graphics, Size size)
		{
			//	Dessine les lignes magnétiques.
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
		public bool ConstrainActive
		{
			//	Action des lignes contraintes.
			get
			{
				return this.constrainActive;
			}

			set
			{
				if (this.constrainActive != value)
				{
					this.constrainActive = value;

					if (this.document.Notifier != null)
					{
						this.document.Notifier.NotifyConstrainChanged();
						this.document.Notifier.NotifySettingsChanged();

						if (this.viewer != null && !this.viewer.IsMiniature)
						{
							this.document.SetDirtySerialize(CacheBitmapChanging.None);
						}
					}
				}
			}
		}

		public ConstrainAngle ConstrainAngle
		{
			//	Angles supplémentaires pour les contraintes.
			get
			{
				return this.constrainAngle;
			}
			set
			{
				this.constrainAngle = value;
			}
		}

		public void ConstrainClear()
		{
			//	Efface toutes les contraintes.
			this.constrainList.Clear();
		}

		public void ConstrainAddRect(Point corner, Point opp, Point left, Point right, bool isVisible, int handleRank)
		{
			//	Ajoute des contraintes pour déplacer le sommet d'un rectangle.
			this.ConstrainAddLine(corner, opp, isVisible, handleRank);
			this.ConstrainAddLine(corner, left, isVisible, handleRank);
			this.ConstrainAddLine(corner, right, isVisible, handleRank);
		}

		public void ConstrainAddCenter(Point pos, bool isVisible, int handleRank)
		{
			//	Ajoute un centre de rotation pour les contraintes, permettant des
			//	rotations multiples de 45 degrés.
			this.ConstrainAddHV(pos, isVisible, handleRank);
			this.ConstrainAddHomo(pos, isVisible, handleRank);
		}

		public void ConstrainAddHomo(Point pos, bool isVisible, int handleRank)
		{
			//	Ajoute une croix de zoom à 45 degrés pour les contraintes.
			this.ConstrainAddLine(pos, new Point(pos.X+1.0, pos.Y+1.0), isVisible, handleRank);
			this.ConstrainAddLine(pos, new Point(pos.X+1.0, pos.Y-1.0), isVisible, handleRank);
		}

		public void ConstrainAddHV(Point pos, bool isVisible, int handleRank)
		{
			//	Ajoute une contrainte horizontale et verticale (+).
			this.ConstrainAddHorizontal(pos.Y, isVisible, handleRank);
			this.ConstrainAddVertical(pos.X, isVisible, handleRank);

			if (this.constrainAngle == ConstrainAngle.Quarter)
			{
				Point r = new Point(pos.X+1.0, pos.Y);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 45.0*1, r), isVisible, handleRank);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 45.0*3, r), isVisible, handleRank);
			}

			if (this.constrainAngle == ConstrainAngle.Sixth)
			{
				Point r = new Point(pos.X+1.0, pos.Y);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 30.0*1, r), isVisible, handleRank);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 30.0*2, r), isVisible, handleRank);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 30.0*4, r), isVisible, handleRank);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 30.0*5, r), isVisible, handleRank);
			}

			if (this.constrainAngle == ConstrainAngle.Eight)
			{
				Point r = new Point(pos.X+1.0, pos.Y);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 22.5*1, r), isVisible, handleRank);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 22.5*2, r), isVisible, handleRank);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 22.5*3, r), isVisible, handleRank);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 22.5*5, r), isVisible, handleRank);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 22.5*6, r), isVisible, handleRank);
				this.ConstrainAddLine(pos, Transform.RotatePointDeg(pos, 22.5*7, r), isVisible, handleRank);
			}
		}

		public void ConstrainAddHorizontal(double y, bool isVisible, int handleRank)
		{
			//	Ajoute une contrainte horizontale (-).
			this.ConstrainAddLine(new Point(0.0, y), new Point(1.0, y), isVisible, handleRank);
		}

		public void ConstrainAddVertical(double x, bool isVisible, int handleRank)
		{
			//	Ajoute une contrainte verticale (|), issue d'une poignée spécifique.
			this.ConstrainAddLine(new Point(x, 0.0), new Point(x, 1.0), isVisible, handleRank);
		}

		public void ConstrainAddLine(Point p1, Point p2, bool isVisible, int handleRank)
		{
			//	Ajoute une contrainte quelconque.
			if ( p1 == p2 )  return;

			MagnetLine line = new MagnetLine(this.document, this, MagnetLine.Type.Constrain);
			line.Initialize(p1, p2, true, isVisible, handleRank);

			foreach (MagnetLine exist in this.constrainList)
			{
				if (line.Compare(exist))  return;
			}

			line.IsVisible = this.constrainActive|isVisible;
			this.constrainList.Add(line);
		}

		public void ConstrainAddCircle(Point center, Point ext, bool isVisible, int handleRank)
		{
			//	Ajoute une contrainte de distance (circulaire).
			if ( center == ext )  return;

			MagnetLine line = new MagnetLine(this.document, this, MagnetLine.Type.Circle);
			line.Initialize(center, ext, true, isVisible, handleRank);

			line.IsVisible = this.constrainActive;
			this.constrainList.Add(line);
		}

		public void SnapPos(ref Point pos)
		{
			//	Retourne une position éventuellement contrainte, d'abord sur une
			//	contrainte magnétique, sinon sur la grille.
			if ( this.ConstrainSnapPos(ref pos) )  return;
			this.SnapGrid(ref pos);
		}

		public bool ConstrainSnapPos(ref Point pos)
		{
			//	Retourne une position éventuellement contrainte, en fonction du nombre
			//	quelconque de contraintes existantes.
			if (this.ConstrainVisibleCount == 0)
			{
				return this.MagnetSnapPos(ref pos);
			}

			//	Met toutes les lignes proches dans une table avec les distances
			//	respectives.
			double margin = this.MagnetMargin*2.0;
			int detect = 0;
			MagnetLine[] table = new MagnetLine[20];
			double[] dist = new double[20];
			foreach ( MagnetLine line in this.constrainList )
			{
				double d = line.Distance(pos);
				if ( d <= margin )
				{
					System.Diagnostics.Debug.Assert(detect<20, "Too many magnet constrain.");
					table[detect] = line;
					dist[detect++] = d;
				}
				line.Temp = false;
			}

			bool snap = false;
			if ( detect >= 2 )
			{
				//	Trie les lignes détectées, afin d'avoir la plus proche en premier.
				//	Bubble sort peu efficace, mais c'est sans grande importance vu
				//	le petit nombre de lignes à trier (<20).
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

				//	Calcule l'intersection entre les 2 lignes les plus proches.
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

						//	S'il existe plus de 2 lignes faisant partie de
						//	l'intersection, on les ajoute ici (pour faire joli).
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

			//	Si on n'a pas trouvé d'intersection, ajuste la position sur la
			//	ligne la plus proche (projection).
			if ( !snap && detect >= 1 )
			{
				table[0].Snap(ref pos, margin);
				table[0].Temp = true;
				snap = true;
			}

			//	Modifie la propriété FlyOver une seule fois, pour éviter de redessiner
			//	inutilement des grandes zones.
			foreach ( MagnetLine line in this.constrainList )
			{
				line.FlyOver = line.Temp;
			}

			return snap;
		}

		public bool ConstrainSpacePressed(Point pos, Objects.Abstract obj, int excludeHandle)
		{
			//	Modifie les contraintes suite à la pression de la barre d'espace.
			//	Retourne true s'il existe au moins une contrainte.
			if (obj != null)
			{
				int rank = obj.DetectHandle(pos, excludeHandle);
				if (rank != -1)
				{
					if (this.ConstrainIsHide(rank))
					{
						this.ConstrainShow(rank);
					}
					else if (!this.ConstrainDelete(rank))
					{
						pos = obj.Handle(rank).Position;
						this.ConstrainAddHV(pos, true, rank);
					}
					
					return (this.ConstrainVisibleCount != 0);
				}
			}

			int count = this.ConstrainVisibleCount;

			if (count == this.constrainList.Count)  // toutes les lignes visibles ?
			{
				foreach (MagnetLine line in this.constrainList)
				{
					line.IsVisible = line.FlyOver;  // ne garde que les lignes actives
				}
			}
			else if (count == 0)  // aucune ligne visible ?
			{
				foreach (MagnetLine line in this.constrainList)
				{
					line.IsVisible = true;  // remontre toutes les lignes
					line.FlyOver = false;
				}
			}
			else	// pas toutes les lignes visibles ?
			{
				foreach (MagnetLine line in this.constrainList)
				{
					line.IsVisible = false;  // cache toutes les lignes
				}
			}

			return (this.ConstrainVisibleCount != 0);
		}

		protected bool ConstrainIsHide(int handleRank)
		{
			foreach (MagnetLine line in this.constrainList)
			{
				if (line.HandleRank == handleRank && !line.IsVisible)
				{
					return true;
				}
			}
			return false;
		}

		protected void ConstrainShow(int handleRank)
		{
			foreach (MagnetLine line in this.constrainList)
			{
				if (line.HandleRank == handleRank)
				{
					line.IsVisible = true;
					line.FlyOver = false;
				}
			}
		}

		protected bool ConstrainDelete(int handleRank)
		{
			//	Supprime toutes les contraintes issues d'une poignée spécifique.
			//	Retourne true si au moins une contrainte visible a été supprimée.
			bool deleted = false;
			int i = 0;
			while (i < this.constrainList.Count)
			{
				MagnetLine line = this.constrainList[i];
				if (handleRank != -1 && line.HandleRank == handleRank)
				{
					if (line.IsVisible)
					{
						deleted = true;
					}

					line.Clear();  // pour forcer le redessin
					this.constrainList.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}

			return deleted;
		}

		protected int ConstrainVisibleCount
		{
			//	Retourne le nombre de contraintes visibles.
			get
			{
				int count = 0;
				foreach (MagnetLine line in this.constrainList)
				{
					if (line.IsVisible)
					{
						count++;
					}
				}
				return count;
			}
		}

		public void ConstrainDelStarting()
		{
			//	Enlève le point initial pour les contraintes.
			foreach ( MagnetLine line in this.constrainList )
			{
				line.Clear();
			}
		}

		public void DrawConstrain(Graphics graphics, Size size)
		{
			//	Dessine les contraintes.
			double max = System.Math.Max(size.Width, size.Height);
			foreach ( MagnetLine line in this.constrainList )
			{
				line.Draw(graphics, max);
			}
		}
		#endregion


		#region GetColors
		static public Color ColorMulti
		{
			//	Retourne la couleur pour indiquer une sélection multiple.
			get { return Color.FromAlphaRgb(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Color ColorMultiBack
		{
			//	Retourne la couleur pour indiquer une sélection multiple.
			get { return Color.FromAlphaRgb(0.15, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Color ColorStyle
		{
			//	Retourne la couleur pour indiquer un style.
			get { return Color.FromAlphaRgb(0.3, 1.0, 0.75, 0.0); }  // orange
		}

		static public Color ColorStyleBack
		{
			//	Retourne la couleur pour indiquer un style.
			get { return Color.FromAlphaRgb(0.15, 1.0, 0.75, 0.0); }  // orange
		}

		static public Color ColorHandleOutline
		{
			//	Retourne la couleur du pourtour d'une poignée.
			get { return Color.FromAlphaRgb(1.0, 0.0, 0.0, 0.0); }  // noir
		}

		static public Color ColorHandleMain
		{
			//	Retourne la couleur d'une poignée principale.
			get { return Color.FromAlphaRgb(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Color ColorSelectedSegment
		{
			//	Retourne la couleur d'un segment sélectionné.
			get { return Color.FromAlphaRgb(0.5, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Color ColorHandleStart
		{
			//	Retourne la couleur d'une poignée de début/fin.
			get { return Color.FromAlphaRgb(1.0, 0.0, 1.0, 0.0); }  // vert
		}

		static public Color ColorHandleProperty
		{
			//	Retourne la couleur d'une poignée de propriété.
			get { return Color.FromAlphaRgb(1.0, 0.0, 1.0, 1.0); }  // cyan
		}

		static public Color ColorHandleGlobal
		{
			//	Retourne la couleur d'une poignée de sélection globale.
			get { return Color.FromAlphaRgb(1.0, 1.0, 1.0, 1.0); }  // blanc
		}

		static public Color ColorConstrain
		{
			//	Retourne la couleur pour dessiner une contrainte.
			get { return Color.FromAlphaRgb(0.5, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Color ColorFrameEdit
		{
			//	Retourne la couleur du cadre pendant l'édition.
			get { return Color.FromAlphaRgb(1.0, 0.0, 0.7, 1.0); }  // bleu
		}

		static public Color ColorCursorEdit(bool active)
		{
			//	Retourne la couleur du curseur pendant l'édition.
			if ( active )
			{
				return Color.FromAlphaRgb(1.0, 0.0, 0.0, 1.0);  // bleu
			}
			else
			{
				return Color.FromBrightness(0.85);  // gris
			}
		}

		static public Color ColorSelectEdit(bool active)
		{
			//	Retourne la couleur des sélections pendant l'édition.
			if ( active )
			{
				return Color.FromAlphaRgb(1.0, 0.73, 0.81, 0.98);  // bleu
			}
			else
			{
				return Color.FromBrightness(0.85);  // gris
			}
		}

		static public Color ColorTabZombie
		{
			//	Retourne la couleur du fond d'un tabulateur supprimé.
			get { return Color.FromAlphaRgb(1.0, 1.0, 0.4, 0.0); }  // rouge-orange
		}
		#endregion


		#region ImageNameFilter
		public string GetImageNameFilter(int rank)
		{
			//	Donne le nom d'un filtre pour l'image.
			System.Diagnostics.Debug.Assert(rank >= 0 && rank < this.imageNameFilters.Length);
			return this.imageNameFilters[rank];
		}

		public void SetImageNameFilter(int rank, string name)
		{
			//	Modifie le nom d'un filtre pour l'image.
			System.Diagnostics.Debug.Assert(rank >= 0 && rank < this.imageNameFilters.Length);
			this.imageNameFilters[rank] = name;
		}
		#endregion


		#region RootStack
		internal System.Collections.ArrayList GetRootStack()
		{
			//	Document doit pouvoir sérialiser la pile comme un ArrayList.
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			foreach (int item in this.rootStack)
			{
				list.Add (item);
			}
			return list;
		}

		internal void SetRootStack(System.Collections.ArrayList list)
		{
			//	Document a sérialisé la pile comme un ArrayList.
			this.rootStack.Clear ();
			foreach (int item in list)
			{
				this.rootStack.Add (item);
			}
		}
		
		public List<int> RootStack
		{
			//	Donne toute la pile.
			get { return this.rootStack; }
		}

		public void RootStackClear()
		{
			//	Vide toute la pile.
			this.rootStack.Clear();
		}

		public bool RootStackIsEmpty
		{
			//	Indique si la pile est vide.
			get { return (this.rootStack.Count < 2); }  // au moins page.calque ?
		}

		public bool RootStackIsBase
		{
			//	Indique si on est à la racine (donc dans page.calque).
			get { return (this.rootStack.Count == 2); }
		}

		public int RootStackDeep
		{
			//	Retourne la profondeur.
			get { return this.rootStack.Count; }
		}

		public void RootStackPush(int index)
		{
			//	Ajoute un nouvel élément.
			this.InsertOpletRootStack();
			this.rootStack.Add(index);
		}

		public int RootStackPop()
		{
			//	Retire le dernier élément.
			if ( this.rootStack.Count == 0 )  return -1;
			this.InsertOpletRootStack();
			int index = (int) this.rootStack[this.rootStack.Count-1];
			this.rootStack.RemoveAt(this.rootStack.Count-1);
			return index;
		}

		public Objects.Abstract RootObject()
		{
			//	Retourne l'objet racine le plus profond.
			//	Il s'agira d'un calque ou d'un groupe.
			return this.RootObject(1000);
		}

		public Objects.Abstract RootObject(int deepMax)
		{
			//	Retourne l'objet racine à une profondeur donnée.
			UndoableList list = this.document.DocumentObjects;
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


		public void PageLayer(int page, int layer)
		{
			//	Spécifie une page et un calque.
			this.InsertOpletRootStack();
			this.InternalPageLayer(page, layer);
		}

		public void InternalPageLayer(int page, int layer)
		{
			//	Spécifie une page et un calque.
			bool ie = this.document.Modifier.OpletQueueEnable;
			this.document.Modifier.OpletQueueEnable = false;
			this.RootStackClear();
			this.RootStackPush(page);
			this.RootStackPush(layer);
			this.UpdateAfterPageChanged();
			this.document.Modifier.OpletQueueEnable = ie;
		}

		public int TotalPages()
		{
			//	Retourne le nombre total de pages.
			return this.document.DocumentObjects.Count;
		}

		public int TotalLayers()
		{
			//	Retourne le nombre total de calques.
			Objects.Abstract page = this.RootObject(1);
			return page.Objects.Count;
		}

		public int CurrentPage
		{
			//	Page courante.
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
						Size oldSize = this.PageSize;

						this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.PageChange);
						this.document.Modifier.InitiateChangingPage();
						this.document.Modifier.TerminateChangingPage(newPage);
						this.document.Modifier.OpletQueueValidateAction();

						Size newSize = this.PageSize;
						if ( oldSize != newSize )
						{
							this.ZoomAndCenter(this.zoom, newSize.Width/2, newSize.Height/2, false);
						}
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

		public int CurrentLayer
		{
			//	Calque courant.
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

		public IList<Objects.Page> MasterPageList
		{
			//	Donne la liste des pages maître à utiliser.
			get { return this.masterPageList; }
		}

		public IList<Objects.Layer> MagnetLayerList
		{
			//	Donne la liste des calques magnétiques à utiliser.
			get { return this.magnetLayerList; }
		}

		public void UpdateAfterPageChanged()
		{
			//	Met à jour masterPageList et magnetLayerList après un changement de page.
			this.masterPageList = this.document.Modifier.ComputeMasterPageList(this.CurrentPage);
			this.magnetLayerList = this.document.Modifier.ComputeMagnetLayerList(this.CurrentPage);
		}

		public void UpdateAfterLayerChanged()
		{
			//	Met à jour magnetLayerList après un changement de page.
			this.magnetLayerList = this.document.Modifier.ComputeMagnetLayerList(this.CurrentPage);
		}
		#endregion

		#region OpletRootStack
		protected void InsertOpletRootStack()
		{
			//	Ajoute un oplet pour mémoriser les informations de sélection de l'objet.
			if ( this.document.Modifier == null )  return;
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletRootStack oplet = new OpletRootStack(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise tout le RootStack.
		protected class OpletRootStack : AbstractOplet
		{
			public OpletRootStack(DrawingContext host)
			{
				this.host = host;

				this.rootStack = new List<int>();
				int total = this.host.rootStack.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					int index = (int) this.host.rootStack[i];
					this.rootStack.Add(index);
				}
			}

			protected void Swap()
			{
				List<int> temp = new List<int> ();

				//	this.host.rootStack -> temp
				int total = this.host.rootStack.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					int index = (int) this.host.rootStack[i];
					temp.Add(index);
				}

				//	this.rootStack -> this.host.rootStack
				this.host.rootStack.Clear();
				total = this.rootStack.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					int index = (int) this.rootStack[i];
					this.host.rootStack.Add(index);
				}

				//	temp -> this.rootStack
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
			protected List<int>						rootStack;
		}
		#endregion


		protected Document						document;
		protected Viewer						viewer;
		protected Drawer						drawer;
		protected Size							containerSize;
		protected double						zoom = 1;
		protected double						fitScale = 0.96;
		protected double						originX;
		protected double						originY;
		protected LayerDrawingMode				layerDrawingMode = LayerDrawingMode.DimmedInactive;
		protected bool							previewActive;
		protected bool							fillEmptyPlaceholders;
		protected bool							gridActive;
		protected bool							gridShow;
		protected Point							gridStep = new Point(1, 1);
		protected Point							gridSubdiv = new Point(1, 1);
		protected Point							gridOffset;
		protected bool							textGridShow;
		protected bool							textShowControlCharacters = true;
		protected bool							textFontFilter = true;
		protected bool							textFontSampleAbc;
		protected double						textFontSampleHeight = 30;
		protected double						textGridStep;
		protected double						textGridSubdiv;
		protected double						textGridOffset;
		protected bool							guidesActive = true;
		protected bool							guidesShow = true;
		protected bool							guidesMouse = true;
		protected bool							magnetActive = true;
		protected bool							rulersShow = true;
		protected bool							labelsShow;
		protected bool							aggregatesShow;
		protected bool							hideHalfActive = true;
		protected bool							visibleHandles = true;
		protected bool							isDimmed;
		protected bool							isBitmap;
		protected bool							isDrawBoxThin;
		protected bool							isDrawBoxGeom;
		protected bool							isDrawBoxFull;
		protected const double					minimalSize = 3;
		protected const double					minimalWidth = 5;
		protected const double					closeMargin = 10;
		protected const double					guideMargin = 8;
		protected const double					magnetMargin = 6;
		protected double						hiliteSize = 4;
		protected const double					handleSize = 8;
		protected bool							isShift;
		protected bool							isCtrl;
		protected bool							isAlt;
		protected bool							constrainActive;
		protected ConstrainAngle				constrainAngle;
		protected List<MagnetLine>				constrainList;
		protected bool							isMagnetStarting;
		protected Point							magnetStarting;
		protected MagnetLine					magnetLineMain;
		protected MagnetLine					magnetLineBegin;
		protected MagnetLine					magnetLineEnd;
		protected MagnetLine					magnetLineMiddle;
		protected MagnetLine					magnetLinePerp;
		protected MagnetLine					magnetLineInter;
		protected MagnetLine					magnetLineProj;
		protected List<int>						rootStack;
		protected List<Objects.Page>			masterPageList;
		protected List<Objects.Layer>			magnetLayerList;
		protected string[]						imageNameFilters;
	}
}
