using System.Collections.Generic;
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
			if ( type == Properties.Type.Frame )  return true;
			if ( type == Properties.Type.Corner )  return true;
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


		public override string IconUri
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
				drawingContext.ConstrainClear();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
						 if ( rank == 0 )  drawingContext.ConstrainAddRect(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position, false, -1);
					else if ( rank == 1 )  drawingContext.ConstrainAddRect(this.Handle(1).Position, this.Handle(0).Position, this.Handle(3).Position, this.Handle(2).Position, false, -1);
					else if ( rank == 2 )  drawingContext.ConstrainAddRect(this.Handle(2).Position, this.Handle(3).Position, this.Handle(0).Position, this.Handle(1).Position, false, -1);
					else if ( rank == 3 )  drawingContext.ConstrainAddRect(this.Handle(3).Position, this.Handle(2).Position, this.Handle(1).Position, this.Handle(0).Position, false, -1);
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
			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHomo(pos, false, -1);
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

		public override void Reset()
		{
			//	Remet l'objet droit et d'équerre.
			if (this.handles.Count >= 4)
			{
				Drawing.Rectangle box = this.BoundingBoxThin;
				this.Handle(0).Position = box.BottomLeft;
				this.Handle(1).Position = box.TopRight;
				this.Handle(2).Position = box.TopLeft;
				this.Handle(3).Position = box.BottomRight;
			}
		}


		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			var frame = this.PropertyFrame;

			Path pathImage = this.PathBuildImage ();
			Path pathSurface = this.PathBuildSurface ();
			Path pathOutline = this.PathBuildOutline ();

			var shapes = new List<Shape> ();

			//	Trait du rectangle.
			{
				var shape = new Shape ();
				shape.Path = pathOutline;
				shape.Type = Type.Stroke;
				if (drawingContext != null && (drawingContext.FillEmptyPlaceholders || drawingContext.PreviewActive || (drawingContext.DrawImageFilter != null && drawingContext.DrawImageFilter (new DrawingContext.DrawImageFilterInfo (this, "box")) == false)))
				{
					shape.Aspect = Aspect.InvisibleBox;  // n'affiche pas le pourtour pointillé
				}

				shapes.Add (shape);
			}

			//	Image bitmap.
			var imageShape = new Shape ();
			imageShape.SetImageObject (this);

			if (!simplify && (frame == null || frame.FrameType == Properties.FrameType.None))  // pas de cadre ?
			{
				shapes.Add (imageShape);
			}
			else  // cadre ?
			{
				var objectShapes = new List<Shape> ();
				objectShapes.Add (imageShape);

				frame.AddShapes (shapes, objectShapes, port, drawingContext, this.GetImagePixelPolygons (), this.PropertyCorner);
			}

			//	Rectangle complet pour bbox et détection.
			{
				var shape = new Shape ();
				shape.Path = pathSurface;
				shape.Type = Type.Surface;
				shape.Aspect = Aspect.InvisibleBox;

				shapes.Add (shape);
			}

			//	Rectangle complet pour bbox et détection.
			{
				var shape = new Shape ();
				shape.Path = pathImage;
				shape.Type = Type.Surface;
				shape.Aspect = Aspect.InvisibleBox;

				shapes.Add (shape);
			}

			return shapes.ToArray ();
		}

		protected List<Polygon> GetImagePixelPolygons()
		{
			//	Crée le chemin correspondant à la partie réelle de l'image, inclue dans le rectangle englobant.
			ImageCache.Item item = this.Item;

			if (item == null)
			{
				return this.GetImagePolygons ();
			}
			else
			{
				var polygons = new List<Polygon> ();
				var polygon = new Polygon ();
				polygons.Add (polygon);

				Point center;
				double width, height, angle;
				this.ImageGeometry (out center, out width, out height, out angle);

				Properties.Image pi = this.PropertyImage;
				Margins crop = pi.CropMargins;

				Drawing.Rectangle cropRect = new Drawing.Rectangle (0, 0, item.Size.Width, item.Size.Height);
				cropRect.Deflate (crop);

				double sx = cropRect.Width/width;
				double sy = cropRect.Height/height;
				double scale = System.Math.Max (sx, sy);

				width = cropRect.Width/scale;
				height = cropRect.Height/scale;

				Point p = new Point ();

				p.X = center.X-width/2;
				p.Y = center.Y-height/2;
				polygon.Points.Add (Transform.RotatePointDeg (center, angle, p));

				p.X = center.X+width/2;
				p.Y = center.Y-height/2;
				polygon.Points.Add (Transform.RotatePointDeg (center, angle, p));

				p.X = center.X+width/2;
				p.Y = center.Y+height/2;
				polygon.Points.Add (Transform.RotatePointDeg (center, angle, p));

				p.X = center.X-width/2;
				p.Y = center.Y+height/2;
				polygon.Points.Add (Transform.RotatePointDeg (center, angle, p));

				return polygons;
			}
		}

		protected Path PathBuildImage()
		{
			//	Crée le chemin de l'objet pour dessiner la surface exacte de l'image.
			var polygons = this.GetImagePolygons ();
			return Polygon.GetPolygonPath (polygons);
		}

		protected List<Polygon> GetImagePolygons()
		{
			//	Crée le chemin de l'objet pour dessiner la surface exacte de l'image.
			var polygons = new List<Polygon> ();
			var polygon = new Polygon ();
			polygons.Add (polygon);

			Point center;
			double width, height, angle;
			this.ImageGeometry (out center, out width, out height, out angle);

			Point p = new Point ();

			p.X = center.X-width/2;
			p.Y = center.Y-height/2;
			polygon.Points.Add (Transform.RotatePointDeg (center, angle, p));

			p.X = center.X+width/2;
			p.Y = center.Y-height/2;
			polygon.Points.Add (Transform.RotatePointDeg (center, angle, p));

			p.X = center.X+width/2;
			p.Y = center.Y+height/2;
			polygon.Points.Add (Transform.RotatePointDeg (center, angle, p));

			p.X = center.X-width/2;
			p.Y = center.Y+height/2;
			polygon.Points.Add (Transform.RotatePointDeg (center, angle, p));

			return polygons;
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

		public Size ImageBitmapMaxFill
		{
			//	Donne la place maximale que peut prendre le bitmap rectangulaire de l'image.
			get
			{
				Point center;
				double width, height, angle;
				this.ImageGeometry(out center, out width, out height, out angle);
				return new Size(width, height);
			}
		}

		public Size ImageBitmapSize
		{
			//	Donne les dimensions effectives utilisées par le bitmap rectangulaire de l'image.
			get
			{
				Point center;
				double width, height, angle;
				this.ImageGeometry(out center, out width, out height, out angle);

				Size imageSize = Size.Zero;
				ImageCache.Item item = this.Item;
				if (item != null && item.Size != Size.Zero)
				{
					imageSize = item.Size;
				}

				if (width > 0 && height > 0 && imageSize != Size.Zero)
				{
					Properties.Image property = this.PropertyImage;

					if ( property.Homo )  // conserve les proportions ?
					{
						double rapport = imageSize.Height/imageSize.Width;
						if ( rapport < height/width )  height = width*rapport;
						else                           width  = height/rapport;
					}
				}

				return new Size(width, height);
			}
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

			Properties.Image pi = this.PropertyImage;
			if (pi != null)
			{
				if (pi.RotationMode == Properties.Image.Rotation.Angle90)
				{
					angle += 90;
				}

				if (pi.RotationMode == Properties.Image.Rotation.Angle180)
				{
					angle += 180;
				}
				
				if (pi.RotationMode == Properties.Image.Rotation.Angle270)
				{
					angle += 270;
				}

				if (pi.RotationMode == Properties.Image.Rotation.Angle90 || pi.RotationMode == Properties.Image.Rotation.Angle270)
				{
					Misc.Swap(ref width, ref height);
				}
			}
		}

		public ImageCache.Item Item
		{
			//	Retourne l'item de l'image cachée, s'il existe.
			get
			{
				Properties.Image pi = this.PropertyImage;
				return this.document.ImageCache.Find(pi.FileName, pi.FileDate);
			}
		}

		protected override string NameToDisplay
		{
			//	Retourne le nom de l'objet à afficher (Label) en haut à gauche.
			//	Le nom est composé de trois parties:
			//	1) Le nom de l'objet (s'il existe)
			//	2) Le nom du fichier bitmap (sans dossiers ni extension)
			//	3) La résolution en dpi (entre parenthéses)
			get
			{
				string name = base.NameToDisplay;

				Properties.Image pi = this.PropertyImage;
				if (pi != null)
				{
					if (string.IsNullOrEmpty(name))
					{
						name = Misc.ExtractName(pi.FileName);
					}
					else
					{
						name = string.Concat(name, " ", Misc.ExtractName(pi.FileName));
					}

					int dpi = this.GetOutputDpi();

					if (dpi > 0)
					{
						name = string.Concat(name, " (", dpi.ToString(), " dpi)");
					}
				}

				return name;
			}
		}

		public int GetOutputDpi()
		{
			Properties.Image pi = this.PropertyImage;
			int dpi = 0;

			if (pi != null)
			{
				ImageCache.Item item = this.Item;
				if (item != null)
				{
					Drawing.Image image = item.Image;
					if (image != null)
					{
						Point center;
						double width, height, angle;
						this.ImageGeometry(out center, out width, out height, out angle);

						if (width > 0 && height > 0)
						{
							Drawing.Rectangle cropRect = new Drawing.Rectangle(0, 0, image.Width*item.Scale, image.Height*item.Scale);
							cropRect.Deflate(pi.CropMargins);

							if (!cropRect.IsSurfaceZero)
							{
								if (pi.Homo)  // conserve les proportions ?
								{
									double rapport = cropRect.Height/cropRect.Width;
									if (rapport < height/width)
									{
										height = width*rapport;
									}
									else
									{
										width  = height/rapport;
									}
								}

								dpi = (int) (cropRect.Width/width*254);
							}
						}
					}
				}
			}
			return dpi;
		}

		public ImageFilter GetFilter(IPaintPort port, DrawingContext drawingContext)
		{
			//	Retourne le filtre à utiliser pour l'image. Le filtre n'est pas le même selon
			//	les dimensions de l'objet image, car il faut utiliser un filtre 'Resampling*'
			//	lors d'une réduction, pour éviter les moirés.
			Size size = Size.Zero;
			double scale = 1;

			ImageCache.Item item = this.Item;
			if (item != null && item.Size != Size.Zero)
			{
				size = item.Size;
				scale = item.Scale;
			}

			if (size != Size.Zero)
			{
				Properties.Image pi = this.PropertyImage;
				Margins crop = pi.CropMargins;
				crop.Left   /= scale;
				crop.Right  /= scale;
				crop.Bottom /= scale;
				crop.Top    /= scale;

				Point center;
				double width, height, angle;
				this.ImageGeometry(out center, out width, out height, out angle);

				double zoom = port.Transform.GetZoom();
				width  *= zoom;
				height *= zoom;

				if (width > 0 && height > 0)
				{
					Drawing.Rectangle cropRect = new Drawing.Rectangle(0, 0, size.Width, size.Height);
					cropRect.Deflate(crop);

					if (!cropRect.IsSurfaceZero)
					{
						if (pi.Homo)  // conserve les proportions ?
						{
							double rapport = cropRect.Height/cropRect.Width;
							if (rapport < height/width)
							{
								height = width*rapport;
							}
							else
							{
								width  = height/rapport;
							}
						}

						bool resampling = (width < cropRect.Width || height < cropRect.Height);
						return Properties.Image.CategoryToFilter(drawingContext, pi.FilterCategory, resampling);
					}
				}
			}

			return new ImageFilter();
		}

		public override void DrawImage(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine l'image.
			if ( this.TotalHandle < 2 )  return;

			Drawing.Image image = null;
			ImageCache.Item item = this.Item;
			Size size = this.ImageBitmapSize;
			Properties.Image pi = this.PropertyImage;
			Margins crop = pi.CropMargins;
			port.ImageFinalSize = size;

			ImageFilter filter = this.GetFilter(port, drawingContext);

			if (item != null)
			{
				double scale = item.Scale;
				crop.Left   /= scale;
				crop.Right  /= scale;
				crop.Bottom /= scale;
				crop.Top    /= scale;
				port.ImageCrop = crop;

				if (port is PDF.Port)  // exportation PDF ?
				{
					PDF.Port pdfPort = port as PDF.Port;
					PDF.ImageSurface surface = pdfPort.SearchImageSurface(pi.FileName, size, crop, filter);
					System.Diagnostics.Debug.Assert(surface != null);
					image = (surface == null) ? null : surface.DrawingImage;

					port.ImageCrop = crop;
				}
				else
				{
					//?image = drawingContext.IsDimmed ? item.DimmedImage : item.Image;
					image = item.Image;
				}
			}

			if ( image == null )
			{
				if (!drawingContext.PreviewActive)
				{
					if (drawingContext.FillEmptyPlaceholders)
					{
						using (Path path = this.PathBuildSurface())
						{
							port.Color = Color.FromAlphaRgb (1.0, 0.5, 0.5, 1);
							port.PaintSurface(path);  // dessine une surface bleue
							port.Color = Color.FromAlphaRgb (1.0, 0.0, 0.0, 1);
							port.LineWidth = 20.0;
							port.LineJoin = JoinStyle.Miter;
							port.PaintOutline(path);
						}
					}
					else
					{
						using (Path path = this.PathBuildOutline ())
						{
							port.Color = Color.FromBrightness (0.5);
							port.LineWidth = 1.0/drawingContext.ScaleX;
							port.PaintOutline(path);  // dessine un rectangle avec une croix
						}
					}
				}
			}
			else
			{
				Transform ot = port.Transform;

				Point center;
				double width, height, angle;
				this.ImageGeometry(out center, out width, out height, out angle);

				if ( width > 0 && height > 0 )
				{
					Drawing.Rectangle cropRect = new Drawing.Rectangle(0, 0, image.Width, image.Height);
					cropRect.Deflate(crop);

					if (!cropRect.IsSurfaceZero)
					{
						if (pi.Homo)  // conserve les proportions ?
						{
							double rapport = cropRect.Height/cropRect.Width;
							if (rapport < height/width)
							{
								height = width*rapport;
							}
							else
							{
								width  = height/rapport;
							}
						}

#if false
						port.TranslateTransform(center.X, center.Y);
						port.RotateTransformDeg(angle, 0, 0);

						double mirrorx = pi.MirrorH ? -1 : 1;
						double mirrory = pi.MirrorV ? -1 : 1;
						port.ScaleTransform(mirrorx, mirrory, 0, 0);

						Drawing.Rectangle rect = new Drawing.Rectangle(-width/2, -height/2, width, height);
						port.PaintImage(image, rect);
#endif
#if true
						port.TranslateTransform(center.X-width/2, center.Y-height/2);
						port.RotateTransformDeg(angle, width/2, height/2);
						port.TranslateTransform(width/2, height/2);
						double sx = pi.MirrorH ? -width  : width;
						double sy = pi.MirrorV ? -height : height;
						port.ScaleTransform(sx, sy, 0.0, 0.0);
						port.TranslateTransform(-0.5, -0.5);

						Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, 1.0, 1.0);
						ImageFilter oldFilter = port.ImageFilter;
						port.ImageFilter = filter;
						port.PaintImage(image, rect, cropRect);
						port.ImageFilter = oldFilter;
#endif
					}
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

		public void ImportClipboard(string filename)
		{
			Properties.Image pi = this.PropertyImage;
			if (pi != null)
			{
				pi.FileName = filename;
				pi.FileDate = this.document.ImageCache.LoadFromFile(pi.FileName);
				pi.InsideDoc = true;
				pi.FromClipboard = true;
			}
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

		public override void ReadCheckWarnings(System.Collections.ArrayList warnings)
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
			if (pi.FileName == null ||			//  pas de nom d'image ?
				pi.FileName.Trim().Length == 0)
			{
				return;
			}

			if ( !System.IO.File.Exists(pi.FileName) )
			{
				string message = string.Format(Res.Strings.Object.Image.Error, pi.FileName);
				if ( !warnings.Contains(message) )
				{
					warnings.Add(message);
				}
			}
		}
		#endregion
	}
}
