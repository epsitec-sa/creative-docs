using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public enum GradientFill
	{
		None,
		Linear,
		Circle,
		Diamond,
		Conic,
	}

	/// <summary>
	/// La classe PropertyGradient représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyGradient : AbstractProperty
	{
		public PropertyGradient()
		{
			this.fill   = GradientFill.None;
			this.color1 = Drawing.Color.FromBrightness(1.0);
			this.color2 = Drawing.Color.FromBrightness(0.5);
			this.angle  = 0.0;
			this.cx     = 0.5;
			this.cy     = 0.5;
			this.sx     = 1.0;
			this.sy     = 1.0;
			this.repeat = 1;
			this.middle = 0.0;
			this.smooth = 0.0;

			for ( int i=0 ; i<3 ; i++ )
			{
				Handle handle = new Handle();
				handle.Type = HandleType.Property;
				this.handles.Add(handle);
			}
		}

		// Mode de remplissage du dégradé.
		[XmlAttribute]
		public GradientFill Fill
		{
			get { return this.fill; }
			set { this.fill = value; }
		}

		// Couleur 1 du dégradé.
		public Drawing.Color Color1
		{
			get { return this.color1; }
			set { this.color1 = value; }
		}

		// Couleur 2 du dégradé.
		public Drawing.Color Color2
		{
			get { return this.color2; }
			set { this.color2 = value; }
		}

		// Angle du dégradé.
		[XmlAttribute]
		public double Angle
		{
			get { return this.angle; }

			set
			{
				value = System.Math.Max(value, -360);
				value = System.Math.Min(value,  360);
				this.angle = value;
			}
		}

		// Centre x du dégradé.
		[XmlAttribute]
		public double Cx
		{
			get { return this.cx; }
			set { this.cx = value; }
		}

		// Centre y du dégradé.
		[XmlAttribute]
		public double Cy
		{
			get { return this.cy; }
			set { this.cy = value; }
		}

		// Echelle x du dégradé.
		[XmlAttribute]
		public double Sx
		{
			get { return this.sx; }
			set { this.sx = value; }
		}

		// Echelle y du dégradé.
		[XmlAttribute]
		public double Sy
		{
			get { return this.sy; }
			set { this.sy = value; }
		}

		// Nombre de répétitions.
		[XmlAttribute]
		public int Repeat
		{
			get { return this.repeat; }

			set
			{
				value = System.Math.Max(value, 1);
				value = System.Math.Min(value, 8);
				this.repeat = value;
			}
		}

		// Point milieu des couleurs.
		[XmlAttribute]
		public double Middle
		{
			get { return this.middle; }

			set
			{
				value = System.Math.Max(value, -1.0);
				value = System.Math.Min(value,  1.0);
				this.middle = value;
			}
		}

		// Rayon du flou.
		[XmlAttribute]
		public double Smooth
		{
			get { return this.smooth; }

			set
			{
				value = System.Math.Max(value,  0.0);
				value = System.Math.Min(value, 10.0);
				this.smooth = value;
			}
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			get { return true; }  // (*)
		}
		// (*)	Doit rendre "true" à cause de BoundingBox, lorsque this.fill passe
		//		de GradientFill.None à autre chose, et inversément.

		// Indique si le dégradé est visible.
		public bool IsVisible()
		{
			if ( this.fill == GradientFill.None )
			{
				return ( this.color1.A > 0 );
			}
			else
			{
				return ( this.color1.A > 0 || this.color2.A > 0 );
			}
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyGradient p = property as PropertyGradient;
			p.Fill   = this.fill;
			p.Color1 = this.color1;
			p.Color2 = this.color2;
			p.Angle  = this.angle;
			p.Cx     = this.cx;
			p.Cy     = this.cy;
			p.Sx     = this.sx;
			p.Sy     = this.sy;
			p.Repeat = this.repeat;
			p.Middle = this.middle;
			p.Smooth = this.smooth;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyGradient p = property as PropertyGradient;
			if ( p.Fill   != this.fill   )  return false;
			if ( p.Color1 != this.color1 )  return false;
			if ( p.Color2 != this.color2 )  return false;
			if ( p.Angle  != this.angle  )  return false;
			if ( p.Cx     != this.cx     )  return false;
			if ( p.Cy     != this.cy     )  return false;
			if ( p.Sx     != this.sx     )  return false;
			if ( p.Sy     != this.sy     )  return false;
			if ( p.Repeat != this.repeat )  return false;
			if ( p.Middle != this.middle )  return false;
			if ( p.Smooth != this.smooth )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelGradient();
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
				Drawing.Point p = Drawing.Transform.RotatePoint(System.Math.PI/4, new Drawing.Point(progress*System.Math.Sqrt(2), delta));
				progress = p.Y;
			}
			return progress;
		}

		// Effectue le rendu du chemin courant avec le dégradé.
		public void Render(Drawing.Graphics graphics, IconContext iconContext,
						   Drawing.Path path, Drawing.Rectangle bbox)
		{
			if ( bbox.IsSurfaceZero )  return;

			Drawing.Graphics mask = null;

			if ( this.smooth > 0 )  // flou ?
			{
				double sx = 1;
				double sy = 1;

				if ( iconContext != null )
				{
					sx = iconContext.ScaleX;
					sy = iconContext.ScaleY;
				}

				mask = graphics.CreateAlphaMask();

				int step = (int)(this.smooth*sx);
				if ( step <  0 )  step =  2;
				if ( step > 20 )  step = 20;
				for ( int i=0 ; i<step ; i++ )
				{
					double width = this.smooth-i*this.smooth/step;
					mask.Rasterizer.AddOutline(path, width*2, Drawing.CapStyle.Round, Drawing.JoinStyle.Round);
					double intensity = (i+1.0)/step;
					mask.RenderSolid(Drawing.Color.FromBrightness(intensity));
				}
				mask.Rasterizer.AddSurface(path);
				mask.RenderSolid(Drawing.Color.FromBrightness(1));

				graphics.SolidRenderer.SetAlphaMask(mask.Pixmap, Drawing.MaskComponent.R);

				bbox.Inflate(this.smooth, this.smooth);
				graphics.AddFilledRectangle(bbox);
			}
			else
			{
				graphics.Rasterizer.AddSurface(path);
			}

			if ( this.fill == GradientFill.None )  // uniforme ?
			{
				Drawing.Color c1 = this.color1;
				if ( iconContext != null )
				{
					c1 = iconContext.AdaptColor(c1);
				}

				graphics.RenderSolid(c1);
			}
			else	// dégradé ?
			{
				Drawing.Color c1 = this.color1;
				Drawing.Color c2 = this.color2;
				if ( iconContext != null )
				{
					c1 = iconContext.AdaptColor(c1);
					c2 = iconContext.AdaptColor(c2);
				}

				graphics.Rasterizer.FillMode = Drawing.FillMode.NonZero;

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

				Drawing.Transform ot = graphics.GradientRenderer.Transform;
				Drawing.Transform t = new Drawing.Transform();

				if ( this.fill == GradientFill.Linear )
				{
#if false
					graphics.GradientRenderer.Fill = Drawing.GradientFill.X;
					Drawing.Point center = new Drawing.Point((bbox.Left+bbox.Right)/2, (bbox.Bottom+bbox.Top)/2);
					double a = ((this.angle-90.0)*System.Math.PI/180);  // en radians
					a = System.Math.Sin(a);
					a = System.Math.Abs(a);
					a = System.Math.Asin(a);
					Drawing.Point p1 = Drawing.Transform.RotatePoint(center, a, new Drawing.Point(bbox.Right, center.Y));
					Drawing.Point p2 = Drawing.Point.Projection(center, p1, new Drawing.Point(bbox.Right, bbox.Top));
					double len = Drawing.Point.Distance(center, p2)*2;
					graphics.GradientRenderer.SetParameters(0, len);
					t.Translate(-len/2, 0);
					t.Rotate(this.angle-90.0);
					t.Scale(this.sx, this.sy);
					t.Translate(center);
#else
					graphics.GradientRenderer.Fill = Drawing.GradientFill.Y;
					Drawing.Point center = new Drawing.Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
					graphics.GradientRenderer.SetParameters(-100, 100);
					t.Scale(bbox.Width/100/2*this.sx, bbox.Height/100/2*this.sy);
					t.Translate(center);
					t.Rotate(this.angle+180, center);
#endif
				}
				else if ( this.fill == GradientFill.Circle )
				{
					graphics.GradientRenderer.Fill = Drawing.GradientFill.Circle;
					Drawing.Point center = new Drawing.Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, 100);
					t.Scale(bbox.Width/100/2*this.sx, bbox.Height/100/2*this.sy);
					t.Translate(center);
					t.Rotate(this.angle, center);
				}
				else if ( this.fill == GradientFill.Diamond )
				{
					graphics.GradientRenderer.Fill = Drawing.GradientFill.Diamond;
					Drawing.Point center = new Drawing.Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, 100);
					t.Scale(bbox.Width/100/2*this.sx, bbox.Height/100/2*this.sy);
					t.Translate(center);
					t.Rotate(this.angle, center);
				}
				else if ( this.fill == GradientFill.Conic )
				{
					graphics.GradientRenderer.Fill = Drawing.GradientFill.Conic;
					Drawing.Point center = new Drawing.Point(bbox.Left+bbox.Width*this.cx, bbox.Bottom+bbox.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, 250);
					t.Scale(bbox.Width/100/2*this.sx, bbox.Height/100/2*this.sy);
					t.Translate(center);
					t.Rotate(this.angle-90.0, center);
				}

				graphics.GradientRenderer.Transform = t;
				graphics.RenderGradient();
				graphics.GradientRenderer.Transform = ot;
			}

			if ( this.smooth > 0 )  // flou ?
			{
				graphics.SolidRenderer.SetAlphaMask(null, Drawing.MaskComponent.None);
				mask.Dispose();
			}
		}


		// Calcule la bbox pour la représentation du dégradé.
		public Drawing.Rectangle BoundingBoxGeom(Drawing.Rectangle bbox)
		{
			bbox.Inflate(this.smooth);
			return bbox;
		}

		// Calcule la bbox pour la représentation du dégradé.
		public Drawing.Rectangle BoundingBoxFull(Drawing.Rectangle bbox)
		{
			if ( this.fill != GradientFill.None )
			{
				Drawing.Point center = this.Handle(0, bbox).Position;
				double dx = this.sx*bbox.Width/2;
				double dy = this.sy*bbox.Height/2;
				Drawing.Point p1 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point( dx,  dy));
				Drawing.Point p2 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point(-dx,  dy));
				Drawing.Point p3 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point(-dx, -dy));
				Drawing.Point p4 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point( dx, -dy));

				bbox.MergeWith(p1);
				bbox.MergeWith(p2);
				bbox.MergeWith(p3);
				bbox.MergeWith(p4);
			}
			return bbox;
		}


		// Nombre de poignées.
		public override int TotalHandle
		{
			get
			{
				//return this.IsHandleVisible() ? this.handles.Count : 0;
				if ( !this.IsHandleVisible() )  return 0;
				if ( this.fill == GradientFill.Linear )  return 2;
				return 3;
			}
		}

		// Donne une poignée de la propriété.
		public override Handle Handle(int rank, Drawing.Rectangle bbox)
		{
			Handle handle = this.handles[rank] as Handle;

			Drawing.Point center = new Drawing.Point();
			center.X = bbox.Left+bbox.Width*this.cx;
			center.Y = bbox.Bottom+bbox.Height*this.cy;

			if ( rank == 0 )  // centre ?
			{
				handle.Position = center;
			}

			if ( rank == 1 )  // angle et échelle y ?
			{
				Drawing.Point p = center;
				p.Y += this.sy*bbox.Height/2;
				p = Drawing.Transform.RotatePoint(center, this.angle*System.Math.PI/180, p);
				handle.Position = p;
			}

			if ( rank == 2 )  // angle et échelle x ?
			{
				Drawing.Point p = center;
				p.X += this.sx*bbox.Width/2;
				p = Drawing.Transform.RotatePoint(center, this.angle*System.Math.PI/180, p);
				handle.Position = p;
			}

			return handle;
		}

		// Début du déplacement d'une poignée de la propriété.
		public override void MoveHandleStarting(int rank, Drawing.Point pos, Drawing.Rectangle bbox, IconContext iconContext)
		{
			pos = this.Handle(0, bbox).Position;
			iconContext.ConstrainFixStarting(pos);
		}

		// Déplace une poignée de la propriété.
		public override void MoveHandleProcess(int rank, Drawing.Point pos, Drawing.Rectangle bbox, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);

			if ( rank == 0 )  // centre ?
			{
				this.cx = (pos.X-bbox.Left)/bbox.Width;
				this.cy = (pos.Y-bbox.Bottom)/bbox.Height;
			}

			if ( rank == 1 )  // angle et échelle y ?
			{
				Drawing.Point center = this.Handle(0, bbox).Position;
				double a = Drawing.Point.ComputeAngle(center, pos)-System.Math.PI/2;
				this.angle = a*180/System.Math.PI;  // en degrés
				pos = Drawing.Transform.RotatePoint(center, -this.angle*System.Math.PI/180, pos);
				this.sy = (pos.Y-center.Y)/bbox.Height*2;
			}

			if ( rank == 2 )  // angle et échelle x ?
			{
				Drawing.Point center = this.Handle(0, bbox).Position;
				double a = Drawing.Point.ComputeAngle(center, pos);
				this.angle = a*180/System.Math.PI;  // en degrés
				pos = Drawing.Transform.RotatePoint(center, -this.angle*System.Math.PI/180, pos);
				this.sx = (pos.X-center.X)/bbox.Width*2;
			}
		}

		// Indique si les poignées sont visibles.
		public override bool IsHandleVisible()
		{
			if ( !this.extendedSize )  return false;
			if ( !this.editProperties )  return false;
			if ( this.fill == GradientFill.None )  return false;
			return true;
		}

		// Calcule l'extrémité gauche ou droite de la flèche.
		protected Drawing.Point ComputeExtremity(Drawing.Point p1, Drawing.Point p2, double para, double perp, int rank)
		{
			double distPara = Drawing.Point.Distance(p1, p2)*para;
			double distPerp = Drawing.Point.Distance(p1, p2)*perp;
			Drawing.Point c = Drawing.Point.Move(p2, p1, distPara);
			Drawing.Point p = Drawing.Point.Move(c, Drawing.Point.Symmetry(p2, p1), distPerp);
			double angle = (rank==0) ? System.Math.PI/2 : -System.Math.PI/2;
			return Drawing.Transform.RotatePoint(c, angle, p);
		}

		// Dessine les traits de construction avant les poignées.
		public override void DrawEdit(Drawing.Graphics graphics, IconContext iconContext, Drawing.Rectangle bbox)
		{
			if ( !this.IsHandleVisible() )  return;

			Drawing.Point center = this.Handle(0, bbox).Position;
			double dx = this.sx*bbox.Width/2;
			double dy = this.sy*bbox.Height/2;
			Drawing.Point p1 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point( dx,  dy));
			Drawing.Point p2 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point(-dx,  dy));
			Drawing.Point p3 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point(-dx, -dy));
			Drawing.Point p4 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point( dx, -dy));
			Drawing.Point p5 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point(  0, -dy));
			Drawing.Point p6 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point(  0,  dy));
			Drawing.Point p7 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point(-dx,   0));
			Drawing.Point p8 = center + Drawing.Transform.RotatePoint(this.angle*System.Math.PI/180, new Drawing.Point( dx,   0));

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/iconContext.ScaleX;

			if ( this.fill == GradientFill.Linear )
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


		protected GradientFill			fill;
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
