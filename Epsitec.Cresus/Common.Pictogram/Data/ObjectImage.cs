using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectImage est la classe de l'objet graphique "image bitmap".
	/// </summary>
	public class ObjectImage : AbstractObject
	{
		public ObjectImage()
		{
			PropertyName name = new PropertyName();
			name.Type = PropertyType.Name;
			this.AddProperty(name);

			PropertyImage image = new PropertyImage();
			image.Type = PropertyType.Image;
			this.AddProperty(image);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectImage();
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/image.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			return AbstractObject.DetectSurface(this.PathBuildSurface(), pos);
		}


		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, iconContext);
				return;
			}

			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);

			     if ( rank == 0 )  this.MoveCorner(pos, 0, 2,3, 1);
			else if ( rank == 1 )  this.MoveCorner(pos, 1, 3,2, 0);
			else if ( rank == 2 )  this.MoveCorner(pos, 2, 0,1, 3);
			else if ( rank == 3 )  this.MoveCorner(pos, 3, 1,0, 2);
			else                   this.Handle(rank).Position = pos;

			this.dirtyBbox = true;
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos, ConstrainType.Square);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			iconContext.ConstrainDelStarting();

			// Crée les 2 autres poignées dans les coins opposés.
			Drawing.Rectangle rect = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);
			Drawing.Point p1 = rect.BottomLeft;
			Drawing.Point p2 = rect.TopRight;
			this.Handle(0).Position = p1;
			this.Handle(1).Position = p2;
			this.HandleAdd(new Drawing.Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Drawing.Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}

		// Indique s'il faut sélectionner l'objet après sa création.
		public override bool SelectAfterCreation()
		{
			return true;
		}


		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Drawing.Path path = this.PathBuildSurface();
			this.bboxThin = path.ComputeBounds();

			path = this.PathBuildImage();
			this.bboxThin.MergeWith(path.ComputeBounds());

			this.bboxGeom = this.bboxThin;
			this.bboxFull = this.bboxThin;
		}

		// Crée le chemin de l'objet pour dessiner la surface exacte de l'image.
		protected Drawing.Path PathBuildImage()
		{
			Drawing.Point center;
			double width, height, angle;
			this.ImageGeometry(out center, out width, out height, out angle);

			Drawing.Point p = new Drawing.Point();

			p.X = center.X-width/2;
			p.Y = center.Y-height/2;
			Drawing.Point p1 = Drawing.Transform.RotatePointDeg(center, angle, p);

			p.X = center.X+width/2;
			p.Y = center.Y-height/2;
			Drawing.Point p2 = Drawing.Transform.RotatePointDeg(center, angle, p);

			p.X = center.X+width/2;
			p.Y = center.Y+height/2;
			Drawing.Point p3 = Drawing.Transform.RotatePointDeg(center, angle, p);

			p.X = center.X-width/2;
			p.Y = center.Y+height/2;
			Drawing.Point p4 = Drawing.Transform.RotatePointDeg(center, angle, p);

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		// Crée le chemin de l'objet pour dessiner sa surface.
		protected Drawing.Path PathBuildSurface()
		{
			Drawing.Point pbl, pbr, ptl, ptr;
			this.Corners(out pbl, out pbr, out ptl, out ptr);

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(pbl);
			path.LineTo(pbr);
			path.LineTo(ptr);
			path.LineTo(ptl);
			path.Close();
			return path;
		}

		// Crée le chemin de l'objet pour dessiner son pourtour.
		protected Drawing.Path PathBuildOutline()
		{
			Drawing.Point pbl, pbr, ptl, ptr;
			this.Corners(out pbl, out pbr, out ptl, out ptr);

			Drawing.Path path = new Drawing.Path();
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
		protected void Corners(out Drawing.Point pbl, out Drawing.Point pbr, out Drawing.Point ptl, out Drawing.Point ptr)
		{
			pbl = this.Handle(0).Position;
			ptr = this.Handle(1).Position;

			if ( this.handles.Count < 4 )
			{
				ptl = new Drawing.Point(pbl.X, ptr.Y);
				pbr = new Drawing.Point(ptr.X, pbl.Y);
			}
			else
			{
				ptl = this.Handle(2).Position;
				pbr = this.Handle(3).Position;
			}
		}

		// Calcule le centre, les dimensions et l'angle de l'image en fonction
		// du quadrilatère de l'objet, qui n'est pas forcément rectangulaire.
		protected void ImageGeometry(out Drawing.Point center, out double width, out double height, out double angle)
		{
			Drawing.Point pbl, pbr, ptl, ptr;
			this.Corners(out pbl, out pbr, out ptl, out ptr);

			center = (pbl+pbr+ptl+ptr)/4;
			width  = System.Math.Min(Drawing.Point.Distance(pbl,pbr), Drawing.Point.Distance(ptl,ptr));
			height = System.Math.Min(Drawing.Point.Distance(pbl,ptl), Drawing.Point.Distance(pbr,ptr));
			angle  = Drawing.Point.ComputeAngleRad(pbl, pbr);
		}

		// Ouvre le bitmap de l'image si nécessaire.
		protected void OpenBitmapOriginal()
		{
			PropertyImage image = this.PropertyImage(1);
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

		// Ouvre le bitmap de la variante estompée de l'image si nécessaire.
		protected void OpenBitmapDimmed(IconContext iconContext)
		{
			if ( this.imageOriginal == null )  return;
			if ( this.imageDimmed != null )  return;
			if ( !iconContext.IsDimmed )  return;

			this.imageDimmed = Drawing.Bitmap.CopyImage(this.imageOriginal);
			Drawing.Pixmap.RawData data = new Drawing.Pixmap.RawData(this.imageDimmed);
			using ( data )
			{
				Drawing.Color pixel;
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
						pixel.A *= 0.2;  // très transparent

						data[x,y] = pixel;
					}
				}
			}
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext, iconObjects);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Path path;

			if ( this.TotalHandle == 2 )  // construction ?
			{
				path = this.PathBuildSurface();
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(iconContext.HiliteSurfaceColor);

				path = this.PathBuildOutline();
				graphics.Rasterizer.AddOutline(path, 1.0/iconContext.ScaleX);
				graphics.RenderSolid(iconContext.HiliteOutlineColor);

				return;
			}

			this.OpenBitmapOriginal();
			this.OpenBitmapDimmed(iconContext);

			Drawing.Image image = iconContext.IsDimmed ? this.imageDimmed : this.imageOriginal;
			if ( image == null )
			{
				path = this.PathBuildOutline();
				graphics.Rasterizer.AddOutline(path, 1.0/iconContext.ScaleX);
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));
			}
			else
			{
				Drawing.Transform ot = graphics.SaveTransform();

				Drawing.Point center;
				double width, height, angle;
				this.ImageGeometry(out center, out width, out height, out angle);

				if ( width > 0 && height > 0 )
				{
					PropertyImage property = this.PropertyImage(1);

					if ( property.Homo )  // conserve les proportions ?
					{
						double rapport = image.Height/image.Width;
						if ( rapport < height/width )  height = width*rapport;
						else                           width  = height/rapport;
					}

					graphics.TranslateTransform(center.X, center.Y);

					graphics.RotateTransformRad(angle, 0, 0);

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
				graphics.Rasterizer.AddOutline(path, 1.0/iconContext.ScaleX);
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));

				path = new Drawing.Path();
				path.MoveTo(this.Handle(0).Position);
				path.LineTo(this.Handle(3).Position);
				graphics.Rasterizer.AddOutline(path, 3.0/iconContext.ScaleX);
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.5));
			}

			if ( this.IsHilite && iconContext.IsEditable )
			{
				path = this.PathBuildSurface();
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(iconContext.HiliteSurfaceColor);

				path = this.PathBuildOutline();
				graphics.Rasterizer.AddOutline(path, 1.0/iconContext.ScaleX);
				graphics.RenderSolid(iconContext.HiliteOutlineColor);
			}
		}


		protected string					filename;
		protected Drawing.Image				imageOriginal;
		protected Drawing.Image				imageDimmed;
	}
}
