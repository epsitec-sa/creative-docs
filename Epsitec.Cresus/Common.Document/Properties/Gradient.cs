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
		Hatch   = 5,
		Dots    = 6,
		Squares = 7,
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
			else if ( this.type == Type.FillGradientVT )
			{
				this.color1 = Drawing.Color.FromARGB(0.95, 0.8, 0.7, 0.7);
				this.color2 = Drawing.Color.FromARGB(0.95, 0.6, 0.5, 0.5);
			}
			else if ( this.type == Type.FillGradientVL )
			{
				this.color1 = Drawing.Color.FromARGB(0.95, 0.7, 0.8, 0.7);
				this.color2 = Drawing.Color.FromARGB(0.95, 0.5, 0.6, 0.5);
			}
			else if ( this.type == Type.FillGradientVR )
			{
				this.color1 = Drawing.Color.FromARGB(0.95, 0.7, 0.7, 0.8);
				this.color2 = Drawing.Color.FromARGB(0.95, 0.5, 0.5, 0.6);
			}
			else
			{
				this.color1 = Drawing.Color.FromBrightness(0.8);
				this.color2 = Drawing.Color.FromBrightness(0.5);
			}
			this.angle  = 0.0;
			this.cx     = 0.5;
			this.cy     = 0.5;
			this.sx     = 0.5;
			this.sy     = 0.5;
			this.repeat = 1;
			this.middle = 0.0;
			this.smooth = 0.0;

			this.hatchAngle    = new double[Gradient.HatchMax];
			this.hatchWidth    = new double[Gradient.HatchMax];
			this.hatchDistance = new double[Gradient.HatchMax];

			this.hatchAngle[0] = 45.0;
			this.hatchAngle[1] =  0.0;

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.hatchWidth[0]    = 1.0;
				this.hatchDistance[0] = 5.0;
				this.hatchDistance[1] = 5.0;
			}
			else
			{
				this.hatchWidth[0]    =  5.0;  // 0.5mm
				this.hatchDistance[0] = 50.0;  // 5.0mm
				this.hatchDistance[1] = 50.0;  // 5.0mm
			}

			this.hatchWidth[1] = 0.0;
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
				value = System.Math.Max(value, -5.0);
				value = System.Math.Min(value,  5.0);
				
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

		// Angle des hachures.
		public double HatchAngle1
		{
			get
			{
				return this.hatchAngle[0];
			}

			set
			{
				if ( this.hatchAngle[0] != value )
				{
					this.NotifyBefore();
					this.hatchAngle[0] = value;
					this.NotifyAfter();
				}
			}
		}

		public double HatchAngle2
		{
			get
			{
				return this.hatchAngle[1];
			}

			set
			{
				if ( this.hatchAngle[1] != value )
				{
					this.NotifyBefore();
					this.hatchAngle[1] = value;
					this.NotifyAfter();
				}
			}
		}

		// Epaisseur des hachures.
		public double HatchWidth1
		{
			get
			{
				return this.hatchWidth[0];
			}

			set
			{
				if ( this.hatchWidth[0] != value )
				{
					this.NotifyBefore();
					this.hatchWidth[0] = value;
					this.NotifyAfter();
				}
			}
		}

		public double HatchWidth2
		{
			get
			{
				return this.hatchWidth[1];
			}

			set
			{
				if ( this.hatchWidth[1] != value )
				{
					this.NotifyBefore();
					this.hatchWidth[1] = value;
					this.NotifyAfter();
				}
			}
		}

		// Distance des hachures.
		public double HatchDistance1
		{
			get
			{
				return this.hatchDistance[0];
			}

			set
			{
				if ( this.hatchDistance[0] != value )
				{
					this.NotifyBefore();
					this.hatchDistance[0] = value;
					this.NotifyAfter();
				}
			}
		}

		public double HatchDistance2
		{
			get
			{
				return this.hatchDistance[1];
			}

			set
			{
				if ( this.hatchDistance[1] != value )
				{
					this.NotifyBefore();
					this.hatchDistance[1] = value;
					this.NotifyAfter();
				}
			}
		}

		public double GetHatchAngle(int rank)
		{
			return this.hatchAngle[rank];
		}

		public void SetHatchAngle(int rank, double value)
		{
			if ( this.hatchAngle[rank] != value )
			{
				this.NotifyBefore();
				this.hatchAngle[rank] = value;
				this.NotifyAfter();
			}
		}

		public double GetHatchWidth(int rank)
		{
			return this.hatchWidth[rank];
		}

		public void SetHatchWidth(int rank, double value)
		{
			if ( this.hatchWidth[rank] != value )
			{
				this.NotifyBefore();
				this.hatchWidth[rank] = value;
				this.NotifyAfter();
			}
		}

		public double GetHatchDistance(int rank)
		{
			return this.hatchDistance[rank];
		}

		public void SetHatchDistance(int rank, double value)
		{
			if ( this.hatchDistance[rank] != value )
			{
				this.NotifyBefore();
				this.hatchDistance[rank] = value;
				this.NotifyAfter();
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

			for ( int i=0 ; i<Gradient.HatchMax ; i++ )
			{
				p.hatchAngle[i]    = this.hatchAngle[i];
				p.hatchWidth[i]    = this.hatchWidth[i];
				p.hatchDistance[i] = this.hatchDistance[i];
			}
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


			for ( int i=0 ; i<Gradient.HatchMax ; i++ )
			{
				if ( p.hatchAngle[i]    != this.hatchAngle[i]    )  return false;
				if ( p.hatchWidth[i]    != this.hatchWidth[i]    )  return false;
				if ( p.hatchDistance[i] != this.hatchDistance[i] )  return false;
			}

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
				if ( this.middle > 0.0 )
				{
					progress = 1.0-System.Math.Pow(1.0-progress, 1.0+this.middle);
				}
				else
				{
					progress = System.Math.Pow(progress, 1.0-this.middle);
				}
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

			if ( this.fillType == GradientFillType.Hatch )
			{
				this.RenderHatch(graphics, drawingContext, path, bbox);
				return;
			}
			if ( this.fillType == GradientFillType.Dots )
			{
				this.RenderDots(graphics, drawingContext, path, bbox);
				return;
			}
			if ( this.fillType == GradientFillType.Squares )
			{
				this.RenderSquares(graphics, drawingContext, path, bbox);
				return;
			}

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

			bool flat;
			if ( this.fillType == GradientFillType.None )
			{
				flat = true;
			}
			else if ( this.fillType == GradientFillType.Linear )
			{
				flat = ( System.Math.Abs(this.sx) < 0.01 &&
						 System.Math.Abs(this.sy) < 0.01 );
			}
			else
			{
				flat = ( System.Math.Abs(this.sx) < 0.01 ||
						 System.Math.Abs(this.sy) < 0.01 );
			}

			if ( flat )  // uniforme ?
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

				graphics.FillMode = FillMode.NonZero;

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

				Point center = new Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
				Point corner = center+new Size(bbox.Width*this.sx, bbox.Height*this.sy);

				if ( this.fillType == GradientFillType.Linear )
				{
					double a = Point.ComputeAngleDeg(center, corner)-90;
					double d = Point.Distance(center, corner);
					graphics.GradientRenderer.Fill = GradientFill.Y;
					graphics.GradientRenderer.SetParameters(-255, 255);
					t.RotateDeg(a);
					t.Scale(1.0, d/255);
					t.Translate(center);
				}
				else if ( this.fillType == GradientFillType.Circle )
				{
					graphics.GradientRenderer.Fill = GradientFill.Circle;
					graphics.GradientRenderer.SetParameters(0, 255);
					t.RotateDeg(this.angle);
					t.Scale(bbox.Width/255*this.sx, bbox.Height/255*this.sy);
					t.Translate(center);
				}
				else if ( this.fillType == GradientFillType.Diamond )
				{
					graphics.GradientRenderer.Fill = GradientFill.Diamond;
					graphics.GradientRenderer.SetParameters(0, 255);
					t.RotateDeg(this.angle);
					t.Scale(bbox.Width/255*this.sx, bbox.Height/255*this.sy);
					t.Translate(center);
				}
				else if ( this.fillType == GradientFillType.Conic )
				{
					graphics.GradientRenderer.Fill = GradientFill.Conic;
					graphics.GradientRenderer.SetParameters(0, 250);
					t.RotateDeg(this.angle-90.0);
					t.Scale(bbox.Width/255*this.sx, bbox.Height/255*this.sy);
					t.Translate(center);
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

		// Effectue le rendu d'une zone quelconque hachurée.
		public void RenderHatch(Graphics graphics, DrawingContext drawingContext, Path path, Rectangle bbox)
		{
			Drawing.Color initialColor = graphics.Color;

			graphics.Color = this.color1;
			graphics.PaintSurface(path);  // dessine le fond

			for ( int i=0 ; i<Gradient.HatchMax ; i++ )
			{
				if ( this.hatchWidth[i] == 0.0 || this.hatchDistance[i] == 0.0 )  continue;

				Point p1, p2, p3, p4;
				double offset;
				this.MinRectMotif(bbox, i, out p1, out p2, out p3, out p4, out offset);
				double width = System.Math.Min(this.hatchWidth[i], this.hatchDistance[i]);

				Path pathLines = new Path();
				double len = Point.Distance(p1, p2);
				double pos = offset;
				while ( pos < len )
				{
					Point pp1 = Point.Move(p1, p2, pos);
					Point pp2 = Point.Move(p1, p2, pos+width);
					Point pp3 = Point.Move(p3, p4, pos);
					Point pp4 = Point.Move(p3, p4, pos+width);

					pathLines.MoveTo(pp1);
					pathLines.LineTo(pp2);
					pathLines.LineTo(pp4);
					pathLines.LineTo(pp3);
					pathLines.Close();

					pos += this.hatchDistance[i];
				}

				pathLines = Path.Combine(pathLines, path, PathOperation.And);

				graphics.Color = this.color2;
				graphics.PaintSurface(pathLines);
			}

			graphics.Color = initialColor;
		}

		// Effectue le rendu d'une zone quelconque de points.
		public void RenderDots(Graphics graphics, DrawingContext drawingContext, Path path, Rectangle bbox)
		{
			Drawing.Color initialColor = graphics.Color;

			graphics.Color = this.color1;
			graphics.PaintSurface(path);  // dessine le fond

			for ( int i=0 ; i<Gradient.HatchMax ; i++ )
			{
				if ( this.hatchWidth[i] == 0.0 || this.hatchDistance[i] == 0.0 )  continue;

				Point p1, p2, p3, p4;
				double offset;
				this.MinRectMotif(bbox, i, out p1, out p2, out p3, out p4, out offset);
				double width = System.Math.Min(this.hatchWidth[i], this.hatchDistance[i]/2);

				Path pathLines = new Path();
				double lenh = Point.Distance(p1, p2);
				double posh = offset;
				while ( posh < lenh )
				{
					Point pp1 = Point.Move(p1, p2, posh);
					Point pp3 = Point.Move(p3, p4, posh);

					double lenv = Point.Distance(p1, p3);
					double posv = 0.0;
					while ( posv < lenv )
					{
						Point center = Point.Move(pp1, pp3, posv);
						pathLines.AppendCircle(center, width);

						posv += this.hatchDistance[i];
					}

					posh += this.hatchDistance[i];
				}

				if ( !drawingContext.PreviewActive )
				{
					Path pathApprox = new Path();
					pathApprox.Append(pathLines, 0.01, 0.0);
					pathLines = pathApprox;
				}

				pathLines = Path.Combine(pathLines, path, PathOperation.And);

				graphics.Color = this.color2;
				graphics.PaintSurface(pathLines);
			}

			graphics.Color = initialColor;
		}

		// Effectue le rendu d'une zone quelconque de carrés.
		public void RenderSquares(Graphics graphics, DrawingContext drawingContext, Path path, Rectangle bbox)
		{
			Drawing.Color initialColor = graphics.Color;

			graphics.Color = this.color1;
			graphics.PaintSurface(path);  // dessine le fond

			for ( int i=0 ; i<Gradient.HatchMax ; i++ )
			{
				if ( this.hatchWidth[i] == 0.0 || this.hatchDistance[i] == 0.0 )  continue;

				Point p1, p2, p3, p4;
				double offset;
				this.MinRectMotif(bbox, i, out p1, out p2, out p3, out p4, out offset);
				double width = System.Math.Min(this.hatchWidth[i], this.hatchDistance[i]/2);

				Path pathLines = new Path();
				double lenh = Point.Distance(p1, p2);
				double posh = offset;
				while ( posh < lenh )
				{
					Point pp1 = Point.Move(p1, p2, posh);
					Point pp3 = Point.Move(p3, p4, posh);

					double lenv = Point.Distance(p1, p3);
					double posv = 0.0;
					while ( posv < lenv )
					{
						Point center = Point.Move(pp1, pp3, posv);
						center.X -= width;
						center.Y -= width;
						pathLines.AppendRectangle(center, new Size(width*2, width*2));

						posv += this.hatchDistance[i];
					}

					posh += this.hatchDistance[i];
				}

				pathLines = Path.Combine(pathLines, path, PathOperation.And);

				graphics.Color = this.color2;
				graphics.PaintSurface(pathLines);
			}

			graphics.Color = initialColor;
		}

		// Calcule le rectangle le plus petit possible qui sera rempli par le motif.
		// L'offset permet à deux objets proches d'avoir des hachures jointives.
		protected void MinRectMotif(Rectangle bbox, int i, out Point p1, out Point p2, out Point p3, out Point p4, out double offset)
		{
			double b = Math.ClipAngleDeg(this.hatchAngle[i]);
			double a = Math.DegToRad(b%90.0);

			double ha = System.Math.Cos(a)*bbox.Width;
			double hx = System.Math.Cos(a)*ha;
			double hy = System.Math.Sin(a)*ha;

			double va = System.Math.Cos(a)*bbox.Height;
			double vx = System.Math.Cos(a)*va;
			double vy = System.Math.Sin(a)*va;

			Point[] rect = new Point[4];
			rect[0] = new Point();
			rect[1] = new Point();
			rect[2] = new Point();
			rect[3] = new Point();

			rect[0].X = bbox.Right-hx;
			rect[0].Y = bbox.Bottom-hy;

			rect[1].X = bbox.Right+vy;
			rect[1].Y = bbox.Top-vx;

			rect[2].X = bbox.Left+hx;
			rect[2].Y = bbox.Top+hy;

			rect[3].X = bbox.Left-vy;
			rect[3].Y = bbox.Bottom+vx;

			int j = ((int) (b/90.0)) % 4;
			p1 = rect[(j+0)%4];
			p2 = rect[(j+1)%4];
			p4 = rect[(j+2)%4];
			p3 = rect[(j+3)%4];

			Point pp1 = Transform.RotatePointDeg(-this.hatchAngle[i], p1);
			offset = -(pp1.X+100000.0)%this.hatchDistance[i];
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

			Point center = new Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
			Point p1 = center+new Size( bbox.Width*this.sx,  bbox.Height*this.sy);
			Point p2 = center+new Size(-bbox.Width*this.sx,  bbox.Height*this.sy);
			Point p3 = center+new Size(-bbox.Width*this.sx, -bbox.Height*this.sy);
			Point p4 = center+new Size( bbox.Width*this.sx, -bbox.Height*this.sy);

			if ( this.fillType == GradientFillType.Linear )
			{
				bboxFull.MergeWith(p1);
				bboxFull.MergeWith(p3);
				bboxFull.MergeWith(this.ComputeExtremity(p3, p1, 0.0, 0.2, 0));
				bboxFull.MergeWith(this.ComputeExtremity(p3, p1, 0.0, 0.2, 1));
				bboxFull.MergeWith(this.ComputeExtremity(p1, p3, 0.0, 0.2, 0));
				bboxFull.MergeWith(this.ComputeExtremity(p1, p3, 0.0, 0.2, 1));
			}
			else
			{
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
			else if ( this.fillType == GradientFillType.Conic )
			{
				return (rank < 3);
			}
			else if ( this.fillType == GradientFillType.Hatch   ||
					  this.fillType == GradientFillType.Dots    ||
					  this.fillType == GradientFillType.Squares )
			{
				return (rank < 3);
			}
			else
			{
				return (rank < 2);
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
			Point corner = new Point();
			Rectangle bbox = this.BoundingBoxHandlesGradient(obj);
			center.X = bbox.Left+bbox.Width*this.cx;
			center.Y = bbox.Bottom+bbox.Height*this.cy;
			corner.X = center.X+bbox.Width*this.sx;
			corner.Y = center.Y+bbox.Height*this.sy;

			if ( this.fillType == GradientFillType.Hatch   ||
				 this.fillType == GradientFillType.Dots    ||
				 this.fillType == GradientFillType.Squares )
			{
				if ( rank == 0 )  // angle ?
				{
					double radius = System.Math.Min(bbox.Width, bbox.Height)*0.45;
					pos = bbox.Center+Transform.RotatePointDeg(this.hatchAngle[0], new Point(0, radius));
				}

				if ( rank == 1 )  // épaisseur ?
				{
					double radius = this.hatchWidth[0];
					pos = bbox.Center+Transform.RotatePointDeg(this.hatchAngle[0]-90, new Point(0, radius));
				}

				if ( rank == 2 )  // distance ?
				{
					double radius = this.hatchDistance[0];
					pos = bbox.Center+Transform.RotatePointDeg(this.hatchAngle[0]-90, new Point(0, radius));
				}
			}
			else
			{
				if ( rank == 0 )  // centre ?
				{
					pos = center;
				}

				if ( rank == 1 )  // coin ?
				{
					pos = corner;
				}

				if ( rank == 2 )  // angle ?
				{
					double radius = System.Math.Min(System.Math.Abs(this.sx*bbox.Width), System.Math.Abs(this.sy*bbox.Height));
					pos = center+Transform.RotatePointDeg(this.angle, new Point(0, radius));
				}
			}

			return pos;
		}

		// Modifie la position d'une poignée.
		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			Rectangle bbox = this.BoundingBoxHandlesGradient(obj);

			if ( this.fillType == GradientFillType.Hatch   ||
				 this.fillType == GradientFillType.Dots    ||
				 this.fillType == GradientFillType.Squares )
			{
				if ( rank == 0 )  // angle ?
				{
					this.HatchAngle1 = Point.ComputeAngleDeg(bbox.Center, pos)-90;
				}

				if ( rank == 1 )  // épaisseur ?
				{
					this.HatchWidth1 = Point.Distance(bbox.Center, pos);
				}

				if ( rank == 2 )  // distance ?
				{
					this.HatchDistance1 = Point.Distance(bbox.Center, pos);
				}
			}
			else
			{
				if ( rank == 0 )  // centre ?
				{
					this.Cx = (pos.X-bbox.Left)/bbox.Width;
					this.Cy = (pos.Y-bbox.Bottom)/bbox.Height;
				}

				if ( rank == 1 )  // coin ?
				{
					this.Sx = (pos.X-bbox.Left-bbox.Width*this.cx)/bbox.Width;
					this.Sy = (pos.Y-bbox.Bottom-bbox.Height*this.cy)/bbox.Height;
				}

				if ( rank == 2 )  // angle ?
				{
					Point center = new Point();
					center.X = bbox.Left+bbox.Width*this.cx;
					center.Y = bbox.Bottom+bbox.Height*this.cy;
					this.Angle = Point.ComputeAngleDeg(center, pos)-90;
				}
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
			Point center = new Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
			Point p1 = center+new Size( bbox.Width*this.sx,  bbox.Height*this.sy);
			Point p2 = center+new Size(-bbox.Width*this.sx,  bbox.Height*this.sy);
			Point p3 = center+new Size(-bbox.Width*this.sx, -bbox.Height*this.sy);
			Point p4 = center+new Size( bbox.Width*this.sx, -bbox.Height*this.sy);

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/drawingContext.ScaleX;

			if ( this.fillType == GradientFillType.Linear )
			{
				graphics.AddLine(p3, p1);
				graphics.AddLine(p1, this.ComputeExtremity(p3, p1, 0.2, 0.1, 0));
				graphics.AddLine(p1, this.ComputeExtremity(p3, p1, 0.2, 0.1, 1));  // flèche

				graphics.AddLine(this.ComputeExtremity(p3, p1, 0.0, 0.2, 0), this.ComputeExtremity(p3, p1, 0.0, 0.2, 1));
				graphics.AddLine(this.ComputeExtremity(p1, p3, 0.0, 0.2, 0), this.ComputeExtremity(p1, p3, 0.0, 0.2, 1));
			}
			else if ( this.fillType == GradientFillType.Hatch   ||
					  this.fillType == GradientFillType.Dots    ||
					  this.fillType == GradientFillType.Squares )
			{
				center = bbox.Center;
				double radius = System.Math.Min(bbox.Width, bbox.Height)*0.5;
				Point pa = center+Transform.RotatePointDeg(this.hatchAngle[0], new Point(0, radius*0.9));
				graphics.AddLine(pa, this.ComputeExtremity(center, pa, 0.4, 0.2, 0));
				graphics.AddLine(pa, this.ComputeExtremity(center, pa, 0.4, 0.2, 1));  // flèche
				graphics.AddLine(center, pa);

				Point pb = center+Transform.RotatePointDeg(this.hatchAngle[0]+90, new Point(0, radius));
				Point pc = center+Transform.RotatePointDeg(this.hatchAngle[0]-90, new Point(0, radius));
				graphics.AddLine(pb, pc);
			}
			else
			{
				graphics.AddLine(p1, p2);
				graphics.AddLine(p2, p3);
				graphics.AddLine(p3, p4);
				graphics.AddLine(p4, p1);

				if ( this.fillType == GradientFillType.Circle )
				{
					graphics.AddCircle(center.X, center.Y, System.Math.Abs(this.sx*bbox.Width), System.Math.Abs(this.sy*bbox.Height));
				}

				if ( this.fillType == GradientFillType.Diamond )
				{
					graphics.AddLine(p1, p3);
					graphics.AddLine(p2, p4);
				}

				if ( this.fillType == GradientFillType.Conic )
				{
					double radius = System.Math.Min(System.Math.Abs(this.sx*bbox.Width), System.Math.Abs(this.sy*bbox.Height));
					Point pa = center+Transform.RotatePointDeg(this.angle, new Point(0, radius));
					//?graphics.AddCircle(center, radius);
					graphics.AddLine(pa, this.ComputeExtremity(center, pa, 0.4, 0.2, 0));
					graphics.AddLine(pa, this.ComputeExtremity(center, pa, 0.4, 0.2, 1));  // flèche
					graphics.AddLine(center, pa);
				}
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
			if ( this.fillType == GradientFillType.Hatch )
			{
				info.AddValue("HatchAngle", this.hatchAngle);
				info.AddValue("HatchWidth", this.hatchWidth);
				info.AddValue("HatchDistance", this.hatchDistance);
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
			if ( this.fillType == GradientFillType.Hatch )
			{
				this.hatchAngle    = (double[]) info.GetValue("HatchAngle",    typeof(double[]));
				this.hatchWidth    = (double[]) info.GetValue("HatchWidth",    typeof(double[]));
				this.hatchDistance = (double[]) info.GetValue("HatchDistance", typeof(double[]));
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
		protected double[]				hatchAngle;
		protected double[]				hatchWidth;
		protected double[]				hatchDistance;
		public static readonly int		HatchMax = 2;
	}
}
