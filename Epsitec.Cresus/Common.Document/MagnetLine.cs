using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	//	Ligne magnétique détectée.
	public class MagnetLine
	{
		public enum Type
		{
			Constrain,	// contrainte
			Circle,		// cercle (contrainte de distance)
			Main,		// segment principal détecté
			Perp,		// perpendiculaire
			Inter,		// intersection
			Proj,		// projection
		}

		public MagnetLine(Document document, DrawingContext context, Type type)
		{
			//	Constructeur qui fabrique une ligne magnétique libre et invisible.
			this.document = document;
			this.context = context;
			this.type = type;
			this.isUsed = false;
			this.isVisible = false;
			this.infinite = false;
			this.temp = false;
			this.flyOver = false;
			this.handleRank = -1;
		}

		public void Clear()
		{
			//	Ligne plus utilisée.
			if ( !this.isUsed )  return;
			this.Invalidate();
			this.isUsed = false;
			this.isVisible = false;
		}

		public bool IsUsed
		{
			//	Indique si la ligne est utilisée.
			get { return this.isUsed; }
		}

		public bool IsMain
		{
			//	Indique s'il s'agit de la ligne principale.
			get { return this.type == Type.Main; }
		}

		public bool IsCircle
		{
			//	Indique s'il s'agit d'un cercle de distance.
			get { return this.type == Type.Circle; }
		}

		public bool IsVisible
		{
			//	Indique si la ligne est visible.
			get
			{
				return this.isVisible;
			}

			set
			{
				if ( this.isVisible != value )
				{
					this.Invalidate();
					this.isVisible = value;
					this.Invalidate();
				}
			}
		}

		public int HandleRank
		{
			//	Retourne le rang de la poignée associée.
			get
			{
				return this.handleRank;
			}
		}

		public bool Infinite
		{
			//	Ligne se poursuivant à l'infini, c'est-à-dire au-delà de
			//	p1 et p2 dans les 2 directions.
			get
			{
				return this.infinite;
			}

			set
			{
				if ( this.infinite != value )
				{
					this.Invalidate();
					this.infinite = value;
					this.Invalidate();
				}
			}
		}

		public bool Temp
		{
			//	Flag temporaire.
			get
			{
				return this.temp;
			}

			set
			{
				this.temp = value;
			}
		}

		public bool FlyOver
		{
			//	Ligne survolée par la souris.
			get
			{
				return this.flyOver;
			}

			set
			{
				if ( this.flyOver != value )
				{
					this.Invalidate();
					this.flyOver = value;
					this.Invalidate();
				}
			}
		}

		public void Initialize(Point p1, Point p2, bool infinite)
		{
			//	Initialise la ligne et rend-la visible.
			this.Initialize(p1, p2, infinite, true, -1);
		}

		public void Initialize(Point p1, Point p2, bool infinite, bool isVisible)
		{
			//	Initialise la ligne visible ou invisible.
			this.Initialize(p1, p2, infinite, isVisible, -1);
		}

		public void Initialize(Point p1, Point p2, bool infinite, bool isVisible, int handleRank)
		{
			//	Initialise la ligne visible ou invisible, en donnant la poignée à l'origine de cette contrainte.
			if ( this.p1 != p1 ||
				 this.p2 != p2 ||
				 this.infinite != infinite ||
				 this.isVisible != isVisible ||
				 this.isUsed != true ||
				 this.handleRank != handleRank )
			{
				this.Invalidate();
				this.p1 = p1;
				this.p2 = p2;
				this.infinite = infinite;
				this.isVisible = isVisible;
				this.isUsed = true;
				this.handleRank = handleRank;
				this.Invalidate();
			}
		}

		public Point P1
		{
			//	Donne le loint de départ.
			get { return this.p1; }
		}

		public Point P2
		{
			//	Donne le point d'arrivée.
			get { return this.p2; }
		}

		protected void Invalidate()
		{
			//	Invalide la zone contenant la ligne.
			if ( !this.isUsed || !this.isVisible )  return;

			if ( this.infinite )
			{
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
			}
			else
			{
				Rectangle rect = new Rectangle(this.p1, this.p2);
				this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, rect);
			}
		}

		public bool Detect(Point pos, double margin)
		{
			//	Détecte si une position est sur la ligne. La distance doit être
			//	inférieure ou égale à margin.
			if ( !this.isUsed || !this.isVisible )  return false;

			Point proj = this.Projection(pos);
			return (Point.Distance(proj, pos) <= margin);
		}

		public double Distance(Point pos)
		{
			//	Calcule la distance la plus courte d'un point jusqu'à la ligne.
			if ( !this.isUsed || !this.isVisible )  return 1000000.0;

			Point proj = this.Projection(pos);
			return Point.Distance(proj, pos);
		}

		public Point Projection(Point pos)
		{
			//	Calcule la projection d'un point sur la ligne.
			if ( this.type == Type.Circle )
			{
				double radius = Point.Distance(this.p1, this.p2);
				return Point.Move(this.p1, pos, radius);
			}
			else
			{
				return Point.Projection(this.p1, this.p2, pos);
			}
		}

		static public Point[] Intersect(MagnetLine line1, MagnetLine line2)
		{
			//	Calcule l'intersection entre deux lignes magnétiques.
			//	Est capable de trouver les intersections de deux droites, d'une droite
			//	et d'un cercle et de deux cercles.
			if ( line1.IsCircle && line2.IsCircle )  // 2 cercles ?
			{
				double r1 = Point.Distance(line1.p1, line1.p2);
				double r2 = Point.Distance(line2.p1, line2.p2);
				return Geometry.Intersect(line1.p1, r1, line2.p1, r2);
			}
			else if ( line1.IsCircle || line2.IsCircle )  // 1 cercle et 1 ligne ?
			{
				MagnetLine circle, line;
				if ( line1.IsCircle )
				{
					circle = line1;
					line   = line2;
				}
				else
				{
					circle = line2;
					line   = line1;
				}

				double r = Point.Distance(circle.p1, circle.p2);
				return Geometry.Intersect(line.p1, line.p2, circle.p1, r);
			}
			else	// 2 lignes ?
			{
				return Geometry.Intersect(line1.p1, line1.p2, line2.p1, line2.p2);
			}
		}

		public bool Snap(ref Point pos, double margin)
		{
			//	Essaie de pousser une position sur la ligne, si la distance est
			//	inférieure ou égale à margin.
			if ( !this.isUsed || !this.isVisible )  return false;

			Point proj = this.Projection(pos);
			if ( Point.Distance(proj, pos) > margin )  return false;
			pos = proj;
			return true;
		}

		public bool Compare(MagnetLine line)
		{
			//	Compare la géométrie de deux lignes magnétiques.
			//	Les types sont ignorés.
			if ( this.handleRank != line.handleRank )  return false;

			if ( this.p1.Y == this.p2.Y )  // horizontal ?
			{
				if ( line.p1.Y != line.p2.Y )  return false;
				return this.p1.Y == line.p1.Y;
			}
			else if ( this.p1.X == this.p2.X )  // vertical ?
			{
				if ( line.p1.X != line.p2.X )  return false;
				return this.p1.X == line.p1.X;
			}
			else	// quelconque ?
			{
				double delta1 = (this.p2.Y-this.p1.Y)/(this.p2.X-this.p1.X);
				double delta2 = (line.p2.Y-line.p1.Y)/(line.p2.X-line.p1.X);
				if ( System.Math.Abs(delta1-delta2) > 0.00001 )  return false;

				double start1 = ((this.p1.Y-this.p2.Y)*this.p1.X/(this.p2.X-this.p1.X))+this.p1.Y;
				double start2 = ((line.p1.Y-line.p2.Y)*line.p1.X/(line.p2.X-line.p1.X))+line.p1.Y;
				if ( System.Math.Abs(start1-start2) > 0.00001 )  return false;
			}
			return true;
		}

		public void Draw(Graphics graphics, double max)
		{
			//	Dessine la ligne.
			if ( !this.isUsed || !this.isVisible )  return;

			Point p1 = this.p1;
			Point p2 = this.p2;

			Point pp1 = p1;
			Point pp2 = p2;

			if ( this.Infinite )
			{
				pp1 = Point.Move(p1, p2, -max);
				pp2 = Point.Move(p2, p1, -max);
			}

			if ( this.type == Type.Constrain )  // contrainte ?
			{
				if ( this.flyOver )
				{
					graphics.LineWidth = 2.0/this.context.ScaleX;
					graphics.AddLine(pp1, pp2);
					graphics.RenderSolid(DrawingContext.ColorConstrain);
				}
				else
				{
					Path path = new Path();
					path.MoveTo(pp1);
					path.LineTo(pp2);
					Drawer.DrawPathDash(graphics, context, path, 1.0, 0.0, 3.0, DrawingContext.ColorConstrain);
				}
			}

			if ( this.type == Type.Circle )  // contrainte de distance ?
			{
				if ( this.flyOver )
				{
					graphics.LineWidth = 2.0/this.context.ScaleX;
					graphics.AddCircle(p1, Point.Distance(p1,p2));
					graphics.RenderSolid(DrawingContext.ColorConstrain);
				}
				else
				{
					Path path = new Path();
					path.AppendCircle(p1, Point.Distance(p1,p2));
					Drawer.DrawPathDash(graphics, context, path, 1.0, 0.0, 3.0, DrawingContext.ColorConstrain);
				}
			}

			if ( this.type == Type.Main )  // ligne principale ?
			{
				graphics.LineWidth = 1.0/this.context.ScaleX;
				graphics.AddLine(pp1, pp2);
				graphics.RenderSolid(DrawingContext.ColorConstrain);

				if ( this.Infinite )
				{
					graphics.LineWidth = 3.0/this.context.ScaleX;
					graphics.AddLine(p1, p2);
					graphics.RenderSolid(DrawingContext.ColorConstrain);
				}
			}

			if ( this.type == Type.Perp )  // perpendiculaire ?
			{
				if ( this.Infinite )
				{
					Path path = new Path();
					path.MoveTo(pp1);
					path.LineTo(pp2);
					Drawer.DrawPathDash(graphics, context, path, 1.0, 0.0, 3.0, DrawingContext.ColorConstrain);
				}

				graphics.LineWidth = 1.0/this.context.ScaleX;
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(DrawingContext.ColorConstrain);
			}

			if ( this.type == Type.Inter )  // intersection ?
			{
				Path path = new Path();
				path.MoveTo(pp1);
				path.LineTo(pp2);
				Drawer.DrawPathDash(graphics, context, path, 2.0, 4.0, 4.0, DrawingContext.ColorConstrain);
			}

			if ( this.type == Type.Proj )  // projection ?
			{
				graphics.LineWidth = 4.0/this.context.ScaleX;
				graphics.AddLine(pp1, pp2);
				graphics.RenderSolid(DrawingContext.ColorConstrain);
			}
		}

		protected Document				document;
		protected DrawingContext		context;
		protected Type					type;
		protected bool					isUsed;
		protected bool					isVisible;
		protected bool					infinite;
		protected bool					temp;
		protected bool					flyOver;
		protected Point					p1;
		protected Point					p2;
		protected int					handleRank;
	}
}
