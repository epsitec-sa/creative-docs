using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
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
	/// La classe Corner représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Corner : Abstract
	{
		public Corner(Document document, Type type) : base(document, type)
		{
			this.cornerType = CornerType.Right;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.radius = 2.0;
			}
			else
			{
				this.radius = 50.0;
			}
			this.effect1 = 0.5;
			this.effect2 = 0.5;
		}

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
					this.NotifyBefore();
					this.cornerType = value;
					this.NotifyAfter();
				}
			}
		}

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
					this.NotifyBefore();
					this.radius = value;
					this.NotifyAfter();
				}
			}
		}

		public double Effect1
		{
			get
			{
				return this.effect1;
			}
			
			set
			{
				if ( this.effect1 != value )
				{
					this.NotifyBefore();
					this.effect1 = value;
					this.NotifyAfter();
				}
			}
		}

		public double Effect2
		{
			get
			{
				return this.effect2;
			}
			
			set
			{
				if ( this.effect2 != value )
				{
					this.NotifyBefore();
					this.effect2 = value;
					this.NotifyAfter();
				}
			}
		}

		// Détermine le nom de la propriété dans la liste (Lister).
		public string GetListName()
		{
			return Corner.GetName(this.cornerType);
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
				CornerType t = Corner.ConvType(i);
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
		public override bool AlterBoundingBox
		{
			get { return true; }
		}


		// Nombre de poignées.
		public override int TotalHandle(Objects.Abstract obj)
		{
			if ( obj is Objects.Rectangle )  return 8;
			return 2;
		}

		// Indique si une poignée est visible.
		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			if ( obj is Objects.Poly )
			{
				return (this.cornerType != CornerType.Right && obj.TotalMainHandle > 2);
			}

			return (this.cornerType != CornerType.Right);
		}
		
		// Retourne la position d'une poignée.
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			Point pos = new Point();

			if ( obj is Objects.Rectangle )
			{
				int r1=0, r2=0;
				switch ( rank )
				{
					case 0:  r1 = 0;  r2 = 3;  break;
					case 1:  r1 = 0;  r2 = 2;  break;
					case 2:  r1 = 2;  r2 = 0;  break;
					case 3:  r1 = 2;  r2 = 1;  break;
					case 4:  r1 = 1;  r2 = 2;  break;
					case 5:  r1 = 1;  r2 = 3;  break;
					case 6:  r1 = 3;  r2 = 1;  break;
					case 7:  r1 = 3;  r2 = 0;  break;
				}
				pos = Point.Move(obj.Handle(r1).Position, obj.Handle(r2).Position, this.radius);
			}

			if ( obj is Objects.Poly )
			{
				int r1=0, r2=0;
				if ( rank == 0 )
				{
					r1 = 1;
					r2 = 0;
				}
				if ( rank == 1 )
				{
					r1 = 1;
					r2 = 2;
				}
				pos = Point.Move(obj.Handle(r1).Position, obj.Handle(r2).Position, this.radius);
			}

			if ( obj is Objects.Regular )
			{
				Objects.Regular regular = obj as Objects.Regular;
				Point a, b;
				regular.ComputeCorners(out a, out b);

				if ( rank == 0 )
				{
					pos = Point.Move(obj.Handle(1).Position, a, this.radius);
				}
				if ( rank == 1 )
				{
					pos = Point.Move(obj.Handle(1).Position, b, this.radius);
				}
			}

			return pos;
		}

		// Modifie la position d'une poignée.
		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			if ( obj is Objects.Rectangle )
			{
				if ( rank == 0 || rank == 1 )
				{
					this.Radius = Point.Distance(obj.Handle(0).Position, pos);
				}
				else if ( rank == 2 || rank == 3 )
				{
					this.Radius = Point.Distance(obj.Handle(2).Position, pos);
				}
				else if ( rank == 4 || rank == 5 )
				{
					this.Radius = Point.Distance(obj.Handle(1).Position, pos);
				}
				else if ( rank == 6 || rank == 7 )
				{
					this.Radius = Point.Distance(obj.Handle(3).Position, pos);
				}
			}

			if ( obj is Objects.Poly )
			{
				this.Radius = Point.Distance(obj.Handle(1).Position, pos);
			}

			if ( obj is Objects.Regular )
			{
				this.Radius = Point.Distance(obj.Handle(1).Position, pos);
			}

			base.SetHandlePosition(obj, rank, pos);
		}

		
		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Corner p = property as Corner;
			p.cornerType = this.cornerType;
			p.radius     = this.radius;
			p.effect1    = this.effect1;
			p.effect2    = this.effect2;
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Corner p = property as Corner;
			if ( p.cornerType != this.cornerType )  return false;
			if ( p.radius     != this.radius     )  return false;
			if ( p.effect1    != this.effect1    )  return false;
			if ( p.effect2    != this.effect2    )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Corner(document);
		}


		// Crée le chemin d'un coin.
		// Le rayon donné est plus petit ou égal à this.radius et correspond à la
		// distance p1/c ou p2/c.
		//	o p1
		//	|
		//	| c
		//	o-----o p2
		public void PathCorner(Path path, Point p1, Point c, Point p2, double radius)
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
		protected Point PathDot(double f1, double f2)
		{
			Point pp1 = Point.Move(this.c, this.p1, f1*this.r);
			Point pp2 = Point.Move(this.c, this.p2, f2*this.r);
			return pp1+pp2-this.c;
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("CornerType", this.cornerType);
			if ( this.cornerType != CornerType.Right )
			{
				info.AddValue("Radius", this.radius);
				info.AddValue("Effect1", this.effect1);
				info.AddValue("Effect2", this.effect2);
			}
		}

		// Constructeur qui désérialise la propriété.
		protected Corner(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.cornerType = (CornerType) info.GetValue("CornerType", typeof(CornerType));
			if ( this.cornerType != CornerType.Right )
			{
				this.radius  = info.GetDouble("Radius");
				this.effect1 = info.GetDouble("Effect1");
				this.effect2 = info.GetDouble("Effect2");
			}
			else
			{
				this.radius  = 2.0;
				this.effect1 = 0.5;
				this.effect2 = 0.5;
			}
		}
		#endregion

	
		protected CornerType			cornerType;
		protected double				radius;
		protected double				effect1;
		protected double				effect2;

		protected Point					p1;
		protected Point					c;
		protected Point					p2;
		protected double				r;
	}
}
