using Epsitec.Common.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.NiceIcon
{
	/// <summary>
	/// La classe PropertyGradient représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyGradient : AbstractProperty
	{
		public PropertyGradient()
		{
			this.fill   = Drawing.GradientFill.None;
			this.color1 = Drawing.Color.FromBrightness(1.0);
			this.color2 = Drawing.Color.FromBrightness(0.6);
			this.angle  = 0.0;
			this.cx     = 0.5;
			this.cy     = 0.5;
			this.repeat = 1;
			this.middle = 0.0;
			this.range  = 0.0;
		}

		// Mode de remplissage du dégradé.
		[XmlAttribute]
		public Drawing.GradientFill Fill
		{
			get
			{
				return this.fill;
			}

			set
			{
				this.fill = value;
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
				this.color1 = value;
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
				this.color2 = value;
			}
		}

		// Angle du dégradé.
		[XmlAttribute]
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
				this.angle = value;
			}
		}

		// Centre x du dégradé.
		[XmlAttribute]
		public double Cx
		{
			get
			{
				return this.cx;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, 1);
				this.cx = value;
			}
		}

		// Centre y du dégradé.
		[XmlAttribute]
		public double Cy
		{
			get
			{
				return this.cy;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, 1);
				this.cy = value;
			}
		}

		// Nombre de répétitions.
		[XmlAttribute]
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
				this.repeat = value;
			}
		}

		// Point milieu des couleurs.
		[XmlAttribute]
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
				this.middle = value;
			}
		}

		// Etendue des couleurs.
		[XmlAttribute]
		public double Range
		{
			get
			{
				return this.range;
			}

			set
			{
				value = System.Math.Max(value, -2.0);
				value = System.Math.Min(value,  2.0);
				this.range = value;
			}
		}

		// Indique si le dégradé est visible.
		public bool IsVisible()
		{
			if ( this.fill == Drawing.GradientFill.None )
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
			p.Repeat = this.repeat;
			p.Middle = this.middle;
			p.Range  = this.range;
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
			if ( p.Repeat != this.repeat )  return false;
			if ( p.Middle != this.middle )  return false;
			if ( p.Range  != this.range  )  return false;

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
				Drawing.Point p = Math.RotatePoint(System.Math.PI/4, new Drawing.Point(progress*System.Math.Sqrt(2), delta));
				progress = p.Y;
			}
			return progress;
		}

		// Effectue le rendu du chemin courant avec le dégradé.
		public void Render(Drawing.Graphics graphics, IconContext iconContext, Drawing.Rectangle rect)
		{
			if ( this.fill == Drawing.GradientFill.None )  // uniforme ?
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
				graphics.Rasterizer.FillMode = Drawing.FillMode.NonZero;
				graphics.GradientRenderer.Fill = this.fill;

				Drawing.Color c1 = this.color1;
				Drawing.Color c2 = this.color2;
				if ( iconContext != null )
				{
					c1 = iconContext.AdaptColor(c1);
					c2 = iconContext.AdaptColor(c2);
				}

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

				double zoom = System.Math.Pow(2.0, this.range);

				if ( this.fill == Drawing.GradientFill.X )
				{
					Drawing.Point center = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
					double a = ((this.angle-90.0)*System.Math.PI/180);  // en radians
					a = System.Math.Sin(a);
					a = System.Math.Abs(a);
					a = System.Math.Asin(a);
					Drawing.Point p1 = Math.RotatePoint(center, a, new Drawing.Point(rect.Right, center.Y));
					Drawing.Point p2 = Math.Projection(center, p1, new Drawing.Point(rect.Right, rect.Top));
					double len = Math.Distance(center, p2)*2;
					graphics.GradientRenderer.SetParameters(0, len);
					t.Translate(-len/2, 0);
					t.Rotate(this.angle-90.0);
					t.Scale(zoom, zoom);
					t.Translate(center);
				}
				else if ( this.fill == Drawing.GradientFill.Circle )
				{
					Drawing.Point center = new Drawing.Point(rect.Left+rect.Width*this.cx, rect.Bottom+rect.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, 100);
					t.Scale(rect.Width/100/2*zoom, rect.Height/100/2*zoom);
					t.Translate(center);
				}
				else if ( this.fill == Drawing.GradientFill.Diamond )
				{
					Drawing.Point center = new Drawing.Point(rect.Left+rect.Width*this.cx, rect.Bottom+rect.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, 100);
					t.Scale(rect.Width/100/2*zoom, rect.Height/100/2*zoom);
					t.Translate(center);
					t.Rotate(this.angle, center);
				}
				else if ( this.fill == Drawing.GradientFill.Conic )
				{
					Drawing.Point center = new Drawing.Point(rect.Left+rect.Width*this.cx, rect.Bottom+rect.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, 250);
					t.Translate(center);
					t.Rotate(this.angle-90.0, center);
				}
				else
				{
					Drawing.Point center = new Drawing.Point(rect.Left+rect.Width*this.cx, rect.Bottom+rect.Height*this.cy);
					graphics.GradientRenderer.SetParameters(0, rect.Right-center.X);
					t.Translate(center);
					t.Rotate(this.angle-90.0, center);
				}

				graphics.GradientRenderer.Transform = t;
				graphics.RenderGradient();
				graphics.GradientRenderer.Transform = ot;
			}
		}


		[XmlAttribute]
		protected Drawing.GradientFill	fill;
		protected Drawing.Color			color1;
		protected Drawing.Color			color2;
		protected double				angle;
		protected double				cx;
		protected double				cy;
		protected int					repeat;
		protected double				middle;
		protected double				range;
	}
}
