using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Document
{
	public enum LayerDrawingMode
	{
		ShowInactive,	// affiche normalement tous les calques
		DimmedInactive,	// affiche en estomp� les calques inactifs
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
			this.rootStack = new List<int> ();
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


		protected Size PageSize
		{
			//	Taille de la page courante du document.
			//	Il ne faut pas utiliser Document.PageSize dans DrawingContext, car les ic�nes
			//	n'ont pas de Modifier !
			get
			{
				Size size = this.document.DocumentSize;

				if ( !this.RootStackIsEmpty )
				{
					int pageNumber = this.CurrentPage;
					Objects.Page page = this.document.GetObjects[pageNumber] as Objects.Page;

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

				if ( area.Width < container.Width )  // fen�tre trop grande ?
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

				if ( area.Height < container.Height )  // fen�tre trop grande ?
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

				if ( area.Width < container.Width )  // fen�tre trop grande ?
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

				if ( area.Height < container.Height )  // fen�tre trop grande ?
				{
					return this.MinOriginY;
				}

				return (area.Height+this.MinOriginY) - container.Height;
			}
		}


		public void Origin(Point origin)
		{
			//	Sp�cifie l'origine de la zone visible.
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
			//	V�rifie si on utilise le zoom 100% centr�.
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
			//	V�rifie si on utilise le zoom pleine page centr�.
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
			//	V�rifie si on utilise le zoom pleine page centr�.
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
					return System.Math.Min(zx, zy)*2.54*(96.0/dpi);
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
					return zx*2.54*(96.0/dpi);
				}
			}
		}

		public void ZoomPageAndCenter()
		{
			//	Remet le zoom pleine page et le centre par d�faut.
			this.ZoomAndCenter(this.ZoomPage, this.PageSize.Width/2, this.PageSize.Height/2);
		}

		public void ZoomPageWidthAndCenter()
		{
			//	Remet le zoom largeur page et le centre par d�faut.
			this.ZoomAndCenter(this.ZoomPageWidth, this.PageSize.Width/2, this.PageSize.Height/2);
		}

		public void ZoomDefaultAndCenter()
		{
			//	Remet le zoom 100% et le centre par d�faut.
			this.ZoomAndCenter(1.0, this.PageSize.Width/2, this.PageSize.Height/2);
		}

		public void ZoomAndCenter(double zoom, Point center)
		{
			//	Sp�cifie le zoom et le centre de la zone visible.
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
		#endregion

		public Size ContainerSize
		{
			//	Taille du conteneur, qui peut �tre un viewer ou une taille fixe.
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
			//	Echelles � utiliser pour le dessin pour un zoom donn�.
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
				double scale = (dpi*zoom) / (25.4*10.0);
				return new Point(scale, scale);
			}
		}

		public Point Scale
		{
			//	Echelles � utiliser pour le dessin.
			get
			{
				return this.ScaleForZoom(this.zoom);
			}
		}

		public double ScaleX
		{
			//	Echelle horizontale � utiliser pour le dessin.
			get { return this.Scale.X; }
		}

		public double ScaleY
		{
			//	Echelle verticale � utiliser pour le dessin.
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
			//	Mode "comme imprim�".
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
		public bool GridActive
		{
			//	Action de la grille magn�tique.
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

		public bool GridShow
		{
			//	Affichage de la grille magn�tique.
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

		public Point GridStep
		{
			//	Pas de la grille magn�tique.
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

		public Point GridSubdiv
		{
			//	Subdivisions de la grille magn�tique.
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

		public Point GridOffset
		{
			//	D�calage de la grille magn�tique.
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

		public void SnapGridVectorLength(ref Point vector)
		{
			//	Force la longueur d'un vecteur sur la grille magn�tique, si n�cessaire.
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
			//	Force un point sur la grille magn�tique, si n�cessaire.
			this.SnapGrid(ref pos, this.SnapGridOffset, Rectangle.Empty);
		}

		public void SnapGrid(ref Point pos, Rectangle box)
		{
			//	Force un point sur la grille magn�tique, si n�cessaire.
			this.SnapGrid(ref pos, this.SnapGridOffset, box);
		}

		public void SnapGrid(ref Point pos, Point offset, Rectangle box)
		{
			//	Force un point sur la grille magn�tique, si n�cessaire.
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
			//	Force un point sur la grille magn�tique, toujours.
			pos = Point.GridAlign(pos, this.SnapGridOffset, this.gridStep);
		}

		public void SnapGridForce(ref Point pos, Point offset)
		{
			//	Force un point sur la grille magn�tique, toujours.
			pos = Point.GridAlign(pos, offset, this.gridStep);
		}

		protected Point SnapGridOffset
		{
			//	Retourne l'offset standard pour la grille magn�tique.
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
			//	Affichage de la grille magn�tique pour le texte.
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
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		public double TextGridStep
		{
			//	Pas de la grille magn�tique pour le texte.
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
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		public double TextGridSubdiv
		{
			//	Subdivisions de la grille magn�tique pour le texte.
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
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		public double TextGridOffset
		{
			//	D�calage de la grille magn�tique pour le texte.
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
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		protected void UpdateAllTextForTextGrid()
		{
			//	Met � jour tous les pav�s du document lorsque les lignes magn�tiques ont chang�.
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
			//	Affichage des caract�res de contr�le pour le texte.
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
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		public bool TextFontFilter
		{
			//	Affichage r�duit des caract�res (seulement les caract�res rapides).
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
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		public bool TextFontSampleAbc
		{
			//	Echantillons "Abc" � la place de "AaBbYyZz".
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
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		public double TextFontSampleHeight
		{
			//	Taille des �chantillons de caract�res pour FontSelector.
			get
			{
				return this.textFontSampleHeight;
			}

			set
			{
				if ( this.textFontSampleHeight != value )
				{
					this.textFontSampleHeight = value;

					if ( this.document.Notifier != null )
					{
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}
		#endregion


		#region Ruler
		public bool RulersShow
		{
			//	Affichage des r�gles gradu�es.
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
						this.document.IsDirtySerialize = true;
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
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}
		#endregion


		#region Guides
		public bool GuidesActive
		{
			//	Action des rep�res magn�tiques.
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

		public bool GuidesShow
		{
			//	Affichage des rep�res magn�tiques.
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

		public bool GuidesMouse
		{
			//	D�placement avec la souris des rep�res magn�tiques.
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

		protected void SnapGuides(ref Point pos, Rectangle box, out bool snapX, out bool snapY)
		{
			//	Force un point sur un rep�re magn�tique.
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

		protected void SnapGuides(UndoableList guides, ref Point pos, Rectangle box, ref bool snapX, ref bool snapY)
		{
			//	Force un point sur un rep�re magn�tique d'une liste.
			if ( snapX && snapY )  return;

			int total = guides.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Guide guide = guides[i] as Settings.Guide;
				double apos = guide.AbsolutePosition;

				if ( guide.IsHorizontal )  // rep�re horizontal ?
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
				else	// rep�re vertical ?
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
			//	Mode cach� � moiti� (estomper).
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


		public bool IsActive
		{
			//	Indique si le viewer associ� � ce contexte est actif.
			get
			{
				return ( this.viewer != null && this.viewer == this.document.Modifier.ActiveViewer );
			}
		}

		public bool IsDimmed
		{
			//	Indique si l'ic�ne est estomp�e.
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
			//	Taille minimale que doit avoir un objet � sa cr�ation.
			get { return this.minimalSize/this.ScaleX; }
		}

		public double MinimalWidth
		{
			//	Epaisseur minimale d'un objet pour la d�tection du coutour.
			get { return this.minimalWidth/this.ScaleX; }
		}

		public double CloseMargin
		{
			//	Marge pour fermer un polygone.
			get { return this.closeMargin/this.ScaleX; }
		}

		public double GuideMargin
		{
			//	Marge magn�tique d'un rep�re.
			get { return this.guideMargin/this.ScaleX; }
		}

		public double MagnetMargin
		{
			//	Marge magn�tique des constructions.
			get { return this.magnetMargin/this.ScaleX; }
		}

		public double HiliteSize
		{
			//	Taille suppl�mentaire lorsqu'un objet est survol� par la souris.
			get { return this.hiliteSize/this.ScaleX; }
			set { this.hiliteSize = value*this.ScaleX; }
		}

		public double HandleSize
		{
			//	Taille d'une poign�e.
			get { return this.handleSize/this.ScaleX; }
		}

		public double HandleRedrawSize
		{
			//	Taille de la zone � redessiner d'une poign�e.
			get { return (this.handleSize+1.0)/this.ScaleX; }
		}

		public double SelectMarginSize
		{
			//	Marge � ajouter � la bbox lors du dessin, pour r�soudre le cas des poign�es
			//	qui d�bordent d'un objet avec un trait mince, et du mode Hilite qui augmente
			//	l'�paisseur lors du survol de la souris.
			get { return System.Math.Max(this.handleSize+4, this.hiliteSize)/this.ScaleX/2; }
		}


		public void DimmedColor(ref RichColor color)
		{
			//	Estompe une couleur.
			if ( this.isDimmed )
			{
				double alpha = color.A;
				double intensity = color.Basic.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.05;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.1, 1.0);  // augmente l'intensit�
				color = RichColor.FromBrightness(intensity);
				color.A = alpha*0.2;  // tr�s transparent
			}
		}


		public Color HiliteOutlineColor
		{
			//	Couleur lorsqu'un objet est survol� par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
				Color color = Color.FromColor(adorner.ColorCaption, 0.8);
				if ( this.previewActive )
				{
					color = Color.FromBrightness(color.GetBrightness());
					color.A *= 0.5;
				}
				return color;
			}
		}

		public Color HiliteSurfaceColor
		{
			//	Couleur lorsqu'un objet est survol� par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
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
		public bool IsShift
		{
			//	Indique si la touche Shift est press�e.
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
			//	Indique si la touche Ctrl est press�e.
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
			//	Indique si la touche Alt est press�e.
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
			//	Action des lignes magn�tiques.
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

		public bool MagnetActiveAndExist
		{
			//	Indique s'il existe des lignes magn�tiques activ�es.
			get
			{
				return (this.magnetActive && this.magnetLayerList.Count > 0);
			}
		}

		public void MagnetClearStarting()
		{
			//	Annule le point de d�part.
			this.isMagnetStarting = false;
		}

		public void MagnetFixStarting(Point pos)
		{
			//	Fixe le point de d�part.
			this.isMagnetStarting = true;
			this.magnetStarting = pos;
		}

		public bool MagnetSnapPos(ref Point pos)
		{
			//	Retourne une position �ventuellement contrainte.
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

					//	Ajoute le segment au d�part.
					Point pp1 = Point.Move(p1, p2, this.MagnetMargin);
					Point pb1 = Transform.RotatePointDeg(p1,  90, pp1);
					Point pb2 = Transform.RotatePointDeg(p1, -90, pp1);
					this.magnetLineBegin.Initialize(pb1, pb2, false);

					//	Ajoute le segment � l'arriv�e.
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
			//	Enl�ve le point initial pour les lignes magn�tiques.
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
			//	Dessine les lignes magn�tiques.
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
						this.document.IsDirtySerialize = true;
					}
				}
			}
		}

		public void ConstrainClear()
		{
			//	Efface toutes les contraintes.
			this.constrainList.Clear();
		}

		public void ConstrainAddRect(Point p1, Point p2)
		{
			//	Ajoute 4 contraintes pour former un rectangle HV.
			//	p1 et p2 sont 2 coins oppos�s quelconques du rectangle.
			Point p3 = new Point(p1.X, p2.Y);
			Point p4 = new Point(p2.X, p1.Y);
			this.ConstrainAddLine(p1, p3);
			this.ConstrainAddLine(p3, p2);
			this.ConstrainAddLine(p2, p4);
			this.ConstrainAddLine(p4, p1);
		}

		public void ConstrainAddRect(Point corner, Point opp, Point left, Point right)
		{
			//	Ajoute des contraintes pour d�placer le sommet d'un rectangle.
			this.ConstrainAddLine(corner, opp);
			this.ConstrainAddLine(corner, left);
			this.ConstrainAddLine(corner, right);
		}

		public void ConstrainAddCenter(Point pos)
		{
			//	Ajoute un centre de rotation pour les contraintes, permettant des
			//	rotations multiples de 45 degr�s.
			this.ConstrainAddHV(pos);
			this.ConstrainAddHomo(pos);
		}

		public void ConstrainAddHomo(Point pos)
		{
			//	Ajoute une croix de zoom � 45 degr�s pour les contraintes.
			this.ConstrainAddLine(pos, new Point(pos.X+1.0, pos.Y+1.0));
			this.ConstrainAddLine(pos, new Point(pos.X+1.0, pos.Y-1.0));
		}

		public void ConstrainAddHV(Point pos)
		{
			//	Ajoute une contrainte horizontale et verticale (+).
			this.ConstrainAddHorizontal(pos.Y);
			this.ConstrainAddVertical(pos.X);
		}

		public void ConstrainAddHorizontal(double y)
		{
			//	Ajoute une contrainte horizontale (-).
			this.ConstrainAddLine(new Point(0.0, y), new Point(1.0, y));
		}

		public void ConstrainAddVertical(double x)
		{
			//	Ajoute une contrainte verticale (|).
			this.ConstrainAddLine(new Point(x, 0.0), new Point(x, 1.0));
		}

		public void ConstrainAddLine(Point p1, Point p2)
		{
			//	Ajoute une contrainte quelconque.
			if ( p1 == p2 )  return;

			MagnetLine line = new MagnetLine(this.document, this, MagnetLine.Type.Constrain);
			line.Initialize(p1, p2, true, false);

			foreach ( MagnetLine exist in this.constrainList )
			{
				if ( line.Compare(exist) )  return;
			}

			line.IsVisible = this.constrainActive;
			this.constrainList.Add(line);
		}

		public void ConstrainAddCircle(Point center, Point ext)
		{
			//	Ajoute une contrainte de distance (circulaire).
			if ( center == ext )  return;

			MagnetLine line = new MagnetLine(this.document, this, MagnetLine.Type.Circle);
			line.Initialize(center, ext, true, false);

			line.IsVisible = this.constrainActive;
			this.constrainList.Add(line);
		}

		public void SnapPos(ref Point pos)
		{
			//	Retourne une position �ventuellement contrainte, d'abord sur une
			//	contrainte magn�tique, sinon sur la grille.
			if ( this.ConstrainSnapPos(ref pos) )  return;
			this.SnapGrid(ref pos);
		}

		public bool ConstrainSnapPos(ref Point pos)
		{
			//	Retourne une position �ventuellement contrainte, en fonction du nombre
			//	quelconque de contraintes existantes.
			if (this.ConstrainVisibleCount == 0)
			{
				return this.MagnetSnapPos(ref pos);
			}

			//	Met toutes les lignes proches dans une table avec les distances
			//	respectives.
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
				//	Trie les lignes d�tect�es, afin d'avoir la plus proche en premier.
				//	Bubble sort peu efficace, mais c'est sans grande importance vu
				//	le petit nombre de lignes � trier (<10).
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

			//	Si on n'a pas trouv� d'intersection, ajuste la position sur la
			//	ligne la plus proche (projection).
			if ( !snap && detect >= 1 )
			{
				table[0].Snap(ref pos, margin);
				table[0].Temp = true;
				snap = true;
			}

			//	Modifie la propri�t� FlyOver une seule fois, pour �viter de redessiner
			//	inutilement des grandes zones.
			foreach ( MagnetLine line in this.constrainList )
			{
				line.FlyOver = line.Temp;
			}

			return snap;
		}

		public bool ConstrainSpacePressed()
		{
			//	Modifie les contraintes suite � la pression de la touche Space.
			//	Retourne true s'il existe au moins une contrainte.
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
			//	Enl�ve le point initial pour les contraintes.
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
			//	Retourne la couleur pour indiquer une s�lection multiple.
			get { return Color.FromAlphaRgb(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Color ColorMultiBack
		{
			//	Retourne la couleur pour indiquer une s�lection multiple.
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
			//	Retourne la couleur du pourtour d'une poign�e.
			get { return Color.FromAlphaRgb(1.0, 0.0, 0.0, 0.0); }  // noir
		}

		static public Color ColorHandleMain
		{
			//	Retourne la couleur d'une poign�e principale.
			get { return Color.FromAlphaRgb(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Color ColorSelectedSegment
		{
			//	Retourne la couleur d'un segment s�lectionn�.
			get { return Color.FromAlphaRgb(0.5, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Color ColorHandleStart
		{
			//	Retourne la couleur d'une poign�e de d�but/fin.
			get { return Color.FromAlphaRgb(1.0, 0.0, 1.0, 0.0); }  // vert
		}

		static public Color ColorHandleProperty
		{
			//	Retourne la couleur d'une poign�e de propri�t�.
			get { return Color.FromAlphaRgb(1.0, 0.0, 1.0, 1.0); }  // cyan
		}

		static public Color ColorHandleGlobal
		{
			//	Retourne la couleur d'une poign�e de s�lection globale.
			get { return Color.FromAlphaRgb(1.0, 1.0, 1.0, 1.0); }  // blanc
		}

		static public Color ColorConstrain
		{
			//	Retourne la couleur pour dessiner une contrainte.
			get { return Color.FromAlphaRgb(0.5, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Color ColorFrameEdit
		{
			//	Retourne la couleur du cadre pendant l'�dition.
			get { return Color.FromAlphaRgb(1.0, 0.0, 0.7, 1.0); }  // bleu
		}

		static public Color ColorCursorEdit(bool active)
		{
			//	Retourne la couleur du curseur pendant l'�dition.
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
			//	Retourne la couleur des s�lections pendant l'�dition.
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
			//	Retourne la couleur du fond d'un tabulateur supprim�.
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
			//	Document doit pouvoir s�rialiser la pile comme un ArrayList.
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			foreach (int item in this.rootStack)
			{
				list.Add (item);
			}
			return list;
		}

		internal void SetRootStack(System.Collections.ArrayList list)
		{
			//	Document a s�rialis� la pile comme un ArrayList.
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
			//	Indique si on est � la racine (donc dans page.calque).
			get { return (this.rootStack.Count == 2); }
		}

		public int RootStackDeep
		{
			//	Retourne la profondeur.
			get { return this.rootStack.Count; }
		}

		public void RootStackPush(int index)
		{
			//	Ajoute un nouvel �l�ment.
			this.InsertOpletRootStack();
			this.rootStack.Add(index);
		}

		public int RootStackPop()
		{
			//	Retire le dernier �l�ment.
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
			//	Retourne l'objet racine � une profondeur donn�e.
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


		public void PageLayer(int page, int layer)
		{
			//	Sp�cifie une page et un calque.
			this.InsertOpletRootStack();
			this.InternalPageLayer(page, layer);
		}

		public void InternalPageLayer(int page, int layer)
		{
			//	Sp�cifie une page et un calque.
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
			return this.document.GetObjects.Count;
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
							this.ZoomAndCenter(this.zoom, newSize.Width/2, newSize.Height/2);
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
			//	Donne la liste des pages ma�tre � utiliser.
			get { return this.masterPageList; }
		}

		public IList<Objects.Layer> MagnetLayerList
		{
			//	Donne la liste des calques magn�tiques � utiliser.
			get { return this.magnetLayerList; }
		}

		public void UpdateAfterPageChanged()
		{
			//	Met � jour masterPageList et magnetLayerList apr�s un changement de page.
			this.masterPageList = this.document.Modifier.ComputeMasterPageList (this.CurrentPage);
			this.magnetLayerList = this.document.Modifier.ComputeMagnetLayerList (this.CurrentPage);
		}

		public void UpdateAfterLayerChanged()
		{
			//	Met � jour magnetLayerList apr�s un changement de page.
			this.magnetLayerList = this.document.Modifier.ComputeMagnetLayerList (this.CurrentPage);
		}
		#endregion

		#region OpletRootStack
		protected void InsertOpletRootStack()
		{
			//	Ajoute un oplet pour m�moriser les informations de s�lection de l'objet.
			if ( this.document.Modifier == null )  return;
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletRootStack oplet = new OpletRootStack(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	M�morise tout le RootStack.
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
		protected double						originX;
		protected double						originY;
		protected LayerDrawingMode				layerDrawingMode = LayerDrawingMode.DimmedInactive;
		protected bool							previewActive;
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
		protected bool							isDimmed;
		protected bool							isBitmap;
		protected bool							isDrawBoxThin;
		protected bool							isDrawBoxGeom;
		protected bool							isDrawBoxFull;
		protected double						minimalSize = 3;
		protected double						minimalWidth = 5;
		protected double						closeMargin = 10;
		protected double						guideMargin = 8;
		protected double						magnetMargin = 6;
		protected double						hiliteSize = 4;
		protected double						handleSize = 8;
		protected bool							isShift;
		protected bool							isCtrl;
		protected bool							isAlt;
		protected bool							constrainActive;
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
