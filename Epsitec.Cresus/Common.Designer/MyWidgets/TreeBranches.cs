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
		public TreeBranches() : base()
		{
			this.branches = new List<TreeBranche>();
		}

		public TreeBranches(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public void AddBranche(double p1, double p2)
		{
			//	Ajoute une nouvelle liaison.
			//	p1 est la position supérieure en X.
			//	p2 est la position inférieure en X.
			this.branches.Add(new TreeBranche(p1, p2));
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			//	Dessine toutes les liaisons.
#if false
			foreach (TreeBranche branche in this.branches)
			{
				Point p1 = new Point(branche.P1, rect.Top);
				Point p2 = new Point(branche.P2, rect.Bottom);

				Misc.AlignForLine(graphics, ref p1);
				Misc.AlignForLine(graphics, ref p2);

				graphics.AddLine(p1, p2);
				graphics.RenderSolid(adorner.ColorBorder);

				p2.Y--;
				graphics.AddFilledCircle(p1, 3);
				graphics.AddFilledCircle(p2, 3);
				graphics.RenderSolid(adorner.ColorBorder);
			}
#else
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
#endif
		}


		public class TreeBranche
		{
			//	Classe décrivant une liaison, donc un trait.
			public TreeBranche(double p1, double p2)
			{
				this.p1 = p1;
				this.p2 = p2;
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

			protected double					p1;
			protected double					p2;
		}


		protected List<TreeBranche>				branches;
	}
}
