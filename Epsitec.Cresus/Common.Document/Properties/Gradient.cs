using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plantée lors de la désérialisation.
	public enum GradientFillType
	{
		None    = 0,
		Linear  = 1,
		Circle  = 2,
		Diamond = 3,
		Conic   = 4,
	}

	/// <summary>
	/// La classe Gradient représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Gradient : Abstract
	{
		public Gradient(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.fillType = GradientFillType.None;

			if ( this.type == Type.LineColor )
			{
				this.color1 = Drawing.Color.FromBrightness(0.0);
				this.color2 = Drawing.Color.FromBrightness(0.5);
			}
			else
			{
				this.color1 = Drawing.Color.FromBrightness(1.0);
				this.color2 = Drawing.Color.FromBrightness(0.5);
			}
			this.angle  = 0.0;
			this.cx     = 0.5;
			this.cy     = 0.5;
			this.sx     = 1.0;
			this.sy     = 1.0;
			this.repeat = 1;
			this.middle = 0.0;
			this.smooth = 0.0;
		}

		// Mode de remplissage du dégradé.
		public GradientFillType FillType
		{
			get
			{
				return this.fillType;
			}
			
			set
			{
				if ( this.fillType != value )
				{
					this.NotifyBefore();
					this.fillType = value;
					this.NotifyAfter();
				}
			}
		}

		// Couleur 1 du dégradé.
		public Drawing.Color Color1
		{
			get
			{
				return this.color1;
			}
			
			set
			{
				if ( this.color1 != value )
				{
					this.NotifyBefore();
					this.color1 = value;
					this.NotifyAfter();
				}
			}
		}

		// Couleur 2 du dégradé.
		public Drawing.Color Color2
		{
			get
			{
				return this.color2;
			}
			
			set
			{
				if ( this.color2 != value )
				{
					this.NotifyBefore();
					this.color2 = value;
					this.NotifyAfter();
				}
			}
		}

		// Angle du dégradé.
		public double Angle
		{
			get
			{
				return this.angle;
			}

			set
			{
				value = System.Math.Max(value, -360);
				value = System.Math.Min(value,  360);

				if ( this.angle != value )
				{
					this.NotifyBefore();
					this.angle = value;
					this.NotifyAfter();
				}
			}
		}

		// Centre x du dégradé.
		public double Cx
		{
			get
			{
				return this.cx;
			}
			
			set
			{
				if ( this.cx != value )
				{
					this.NotifyBefore();
					this.cx = value;
					this.NotifyAfter();
				}
			}
		}

		// Centre y du dégradé.
		public double Cy
		{
			get
			{
				return this.cy;
			}
			
			set
			{
				if ( this.cy != value )
				{
					this.NotifyBefore();
					this.cy = value;
					this.NotifyAfter();
				}
			}
		}

		// Echelle x du dégradé.
		public double Sx
		{
			get
			{
				return this.sx;
			}
			
			set
			{
				if ( this.sx != value )
				{
					this.NotifyBefore();
					this.sx = value;
					this.NotifyAfter();
				}
			}
		}

		// Echelle y du dégradé.
		public double Sy
		{
			get
			{
				return this.sy;
			}
			
			set
			{
				if ( this.sy != value )
				{
					this.NotifyBefore();
					this.sy = value;
					this.NotifyAfter();
				}
			}
		}

		// Nombre de répétitions.
		public int Repeat
		{
			get
			{
				return this.repeat;
			}

			set
			{
				value = System.Math.Max(value, 1);
				value = System.Math.Min(value, 8);
				
				if ( this.repeat != value )
				{
					this.NotifyBefore();
					this.repeat = value;
					this.NotifyAfter();
				}
			}
		}

		// Point milieu des couleurs.
		public double Middle
		{
			get
			{
				return this.middle;
			}

			set
			{
				value = System.Math.Max(value, -1.0);
				value = System.Math.Min(value,  1.0);
				
				if ( this.middle != value )
				{
					this.NotifyBefore();
					this.middle = value;
					this.NotifyAfter();
				}
			}
		}

		// Rayon du flou.
		public double Smooth
		{
			get
			{
				return this.smooth;
			}

			set
			{
				value = System.Math.Max(value,  0.0);
				
				if ( this.smooth != value )
				{
					this.NotifyBefore();
					this.smooth = value;
					this.NotifyAfter();
				}
			}
		}

		// Indique si une impression complexe est nécessaire.
		public override bool IsComplexPrinting
		{
			get
			{
				if ( this.fillType != GradientFillType.None )  return true;
				if ( this.color1.A > 0.0 && this.color1.A < 1.0 )  return true;
				return false;
			}
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }  // (*)
		}
		// (*)	Doit rendre "true" à cause de BoundingBox, lorsque this.fillType passe
		//		de GradientFillType.None à autre chose, et inversément.

		// Indique si le dégradé est visible.
		public bool IsVisible()
		{
			if ( this.fillType == GradientFillType.None )
			{
				return ( this.color1.A > 0 );
			}
			else
			{
				return ( this.color1.A > 0 || this.color2.A > 0 );
			}
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Gradient p = property as Gradient;
			p.fillType = this.fillType;
			p.color1   = this.color1;
			p.color2   = this.color2;
			p.angle    = this.angle;
			p.cx       = this.cx;
			p.cy       = this.cy;
			p.sx       = this.sx;
			p.sy       = this.sy;
			p.repeat   = this.repeat;
			p.middle   = this.middle;
			p.smooth   = this.smooth;
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Gradient p = property as Gradient;
			if ( p.fillType != this.fillType )  return false;
			if ( p.color1   != this.color1   )  return false;
			if ( p.color2   != this.color2   )  return false;
			if ( p.angle    != this.angle    )  return false;
			if ( p.cx       != this.cx       )  return false;
			if ( p.cy       != this.cy       )  return false;
			if ( p.sx       != this.sx       )  return false;
			if ( p.sy       != this.sy       )  return false;
			if ( p.repeat   != this.repeat   )  return false;
			if ( p.middle   != this.middle   )  return false;
			if ( p.smooth   != this.smooth   )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Gradient(document);
		}


		// Calcule le facteur de progression dans la couleur [0..1].
		protected double GetFactor(double progress)
		{
			if ( this.repeat > 1 )
			{
				int i = (int)(progress*this.repeat);
				progress = (progress*this.repeat)%1.0;
				if ( i%2 != 0 )  progress = 1.0-progress;
			}
			if ( this.middle != 0.0 )
			{
				double delta = System.Math.Sin(System.Math.PI*progress)*this.middle*0.45;
				Point p = Transform.RotatePointDeg(45, new Point(progress*System.Math.Sqrt(2), delta));
				progress = p.Y;
			}
			return progress;
		}

		// Effectue le rendu de la surface du chemin courant avec le dégradé.
		public void RenderSurface(Graphics graphics, DrawingContext drawingContext,
								  Path path, Rectangle bbox)
		{
			this.RenderOutline(graphics, drawingContext, path, 0.0, Drawing.CapStyle.Round, Drawing.JoinStyle.Round, 0.0, bbox);
		}

		// Effectue le rendu du trait du chemin courant avec le dégradé.
		public void RenderOutline(Graphics graphics, DrawingContext drawingContext,
								  Path path, double width, Drawing.CapStyle cap, Drawing.JoinStyle join,
								  double limit, Rectangle bbox)
		{
			if ( bbox.IsSurfaceZero )  return;

			Graphics mask = null;

			if ( this.smooth > 0 )  // flou ?
			{
				double sx = 1;
				double sy = 1;

				if ( drawingContext != null )
				{
					sx = drawingContext.ScaleX;
					sy = drawingContext.ScaleY;
				}

				mask = graphics.CreateAlphaMask();

				int step = (int)(this.smooth*sx);
				if ( step > 20 )  step = 20;
				if ( drawingContext != null && !drawingContext.PreviewActive )  step /= 4;  // brouillon
				if ( step <  2 )  step =  2;
				for ( int i=0 ; i<step ; i++ )
				{
					double w = this.smooth-i*this.smooth/step;
					mask.Rasterizer.AddOutline(path, width+w*2, cap, join, limit);
					double intensity = (i+1.0)/step;
					mask.RenderSolid(Drawing.Color.FromBrightness(intensity));
				}
				if ( width == 0.0 )
				{
					mask.Rasterizer.AddSurface(path);
				}
				mask.RenderSolid(Drawing.Color.FromBrightness(1));

				graphics.SolidRenderer.SetAlphaMask(mask.Pixmap, MaskComponent.R);

				bbox.Inflate(this.smooth);
				graphics.AddFilledRectangle(bbox);
			}
			else
			{
				if ( width == 0.0 )
				{
					graphics.Rasterizer.AddSurface(path);
				}
				else
				{
					graphics.Rasterizer.AddOutline(path, width, cap, join, limit);
				}
			}

			if ( this.fillType == GradientFillType.None ||
				 System.Math.Abs(this.sx) < 0.0001      ||
				 System.Math.Abs(this.sy) < 0.0001      )  // uniforme ?
			{
				Drawing.Color c1 = this.color1;
				if ( drawingContext != null )
				{
					c1 = drawingContext.AdaptColor(c1);
				}

				graphics.RenderSolid(c1);
			}
			else	// dégradé ?
			{
				Drawing.Color c1 = this.color1;
				Drawing.Color c2 = this.color2;
				if ( drawingContext != null )
				{
					c1 = drawingContext.AdaptColor(c1);
					c2 = drawingContext.AdaptColor(c2);
				}

				graphics.Rasterizer.FillMode = FillMode.NonZero;

				if ( this.repeat == 1 && this.middle == 0.0 )
				{
					graphics.GradientRenderer.SetColors(c1, c2);
				}
				else
				{
					double[] r = new double[256];
					double[] g = new double[256];
					double[] b = new double[256];
					double[] a = new double[256];

					for ( int i=0 ; i<256 ; i++ )
					{
						double factor = this.GetFactor(i/255.0);
						r[i] = c1.R + (c2.R-c1.R)*factor;
						g[i] = c1.G + (c2.G-c1.G)*factor;
						b[i] = c1.B + (c2.B-c1.B)*factor;
						a[i] = c1.A + (c2.A-c1.A)*factor;
					}

					graphics.GradientRenderer.SetColors(r, g, b, a);
				}

				Transform ot = graphics.GradientRenderer.Transform;
				Transform t = new Transform();

				if ( this.fillType == GradientFillType.Linear )
				{
					graphics.GradientRenderer.Fill = GradientFill.Y;
					Point center = new Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
					graphics.GradientRenderer.SetParameters(-100, 100);
					t.Scale(bbox.Width/100/2*this.sx, bbox.Height/100/2*this.sy);
					t.Translate(center);
					t.RotateDeg(this.angle+180, center);
				}
				else if ( this.fillType == GradientFillType.Circle )
				{
					graphics.GradientRenderer.Fill = GradientFill.Circle;
					Point center = new Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, 100);
					t.Scale(bbox.Width/100/2*this.sx, bbox.Height/100/2*this.sy);
					t.Translate(center);
					t.RotateDeg(this.angle, center);
				}
				else if ( this.fillType == GradientFillType.Diamond )
				{
					graphics.GradientRenderer.Fill = GradientFill.Diamond;
					Point center = new Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, 100);
					t.Scale(bbox.Width/100/2*this.sx, bbox.Height/100/2*this.sy);
					t.Translate(center);
					t.RotateDeg(this.angle, center);
				}
				else if ( this.fillType == GradientFillType.Conic )
				{
					graphics.GradientRenderer.Fill = GradientFill.Conic;
					Point center = new Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, 250);
					t.Scale(bbox.Width/100/2*this.sx, bbox.Height/100/2*this.sy);
					t.Translate(center);
					t.RotateDeg(this.angle-90.0, center);
				}

				graphics.GradientRenderer.Transform = t;
				graphics.RenderGradient();
				graphics.GradientRenderer.Transform = ot;
			}

			if ( this.smooth > 0 )  // flou ?
			{
				graphics.SolidRenderer.SetAlphaMask(null, MaskComponent.None);
				mask.Dispose();
			}
		}


		// Calcule la bbox pour la représentation du dégradé.
		public Rectangle BoundingBoxGeom(Rectangle bbox)
		{
			bbox.Inflate(this.smooth);
			return bbox;
		}

		// Retourne la valeur d'engraissement pour la bbox.
		public double InflateBoundingBoxWidth()
		{
			return this.smooth;
		}

		// Engraisse la bbox en fonction de la propriété.
		public override void InflateBoundingBox(Rectangle bbox, ref Rectangle bboxFull)
		{
			if ( this.fillType == GradientFillType.None )  return;

			Point center = new Point();
			center.X = bbox.Left+bbox.Width*this.cx;
			center.Y = bbox.Bottom+bbox.Height*this.cy;
			double dx = this.sx*bbox.Width/2;
			double dy = this.sy*bbox.Height/2;

			if ( this.fillType == GradientFillType.Linear )
			{
				Point p5 = center + Transform.RotatePointDeg(this.angle, new Point(  0, -dy));
				Point p6 = center + Transform.RotatePointDeg(this.angle, new Point(  0,  dy));

				bboxFull.MergeWith(this.ComputeExtremity(p5, p6, 0.0, 0.2, 0));
				bboxFull.MergeWith(this.ComputeExtremity(p5, p6, 0.0, 0.2, 1));
				bboxFull.MergeWith(this.ComputeExtremity(p6, p5, 0.0, 0.2, 0));
				bboxFull.MergeWith(this.ComputeExtremity(p6, p5, 0.0, 0.2, 1));
			}
			else
			{
				Point p1 = center + Transform.RotatePointDeg(this.angle, new Point( dx,  dy));
				Point p2 = center + Transform.RotatePointDeg(this.angle, new Point(-dx,  dy));
				Point p3 = center + Transform.RotatePointDeg(this.angle, new Point(-dx, -dy));
				Point p4 = center + Transform.RotatePointDeg(this.angle, new Point( dx, -dy));

				bboxFull.MergeWith(p1);
				bboxFull.MergeWith(p2);
				bboxFull.MergeWith(p3);
				bboxFull.MergeWith(p4);
			}
		}


		// Nombre de poignées.
		public override int TotalHandle(Objects.Abstract obj)
		{
			return 3;
		}

		// Indique si une poignée est visible.
		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			if ( !this.document.Modifier.IsPropertiesExtended(this.type) )
			{
				return false;
			}

			if ( this.fillType == GradientFillType.None )
			{
				return false;
			}
			else if ( this.fillType == GradientFillType.Linear )
			{
				return (rank < 2);
			}
			else
			{
				return (rank < 3);
			}
		}

		// Retourne le bbox à utiliser pour les poignées du dégradé.
		protected Rectangle BoundingBoxHandlesGradient(Objects.Abstract obj)
		{
			if ( this.type == Properties.Type.LineColor )
			{
				return obj.BoundingBoxGeom;
			}
			return obj.BoundingBoxThin;
		}

		// Retourne la position d'une poignée.
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			Point pos = new Point();
			Point center = new Point();
			Rectangle bbox = this.BoundingBoxHandlesGradient(obj);
			center.X = bbox.Left+bbox.Width*this.cx;
			center.Y = bbox.Bottom+bbox.Height*this.cy;

			if ( rank == 0 )  // centre ?
			{
				pos = center;
			}

			if ( rank == 1 )  // angle et échelle y ?
			{
				pos = center;
				pos.Y += this.sy*bbox.Height/2;
				pos = Transform.RotatePointDeg(center, this.angle, pos);
			}

			if ( rank == 2 )  // angle et échelle x ?
			{
				pos = center;
				pos.X += this.sx*bbox.Width/2;
				pos = Transform.RotatePointDeg(center, this.angle, pos);
			}

			return pos;
		}

		// Modifie la position d'une poignée.
		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			Point center = new Point();
			Rectangle bbox = this.BoundingBoxHandlesGradient(obj);
			center.X = bbox.Left+bbox.Width*this.cx;
			center.Y = bbox.Bottom+bbox.Height*this.cy;

			if ( rank == 0 )  // centre ?
			{
				this.Cx = (pos.X-bbox.Left)/bbox.Width;
				this.Cy = (pos.Y-bbox.Bottom)/bbox.Height;
			}

			if ( rank == 1 )  // angle et échelle y ?
			{
				this.Angle = Point.ComputeAngleDeg(center, pos)-90;
				pos = Transform.RotatePointDeg(center, -this.angle, pos);
				this.Sy = (pos.Y-center.Y)/bbox.Height*2;
			}

			if ( rank == 2 )  // angle et échelle x ?
			{
				this.Angle = Point.ComputeAngleDeg(center, pos);
				pos = Transform.RotatePointDeg(center, -this.angle, pos);
				this.Sx = (pos.X-center.X)/bbox.Width*2;
			}

			base.SetHandlePosition(obj, rank, pos);
		}


		// Calcule l'extrémité gauche ou droite de la flèche.
		protected Point ComputeExtremity(Point p1, Point p2, double para, double perp, int rank)
		{
			double distPara = Point.Distance(p1, p2)*para;
			double distPerp = Point.Distance(p1, p2)*perp;
			Point c = Point.Move(p2, p1, distPara);
			Point p = Point.Move(c, Point.Symmetry(p2, p1), distPerp);
			double angle = (rank==0) ? 90 : -90;
			return Transform.RotatePointDeg(c, angle, p);
		}

		// Dessine les traits de construction avant les poignées.
		public override void DrawEdit(Graphics graphics, DrawingContext drawingContext, Objects.Abstract obj)
		{
			if ( !obj.IsSelected ||
				 obj.IsGlobalSelected ||
				 obj.IsEdited ||
				 !this.IsHandleVisible(obj, 0) )  return;

			Rectangle bbox = this.BoundingBoxHandlesGradient(obj);
			Point center = this.GetHandlePosition(obj, 0);
			double dx = this.sx*bbox.Width/2;
			double dy = this.sy*bbox.Height/2;
			Point p1 = center + Transform.RotatePointDeg(this.angle, new Point( dx,  dy));
			Point p2 = center + Transform.RotatePointDeg(this.angle, new Point(-dx,  dy));
			Point p3 = center + Transform.RotatePointDeg(this.angle, new Point(-dx, -dy));
			Point p4 = center + Transform.RotatePointDeg(this.angle, new Point( dx, -dy));
			Point p5 = center + Transform.RotatePointDeg(this.angle, new Point(  0, -dy));
			Point p6 = center + Transform.RotatePointDeg(this.angle, new Point(  0,  dy));
			Point p7 = center + Transform.RotatePointDeg(this.angle, new Point(-dx,   0));
			Point p8 = center + Transform.RotatePointDeg(this.angle, new Point( dx,   0));

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/drawingContext.ScaleX;

			if ( this.fillType == GradientFillType.Linear )
			{
				graphics.AddLine(p5, p6);
				graphics.AddLine(p6, this.ComputeExtremity(p5, p6, 0.2, 0.1, 0));
				graphics.AddLine(p6, this.ComputeExtremity(p5, p6, 0.2, 0.1, 1));  // flèche

				graphics.AddLine(this.ComputeExtremity(p5, p6, 0.0, 0.2, 0), this.ComputeExtremity(p5, p6, 0.0, 0.2, 1));
				graphics.AddLine(this.ComputeExtremity(p6, p5, 0.0, 0.2, 0), this.ComputeExtremity(p6, p5, 0.0, 0.2, 1));
			}
			else
			{
				graphics.AddLine(p1, p2);
				graphics.AddLine(p2, p3);
				graphics.AddLine(p3, p4);
				graphics.AddLine(p4, p1);  // rectangle

				graphics.AddLine(p5, p6);
				graphics.AddLine(p6, this.ComputeExtremity(p5, p6, 0.2, 0.1, 0));
				graphics.AddLine(p6, this.ComputeExtremity(p5, p6, 0.2, 0.1, 1));  // flèche

				graphics.AddLine(p7, p8);  // horizontale
			}

			graphics.RenderSolid(Drawing.Color.FromBrightness(0.6));

			graphics.LineWidth = initialWidth;
		}


		// Définition de la couleur pour l'impression.
		public bool PaintColor(Printing.PrintPort port, DrawingContext drawingContext)
		{
			if ( !this.color1.IsOpaque )  return false;

			port.Color = this.color1;
			return true;
		}

		
		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("FillType", this.fillType);
			info.AddValue("Color1", this.color1);
			if ( this.fillType != GradientFillType.None )
			{
				info.AddValue("Color2", this.color2);
				info.AddValue("Angle", this.angle);
				info.AddValue("Cx", this.cx);
				info.AddValue("Cy", this.cy);
				info.AddValue("Sx", this.sx);
				info.AddValue("Sy", this.sy);
				info.AddValue("Repeat", this.repeat);
				info.AddValue("Middle", this.middle);
			}
			info.AddValue("Smooth", this.smooth);
		}

		// Constructeur qui désérialise la propriété.
		protected Gradient(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fillType = (GradientFillType) info.GetValue("FillType", typeof(GradientFillType));
			this.color1 = (Drawing.Color) info.GetValue("Color1", typeof(Drawing.Color));
			if ( this.fillType != GradientFillType.None )
			{
				this.color2 = (Drawing.Color) info.GetValue("Color2", typeof(Drawing.Color));
				this.angle  = info.GetDouble("Angle");
				this.cx     = info.GetDouble("Cx");
				this.cy     = info.GetDouble("Cy");
				this.sx     = info.GetDouble("Sx");
				this.sy     = info.GetDouble("Sy");
				this.repeat = info.GetInt32("Repeat");
				this.middle = info.GetDouble("Middle");
			}
			this.smooth = info.GetDouble("Smooth");
		}
		#endregion

	
		protected GradientFillType		fillType;
		protected Drawing.Color			color1;
		protected Drawing.Color			color2;
		protected double				angle;
		protected double				cx;
		protected double				cy;
		protected double				sx;
		protected double				sy;
		protected int					repeat;
		protected double				middle;
		protected double				smooth;
	}
}
