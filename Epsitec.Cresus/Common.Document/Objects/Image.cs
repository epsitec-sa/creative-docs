using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Image est la classe de l'objet graphique "image bitmap".
	/// </summary>
	[System.Serializable()]
	public class Image : Objects.Abstract
	{
		public Image(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.Image )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Image(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectImage"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainFlush();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
						 if ( rank == 0 )  drawingContext.ConstrainAddRect(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position);
					else if ( rank == 1 )  drawingContext.ConstrainAddRect(this.Handle(1).Position, this.Handle(0).Position, this.Handle(3).Position, this.Handle(2).Position);
					else if ( rank == 2 )  drawingContext.ConstrainAddRect(this.Handle(2).Position, this.Handle(3).Position, this.Handle(0).Position, this.Handle(1).Position);
					else if ( rank == 3 )  drawingContext.ConstrainAddRect(this.Handle(3).Position, this.Handle(2).Position, this.Handle(1).Position, this.Handle(0).Position);
				}
				else
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					property.MoveHandleStarting(this, handle.PropertyRank, pos, drawingContext);
				}

				drawingContext.MagnetClearStarting();
			}
		}

		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une poignée.
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			     if ( rank == 0 )  this.MoveCorner(pos, 0, 2,3, 1);
			else if ( rank == 1 )  this.MoveCorner(pos, 1, 3,2, 0);
			else if ( rank == 2 )  this.MoveCorner(pos, 2, 0,1, 3);
			else if ( rank == 3 )  this.MoveCorner(pos, 3, 1,0, 2);
			else                   this.Handle(rank).Position = pos;

			this.SetDirtyBbox();
			this.TextInfoModifRect();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	Début de la création d'un objet.
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHomo(pos);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	Déplacement pendant la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.SetDirtyBbox();
			this.TextInfoModifRect();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);

			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			//	Crée les 2 autres poignées dans les coins opposés.
			Drawing.Rectangle rect = Drawing.Rectangle.FromPoints(this.Handle(0).Position, this.Handle(1).Position);
			Point p1 = rect.BottomLeft;
			Point p2 = rect.TopRight;
			this.Handle(0).Position = p1;
			this.Handle(1).Position = p2;
			this.HandleAdd(new Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3

			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

		public override bool SelectAfterCreation()
		{
			//	Indique s'il faut sélectionner l'objet après sa création.
			return true;
		}


		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			Path pathImage = this.PathBuildImage();
			Path pathSurface = this.PathBuildSurface();
			Path pathOutline = this.PathBuildOutline();

			int totalShapes = 4;

			Shape[] shapes = new Shape[totalShapes];
			int i = 0;
			
			//	Trait du rectangle.
			shapes[i] = new Shape();
			shapes[i].Path = pathOutline;
			shapes[i].Type = Type.Stroke;
			i ++;

			//	Image bitmap.
			shapes[i] = new Shape();
			shapes[i].SetImageObject(this);
			i ++;

			//	Rectangle complet pour bbox et détection.
			shapes[i] = new Shape();
			shapes[i].Path = pathSurface;
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.InvisibleBox;
			i ++;

			//	Rectangle complet pour bbox et détection.
			shapes[i] = new Shape();
			shapes[i].Path = pathImage;
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.InvisibleBox;
			i ++;

			return shapes;
		}

		protected Path PathBuildImage()
		{
			//	Crée le chemin de l'objet pour dessiner la surface exacte de l'image.
			Point center;
			double width, height, angle;
			this.ImageGeometry(out center, out width, out height, out angle);

			Point p = new Point();

			p.X = center.X-width/2;
			p.Y = center.Y-height/2;
			Point p1 = Transform.RotatePointDeg(center, angle, p);

			p.X = center.X+width/2;
			p.Y = center.Y-height/2;
			Point p2 = Transform.RotatePointDeg(center, angle, p);

			p.X = center.X+width/2;
			p.Y = center.Y+height/2;
			Point p3 = Transform.RotatePointDeg(center, angle, p);

			p.X = center.X-width/2;
			p.Y = center.Y+height/2;
			Point p4 = Transform.RotatePointDeg(center, angle, p);

			Path path = new Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		protected Path PathBuildSurface()
		{
			//	Crée le chemin de l'objet pour dessiner sa surface.
			Point pbl, pbr, ptl, ptr;
			this.Corners(out pbl, out pbr, out ptl, out ptr);

			Path path = new Path();
			path.MoveTo(pbl);
			path.LineTo(pbr);
			path.LineTo(ptr);
			path.LineTo(ptl);
			path.Close();
			return path;
		}

		protected Path PathBuildOutline()
		{
			//	Crée le chemin de l'objet pour dessiner son pourtour.
			Point pbl, pbr, ptl, ptr;
			this.Corners(out pbl, out pbr, out ptl, out ptr);

			Path path = new Path();
			path.MoveTo(pbl);
			path.LineTo(pbr);
			path.LineTo(ptr);
			path.LineTo(ptl);  // rectangle
			path.Close();

			path.MoveTo(pbl);
			path.LineTo(ptr);  // / 

			path.MoveTo(pbr);
			path.LineTo(ptl);  // \ 
			return path;
		}

		protected void Corners(out Point pbl, out Point pbr, out Point ptl, out Point ptr)
		{
			//	Donne les 4 coins du rectangle.
			pbl = this.Handle(0).Position;
			ptr = this.Handle(1).Position;

			if ( this.handles.Count < 4 )
			{
				ptl = new Point(pbl.X, ptr.Y);
				pbr = new Point(ptr.X, pbl.Y);
			}
			else
			{
				ptl = this.Handle(2).Position;
				pbr = this.Handle(3).Position;
			}
		}

		public Size ImageBitmapSize()
		{
			//	Donne les dimensions effectives utilisées par le bitmap rectangulaire de l'image.
			Point center;
			double width, height, angle;
			this.ImageGeometry(out center, out width, out height, out angle);

			ImageCache.Item item = this.Item;

			if ( width > 0 && height > 0 && item != null && item.Image != null)
			{
				Properties.Image property = this.PropertyImage;

				if ( property.Homo )  // conserve les proportions ?
				{
					double rapport = item.Image.Height/item.Image.Width;
					if ( rapport < height/width )  height = width*rapport;
					else                           width  = height/rapport;
				}
			}

			return new Size(width, height);
		}

		protected void ImageGeometry(out Point center, out double width, out double height, out double angle)
		{
			//	Calcule le centre, les dimensions et l'angle de l'image en fonction
			//	du quadrilatère de l'objet, qui n'est pas forcément rectangulaire.
			Point pbl, pbr, ptl, ptr;
			this.Corners(out pbl, out pbr, out ptl, out ptr);

			center = (pbl+pbr+ptl+ptr)/4;
			width  = System.Math.Min(Point.Distance(pbl,pbr), Point.Distance(ptl,ptr));
			height = System.Math.Min(Point.Distance(pbl,ptl), Point.Distance(pbr,ptr));
			angle  = Point.ComputeAngleDeg(pbl, pbr);
		}

		protected ImageCache.Item Item
		{
			//	Retourne l'item de l'image cachée, s'il existe.
			get
			{
				Properties.Image pi = this.PropertyImage;
				return this.document.ImageCache.Get(pi.Filename);
			}
		}

		public override void DrawImage(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine l'image.
			if ( this.TotalHandle < 2 )  return;

			Drawing.Image image = null;
			ImageCache.Item item = this.Item;

			if (item != null)
			{
				if (port is PDF.Port)  // exportation PDF ?
				{
					Properties.Image pi = this.PropertyImage;
					PDF.Port pdfPort = port as PDF.Port;
					Size size = this.ImageBitmapSize();
					bool filter = pi.Filter;
					pdfPort.FilterImage = filter;
					PDF.ImageSurface surface = pdfPort.SearchImageSurface(pi.Filename, size, filter);
					System.Diagnostics.Debug.Assert(surface != null);
					image = surface.DrawingImage;
				}
				else
				{
					image = drawingContext.IsDimmed ? item.ImageDimmed : item.Image;
				}
			}

			if ( image == null )
			{
				Path path = this.PathBuildOutline();
				port.Color = Color.FromBrightness(0.5);
				port.LineWidth = 1.0/drawingContext.ScaleX;
				port.PaintOutline(path);
			}
			else
			{
				Transform ot = port.Transform;

				Point center;
				double width, height, angle;
				this.ImageGeometry(out center, out width, out height, out angle);

				if ( width > 0 && height > 0 )
				{
					Properties.Image property = this.PropertyImage;

					if ( property.Homo )  // conserve les proportions ?
					{
						double rapport = image.Height/image.Width;
						if ( rapport < height/width )  height = width*rapport;
						else                           width  = height/rapport;
					}

#if false
					port.TranslateTransform(center.X, center.Y);
					port.RotateTransformDeg(angle, 0, 0);

					double mirrorx = property.MirrorH ? -1 : 1;
					double mirrory = property.MirrorV ? -1 : 1;
					port.ScaleTransform(mirrorx, mirrory, 0, 0);

					Drawing.Rectangle rect = new Drawing.Rectangle(-width/2, -height/2, width, height);
					port.PaintImage(image, rect);
#endif
#if true
					port.TranslateTransform(center.X-width/2, center.Y-height/2);
					port.RotateTransformDeg(angle, width/2, height/2);
					port.TranslateTransform(width/2, height/2);
					double sx = property.MirrorH ? -width  : width;
					double sy = property.MirrorV ? -height : height;
					port.ScaleTransform(sx, sy, 0.0, 0.0);
					port.TranslateTransform(-0.5, -0.5);

					Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, 1.0, 1.0);
					port.FilterImage = this.PropertyImage.Filter;
					port.PaintImage(image, rect);
					//?port.PaintImage(image, rect, property.Filter);  // TODO: passer ce paramètre à AGG
#endif
				}

				port.Transform = ot;
			}
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques.
			Path path = this.PathBuildSurface();
			return path;
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Image(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}

		public override void ReadCheckWarnings(FontFaceInfo[] fonts, System.Collections.ArrayList warnings)
		{
			//	Vérifie si tous les fichiers existent.
			Properties.Image pi = this.PropertyImage;
			if ( pi == null )
			{
				return;
			}

			if (pi.InsideDoc)  // image incorporée au document ?
			{
				return;  // oui -> forcément OK
			}

			if ( !System.IO.File.Exists(pi.Filename) )
			{
				string message = string.Format(Res.Strings.Object.Image.Error, pi.Filename);
				if ( !warnings.Contains(message) )
				{
					warnings.Add(message);
				}
			}
		}
		#endregion
	}
}
