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


		// Nom de l'ic�ne.
		public override string IconName
		{
			get { return @"file:images/image.icon"; }
		}


		// D�tecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			return Geometry.DetectSurface(this.PathBuildSurface(), pos);
		}


		// D�place une poign�e.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.ConstrainSnapPos(ref pos);
			drawingContext.SnapGrid(ref pos);

			     if ( rank == 0 )  this.MoveCorner(pos, 0, 2,3, 1);
			else if ( rank == 1 )  this.MoveCorner(pos, 1, 3,2, 0);
			else if ( rank == 2 )  this.MoveCorner(pos, 2, 0,1, 3);
			else if ( rank == 3 )  this.MoveCorner(pos, 3, 1,0, 2);
			else                   this.Handle(rank).Position = pos;

			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// D�but de la cr�ation d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFixStarting(pos);
			drawingContext.ConstrainFixType(ConstrainType.Square);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// D�placement pendant la cr�ation d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la cr�ation d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();

			// Cr�e les 2 autres poign�es dans les coins oppos�s.
			Drawing.Rectangle rect = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);
			Point p1 = rect.BottomLeft;
			Point p2 = rect.TopRight;
			this.Handle(0).Position = p1;
			this.Handle(1).Position = p2;
			this.HandleAdd(new Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3

			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

		// Indique s'il faut s�lectionner l'objet apr�s sa cr�ation.
		public override bool SelectAfterCreation()
		{
			return true;
		}


		// Met � jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Path path = this.PathBuildSurface();
			this.bboxThin = path.ComputeBounds();

			path = this.PathBuildImage();
			this.bboxThin.MergeWith(path.ComputeBounds());

			this.bboxGeom = this.bboxThin;
			this.bboxFull = this.bboxThin;
		}

		// Cr�e le chemin de l'objet pour dessiner la surface exacte de l'image.
		protected Path PathBuildImage()
		{
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

		// Cr�e le chemin de l'objet pour dessiner sa surface.
		protected Path PathBuildSurface()
		{
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

		// Cr�e le chemin de l'objet pour dessiner son pourtour.
		protected Path PathBuildOutline()
		{
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

		// Donne les 4 coins du rectangle.
		protected void Corners(out Point pbl, out Point pbr, out Point ptl, out Point ptr)
		{
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

		// Calcule le centre, les dimensions et l'angle de l'image en fonction
		// du quadrilat�re de l'objet, qui n'est pas forc�ment rectangulaire.
		protected void ImageGeometry(out Point center, out double width, out double height, out double angle)
		{
			Point pbl, pbr, ptl, ptr;
			this.Corners(out pbl, out pbr, out ptl, out ptr);

			center = (pbl+pbr+ptl+ptr)/4;
			width  = System.Math.Min(Point.Distance(pbl,pbr), Point.Distance(ptl,ptr));
			height = System.Math.Min(Point.Distance(pbl,ptl), Point.Distance(pbr,ptr));
			angle  = Point.ComputeAngleDeg(pbl, pbr);
		}

		// Ouvre le bitmap de l'image si n�cessaire.
		protected void OpenBitmapOriginal()
		{
			Properties.Image image = this.PropertyImage;
			if ( image.Filename == "" )
			{
				this.imageOriginal = null;
				this.imageDimmed = null;
				this.filename = "";
			}
			else
			{
				if ( image.Filename != this.filename )
				{
					this.filename = image.Filename;
					try
					{
						this.imageOriginal = Drawing.Bitmap.FromFile(this.filename);
					}
					catch
					{
						this.imageOriginal = null;
					}
					this.imageDimmed = null;
				}
			}
		}

		// Ouvre le bitmap de la variante estomp�e de l'image si n�cessaire.
		protected void OpenBitmapDimmed(DrawingContext drawingContext)
		{
#if false
			if ( this.imageOriginal == null )  return;
			if ( this.imageDimmed != null )  return;
			if ( !drawingContext.IsDimmed )  return;

			this.imageDimmed = Bitmap.CopyImage(this.imageOriginal);
			Pixmap.RawData data = new Pixmap.RawData(this.imageDimmed);
			using ( data )
			{
				Color pixel;
				double intensity;

				for ( int y=0 ; y<data.Height ; y++ )
				{
					for ( int x=0 ; x<data.Width ; x++ )
					{
						pixel = data[x,y];

						intensity = pixel.GetBrightness();
						intensity = System.Math.Max(intensity*2.0-1.0, 0.0);
						pixel.R = intensity;
						pixel.G = intensity;
						pixel.B = intensity;
						pixel.A *= 0.2;  // tr�s transparent

						data[x,y] = pixel;
					}
				}
			}
#endif
		}

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path path;

			if ( this.TotalHandle == 2 )  // construction ?
			{
				path = this.PathBuildSurface();
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(drawingContext.HiliteSurfaceColor);

				path = this.PathBuildOutline();
				graphics.Rasterizer.AddOutline(path, 1.0/drawingContext.ScaleX);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);

				return;
			}

			this.OpenBitmapOriginal();
			this.OpenBitmapDimmed(drawingContext);

			Drawing.Image image = drawingContext.IsDimmed ? this.imageDimmed : this.imageOriginal;
			if ( image == null )
			{
				path = this.PathBuildOutline();
				graphics.Rasterizer.AddOutline(path, 1.0/drawingContext.ScaleX);
				graphics.RenderSolid(Color.FromBrightness(0.5));
			}
			else
			{
				Transform ot = graphics.Transform;

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

					graphics.TranslateTransform(center.X, center.Y);
					graphics.RotateTransformDeg(angle, 0, 0);

					double mirrorx = property.MirrorH ? -1 : 1;
					double mirrory = property.MirrorV ? -1 : 1;
					graphics.ScaleTransform(mirrorx, mirrory, 0, 0);

					Drawing.Rectangle rect = new Drawing.Rectangle(-width/2, -height/2, width, height);
					graphics.PaintImage(image, rect);
				}

				graphics.Transform = ot;
			}

			if ( this.selected )
			{
				path = this.PathBuildOutline();
				graphics.Rasterizer.AddOutline(path, 1.0/drawingContext.ScaleX);
				graphics.RenderSolid(Color.FromBrightness(0.5));

				path = new Path();
				path.MoveTo(this.Handle(0).Position);
				path.LineTo(this.Handle(3).Position);
				graphics.Rasterizer.AddOutline(path, 3.0/drawingContext.ScaleX);
				graphics.RenderSolid(Color.FromBrightness(0.5));
			}

			if ( this.IsHilite && drawingContext.IsActive )
			{
				path = this.PathBuildSurface();
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(drawingContext.HiliteSurfaceColor);

				path = this.PathBuildOutline();
				graphics.Rasterizer.AddOutline(path, 1.0/drawingContext.ScaleX);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			this.OpenBitmapOriginal();
			this.OpenBitmapDimmed(drawingContext);

			Drawing.Image image = drawingContext.IsDimmed ? this.imageDimmed : this.imageOriginal;
			if ( image == null )
			{
				Path path = this.PathBuildOutline();
				port.LineWidth = 1.0/drawingContext.ScaleX;
				port.Color = Color.FromBrightness(0.5);
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

					port.TranslateTransform(center.X, center.Y);
					port.RotateTransformDeg(angle, 0, 0);

					double mirrorx = property.MirrorH ? -1 : 1;
					double mirrory = property.MirrorV ? -1 : 1;
					port.ScaleTransform(mirrorx, mirrory, 0, 0);

					Drawing.Rectangle rect = new Drawing.Rectangle(-width/2, -height/2, width, height);
					port.PaintImage(image, rect);
				}

				port.Transform = ot;
			}
		}


		#region Serialization
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected Image(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected string					filename;
		protected Drawing.Image				imageOriginal;
		protected Drawing.Image				imageDimmed;
	}
}
