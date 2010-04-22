using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Regular est la classe de l'objet graphique "polygone r�gulier".
	/// </summary>
	[System.Serializable()]
	public class Regular : Objects.Abstract
	{
		public Regular(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.FillGradient )  return true;
			if ( type == Properties.Type.Regular )  return true;
			if ( type == Properties.Type.Corner )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Regular(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		public override string IconUri
		{
			//	Nom de l'ic�ne.
			get { return Misc.Icon("ObjectRegular"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�but du d�placement une poign�e.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				drawingContext.ConstrainClear();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					if ( rank == 0 )  // centre ?
					{
						drawingContext.ConstrainAddHV(this.Handle(0).Position, false, -1);
					}
					if ( rank == 1 )  // extr�mit� ?
					{
						drawingContext.ConstrainAddCenter(this.Handle(0).Position, false, -1);
						drawingContext.ConstrainAddCircle(this.Handle(0).Position, this.Handle(1).Position, false, -1);
					}
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
			//	D�place une poign�e.
			if ( rank >= 2 )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if ( rank == 0 )  // centre ?
			{
				Point move = pos-this.Handle(rank).Position;
				this.Handle(0).Position = pos;
				this.Handle(1).Position += move;
			}
			if ( rank == 1 )  // extr�mit� ?
			{
				this.Handle(1).Position = pos;
			}

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModifCircle();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	D�but de la cr�ation d'un objet.
			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHV(pos, false, -1);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	D�placement pendant la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.SetDirtyBbox();
			this.TextInfoModifCircle();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}


		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			Properties.Regular pr = this.PropertyRegular;

			int totalShapes = 3;

			bool support = false;
			if ( this.IsSelected &&
				 this.document.Modifier.IsPropertiesExtended(Properties.Type.Regular) &&
				 (pr.RegularType != Properties.RegularType.Norm) &&
				 drawingContext != null && drawingContext.IsActive &&
				 !this.IsGlobalSelected )
			{
				support = true;
				totalShapes ++;
			}
			
			Path path = this.PathBuild(drawingContext, simplify);
			Shape[] shapes = new Shape[totalShapes];
			int i = 0;

			//	Forme de la surface.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].SetPropertySurface(port, this.PropertyFillGradient);
			i ++;

			//	Forme du chemin.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
			i ++;

			Point center = this.Handle(0).Position;
			double radius = Point.Distance(center, this.Handle(1).Position);

			//	Forme des traits de construction.
			if ( support )
			{
				Path pathSupport = new Path();

				pathSupport.AppendCircle(center, radius);
				pathSupport.AppendCircle(center, radius*(1.0-pr.Deep.R));

				if (pr.RegularType == Properties.RegularType.Flower1 || pr.RegularType == Properties.RegularType.Flower2)
				{
					Point p1, s1, s2, p2;
					this.ComputeCurve(0, out p1, out s1, out s2, out p2);
					pathSupport.MoveTo(p1);
					pathSupport.LineTo(s1);
					pathSupport.MoveTo(p2);
					pathSupport.LineTo(s2);
				}

				if (pr.RegularType == Properties.RegularType.Flower2)
				{
					Point p1, s1, s2, p2;
					this.ComputeCurve(pr.NbFaces*2-1, out p1, out s1, out s2, out p2);
					pathSupport.MoveTo(p1);
					pathSupport.LineTo(s1);
					pathSupport.MoveTo(p2);
					pathSupport.LineTo(s2);
				}

				shapes[i] = new Shape();
				shapes[i].Path = pathSupport;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Support;
				shapes[i].IsVisible = true;
				i ++;
			}

			//	Pour bbox et d�tection
			Path pathBbox = new Path();

			pathBbox.AppendRectangle(center.X-radius, center.Y-radius, radius*2, radius*2);

			shapes[i] = new Shape();
			shapes[i].Path = pathBbox;
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.InvisibleBox;
			i ++;

			return shapes;
		}

		public void ComputeCorners(out Point a, out Point b)
		{
			//	Calcule les points pour les poign�es de la propri�t� PropertyCorner.
			Point t, s1, s2;

			Properties.Regular pr = this.PropertyRegular;
			if ( pr.RegularType == Properties.RegularType.Norm )  // polygone ?
			{
				this.ComputeCurve(0, out t, out s1, out s2, out a);
				this.ComputeCurve(pr.NbFaces-1, out b, out s1, out s2, out t);
			}
			else	// �toile ?
			{
				this.ComputeCurve(0, out t, out s1, out s2, out a);
				this.ComputeCurve(pr.NbFaces*2-1, out b, out s1, out s2, out t);
			}
		}

		protected void ComputeCurve(int i, out Point p1, out Point s1, out Point s2, out Point p2)
		{
			//	Calcule une courbe de l'objet.
			Properties.Regular reg = this.PropertyRegular;
			int total = reg.NbFaces;
			Point center = this.Handle(0).Position;
			Point corner = this.Handle(1).Position;

			p1 = Point.Zero;
			s1 = Point.Zero;
			s2 = Point.Zero;
			p2 = Point.Zero;

			if (reg.RegularType == Properties.RegularType.Flower1 || reg.RegularType == Properties.RegularType.Flower2)  // fleur ?
			{
				Point star = Point.Scale(corner, center, reg.Deep.R);
				double a = reg.Deep.A;

				if (i%2 == 0)
				{
					double a1 = 360.0*(i+0)/(total*2);
					double a2 = 360.0*(i+1)/(total*2)+a;

					p1 = this.PolarToPoint(1, a1);
					s1 = this.PolarToPoint(1-reg.Deep.R*reg.E1.R, Regular.Scale(a1,a2,reg.E1.R)+reg.E1.A);
					s2 = this.PolarToPoint(1-reg.Deep.R*reg.I1.R, Regular.Scale(a1,a2,reg.I1.R)+reg.I1.A);
					p2 = this.PolarToPoint(1-reg.Deep.R, a2);
				}
				else
				{
					double a1 = 360.0*(i+0)/(total*2)+a;
					double a2 = 360.0*(i+1)/(total*2);

					p1 = this.PolarToPoint(1-reg.Deep.R, a1);
					s1 = this.PolarToPoint(1-reg.Deep.R*reg.I2.R, Regular.Scale(a2,a1,reg.I2.R)+reg.I2.A);
					s2 = this.PolarToPoint(1-reg.Deep.R*reg.E2.R, Regular.Scale(a2,a1,reg.E2.R)+reg.E2.A);
					p2 = this.PolarToPoint(1, a2);
				}
			}
			else if (reg.RegularType == Properties.RegularType.Star)  // �toile ?
			{
				Point star = Point.Scale(corner, center, reg.Deep.R);
				double a = reg.Deep.A;

				if (i%2 == 0)
				{
					double a1 = 360.0*(i+0)/(total*2);
					double a2 = 360.0*(i+1)/(total*2)+a;

					p1 = this.PolarToPoint(1, a1);
					p2 = this.PolarToPoint(1-reg.Deep.R, a2);
				}
				else
				{
					double a1 = 360.0*(i+0)/(total*2)+a;
					double a2 = 360.0*(i+1)/(total*2);

					p1 = this.PolarToPoint(1-reg.Deep.R, a1);
					p2 = this.PolarToPoint(1, a2);
				}
			}
			else	// polygone ?
			{
				p1 = Transform.RotatePointDeg(center, 360.0*(i+0)/total, corner);
				p2 = Transform.RotatePointDeg(center, 360.0*(i+1)/total, corner);
			}
		}

		static protected double Scale(double a, double b, double scale)
		{
			return a + (b-a)*scale;
		}

		public Point PolarToPoint(double r, double a)
		{
			//	Conversion d'une coordonn�e polaire (scale;angle) en coordonn�e (x;y), dans l'espace de l'objet.
			return this.PolarToPoint(new Polar(r, a));
		}

		public Point PolarToPoint(Polar p)
		{
			//	Conversion d'une coordonn�e polaire (scale;angle) en coordonn�e (x;y), dans l'espace de l'objet.
			Point center = this.Handle(0).Position;

			Point pos = Point.Scale(center, this.Handle(1).Position, p.R);
			return Transform.RotatePointDeg(center, p.A, pos);
		}

		public Polar PointToPolar(Point pos)
		{
			//	Conversion d'une coordonn�e (x;y) en coordonn�e polaire (scale;angle), dans l'espace de l'objet.
			double scale, angle;

			Point center = this.Handle(0).Position;

			double d1 = Point.Distance(center, this.Handle(1).Position);
			double d2 = Point.Distance(center, pos);
			if (d1 == 0)
			{
				scale = 0;
			}
			else
			{
				scale = d2/d1;
			}

			double a1 = Point.ComputeAngleDeg(center, this.Handle(1).Position);
			double a2 = Point.ComputeAngleDeg(center, pos);
			angle = a2-a1;

			if (angle >  180)  angle -= 360;
			if (angle < -180)  angle += 360;

			return new Polar(scale, angle);
		}

		protected Path PathBuild(DrawingContext drawingContext, bool simplify)
		{
			//	Cr�e le chemin d'un polygone r�gulier.
			Properties.Regular reg = this.PropertyRegular;

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);;

			int total = reg.NbFaces;
			if (reg.RegularType != Properties.RegularType.Norm)  total *= 2;  // �toile ?

			Properties.Corner corner = this.PropertyCorner;

			if ( corner.CornerType == Properties.CornerType.Right || simplify )  // coins droits ?
			{
				for ( int i=0 ; i<total ; i++ )
				{
					Point p1, s1, s2, p2;
					this.ComputeCurve(i, out p1, out s1, out s2, out p2);

					if (reg.RegularType == Properties.RegularType.Flower1 || reg.RegularType == Properties.RegularType.Flower2)
					{
						if (i == 0)
						{
							path.MoveTo(p1);
						}
						path.CurveTo(s1, s2, p2);
					}
					else
					{
						if (i == 0)
						{
							path.MoveTo(p1);
						}
						path.LineTo(p2);
					}
				}
				path.Close();
			}
			else	// coins quelconques ?
			{
				for (int i=0; i<total; i++)
				{
					int prev = i-1;
					if (prev < 0)
					{
						prev = total-1;
					}

					int next = i+1;
					if (next >= total)
					{
						next = 0;
					}

					if (reg.RegularType == Properties.RegularType.Flower1 || reg.RegularType == Properties.RegularType.Flower2)
					{
						Point p1, s1, s2, p2;
						Point p3, s3, s4, p4;
						Point p5, s5, s6, p6;
						this.ComputeCurve(prev, out p1, out s1, out s2, out p2);
						this.ComputeCurve(i,    out p3, out s3, out s4, out p4);
						this.ComputeCurve(next, out p5, out s5, out s6, out p6);

						Point c2, c3, c4, c5;
						double r1, r2;
						this.PathCorner(path, s2, p3, s3, corner, out c2, out c3, out r1);
						this.PathCorner(path, s4, p5, s5, corner, out c4, out c5, out r2);

						if (i == 0)
						{
							path.MoveTo(c2);
						}
						corner.PathCorner(path, c2,p3,c3, r1);
						
						path.CurveTo(s3, s4, c4);
					}
					else
					{
						Point p1, s1, s2, p2, s;
						this.ComputeCurve(prev, out p1, out s1, out s2, out s );
						this.ComputeCurve(i,    out s,  out s1, out s2, out p2);

						Point c1, c2;
						double radius;
						this.PathCorner(path, p1, s, p2, corner, out c1, out c2, out radius);

						if (i == 0)
						{
							path.MoveTo(c1);
						}
						else
						{
							path.LineTo(c1);
						}
						corner.PathCorner(path, c1,s,c2, radius);
					}
				}
				path.Close();
			}

			return path;
		}

		protected void PathCorner(Path path, Point p1, Point s, Point p2, Properties.Corner corner, out Point c1, out Point c2, out double radius)
		{
			//	Cr�e le chemin d'un coin.
			double l1 = Point.Distance(p1, s);
			double l2 = Point.Distance(p2, s);
			radius = System.Math.Min(corner.Radius, System.Math.Min(l1,l2)/2);
			c1 = Point.Move(s, p1, radius);
			c2 = Point.Move(s, p2, radius);
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin g�om�trique de l'objet pour les constructions
			//	magn�tiques.
			return this.PathBuild(null, true);
		}

		protected override Path GetPath()
		{
			//	Retourne le chemin g�om�trique de l'objet.
			return this.PathBuild(null, false);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Regular(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise l'objet.
		}
		#endregion
	}
}
