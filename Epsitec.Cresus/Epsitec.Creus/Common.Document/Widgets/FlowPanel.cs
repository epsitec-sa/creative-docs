using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// Conteneur qui place les widgets contenus comme des caractères dans un texte.
	/// </summary>
	public class FlowPanel : Widget
	{
		public FlowPanel() : base()
		{
		}

		public FlowPanel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		public override Margins GetInternalPadding()
		{
			return new Drawing.Margins(2, 2, 2, 2);
		}

		protected override void MeasureMinMax(ref Size min, ref Size max)
		{
			//	Appelé dans une première passe pour prendre note des besoins du widget.
			base.MeasureMinMax(ref min, ref max);

			double h = this.Justif(false);  // calcule la hauteur nécessaire
			min.Height = h;
			max.Height = h;
		}

		protected override void ManualArrange()
		{
			//	Appelé dans une deuxième passe pour arranger les widgets.
			base.ManualArrange();

			this.Justif(true);  // déplace les widgets enfants
		}

		protected double Justif(bool setManuelBounds)
		{
			//	Justifie tous les widgets contenus comme des caractères dans un paragraphe en drapeau.
			//	Retourne la hauteur nécessaire.
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.Padding);
			rect.Deflate(this.GetInternalPadding());

			double x = 0;
			double y = 0;
			double dy = 0;
			int first = 0;
			int i = 0;
			while (i < this.Children.Count)
			{
				Widget child = this.Children[i] as Widget;

				double childWidth = child.PreferredWidth + child.Margins.Left + child.Margins.Right;
				double childHeight = child.PreferredHeight + child.Margins.Top + child.Margins.Bottom;

				if (x+childWidth <= rect.Width)  // assez de place ?
				{
					x += childWidth;
					dy = System.Math.Max(dy, childHeight);
					i++;
				}

				if (x+childWidth > rect.Width || i == this.Children.Count)  // dépasse à droite, ou dernier ?
				{
					if (i == first)  // aucune place ?
					{
						dy = childHeight;
						i++;
					}

					x = 0;
					for (int j=first; j<i; j++)
					{
						child = this.Children[j] as Widget;

						if (setManuelBounds)
						{
							child.SetManualBounds(new Drawing.Rectangle(rect.Left+x, rect.Top-y-child.PreferredHeight, child.PreferredWidth, child.PreferredHeight));
						}

						x += child.PreferredWidth + child.Margins.Left + child.Margins.Right;
					}

					first = i;
					x = 0;
					y += dy;
				}
			}

			return y;
		}


		public override void PaintHandler(Graphics graphics, Rectangle repaint, IPaintFilter paintFilter)
		{
			//	PaintHandler exécute dans l'ordre:
			//	1.	PaintBackgroundImplementation
			//	2.	PaintHandler de tous les enfants
			//	3.	PaintForegroundImplementation
			Rectangle initialClipping = graphics.SaveClippingRectangle();

			Drawing.Rectangle rect = this.MapClientToRoot(this.Client.Bounds);
			rect.Deflate(this.Padding);
			rect.Deflate(this.GetInternalPadding());
			graphics.SetClippingRectangle(rect);

			base.PaintHandler(graphics, repaint, paintFilter);

			graphics.RestoreClippingRectangle(initialClipping);
		}
	}
}
