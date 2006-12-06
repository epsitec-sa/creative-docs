using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant de dessiner les branches d'un arbre de widgets.
	/// </summary>
	public class TreeBranches : Widget
	{
		public enum TreeBrancheStyle
		{
			Simple,		// droites simples
			Rich,		// droites avec cercles aux points d'attache
			Curve,		// courbes simples
		}


		public TreeBranches() : base()
		{
			this.style = TreeBrancheStyle.Curve;
			this.branches = new List<TreeBranche>();
		}

		public TreeBranches(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		public TreeBrancheStyle Style
		{
			//	Style de repr�sentation pour les liaisons.
			get
			{
				return this.style;
			}
			set
			{
				if (this.style != value)
				{
					this.style = value;
					this.Invalidate();
				}
			}
		}

		public void AddBranche(double p1, double p2)
		{
			//	Ajoute une nouvelle liaison.
			//	p1 est la position sup�rieure en X.
			//	p2 est la position inf�rieure en X.
			this.branches.Add(new TreeBranche(p1, p2));
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			//	Dessine toutes les liaisons.
			if (this.style == TreeBrancheStyle.Simple)
			{
				this.UpdateShift();

				foreach (TreeBranche branche in this.branches)
				{
					Point p1 = new Point(branche.P1+branche.Shift, rect.Top);
					Point p2 = new Point(branche.P2, rect.Bottom);

					Misc.AlignForLine(graphics, ref p1);
					Misc.AlignForLine(graphics, ref p2);

					graphics.AddLine(p1, p2);
				}

				graphics.RenderSolid(adorner.ColorBorder);
			}

			if (this.style == TreeBrancheStyle.Curve)
			{
				this.UpdateShift();

				Path path = new Path();
				double h = rect.Height*0.6;

				foreach (TreeBranche branche in this.branches)
				{
					Point p1 = new Point(branche.P1+branche.Shift, rect.Top);
					Point p2 = new Point(branche.P2, rect.Bottom);

					Misc.AlignForLine(graphics, ref p1);
					Misc.AlignForLine(graphics, ref p2);

					p1.Y += 0.5;
					p2.Y -= 0.5;

					path.MoveTo(p1);
					path.CurveTo(new Point(p1.X, p1.Y-h), new Point(p2.X, p2.Y+h), p2);
				}

				graphics.Rasterizer.AddOutline(path);
				graphics.RenderSolid(adorner.ColorBorder);
			}

			if (this.style == TreeBrancheStyle.Rich)
			{
				double r1 = 3;
				double r2 = 3;

				foreach (TreeBranche branche in this.branches)
				{
					Point p1 = new Point(branche.P1, rect.Top);
					Point p2 = new Point(branche.P2, rect.Bottom);
					p2.Y--;

					Misc.AlignForLine(graphics, ref p1);
					Misc.AlignForLine(graphics, ref p2);

					Point pp1 = Point.Move(p1, p2, r1);
					Point pp2 = Point.Move(p2, p1, r2);

					graphics.AddLine(pp1, pp2);
					graphics.RenderSolid(adorner.ColorBorder);

					graphics.AddCircle(p1, r1);
					graphics.AddCircle(p2, r2);
					graphics.RenderSolid(adorner.ColorBorder);
				}
			}
		}


		protected void UpdateShift()
		{
			//	Met � jour le d�calage, pour que les points de d�part d'un m�me
			//	parent soient r�partis.
			double last = -999;
			int first = -1;

			for (int i=0; i<=this.branches.Count; i++)
			{
				if (i == this.branches.Count || last != this.branches[i].P1)
				{
					if (first != -1 && i-first > 1)
					{
						double width = 10;
						double delta = width/(i-first-1);
						delta = System.Math.Min(3, delta);

						for (int j=first; j<i; j++)
						{
							this.branches[j].Shift = delta*((j-first)-(i-first-1)/2.0);
						}
					}

					if (i < this.branches.Count)
					{
						last = this.branches[i].P1;
						first = i;
					}
				}
			}
		}


		public class TreeBranche
		{
			//	Classe d�crivant une liaison, donc un trait.
			public TreeBranche(double p1, double p2)
			{
				this.p1 = p1;
				this.p2 = p2;
				this.shift = 0;
			}

			public double P1
			{
				get
				{
					return this.p1;
				}
			}

			public double P2
			{
				get
				{
					return this.p2;
				}
			}

			public double Shift
			{
				get
				{
					return this.shift;
				}
				set
				{
					this.shift = value;
				}
			}

			protected double					p1;
			protected double					p2;
			protected double					shift;
		}


		protected TreeBrancheStyle				style;
		protected List<TreeBranche>				branches;
	}
}
