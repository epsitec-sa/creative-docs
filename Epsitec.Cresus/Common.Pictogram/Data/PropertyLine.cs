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
			this.patternSize  = 1.0;
			this.patternShift = 0.0;
			this.patternAngle = 0.0;
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

		[XmlAttribute]
		public int PatternId
		{
			get { return this.patternId; }
			set { this.patternId = value; }
		}

		[XmlAttribute]
		public double PatternSize
		{
			get { return this.patternSize; }
			set { this.patternSize = value; }
		}

		[XmlAttribute]
		public double PatternShift
		{
			get { return this.patternShift; }
			set { this.patternShift = value; }
		}

		[XmlAttribute]
		public double PatternAngle
		{
			get { return this.patternAngle; }
			set { this.patternAngle = value; }
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
			p.PatternSize  = this.patternSize;
			p.PatternShift = this.patternShift;
			p.PatternAngle = this.patternAngle;
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
			if ( p.PatternSize  != this.patternSize  )  return false;
			if ( p.PatternShift != this.patternShift )  return false;
			if ( p.PatternAngle != this.patternAngle )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelLine();
		}


		// Engraisse la bbox selon le trait.
		public void InflateBoundingBox(ref Drawing.Rectangle bbox)
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
									   ObjectPattern pattern, ref double advance,
									   Drawing.Point p1, Drawing.Point p2)
		{
			ObjectPage page = pattern.Objects[0] as ObjectPage;
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;

			double len = Drawing.Point.Distance(p1,p2);
			while ( advance <= len )
			{
				Drawing.Point pos  = Drawing.Point.Move(p1,p2, advance);
				Drawing.Point next = Drawing.Point.Move(p1,p2, advance+0.001);
				advance += this.width;

				double angle = Drawing.Point.ComputeAngleRad(pos, next);

				if ( this.patternShift != 0 )
				{
					pos = Drawing.Transform.RotatePointRad(pos, angle+System.Math.PI/2, new Drawing.Point(pos.X+this.width*this.patternShift, pos.Y));
				}

				Drawing.Transform ot = graphics.SaveTransform();
				graphics.TranslateTransform(pos.X, pos.Y);
				graphics.ScaleTransform(this.width/10*this.patternSize, this.width/10*this.patternSize, 0, 0);
				graphics.RotateTransformRad((angle+this.patternAngle), 0, 0);
				graphics.TranslateTransform(-5, -5);
				iconObjects.DrawGeometry(page.Objects, graphics, iconContext, iconObjects, adorner, Drawing.Rectangle.Infinite, true, false);
				graphics.Transform = ot;
			}
			advance -= len;
		}

		// Dessine le pattern le long d'une courbe.
		protected void DrawPatternCurve(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects,
										ObjectPattern pattern, ref double advance,
										Drawing.Point p1, Drawing.Point s1, Drawing.Point s2, Drawing.Point p2)
		{
			Drawing.Point pos = p1;
			int total = (int)(1.0/PropertyLine.step);
			for ( int rank=1 ; rank<=100 ; rank ++ )
			{
				double t = PropertyLine.step*rank;
				Drawing.Point next = Drawing.Point.Bezier(p1,s1,s2,p2, t);
				this.DrawPatternLine(graphics, iconContext, iconObjects, pattern, ref advance, pos, next);
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

			Drawing.Point start = new Drawing.Point(0, 0);
			Drawing.Point current = new Drawing.Point(0, 0);
			Drawing.Point p1 = new Drawing.Point(0, 0);
			Drawing.Point p2 = new Drawing.Point(0, 0);
			Drawing.Point p3 = new Drawing.Point(0, 0);
			double advance = 0;
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
						this.DrawPatternLine(graphics, iconContext, iconObjects, pattern, ref advance, current,p1);
						current = p1;
						break;

					case Drawing.PathElement.Curve3:
						p1 = points[i++];
						p2 = points[i++];
						this.DrawPatternCurve(graphics, iconContext, iconObjects, pattern, ref advance, current,p1,p1,p2);
						current = p2;
						break;

					case Drawing.PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						this.DrawPatternCurve(graphics, iconContext, iconObjects, pattern, ref advance, current,p1,p2,p3);
						current = p3;
						break;

					default:
						if ( (elements[i] & Drawing.PathElement.FlagClose) != 0 )
						{
							this.DrawPatternLine(graphics, iconContext, iconObjects, pattern, ref advance, current,start);
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

			if ( this.patternId == 0 )
			{
				graphics.Rasterizer.AddOutline(path, this.width, this.cap, this.join, this.limit);
				graphics.RenderSolid(iconContext.AdaptColor(color));
			}
			else
			{
				ObjectPattern pattern = PropertyLine.SearchPattern(iconObjects, this.patternId);
				if ( pattern == null )
				{
					graphics.Rasterizer.AddOutline(path, this.width, this.cap, this.join, this.limit);
					graphics.RenderSolid(iconContext.AdaptColor(color));
				}
				else
				{
					this.DrawPattern(graphics, iconContext, iconObjects, pattern, path);
				}
			}
		}


		protected double					width;
		protected Drawing.CapStyle			cap;
		protected Drawing.JoinStyle			join;
		protected double					limit;  // longueur (et non angle) !
		protected int						patternId;
		protected double					patternSize;
		protected double					patternShift;
		protected double					patternAngle;
		protected static readonly double	step = 0.01;
	}
}
