using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public enum ArrowType
	{
		None,
		Right,
		ArrowSimply,
		ArrowFantasy1,
		ArrowCurve1,
		ArrowCurve2,
		ArrowOutline,
		Slash,
		Dot,
		Square,
		Diamond,
	}

	/// <summary>
	/// La classe PropertyArrow représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyArrow : AbstractProperty
	{
		public PropertyArrow()
		{
			this.arrowType = new ArrowType[2];
			this.length    = new double[2];
			this.effect1   = new double[2];
			this.effect2   = new double[2];

			this.arrowType[0] = ArrowType.Right;
			this.arrowType[1] = ArrowType.Right;
			this.length[0]    = 2.0;
			this.length[1]    = 2.0;
			this.effect1[0]   = 0.5;
			this.effect1[1]   = 0.5;
			this.effect2[0]   = 0.5;
			this.effect2[1]   = 0.5;
		}

		[XmlAttribute]
		public ArrowType ArrowType1
		{
			get
			{
				return this.arrowType[0];
			}

			set
			{
				if ( this.arrowType[0] != value )
				{
					this.arrowType[0] = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double Length1
		{
			get
			{
				return this.length[0];
			}

			set
			{
				if ( this.length[0] != value )
				{
					this.length[0] = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double Effect11
		{
			get { return this.effect1[0]; }
			set { this.effect1[0] = value; }
		}

		[XmlAttribute]
		public double Effect12
		{
			get { return this.effect2[0]; }
			set { this.effect2[0] = value; }
		}


		[XmlAttribute]
		public ArrowType ArrowType2
		{
			get
			{
				return this.arrowType[1];
			}

			set
			{
				if ( this.arrowType[1] != value )
				{
					this.arrowType[1] = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double Length2
		{
			get
			{
				return this.length[1];
			}

			set
			{
				if ( this.length[1] != value )
				{
					this.length[1] = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double Effect21
		{
			get { return this.effect1[1]; }
			set { this.effect1[1] = value; }
		}

		[XmlAttribute]
		public double Effect22
		{
			get { return this.effect2[1]; }
			set { this.effect2[1] = value; }
		}


		public ArrowType GetArrowType(int extremity)
		{
			return this.arrowType[extremity];
		}

		public void SetArrowType(int extremity, ArrowType type)
		{
			if ( this.arrowType[extremity] == type )  return;
			this.arrowType[extremity] = type;
			this.OnChanged();
		}

		public double GetLength(int extremity)
		{
			return this.length[extremity];
		}

		public void SetLength(int extremity, double length)
		{
			if ( this.length[extremity] == length )  return;
			this.length[extremity] = length;
			this.OnChanged();
		}

		public double GetEffect1(int extremity)
		{
			return this.effect1[extremity];
		}

		public void SetEffect1(int extremity, double effect)
		{
			if ( this.effect1[extremity] == effect )  return;
			this.effect1[extremity] = effect;
			this.OnChanged();
		}

		public double GetEffect2(int extremity)
		{
			return this.effect2[extremity];
		}

		public void SetEffect2(int extremity, double effect)
		{
			if ( this.effect2[extremity] == effect )  return;
			this.effect2[extremity] = effect;
			this.OnChanged();
		}


		// Détermine le nom de la propriété dans la liste (Lister).
		public string GetListName()
		{
			return PropertyArrow.GetName(this.arrowType[0]) + ", " +
				   PropertyArrow.GetName(this.arrowType[1]);
		}

		// Cherche le type correspondant à un index donné.
		// Ceci détermine l'ordre dans le TextFieldCombo du panneau.
		public static ArrowType ConvType(int index)
		{
			ArrowType type = ArrowType.None;
			switch ( index )
			{
				case  0:  type = ArrowType.Right;          break;
				case  1:  type = ArrowType.ArrowSimply;    break;
				case  2:  type = ArrowType.ArrowFantasy1;  break;
				case  3:  type = ArrowType.ArrowCurve1;    break;
				case  4:  type = ArrowType.ArrowCurve2;    break;
				case  5:  type = ArrowType.ArrowOutline;   break;
				case  6:  type = ArrowType.Slash;          break;
				case  7:  type = ArrowType.Dot;            break;
				case  8:  type = ArrowType.Square;         break;
				case  9:  type = ArrowType.Diamond;        break;
			}
			return type;
		}

		// Cherche le rang d'un type donné.
		public static int ConvType(ArrowType type)
		{
			for ( int i=0 ; i<100 ; i++ )
			{
				ArrowType t = PropertyArrow.ConvType(i);
				if ( t == ArrowType.None )  break;
				if ( t == type )  return i;
			}
			return -1;
		}

		// Retourne le nom d'un type donné.
		public static string GetName(ArrowType type)
		{
			string name = "";
			switch ( type )
			{
				case ArrowType.Right:          name = "Rien";              break;
				case ArrowType.ArrowSimply:    name = "Flèche simple";     break;
				case ArrowType.ArrowFantasy1:  name = "Flèche fantaisie";  break;
				case ArrowType.ArrowCurve1:    name = "Flèche courbe 1";   break;
				case ArrowType.ArrowCurve2:    name = "Flèche courbe 2";   break;
				case ArrowType.ArrowOutline:   name = "Flèche filaire";    break;
				case ArrowType.Slash:          name = "Barre oblique";     break;
				case ArrowType.Dot:            name = "Point";             break;
				case ArrowType.Square:         name = "Carré";             break;
				case ArrowType.Diamond:        name = "Losange";           break;
			}
			return name;
		}

		// Retourne les valeurs par défaut et les min/max pour un type donné.
		public static void GetFieldsParam(ArrowType type, out bool enableLength,
										  out bool enable1, out double effect1, out double min1, out double max1,
										  out bool enable2, out double effect2, out double min2, out double max2)
		{
			enableLength = true;
			enable1 = true;  effect1 = 0.50;  min1 = 0.00;  max1 = 2.00;
			enable2 = true;  effect2 = 0.50;  min2 = 0.00;  max2 = 2.00;

			switch ( type )
			{
				case ArrowType.Right:
					enableLength = false;
					enable1 = false;  effect1 = 0.50;  min1 = 0.00;  max1 = 2.00;
					enable2 = false;  effect2 = 0.50;  min2 = 0.00;  max2 = 2.00;  break;

				case ArrowType.ArrowSimply:
					enable1 = true;   effect1 = 0.50;  min1 = 0.20;  max1 = 2.00;
					enable2 = true;   effect2 = 1.00;  min2 = 0.50;  max2 = 1.00;  break;

				case ArrowType.ArrowFantasy1:
					enable1 = true;   effect1 = 1.00;  min1 = 0.50;  max1 = 2.00;
					enable2 = true;   effect2 = 0.50;  min2 = 0.20;  max2 = 1.00;  break;

				case ArrowType.ArrowCurve1:
					enable1 = true;   effect1 = 0.75;  min1 = 0.20;  max1 = 1.00;
					enable2 = true;   effect2 = 0.00;  min2 =-0.75;  max2 = 0.75;  break;

				case ArrowType.ArrowCurve2:
					enable1 = true;   effect1 = 0.50;  min1 = 0.20;  max1 = 1.00;
					enable2 = true;   effect2 = 0.50;  min2 =-1.00;  max2 = 0.75;  break;

				case ArrowType.ArrowOutline:
					enable1 = true;   effect1 = 0.50;  min1 = 0.20;  max1 = 2.00;
					enable2 = false;  effect2 = 0.50;  min2 = 0.00;  max2 = 2.00;  break;

				case ArrowType.Slash:
					enable1 = true;   effect1 = 1.00;  min1 =-1.00;  max1 = 1.00;
					enable2 = false;  effect2 = 0.50;  min2 = 0.00;  max2 = 2.00;  break;

				case ArrowType.Dot:
					enable1 = false;  effect1 = 0.50;  min1 = 0.00;  max1 = 2.00;
					enable2 = false;  effect2 = 0.50;  min2 = 0.00;  max2 = 2.00;  break;

				case ArrowType.Square:
					enable1 = false;  effect1 = 0.50;  min1 = 0.0;  max1 = 2.0;
					enable2 = false;  effect2 = 0.50;  min2 = 0.0;  max2 = 2.0;  break;

				case ArrowType.Diamond:
					enable1 = false;  effect1 = 0.50;  min1 = 0.0;  max1 = 2.0;
					enable2 = false;  effect2 = 0.50;  min2 = 0.0;  max2 = 2.0;  break;
			}
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
			PropertyArrow p = property as PropertyArrow;
			for ( int j=0 ; j<2 ; j++ )
			{
				p.SetArrowType(j, this.arrowType[j]);
				p.SetLength   (j, this.length[j]);
				p.SetEffect1  (j, this.effect1[j]);
				p.SetEffect2  (j, this.effect2[j]);
			}
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyArrow p = property as PropertyArrow;
			for ( int j=0 ; j<2 ; j++ )
			{
				if ( p.arrowType[j] != this.arrowType[j] )  return false;
				if ( p.length[j]    != this.length[j]    )  return false;
				if ( p.effect1[j]   != this.effect1[j]   )  return false;
				if ( p.effect2[j]   != this.effect2[j]   )  return false;
			}
			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelArrow();
		}


		// Crée le chemin à l'extrémité p1, et retourne pp1, le remplaçant de p1.
		public Drawing.Point PathExtremity(Drawing.Path path, int extremity,
										   double width, Drawing.CapStyle cap,
										   Drawing.Point p1, Drawing.Point p2,
										   out bool outline, out bool surface)
		{
			outline = false;
			surface = false;
			if ( this.arrowType[extremity] == ArrowType.Right )  return p1;

			Drawing.Point pp;
			double len = this.length[extremity];
			double ef1 = this.effect1[extremity];
			double ef2 = this.effect2[extremity];
			double mx2 = System.Math.Max(ef2, 0);
			double limit;

			switch ( this.arrowType[extremity] )
			{
				case ArrowType.ArrowSimply:
					limit = this.LimitPara(extremity, width, p1, p2);
					pp = PropertyArrow.Extremity(p1, p2, len*System.Math.Max(ef2, limit), 0);
					path.MoveTo(pp);
					path.LineTo(PropertyArrow.Extremity(p1, p2, len, len*ef1));
					path.LineTo(p1);
					path.LineTo(PropertyArrow.Extremity(p1, p2, len, -len*ef1));
					path.Close();
					if ( cap == Drawing.CapStyle.Square )
					{
						pp = Drawing.Point.Move(pp, p2, width/2);
					}
					p1 = pp;
					surface = true;
					break;

				case ArrowType.ArrowFantasy1:
					limit = this.LimitPara(extremity, width, p1, p2);
					pp = PropertyArrow.Extremity(p1, p2, len*System.Math.Max(ef2, limit), 0.0);
					path.MoveTo(pp);
					path.LineTo(PropertyArrow.Extremity(p1, p2, len+len*ef2, len*ef1));
					path.LineTo(PropertyArrow.Extremity(p1, p2, len, len*ef1));
					path.LineTo(p1);
					path.LineTo(PropertyArrow.Extremity(p1, p2, len, -len*ef1));
					path.LineTo(PropertyArrow.Extremity(p1, p2, len+len*ef2, -len*ef1));
					path.Close();
					if ( cap == Drawing.CapStyle.Square )
					{
						pp = Drawing.Point.Move(pp, p2, width/2);
					}
					p1 = pp;
					surface = true;
					break;

				case ArrowType.ArrowCurve1:
					limit = this.LimitPara(extremity, width, p1, p2);
					limit = System.Math.Max(0.25, limit);
					pp = PropertyArrow.Extremity(p1, p2, len*limit, 0.0);
					path.MoveTo(pp);
					path.CurveTo(PropertyArrow.Extremity(p1, p2, len*(mx2+limit), 0),
								 PropertyArrow.Extremity(p1, p2, len*(1-ef2), 0),
								 PropertyArrow.Extremity(p1, p2, len, len*ef1));
					path.LineTo(p1);
					path.LineTo(PropertyArrow.Extremity(p1, p2, len, -len*ef1));
					path.CurveTo(PropertyArrow.Extremity(p1, p2, len*(1-ef2), 0),
								 PropertyArrow.Extremity(p1, p2, len*(mx2+limit), 0),
								 pp);
					path.Close();
					if ( cap != Drawing.CapStyle.Butt )
					{
						pp = Drawing.Point.Move(pp, p2, width/2);
					}
					p1 = pp;
					surface = true;
					break;

				case ArrowType.ArrowCurve2:
					pp = PropertyArrow.Extremity(p1, p2, len, 0.0);
					path.MoveTo(pp);
					path.LineTo(PropertyArrow.Extremity(p1, p2, len, len*ef1));
					path.CurveTo(PropertyArrow.Extremity(p1, p2, len, len*ef1*(1-ef2)),
								 PropertyArrow.Extremity(p1, p2, len*mx2, 0),
								 p1);
					path.CurveTo(PropertyArrow.Extremity(p1, p2, len*mx2, 0),
								 PropertyArrow.Extremity(p1, p2, len, -len*ef1*(1-ef2)),
								 PropertyArrow.Extremity(p1, p2, len, -len*ef1));
					path.Close();
					if ( cap == Drawing.CapStyle.Square )
					{
						pp = Drawing.Point.Move(pp, p2, width/2);
					}
					p1 = pp;
					surface = true;
					break;

				case ArrowType.ArrowOutline:
					path.MoveTo(PropertyArrow.Extremity(p1, p2, len, len*ef1));
					path.LineTo(p1);
					path.LineTo(PropertyArrow.Extremity(p1, p2, len, -len*ef1));
					outline = true;
					break;

				case ArrowType.Slash:
					path.MoveTo(PropertyArrow.Extremity(p1, p2, len*ef1, len));
					path.LineTo(PropertyArrow.Extremity(p1, p2, -len*ef1, -len));
					outline = true;
					break;

				case ArrowType.Dot:
					path.AppendCircle(p1, len);
					surface = true;
					break;

				case ArrowType.Square:
					path.MoveTo(PropertyArrow.Extremity(p1, p2, -len, -len));
					path.LineTo(PropertyArrow.Extremity(p1, p2, -len,  len));
					path.LineTo(PropertyArrow.Extremity(p1, p2,  len,  len));
					path.LineTo(PropertyArrow.Extremity(p1, p2,  len, -len));
					path.Close();
					if ( cap == Drawing.CapStyle.Square )
					{
						p1 = Drawing.Point.Move(p1, p2, width/2);
					}
					surface = true;
					break;

				case ArrowType.Diamond:
					path.MoveTo(PropertyArrow.Extremity(p1, p2, -len, 0));
					path.LineTo(PropertyArrow.Extremity(p1, p2, 0, len));
					path.LineTo(PropertyArrow.Extremity(p1, p2, len, 0));
					path.LineTo(PropertyArrow.Extremity(p1, p2, 0, -len));
					path.Close();
					surface = true;
					break;
			}

			return p1;
		}

		// Calcule l'effet limite parallèlement à p1-p2.
		protected double LimitPara(int extremity, double width,
								   Drawing.Point p1, Drawing.Point p2)
		{
			double len = this.length[extremity];
			double ef1 = this.effect1[extremity];
			Drawing.Point pa = PropertyArrow.Extremity(p1, p2, len, len*ef1);
			Drawing.Point pp1 = Drawing.Point.Move(p1, p2, len);
			double d = Drawing.Point.Distance(pa, pp1);
			return (width/2)/d;
		}

		// Calcule l'extrémité gauche ou droite d'une flèche.
		static protected Drawing.Point Extremity(Drawing.Point p1, Drawing.Point p2,
												 double distPara, double distPerp)
		{
			Drawing.Point c = Drawing.Point.Move(p1, p2, distPara);
			Drawing.Point p = Drawing.Point.Move(c, p2, System.Math.Abs(distPerp));
			double angle = (distPerp > 0) ? System.Math.PI/2 : -System.Math.PI/2;
			return Drawing.Transform.RotatePoint(c, angle, p);
		}


		protected ArrowType[]			arrowType;
		protected double[]				length;
		protected double[]				effect1;
		protected double[]				effect2;
	}
}
