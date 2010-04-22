//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget est un conteneur générique, qui peut être sélectionné. L'un de ses côté est
	/// alors une flèche (qui déborde de son Client.Bounds) qui pointe vers son enfant.
	/// </summary>
	public class TileContainer : FrameBox
	{
		public TileContainer()
		{
		}

		public TileContainer(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		/// <summary>
		/// Marge supplémentaire nécessaire pour la flèche. Le côté dépend de ChildrenLocation.
		/// </summary>
		/// <value>The arrow deep.</value>
		public static double ArrowBreadth
		{
			get
			{
				return TileContainer.arrowBreadth;
			}
		}


		/// <summary>
		/// Détermine le côté sur lequel s'affiche la flèche.
		/// </summary>
		/// <value>The children location.</value>
		public Direction ChildrenLocation
		{
			get
			{
				return this.childrenLocation;
			}
			set
			{
				this.childrenLocation = value;
			}
		}

		/// <summary>
		/// Rectangle disponible pour le contenu, qui exclut donc la flèche.
		/// Si la flèche n'est pas dessinée, sa surface est toujours exclue.
		/// </summary>
		/// <value>The content rectangle.</value>
		public Rectangle ContentBounds
		{
			get
			{
				Rectangle box;
				Point p1, p2, p3;
				this.ArrowGeometry (out box, out p1, out p2, out p3);

				return box;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Path path = this.FramePath;

			//	Dessine toujous le fond.
			Color backColor = this.BackColor;

			if (backColor.IsVisible == false)
			{
				backColor = adorner.ColorTextBackground;
			}

			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid (backColor);

			//	En mode 'survolé' ou 'sélectionné', hilite le fond.
			if (this.IsEntered || this.IsSelected)
			{
				double alpha = 0.05;

				if (this.IsEntered)
				{
					alpha *= 2;
				}

				if (this.IsSelected)
				{
					alpha *= 4;
				}

				backColor = new Color (alpha, adorner.ColorCaption.R, adorner.ColorCaption.G, adorner.ColorCaption.B);

				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (backColor);
			}

			//	Dessine le cadre.
			graphics.Rasterizer.AddOutline (path);
			graphics.RenderSolid (adorner.ColorBorder);

		}

		/// <summary>
		/// Chemin permettant de dessiner la cadre du widget, avec ou sans flèche, selon l'état du widget.
		/// </summary>
		/// <value>The frame path.</value>
		private Path FramePath
		{
			get
			{
				Path path = new Path ();

				Rectangle box;
				Point p1, p2, p3;
				this.ArrowGeometry (out box, out p1, out p2, out p3);

				if (this.IsSelected)
				{
					switch (this.childrenLocation)
					{
						case Direction.Left:
							path.MoveTo (p2);
							path.LineTo (p3);
							path.LineTo (box.BottomLeft);
							path.LineTo (box.BottomRight);
							path.LineTo (box.TopRight);
							path.LineTo (box.TopLeft);
							path.LineTo (p1);
							path.Close ();
							break;

						case Direction.Right:
							path.MoveTo (p2);
							path.LineTo (p3);
							path.LineTo (box.TopRight);
							path.LineTo (box.TopLeft);
							path.LineTo (box.BottomLeft);
							path.LineTo (box.BottomRight);
							path.LineTo (p1);
							path.Close ();
							break;

						case Direction.Up:
							path.MoveTo (p2);
							path.LineTo (p3);
							path.LineTo (box.TopLeft);
							path.LineTo (box.BottomLeft);
							path.LineTo (box.BottomRight);
							path.LineTo (box.TopRight);
							path.LineTo (p1);
							path.Close ();
							break;

						case Direction.Down:
							path.MoveTo (p2);
							path.LineTo (p3);
							path.LineTo (box.BottomRight);
							path.LineTo (box.TopRight);
							path.LineTo (box.TopLeft);
							path.LineTo (box.BottomLeft);
							path.LineTo (p1);
							path.Close ();
							break;
					}
				}
				else
				{
					path.AppendRectangle (box);
				}

				return path;
			}
		}

		/// <summary>
		/// Calcule la géométrie pour la flèche. Les points p1, p2 et p3 sont dans le sens CCW.
		/// </summary>
		/// <param name="box">Boîte sans la flèche.</param>
		/// <param name="p1">Départ de la flèche.</param>
		/// <param name="p2">Pointe de la flèche.</param>
		/// <param name="p3">Arrivée de la flèche.</param>
		/// <value>The arrow rectangle.</value>
		private void ArrowGeometry(out Rectangle box, out Point p1, out Point p2, out Point p3)
		{
			Rectangle bounds = this.Client.Bounds;
			bounds.Deflate (0.5);

			double width;

			switch (this.childrenLocation)
			{
				default:
				case Direction.Left:
					box = new Rectangle (bounds.Left+TileContainer.arrowBreadth, bounds.Bottom, bounds.Width-TileContainer.arrowBreadth, bounds.Height);
					width = System.Math.Min (TileContainer.arrowWidth, bounds.Height);
					p2 = Point.Scale (bounds.TopLeft, bounds.BottomLeft, 0.5);
					p1 = new Point (box.Left, p2.Y+width);
					p3 = new Point (box.Left, p2.Y-width);
					break;

				case Direction.Right:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width-TileContainer.arrowBreadth, bounds.Height);
					width = System.Math.Min (TileContainer.arrowWidth, bounds.Height);
					p2 = Point.Scale (bounds.TopRight, bounds.BottomRight, 0.5);
					p1 = new Point (box.Right, p2.Y-width);
					p3 = new Point (box.Right, p2.Y+width);
					break;

				case Direction.Up:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-TileContainer.arrowBreadth);
					width = System.Math.Min (TileContainer.arrowWidth, bounds.Width);
					p2 = Point.Scale (bounds.TopLeft, bounds.TopRight, 0.5);
					p1 = new Point (p2.X+width, box.Top);
					p3 = new Point (p2.X-width, box.Top);
					break;

				case Direction.Down:
					box = new Rectangle (bounds.Left, bounds.Bottom+TileContainer.arrowBreadth, bounds.Width, bounds.Height-TileContainer.arrowBreadth);
					width = System.Math.Min (TileContainer.arrowWidth, bounds.Width);
					p2 = Point.Scale (bounds.BottomLeft, bounds.BottomRight, 0.5);
					p1 = new Point (p2.X-width, box.Bottom);
					p3 = new Point (p2.X+width, box.Bottom);
					break;
			}
		}


		private static readonly double arrowWidth = 24;
		private static readonly double arrowBreadth = 8;

		private Direction childrenLocation;
	}
}
