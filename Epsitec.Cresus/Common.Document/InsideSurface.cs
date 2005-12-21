using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe InsideSurface permet de calculer si un point est dans une surface
	/// fermée quelconque constituée de segments de droites ou de courbes de Bézier.
	/// </summary>
	public class InsideSurface
	{
		public InsideSurface(Drawing.Point p, int max)
		{
			//	Constructeur. Il faut donner le point dont on désire savoir s'il est
			//	dans la surface, ainsi que le nombre maximum de lignes qui seront
			//	ajoutées. Une courbe de Bézier compte pour InsideSurface.bezierStep.
			this.p = p;
			this.total = 0;
			this.list = new double[max+10];
		}

		public void AddLine(Drawing.Point a, Drawing.Point b)
		{
			//	Ajoute un segment de droite.
			Drawing.Point i;
			if ( Drawing.Point.IntersectsWithHorizontal(a,b, this.p.Y, out i) )
			{
				if ( a.Y == b.Y )  return;  // ligne horizontale ?
				if ( this.total < this.list.Length )
				{
					this.list[this.total++] = i.X;
				}
			}
		}

		public void AddBezier(Drawing.Point p1, Drawing.Point s1, Drawing.Point s2, Drawing.Point p2)
		{
			//	Ajoute un segment de Bézier.
			Drawing.Point a = p1;
			double step = 1.0/InsideSurface.bezierStep;
			for ( double t=step ; t<1.0 ; t+=step )
			{
				Drawing.Point b = Drawing.Point.FromBezier(p1, s1, s2, p2, t);
				this.AddLine(a, b);
				a = b;
			}
			this.AddLine(a, p2);
		}

		public bool IsInside()
		{
			//	Indique si le point donné dans le constructeur est à l'intérieur de la surface.
			int nb = 0;
			for ( int i=0 ; i<this.total ; i++ )
			{
				if ( this.p.X < this.list[i] )  nb ++;
			}
			return ( nb%2 != 0 );  // magiqne, non ?
		}


		protected Drawing.Point			p;
		protected int					total;
		protected double[]				list;
		public static readonly int		bezierStep = 10;
	}
}
