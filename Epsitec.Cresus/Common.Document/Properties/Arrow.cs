using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plantée lors de la désérialisation.
	public enum ArrowType
	{
		None          = 0,
		Right         = 1,
		ArrowSimply   = 2,
		ArrowFantasy1 = 3,
		ArrowCurve1   = 4,
		ArrowCurve2   = 5,
		ArrowOutline  = 6,
		Slash         = 7,
		Dot           = 8,
		Square        = 9,
		Diamond       = 10,
	}

	/// <summary>
	/// La classe Arrow représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Arrow : Abstract
	{
		public Arrow(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.arrowType = new ArrowType[2];
			this.length    = new double[2];
			this.effect1   = new double[2];
			this.effect2   = new double[2];

			this.arrowType[0] = ArrowType.Right;
			this.arrowType[1] = ArrowType.Right;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.length[0] = 3.0;
				this.length[1] = 3.0;
			}
			else
			{
				this.length[0] = 50.0;
				this.length[1] = 50.0;
			}
			this.effect1[0] = 0.5;
			this.effect1[1] = 0.5;
			this.effect2[0] = 0.5;
			this.effect2[1] = 0.5;
		}

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
					this.NotifyBefore();
					this.arrowType[0] = value;
					this.NotifyAfter();
				}
			}
		}

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
					this.NotifyBefore();
					this.length[0] = value;
					this.NotifyAfter();
				}
			}
		}

		public double Effect11
		{
			get
			{
				return this.effect1[0];
			}
			
			set
			{
				if ( this.effect1[0] != value )
				{
					this.NotifyBefore();
					this.effect1[0] = value;
					this.NotifyAfter();
				}
			}
		}

		public double Effect12
		{
			get
			{
				return this.effect2[0];
			}
			
			set
			{
				if ( this.effect2[0] != value )
				{
					this.NotifyBefore();
					this.effect2[0] = value;
					this.NotifyAfter();
				}
			}
		}


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
					this.NotifyBefore();
					this.arrowType[1] = value;
					this.NotifyAfter();
				}
			}
		}

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
					this.NotifyBefore();
					this.length[1] = value;
					this.NotifyAfter();
				}
			}
		}

		public double Effect21
		{
			get
			{
				return this.effect1[1];
			}
			
			set
			{
				if ( this.effect1[1] != value )
				{
					this.NotifyBefore();
					this.effect1[1] = value;
					this.NotifyAfter();
				}
			}
		}

		public double Effect22
		{
			get
			{
				return this.effect2[1];
			}
			
			set
			{
				if ( this.effect2[1] != value )
				{
					this.NotifyBefore();
					this.effect2[1] = value;
					this.NotifyAfter();
				}
			}
		}


		public ArrowType GetArrowType(int extremity)
		{
			return this.arrowType[extremity];
		}

		public void SetArrowType(int extremity, ArrowType type)
		{
			if ( this.arrowType[extremity] != type )
			{
				this.NotifyBefore();
				this.arrowType[extremity] = type;
				this.NotifyAfter();
			}
		}

		public double GetLength(int extremity)
		{
			return this.length[extremity];
		}

		public void SetLength(int extremity, double length)
		{
			if ( this.length[extremity] != length )
			{
				this.NotifyBefore();
				this.length[extremity] = length;
				this.NotifyAfter();
			}
		}

		public double GetEffect1(int extremity)
		{
			return this.effect1[extremity];
		}

		public void SetEffect1(int extremity, double effect)
		{
			if ( this.effect1[extremity] != effect )
			{
				this.NotifyBefore();
				this.effect1[extremity] = effect;
				this.NotifyAfter();
			}
		}

		public double GetEffect2(int extremity)
		{
			return this.effect2[extremity];
		}

		public void SetEffect2(int extremity, double effect)
		{
			if ( this.effect2[extremity] != effect )
			{
				this.NotifyBefore();
				this.effect2[extremity] = effect;
				this.NotifyAfter();
			}
		}


		// Détermine le nom de la propriété dans la liste (Lister).
		public string GetListName()
		{
			return Arrow.GetName(this.arrowType[0]) + ", " +
				   Arrow.GetName(this.arrowType[1]);
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
				ArrowType t = Arrow.ConvType(i);
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
		public override bool AlterBoundingBox
		{
			get { return true; }
		}


		// Nombre de poignées.
		public override int TotalHandle(Objects.Abstract obj)
		{
			return 2;
		}

		// Indique si une poignée est visible.
		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			return (this.arrowType[rank] != ArrowType.Right);
		}
		
		// Retourne la position d'une poignée.
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			Point pos = new Point();

			if ( obj is Objects.Line )
			{
				int r1 = 0, r2 = 0;
				if ( rank == 0 )
				{
					r1 = 0;
					r2 = 1;
				}
				if ( rank == 1 )
				{
					r1 = 1;
					r2 = 0;
				}
				pos = Point.Move(obj.Handle(r1).Position, obj.Handle(r2).Position, this.length[rank]);
			}

			if ( obj is Objects.Poly )
			{
				int r1 = 0, r2 = 0;
				if ( rank == 0 )
				{
					r1 = 0;
					r2 = 1;
				}
				if ( rank == 1 )
				{
					r1 = obj.TotalMainHandle-1;
					r2 = obj.TotalMainHandle-2;
				}
				pos = Point.Move(obj.Handle(r1).Position, obj.Handle(r2).Position, this.length[rank]);
			}

			if ( obj is Objects.Bezier )
			{
				Objects.Bezier bezier = obj as Objects.Bezier;
				Point a, b;
				bezier.ComputeExtremity((rank==0), out a, out b);
				pos = Point.Move(a, b, this.length[rank]);
			}

			return pos;
		}

		// Modifie la position d'une poignée.
		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			int r1 = 0;

			if ( obj is Objects.Line )
			{
				r1 = rank;
			}

			if ( obj is Objects.Poly )
			{
				if ( rank == 0 )
				{
					r1 = 0;
				}
				if ( rank == 1 )
				{
					r1 = obj.TotalMainHandle-1;
				}
			}

			if ( obj is Objects.Bezier )
			{
				if ( rank == 0 )
				{
					r1 = 1;
				}
				if ( rank == 1 )
				{
					r1 = obj.TotalMainHandle-2;
				}
			}

			this.SetLength(rank, Point.Distance(obj.Handle(r1).Position, pos));

			base.SetHandlePosition(obj, rank, pos);
		}
		
		
		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Arrow p = property as Arrow;
			for ( int j=0 ; j<2 ; j++ )
			{
				p.arrowType[j] = this.arrowType[j];
				p.length[j]    = this.length[j];
				p.effect1[j]   = this.effect1[j];
				p.effect2[j]   = this.effect2[j];
			}
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Arrow p = property as Arrow;
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
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Arrow(document);
		}


		// Crée le chemin à l'extrémité p1, et retourne pp1, le remplaçant de p1.
		public Point PathExtremity(Path path, int extremity,
								   double width, CapStyle cap,
								   Point p1, Point p2,
								   out bool outline, out bool surface)
		{
			outline = false;
			surface = false;
			if ( this.arrowType[extremity] == ArrowType.Right )  return p1;
			if ( System.Math.Abs(this.length[extremity]) < 0.0001 )  return p1;

			Point pp;
			double len = this.length[extremity];
			double ef1 = this.effect1[extremity];
			double ef2 = this.effect2[extremity];
			double mx2 = System.Math.Max(ef2, 0);
			double limit;

			switch ( this.arrowType[extremity] )
			{
				case ArrowType.ArrowSimply:
					limit = this.LimitPara(extremity, width, p1, p2);
					pp = Arrow.Extremity(p1, p2, len*System.Math.Max(ef2, limit), 0);
					path.MoveTo(pp);
					path.LineTo(Arrow.Extremity(p1, p2, len, len*ef1));
					path.LineTo(p1);
					path.LineTo(Arrow.Extremity(p1, p2, len, -len*ef1));
					path.Close();
					if ( cap == CapStyle.Square )
					{
						pp = Point.Move(pp, p2, width/2);
					}
					p1 = pp;
					surface = true;
					break;

				case ArrowType.ArrowFantasy1:
					limit = this.LimitPara(extremity, width, p1, p2);
					pp = Arrow.Extremity(p1, p2, len*System.Math.Max(ef2, limit), 0.0);
					path.MoveTo(pp);
					path.LineTo(Arrow.Extremity(p1, p2, len+len*ef2, len*ef1));
					path.LineTo(Arrow.Extremity(p1, p2, len, len*ef1));
					path.LineTo(p1);
					path.LineTo(Arrow.Extremity(p1, p2, len, -len*ef1));
					path.LineTo(Arrow.Extremity(p1, p2, len+len*ef2, -len*ef1));
					path.Close();
					if ( cap == CapStyle.Square )
					{
						pp = Point.Move(pp, p2, width/2);
					}
					p1 = pp;
					surface = true;
					break;

				case ArrowType.ArrowCurve1:
					limit = this.LimitPara(extremity, width, p1, p2);
					limit = System.Math.Max(0.25, limit);
					pp = Arrow.Extremity(p1, p2, len*limit, 0.0);
					path.MoveTo(pp);
					path.CurveTo(Arrow.Extremity(p1, p2, len*(mx2+limit), 0),
								 Arrow.Extremity(p1, p2, len*(1-ef2), 0),
								 Arrow.Extremity(p1, p2, len, len*ef1));
					path.LineTo(p1);
					path.LineTo(Arrow.Extremity(p1, p2, len, -len*ef1));
					path.CurveTo(Arrow.Extremity(p1, p2, len*(1-ef2), 0),
								 Arrow.Extremity(p1, p2, len*(mx2+limit), 0),
								 pp);
					path.Close();
					if ( cap != CapStyle.Butt )
					{
						pp = Point.Move(pp, p2, width/2);
					}
					p1 = pp;
					surface = true;
					break;

				case ArrowType.ArrowCurve2:
					pp = Arrow.Extremity(p1, p2, len, 0.0);
					path.MoveTo(pp);
					path.LineTo(Arrow.Extremity(p1, p2, len, len*ef1));
					path.CurveTo(Arrow.Extremity(p1, p2, len, len*ef1*(1-ef2)),
								 Arrow.Extremity(p1, p2, len*mx2, 0),
								 p1);
					path.CurveTo(Arrow.Extremity(p1, p2, len*mx2, 0),
								 Arrow.Extremity(p1, p2, len, -len*ef1*(1-ef2)),
								 Arrow.Extremity(p1, p2, len, -len*ef1));
					path.Close();
					if ( cap == CapStyle.Square )
					{
						pp = Point.Move(pp, p2, width/2);
					}
					p1 = pp;
					surface = true;
					break;

				case ArrowType.ArrowOutline:
					path.MoveTo(Arrow.Extremity(p1, p2, len, len*ef1));
					path.LineTo(p1);
					path.LineTo(Arrow.Extremity(p1, p2, len, -len*ef1));
					outline = true;
					break;

				case ArrowType.Slash:
					path.MoveTo(Arrow.Extremity(p1, p2, len*ef1, len));
					path.LineTo(Arrow.Extremity(p1, p2, -len*ef1, -len));
					outline = true;
					break;

				case ArrowType.Dot:
					path.AppendCircle(p1, len);
					surface = true;
					break;

				case ArrowType.Square:
					path.MoveTo(Arrow.Extremity(p1, p2, -len, -len));
					path.LineTo(Arrow.Extremity(p1, p2, -len,  len));
					path.LineTo(Arrow.Extremity(p1, p2,  len,  len));
					path.LineTo(Arrow.Extremity(p1, p2,  len, -len));
					path.Close();
					if ( cap == CapStyle.Square )
					{
						p1 = Point.Move(p1, p2, width/2);
					}
					surface = true;
					break;

				case ArrowType.Diamond:
					path.MoveTo(Arrow.Extremity(p1, p2, -len, 0));
					path.LineTo(Arrow.Extremity(p1, p2, 0, len));
					path.LineTo(Arrow.Extremity(p1, p2, len, 0));
					path.LineTo(Arrow.Extremity(p1, p2, 0, -len));
					path.Close();
					surface = true;
					break;
			}

			return p1;
		}

		// Calcule l'effet limite parallèlement à p1-p2.
		protected double LimitPara(int extremity, double width,
								   Point p1, Point p2)
		{
			double len = this.length[extremity];
			double ef1 = this.effect1[extremity];
			Point pa = Arrow.Extremity(p1, p2, len, len*ef1);
			Point pp1 = Point.Move(p1, p2, len);
			double d = Point.Distance(pa, pp1);
			if ( d == 0 )  return 0;
			return (width/2)/d;
		}

		// Calcule l'extrémité gauche ou droite d'une flèche.
		static protected Point Extremity(Point p1, Point p2,
												 double distPara, double distPerp)
		{
			Point c = Point.Move(p1, p2, distPara);
			Point p = Point.Move(c, c+p2-p1, System.Math.Abs(distPerp));
			double angle = (distPerp > 0) ? 90 : -90;
			return Transform.RotatePointDeg(c, angle, p);
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("ArrowType", this.arrowType, typeof(ArrowType[]));
			if ( this.arrowType[0] != ArrowType.Right ||
				 this.arrowType[1] != ArrowType.Right )
			{
				info.AddValue("Length", this.length, typeof(double[]));
				info.AddValue("Effect1", this.effect1, typeof(double[]));
				info.AddValue("Effect2", this.effect2, typeof(double[]));
			}
		}

		// Constructeur qui désérialise la propriété.
		protected Arrow(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.arrowType = (ArrowType[]) info.GetValue("ArrowType", typeof(ArrowType[]));
			if ( this.arrowType[0] != ArrowType.Right ||
				 this.arrowType[1] != ArrowType.Right )
			{
				this.length = (double[]) info.GetValue("Length", typeof(double[]));
				this.effect1 = (double[]) info.GetValue("Effect1", typeof(double[]));
				this.effect2 = (double[]) info.GetValue("Effect2", typeof(double[]));
			}
		}
		#endregion

	
		protected ArrowType[]			arrowType;
		protected double[]				length;
		protected double[]				effect1;
		protected double[]				effect2;
	}
}
