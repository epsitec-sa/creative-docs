using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyLine représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyLine : AbstractProperty
	{
		public PropertyLine()
		{
			this.width        = 1.0;
			this.cap          = Drawing.CapStyle.Round;
			this.join         = Drawing.JoinStyle.Round;
			this.limit        = 5.0;
			this.patternId    = 0;
			this.patternSeed  = 1;
			this.patternMove  = 0.0;
			this.patternSize  = 1.0;
			this.patternShift = 0.0;
			this.patternAngle = 0.0;
			this.rndMove      = false;
			this.rndSize      = false;
			this.rndShift     = false;
			this.rndAngle     = false;
			this.rndPage      = false;

			this.dashPen = new double[PropertyLine.DashMax];
			this.dashGap = new double[PropertyLine.DashMax];
			this.dashPen[0] = 1.0;
			this.dashGap[0] = 1.0;
			for ( int i=1 ; i<PropertyLine.DashMax ; i++ )
			{
				this.dashPen[i] = 0.0;
				this.dashGap[i] = 0.0;
			}
		}

		[XmlAttribute]
		public double Width
		{
			get { return this.width; }
			set { this.width = value; }
		}

		[XmlAttribute]
		public Drawing.CapStyle Cap
		{
			get { return this.cap; }
			set { this.cap = value; }
		}

		[XmlAttribute]
		public Drawing.JoinStyle Join
		{
			get { return this.join; }
			set { this.join = value; }
		}

		[XmlAttribute]
		public double Limit
		{
			get { return this.limit; }
			set { this.limit = value; }
		}

		[XmlAttribute("PI")]
		// 0	= trait normal
		// -1	= traitillé
		// 1..n	= motif
		public int PatternId
		{
			get { return this.patternId; }
			set { this.patternId = value; }
		}

		[XmlAttribute("PS")]
		public int PatternSeed
		{
			get { return this.patternSeed; }
			set { this.patternSeed = value; }
		}

		[XmlAttribute("PX")]
		public double PatternMove
		{
			get { return this.patternMove; }
			set { this.patternMove = value; }
		}

		[XmlAttribute("PZ")]
		public double PatternSize
		{
			get { return this.patternSize; }
			set { this.patternSize = value; }
		}

		[XmlAttribute("PY")]
		public double PatternShift
		{
			get { return this.patternShift; }
			set { this.patternShift = value; }
		}

		[XmlAttribute("PA")]
		public double PatternAngle
		{
			get { return this.patternAngle; }
			set { this.patternAngle = value; }
		}

		[XmlAttribute("PRX")]
		public bool RndMove
		{
			get { return this.rndMove; }
			set { this.rndMove = value; }
		}

		[XmlAttribute("PRZ")]
		public bool RndSize
		{
			get { return this.rndSize; }
			set { this.rndSize = value; }
		}

		[XmlAttribute("PRY")]
		public bool RndShift
		{
			get { return this.rndShift; }
			set { this.rndShift = value; }
		}

		[XmlAttribute("PRA")]
		public bool RndAngle
		{
			get { return this.rndAngle; }
			set { this.rndAngle = value; }
		}

		[XmlAttribute("PRP")]
		public bool RndPage
		{
			get { return this.rndPage; }
			set { this.rndPage = value; }
		}

		[XmlIgnore]
		public Drawing.Rectangle PatternBbox
		{
			get
			{
				return this.patternBbox;
			}

			set
			{
				this.patternBbox = value;

				double left   = this.patternBbox.Left   - 5.0;
				double right  = this.patternBbox.Right  - 5.0;
				double bottom = this.patternBbox.Bottom - 5.0;
				double top    = this.patternBbox.Top    - 5.0;
				double maxh = System.Math.Max(-left, right);
				double maxv = System.Math.Max(-bottom, top);
				this.patternWidth = System.Math.Max(maxh, maxv)*1.415;
			}
		}

		[XmlIgnore]
		public double PatternWidth
		{
			get
			{
				if ( this.patternId <= 0 )
				{
					return this.width;
				}
				else
				{
					double shift = 1.0+System.Math.Abs(this.patternShift);
					return this.patternWidth*this.width/5*this.patternSize*shift;
				}
			}
		}

		[XmlAttribute("DP1")]
		public double DashPen1
		{
			get { return this.dashPen[0]; }
			set { this.dashPen[0] = value; }
		}

		[XmlAttribute("DG1")]
		public double DashGap1
		{
			get { return this.dashGap[0]; }
			set { this.dashGap[0] = value; }
		}

		[XmlAttribute("DP2")]
		public double DashPen2
		{
			get { return this.dashPen[1]; }
			set { this.dashPen[1] = value; }
		}

		[XmlAttribute("DG2")]
		public double DashGap2
		{
			get { return this.dashGap[1]; }
			set { this.dashGap[1] = value; }
		}

		[XmlAttribute("DP3")]
		public double DashPen3
		{
			get { return this.dashPen[2]; }
			set { this.dashPen[2] = value; }
		}

		[XmlAttribute("DG3")]
		public double DashGap3
		{
			get { return this.dashGap[2]; }
			set { this.dashGap[2] = value; }
		}

		public double GetDashPen(int rank)
		{
			return this.dashPen[rank];
		}

		public void SetDashPen(int rank, double value)
		{
			this.dashPen[rank] = value;
		}

		public double GetDashGap(int rank)
		{
			return this.dashGap[rank];
		}

		public void SetDashGap(int rank, double value)
		{
			this.dashGap[rank] = value;
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyLine p = property as PropertyLine;
			p.Width        = this.width;
			p.Cap          = this.cap;
			p.Join         = this.join;
			p.Limit        = this.limit;
			p.PatternId    = this.patternId;
			p.PatternSeed  = this.patternSeed;
			p.PatternMove  = this.patternMove;
			p.PatternSize  = this.patternSize;
			p.PatternShift = this.patternShift;
			p.PatternAngle = this.patternAngle;
			p.PatternBbox  = this.patternBbox;
			p.RndMove      = this.rndMove;
			p.RndSize      = this.rndSize;
			p.RndShift     = this.rndShift;
			p.RndAngle     = this.rndAngle;
			p.RndPage      = this.rndPage;

			for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
			{
				p.SetDashPen(i, this.dashPen[i]);
				p.SetDashGap(i, this.dashGap[i]);
			}
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyLine p = property as PropertyLine;
			if ( p.Width        != this.width        )  return false;
			if ( p.Cap          != this.cap          )  return false;
			if ( p.Join         != this.join         )  return false;
			if ( p.Limit        != this.limit        )  return false;
			if ( p.PatternId    != this.patternId    )  return false;
			if ( p.PatternSeed  != this.patternSeed  )  return false;
			if ( p.PatternMove  != this.patternMove  )  return false;
			if ( p.PatternSize  != this.patternSize  )  return false;
			if ( p.PatternShift != this.patternShift )  return false;
			if ( p.PatternAngle != this.patternAngle )  return false;
			if ( p.RndMove      != this.rndMove      )  return false;
			if ( p.RndSize      != this.rndSize      )  return false;
			if ( p.RndShift     != this.rndShift     )  return false;
			if ( p.RndAngle     != this.rndAngle     )  return false;
			if ( p.RndPage      != this.rndPage      )  return false;

			for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
			{
				if ( p.GetDashPen(i) != this.dashPen[i] )  return false;
				if ( p.GetDashGap(i) != this.dashGap[i] )  return false;
			}

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			return new PanelLine(drawer);
		}


		// Engraisse la bbox selon le trait.
		public void InflateBoundingBox(ref Drawing.Rectangle bbox)
		{
			if ( this.patternId <= 0 )  // trait simple ou traitillé ?
			{
				if ( this.join == Drawing.JoinStyle.Miter )
				{
					bbox.Inflate(this.width*0.5*this.limit);
				}
				else if ( this.cap == Drawing.CapStyle.Square )
				{
					bbox.Inflate(this.width*0.5*1.415);  // augmente de racine de 2
				}
				else
				{
					bbox.Inflate(this.width*0.5);
				}
			}
			else	// trait à base de motif ?
			{
				bbox.Inflate(this.PatternWidth*0.5);
			}
		}


		// Effectue un graphics.Rasterizer.AddOutline.
		public void AddOutline(Drawing.Graphics graphics, Drawing.Path path, double addWidth)
		{
			if ( this.patternId <= 0 )  // trait simple ou traitillé ?
			{
				graphics.Rasterizer.AddOutline(path, this.width+addWidth, this.cap, this.join, this.limit);
			}
			else	// trait à base de motif ?
			{
				graphics.Rasterizer.AddOutline(path, this.PatternWidth+addWidth, Drawing.CapStyle.Round, Drawing.JoinStyle.Round);
			}
		}

		// Cherche le pattern à utiliser d'après son identificateur.
		static protected ObjectPattern SearchPattern(IconObjects iconObjects, int id)
		{
			int total = iconObjects.TotalPatterns();
			for ( int i=1 ; i<total ; i++ )
			{
				ObjectPattern pattern = iconObjects.Objects[i] as ObjectPattern;
				if ( pattern.Id == id )  return pattern;
			}
			return null;
		}

		// Dessine le pattern le long d'une droite.
		protected void DrawPatternLine(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects,
									   ObjectPattern pattern,
									   System.Random rndMove,
									   System.Random rndSize,
									   System.Random rndShift,
									   System.Random rndAngle,
									   System.Random rndPage,
									   ref double advance,
									   ref int page,
									   Drawing.Point p1, Drawing.Point p2)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;

			double len = Drawing.Point.Distance(p1,p2);
			while ( advance <= len )
			{
				ObjectPage objPage;
				if ( this.rndPage )
				{
					page = rndPage.Next(0, pattern.Objects.Count);
					objPage = pattern.Objects[page] as ObjectPage;
				}
				else
				{
					objPage = pattern.Objects[page] as ObjectPage;
					page ++;
					if ( page >= pattern.Objects.Count )  page = 0;
				}

				Drawing.Point pos  = Drawing.Point.Move(p1,p2, advance);
				Drawing.Point next = Drawing.Point.Move(p1,p2, advance+0.001);
				advance += this.width;
				if ( this.rndMove )
				{
					double r = rndMove.Next(0, 100000)/100000.0;
					advance += this.width*this.patternMove*r;
				}
				else
				{
					advance += this.width*this.patternMove;
				}

				double angle = Drawing.Point.ComputeAngleDeg(pos, next);

				double pa = this.patternAngle;
				if ( this.rndAngle )
				{
					double r = rndAngle.Next(-100000, 100000)/100000.0;
					pa *= r;
				}

				double ps = this.patternSize;
				if ( this.rndSize )
				{
					double r = rndSize.Next(0, 100000)/100000.0;
					ps = (ps-1.0)*r+1.0;
				}

				if ( this.patternShift != 0 )
				{
					double shift = this.patternShift;
					if ( this.rndShift )
					{
						double r = rndShift.Next(-100000, 100000)/100000.0;
						shift *= r;
					}

					pos = Drawing.Transform.RotatePointDeg(pos, angle+90, new Drawing.Point(pos.X+this.width*shift, pos.Y));
				}

				Drawing.Transform ot = graphics.Transform;
				graphics.TranslateTransform(pos.X, pos.Y);
				graphics.ScaleTransform(this.width/10*ps, this.width/10*ps, 0, 0);
				graphics.RotateTransformDeg(angle+pa, 0, 0);
				graphics.TranslateTransform(-5, -5);
				double isx = iconContext.ScaleX;
				double isy = iconContext.ScaleY;
				iconContext.ScaleX *= this.width/10*ps;
				iconContext.ScaleY *= this.width/10*ps;
				iconObjects.DrawGeometry(objPage.Objects, graphics, iconContext, iconObjects, adorner, Drawing.Rectangle.Infinite, true, false);
				iconContext.ScaleX = isx;
				iconContext.ScaleY = isy;
				graphics.Transform = ot;
			}
			advance -= len;
		}

		// Dessine le pattern le long d'une courbe.
		protected void DrawPatternCurve(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects,
										ObjectPattern pattern,
										System.Random rndMove,
										System.Random rndSize,
										System.Random rndShift,
										System.Random rndAngle,
										System.Random rndPage,
										ref double advance,
										ref int page,
										Drawing.Point p1, Drawing.Point s1, Drawing.Point s2, Drawing.Point p2)
		{
			Drawing.Point pos = p1;
			int total = (int)(1.0/PropertyLine.step);
			for ( int rank=1 ; rank<=100 ; rank ++ )
			{
				double t = PropertyLine.step*rank;
				Drawing.Point next = Drawing.Point.FromBezier(p1,s1,s2,p2, t);
				this.DrawPatternLine(graphics, iconContext, iconObjects, pattern, rndMove, rndSize, rndShift, rndAngle, rndPage, ref advance, ref page, pos, next);
				pos = next;
			}
		}

		// Dessine le pattern le long d'un chemin.
		protected void DrawPattern(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects,
								   ObjectPattern pattern, Drawing.Path path)
		{
			Drawing.PathElement[] elements;
			Drawing.Point[] points;
			path.GetElements(out elements, out points);

			System.Random rndMove  = new System.Random(this.PatternSeed*4823);
			System.Random rndSize  = new System.Random(this.PatternSeed*1049);
			System.Random rndShift = new System.Random(this.PatternSeed*3687);
			System.Random rndAngle = new System.Random(this.PatternSeed*9551);
			System.Random rndPage  = new System.Random(this.PatternSeed*2473);

			Drawing.Point start = new Drawing.Point(0, 0);
			Drawing.Point current = new Drawing.Point(0, 0);
			Drawing.Point p1 = new Drawing.Point(0, 0);
			Drawing.Point p2 = new Drawing.Point(0, 0);
			Drawing.Point p3 = new Drawing.Point(0, 0);
			double advance = 0;
			int page = 0;
			int i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & Drawing.PathElement.MaskCommand )
				{
					case Drawing.PathElement.MoveTo:
						current = points[i++];
						start = current;
						break;

					case Drawing.PathElement.LineTo:
						p1 = points[i++];
						this.DrawPatternLine(graphics, iconContext, iconObjects, pattern, rndMove, rndSize, rndShift, rndAngle, rndPage, ref advance, ref page, current,p1);
						current = p1;
						break;

					case Drawing.PathElement.Curve3:
						p1 = points[i++];
						p2 = points[i++];
						this.DrawPatternCurve(graphics, iconContext, iconObjects, pattern, rndMove, rndSize, rndShift, rndAngle, rndPage, ref advance, ref page, current,p1,p1,p2);
						current = p2;
						break;

					case Drawing.PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						this.DrawPatternCurve(graphics, iconContext, iconObjects, pattern, rndMove, rndSize, rndShift, rndAngle, rndPage, ref advance, ref page, current,p1,p2,p3);
						current = p3;
						break;

					default:
						if ( (elements[i] & Drawing.PathElement.FlagClose) != 0 )
						{
							this.DrawPatternLine(graphics, iconContext, iconObjects, pattern, rndMove, rndSize, rndShift, rndAngle, rndPage, ref advance, ref page, current,start);
						}
						i ++;
						break;
				}
			}
		}

		// Dessine le trait le long d'un chemin.
		public void DrawPath(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects, Drawing.Path path, Drawing.Color color)
		{
			if ( this.width == 0.0 )  return;
			if ( path.IsEmpty )  return;

			if ( this.patternId == 0 )  // trait simple ?
			{
				graphics.Rasterizer.AddOutline(path, this.width, this.cap, this.join, this.limit);
				graphics.RenderSolid(iconContext.AdaptColor(color));
			}
			else if ( this.patternId == -1 )  // traitillé ?
			{
				Drawing.DashedPath dp = new Drawing.DashedPath();
				dp.DefaultZoom = iconContext.ScaleX;
				dp.Append(path);

				for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
				{
					if ( this.dashGap[i] == 0.0 )  continue;
					double pen = this.dashPen[i];
					if ( pen == 0.0 )  pen = 0.00001;
					dp.AddDash(pen, this.dashGap[i]);
				}

				using ( Drawing.Path temp = dp.GenerateDashedPath() )
				{
					graphics.Rasterizer.AddOutline(temp, this.width, this.cap, this.join, this.limit);
					graphics.RenderSolid(iconContext.AdaptColor(color));
				}
			}
			else	// trait à base de motif ?
			{
				ObjectPattern pattern = PropertyLine.SearchPattern(iconObjects, this.patternId);
				if ( pattern == null )
				{
					graphics.Rasterizer.AddOutline(path, this.width, Drawing.CapStyle.Round, Drawing.JoinStyle.Round);
					graphics.RenderSolid(iconContext.AdaptColor(color));
				}
				else
				{
					this.DrawPattern(graphics, iconContext, iconObjects, pattern, path);
				}
			}
		}


		// Adapte le trait après la destruction d'un pattern.
		public bool AdaptDeletePattern(int rank)
		{
			if ( this.patternId != rank )  return false;
			this.patternId = 0;
			return true;
		}

		// Adapte le trait après la destruction de tous les patterns.
		public bool AdaptDeletePattern()
		{
			if ( this.patternId <= 0 )  return false;  // trait simple ou traitillé ?
			this.patternId = 0;
			return true;
		}


		protected double					width;
		protected Drawing.CapStyle			cap;
		protected Drawing.JoinStyle			join;
		protected double					limit;  // longueur (et non angle) !
		protected int						patternId;
		protected int						patternSeed;
		protected double					patternMove;
		protected double					patternSize;
		protected double					patternShift;
		protected double					patternAngle;
		protected bool						rndMove;
		protected bool						rndSize;
		protected bool						rndShift;
		protected bool						rndAngle;
		protected bool						rndPage;
		protected Drawing.Rectangle			patternBbox = Drawing.Rectangle.Empty;
		protected double					patternWidth;
		protected double[]					dashPen;
		protected double[]					dashGap;
		protected static readonly double	step = 0.01;
		public static readonly int			DashMax = 3;
	}
}
