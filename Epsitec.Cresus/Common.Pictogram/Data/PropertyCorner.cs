using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public enum CornerType
	{
		None,
		Right,
		Round,
		Bevel,
		Line31,
		Line41,
		Line42,
		Line51,
		Line61,
		Line62,
		Curve21,
		Curve22,
		Curve31,
		Fantasy51,
		Fantasy61,
		Fantasy62,
	}

	/// <summary>
	/// La classe PropertyCorner représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyCorner : AbstractProperty
	{
		public PropertyCorner()
		{
			this.cornerType = CornerType.Right;
			this.radius     = 2.0;
			this.effect1    = 0.5;
			this.effect2    = 0.5;
		}

		[XmlAttribute]
		public CornerType CornerType
		{
			get
			{
				return this.cornerType;
			}

			set
			{
				if ( this.cornerType != value )
				{
					this.cornerType = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double Radius
		{
			get
			{
				return this.radius;
			}

			set
			{
				if ( this.radius != value )
				{
					this.radius = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double Effect1
		{
			get { return this.effect1; }
			set { this.effect1 = value; }
		}

		[XmlAttribute]
		public double Effect2
		{
			get { return this.effect2; }
			set { this.effect2 = value; }
		}

		// Détermine le nom de la propriété dans la liste (Lister).
		public string GetListName()
		{
			return PropertyCorner.GetName(this.cornerType);
		}

		// Cherche le type correspondant à un index donné.
		// Ceci détermine l'ordre dans le TextFieldCombo du panneau.
		public static CornerType ConvType(int index)
		{
			CornerType type = CornerType.None;
			switch ( index )
			{
				case  0:  type = CornerType.Right;      break;
				case  1:  type = CornerType.Round;      break;
				case  2:  type = CornerType.Bevel;      break;
				case  3:  type = CornerType.Line31;     break;
				case  4:  type = CornerType.Line41;     break;
				case  5:  type = CornerType.Line42;     break;
				case  6:  type = CornerType.Line51;     break;
				case  7:  type = CornerType.Line61;     break;
				case  8:  type = CornerType.Line62;     break;
				case  9:  type = CornerType.Curve21;    break;
				case 10:  type = CornerType.Curve22;    break;
				case 11:  type = CornerType.Curve31;    break;
				case 12:  type = CornerType.Fantasy51;  break;
				case 13:  type = CornerType.Fantasy61;  break;
				case 14:  type = CornerType.Fantasy62;  break;
			}
			return type;
		}

		// Cherche le rang d'un type donné.
		public static int ConvType(CornerType type)
		{
			for ( int i=0 ; i<100 ; i++ )
			{
				CornerType t = PropertyCorner.ConvType(i);
				if ( t == CornerType.None )  break;
				if ( t == type )  return i;
			}
			return -1;
		}

		// Retourne le nom d'un type donné.
		public static string GetName(CornerType type)
		{
			string name = "";
			switch ( type )
			{
				case CornerType.Right:      name = "Droit";           break;
				case CornerType.Round:      name = "Arrondi";         break;
				case CornerType.Bevel:      name = "Biseau";          break;
				case CornerType.Line31:     name = "Droites 3.1";     break;
				case CornerType.Line41:     name = "Droites 4.1";     break;
				case CornerType.Line42:     name = "Droites 4.2";     break;
				case CornerType.Line51:     name = "Droites 5.1";     break;
				case CornerType.Line61:     name = "Droites 6.1";     break;
				case CornerType.Line62:     name = "Droites 6.2";     break;
				case CornerType.Curve21:    name = "Courbes 2.1";     break;
				case CornerType.Curve22:    name = "Courbes 2.2";     break;
				case CornerType.Curve31:    name = "Courbes 3.1";     break;
				case CornerType.Fantasy51:  name = "Fantaisies 5.1";  break;
				case CornerType.Fantasy61:  name = "Fantaisies 6.1";  break;
				case CornerType.Fantasy62:  name = "Fantaisies 6.2";  break;
			}
			return name;
		}

		// Retourne les valeurs par défaut et les min/max pour un type donné.
		public static void GetFieldsParam(CornerType type, out bool enableRadius,
										  out bool enable1, out double effect1, out double min1, out double max1,
										  out bool enable2, out double effect2, out double min2, out double max2)
		{
			enableRadius = true;
			enable1 = true;  effect1 = 0.5;  min1 = -1.0;  max1 = 2.0;
			enable2 = true;  effect2 = 0.5;  min2 = -1.0;  max2 = 2.0;

			switch ( type )
			{
				case CornerType.Right:
					enableRadius = false;
					enable1 = false;  effect1 = 0.50;  min1 = -1.0;  max1 = 2.0;
					enable2 = false;  effect2 = 0.50;  min2 = -1.0;  max2 = 2.0;  break;

				case CornerType.Round:
					enable1 = true;   effect1 = 0.00;  min1 = -1.0;  max1 = 2.0;
					enable2 = false;  effect2 = 0.50;  min2 = -1.0;  max2 = 2.0;  break;

				case CornerType.Bevel:
					enable1 = true;   effect1 = 0.50;  min1 = -1.0;  max1 = 2.0;
					enable2 = false;  effect2 = 0.50;  min2 = -1.0;  max2 = 2.0;  break;

				case CornerType.Line31:
					enable1 = true;  effect1 = 1.00;  min1 =  0.5;  max1 = 2.0;
					enable2 = true;  effect2 = 0.50;  min2 = -1.0;  max2 = 1.0;  break;

				case CornerType.Line41:
					enable1 = true;  effect1 = 0.25;  min1 = -1.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.25;  min2 = -1.0;  max2 = 2.0;  break;

				case CornerType.Line42:
					enable1 = true;  effect1 = 1.50;  min1 =  0.0;  max1 = 2.0;
					enable2 = true;  effect2 =-0.50;  min2 = -1.0;  max2 = 0.0;  break;

				case CornerType.Line51:
					enable1 = true;  effect1 = 0.50;  min1 = -1.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.50;  min2 =  0.0;  max2 = 1.0;  break;

				case CornerType.Line61:
					enable1 = true;  effect1 = 0.25;  min1 = -1.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.75;  min2 =  0.0;  max2 = 1.0;  break;

				case CornerType.Line62:
					effect1 = 0.25;  min1 = -1.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.75;  min2 =  0.0;  max2 = 1.0;  break;

				case CornerType.Curve21:
					enable1 = true;  effect1 = 0.50;  min1 =  0.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.50;  min2 = -1.0;  max2 = 2.0;  break;

				case CornerType.Curve22:
					enable1 = true;  effect1 = 0.50;  min1 = -1.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.25;  min2 = -1.0;  max2 = 2.0;  break;

				case CornerType.Curve31:
					enable1 = true;  effect1 = 0.25;  min1 = -1.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.00;  min2 = -1.0;  max2 = 2.0;  break;

				case CornerType.Fantasy51:
					enable1 = true;  effect1 = 0.50;  min1 =  0.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.50;  min2 =  0.0;  max2 = 1.0;  break;

				case CornerType.Fantasy61:
					enable1 = true;  effect1 = 0.40;  min1 =  0.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.40;  min2 =  0.0;  max2 = 2.0;  break;

				case CornerType.Fantasy62:
					enable1 = true;  effect1 = 0.60;  min1 =  0.0;  max1 = 1.0;
					enable2 = true;  effect2 = 0.60;  min2 = -1.0;  max2 = 1.0;  break;
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
			PropertyCorner p = property as PropertyCorner;
			p.CornerType = this.cornerType;
			p.Radius     = this.radius;
			p.Effect1    = this.effect1;
			p.Effect2    = this.effect2;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyCorner p = property as PropertyCorner;
			if ( p.CornerType != this.cornerType )  return false;
			if ( p.Radius     != this.radius     )  return false;
			if ( p.Effect1    != this.effect1    )  return false;
			if ( p.Effect2    != this.effect2    )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelCorner();
		}


		// Crée le chemin d'un coin.
		// Le rayon donné est plus petit ou égal à this.radius et correspond à la
		// distance p1/c ou p2/c.
		//	o p1
		//	|
		//	| c
		//	o-----o p2
		public void PathCorner(Drawing.Path path, Drawing.Point p1, Drawing.Point c, Drawing.Point p2, double radius)
		{
			// Paramètres passés à PathDot:
			this.p1 = p1;
			this.c  = c;
			this.p2 = p2;
			this.r  = radius;

			double ef1 = this.effect1;
			double ef2 = this.effect2;

			switch ( this.CornerType )
			{
				case CornerType.Round:
					path.ArcTo(this.PathDot(ef1, ef1), p2);
					break;

				case CornerType.Bevel:
					path.LineTo(this.PathDot(ef1, ef1));
					path.LineTo(p2);
					break;

				case CornerType.Line31:
					path.LineTo(this.PathDot(ef1, ef2));
					path.LineTo(this.PathDot(ef2, ef1));
					path.LineTo(p2);
					break;

				case CornerType.Line41:
					path.LineTo(this.PathDot(1.0, ef1));
					path.LineTo(this.PathDot(ef2, ef2));
					path.LineTo(this.PathDot(ef1, 1.0));
					path.LineTo(p2);
					break;

				case CornerType.Line42:
					path.LineTo(this.PathDot(ef1, ef2));
					path.LineTo(this.PathDot(ef2, ef2));
					path.LineTo(this.PathDot(ef2, ef1));
					path.LineTo(p2);
					break;

				case CornerType.Line51:
					path.LineTo(this.PathDot(1.0, ef1));
					path.LineTo(this.PathDot(ef2, 0.0));
					path.LineTo(this.PathDot(0.0, ef2));
					path.LineTo(this.PathDot(ef1, 1.0));
					path.LineTo(p2);
					break;

				case CornerType.Line61:
					path.LineTo(this.PathDot(1.0, ef1));
					path.LineTo(this.PathDot(ef2, 0.0));
					path.LineTo(c);
					path.LineTo(this.PathDot(0.0, ef2));
					path.LineTo(this.PathDot(ef1, 1.0));
					path.LineTo(p2);
					break;

				case CornerType.Line62:
					path.LineTo(this.PathDot(ef2, ef1));
					path.LineTo(this.PathDot(ef2, 0.0));
					path.LineTo(c);
					path.LineTo(this.PathDot(0.0, ef2));
					path.LineTo(this.PathDot(ef1, ef2));
					path.LineTo(p2);
					break;

				case CornerType.Curve21:
					path.ArcTo(this.PathDot(ef1, 0.0), this.PathDot(ef2, ef2));
					path.ArcTo(this.PathDot(0.0, ef1), p2);
					break;

				case CornerType.Curve22:
					path.ArcTo(this.PathDot(1.0, ef1), this.PathDot(ef2, ef2));
					path.ArcTo(this.PathDot(ef1, 1.0), p2);
					break;

				case CornerType.Curve31:
					path.LineTo(this.PathDot(1.0, ef1));
					path.ArcTo(this.PathDot(ef1+ef2, ef1+ef2), this.PathDot(ef1, 1.0));
					path.LineTo(p2);
					break;

				case CornerType.Fantasy51:
					path.ArcTo(this.PathDot(1.0, ef1), this.PathDot(ef1, ef1));
					path.LineTo(this.PathDot(0.0, ef2));
					path.ArcTo(c, this.PathDot(ef2, 0.0));
					path.LineTo(this.PathDot(ef1, ef1));
					path.ArcTo(this.PathDot(ef1, 1.0), p2);
					break;

				case CornerType.Fantasy61:
					path.LineTo(this.PathDot(ef1, 0.0));
					path.LineTo(this.PathDot(ef2, 1.0));
					path.LineTo(this.PathDot(1.0, 1.0));
					path.LineTo(this.PathDot(1.0, ef2));
					path.LineTo(this.PathDot(0.0, ef1));
					path.LineTo(p2);
					break;

				case CornerType.Fantasy62:
					path.LineTo(this.PathDot(1.0, ef1));
					path.LineTo(this.PathDot(0.0, ef2));
					path.LineTo(c);
					path.LineTo(this.PathDot(ef2, 0.0));
					path.LineTo(this.PathDot(ef1, 1.0));
					path.LineTo(p2);
					break;

				default:
					path.LineTo(p2);
					break;
			}
		}

		// Calcule un point d'un coin.
		protected Drawing.Point PathDot(double f1, double f2)
		{
			Drawing.Point pp1 = Drawing.Point.Move(this.c, this.p1, f1*this.r);
			Drawing.Point pp2 = Drawing.Point.Move(this.c, this.p2, f2*this.r);
			return pp1+pp2-this.c;
		}


		protected CornerType			cornerType;
		protected double				radius;
		protected double				effect1;
		protected double				effect2;

		protected Drawing.Point			p1;
		protected Drawing.Point			c;
		protected Drawing.Point			p2;
		protected double				r;
	}
}
