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
	public enum RectangleBordersShowedEnum
	{
		All   = 0x000f,
		Left  = 0x0001,
		Right = 0x0002,
		Up    = 0x0004,
		Down  = 0x0008,
	}


	/// <summary>
	/// Ce widget est un conteneur générique, qui peut être sélectionné. L'un de ses côté est
	/// alors une flèche (qui déborde de son Client.Bounds) qui pointe vers son enfant.
	/// </summary>
	public class TileContainer : FrameBox
	{
		public TileContainer()
		{
			this.rectangleBordersShowed = RectangleBordersShowedEnum.All;
		}

		public TileContainer(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		/// <summary>
		/// Marge supplémentaire nécessaire pour la flèche. Le côté dépend de ArrowLocation.
		/// </summary>
		/// <value>Epaisseur de la flèche.</value>
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
		/// <value>Position de la flèche.</value>
		public Direction ArrowLocation
		{
			get
			{
				return this.arrowLocation;
			}
			set
			{
				this.arrowLocation = value;
			}
		}

		/// <summary>
		/// Détermine quels sont les bords visibles lorsque le conteneur n'a pas de flèche et
		/// qu'il a alors la forme d'un simple rectangle.
		/// </summary>
		/// <value>The simples borders showed.</value>
		public RectangleBordersShowedEnum RectangleBordersShowed
		{
			get
			{
				return this.rectangleBordersShowed;
			}
			set
			{
				if (this.rectangleBordersShowed != value)
				{
					this.rectangleBordersShowed = value;
					this.Invalidate ();
				}
			}
		}

		/// <summary>
		/// Détermine si le widget est sensible au survol de la souris.
		/// </summary>
		/// <value><c>true</c> if [entered sensitivity]; otherwise, <c>false</c>.</value>
		public bool EnteredSensitivity
		{
			get
			{
				return this.enteredSensitivity;
			}
			set
			{
				this.enteredSensitivity = value;
			}
		}

		/// <summary>
		/// Indique si la tuile permet d'éditer une entité.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is editing; otherwise, <c>false</c>.
		/// </value>
		public bool IsEditing
		{
			get;
			set;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			bool isEntered = this.IsEntered && this.enteredSensitivity;
			Direction arrowLocation = this.arrowLocation;

			if (this.IsSelected && isEntered)
			{
				//	Si la flèche est visible et que la souris survole le widget, on inverse son sens.
				switch (arrowLocation)
				{
					case Direction.Left:
						arrowLocation = Direction.Right;
						break;

					case Direction.Right:
						arrowLocation = Direction.Left;
						break;

					case Direction.Up:
						arrowLocation = Direction.Down;
						break;

					case Direction.Down:
						arrowLocation = Direction.Up;
						break;
				}
			}

			Path outlinePath = this.GetFramePath (true,  arrowLocation);
			Path surfacePath = this.GetFramePath (false, arrowLocation);

			//	Dessine toujous le fond.
			Color backColor = this.BackColor;

			if (backColor.IsVisible == false)
			{
				backColor = adorner.ColorTextBackground;
			}

			graphics.Rasterizer.AddSurface (surfacePath);
			graphics.RenderSolid (backColor);

			//	En mode 'survolé' ou 'sélectionné', hilite le fond.
			if (isEntered || this.IsSelected || this.IsEditing)
			{
				double alpha = 0.05;

				if (isEntered)
				{
					alpha *= 2;
				}

				if (this.IsSelected)
				{
					alpha *= 4;
				}

				if (this.IsEditing)
				{
					alpha *= 2;
				}

				backColor = new Color (alpha, adorner.ColorCaption.R, adorner.ColorCaption.G, adorner.ColorCaption.B);

				graphics.Rasterizer.AddSurface (surfacePath);
				graphics.RenderSolid (backColor);
			}

			//	Dessine le cadre.
			graphics.Rasterizer.AddOutline (outlinePath);
			graphics.RenderSolid (adorner.ColorBorder);

		}

		/// <summary>
		/// Chemin permettant de dessiner la cadre du widget, avec ou sans flèche, selon l'état du widget.
		/// </summary>
		/// <value>The frame path.</value>
		private Path GetFramePath(bool outline, Direction arrowLocation)
		{
			Path path = new Path ();

			Rectangle box;
			Point p1, p2, p3;
			this.ComputeArrowGeometry (out box, out p1, out p2, out p3, arrowLocation);

			if (this.IsSelected)
			{
				switch (arrowLocation)
				{
					case Direction.Left:
						path.MoveTo (p2);
						//?path.LineTo (p3);
						path.LineTo (box.BottomLeft);
						path.LineTo (box.BottomRight);
						path.LineTo (box.TopRight);
						path.LineTo (box.TopLeft);
						//?path.LineTo (p1);
						path.Close ();
						break;

					case Direction.Right:
						path.MoveTo (p2);
						//?path.LineTo (p3);
						path.LineTo (box.TopRight);
						path.LineTo (box.TopLeft);
						path.LineTo (box.BottomLeft);
						path.LineTo (box.BottomRight);
						//?path.LineTo (p1);
						path.Close ();
						break;

					case Direction.Up:
						path.MoveTo (p2);
						//?path.LineTo (p3);
						path.LineTo (box.TopLeft);
						path.LineTo (box.BottomLeft);
						path.LineTo (box.BottomRight);
						path.LineTo (box.TopRight);
						//?path.LineTo (p1);
						path.Close ();
						break;

					case Direction.Down:
						path.MoveTo (p2);
						//?path.LineTo (p3);
						path.LineTo (box.BottomRight);
						path.LineTo (box.TopRight);
						path.LineTo (box.TopLeft);
						path.LineTo (box.BottomLeft);
						//?path.LineTo (p1);
						path.Close ();
						break;
				}
			}
			else
			{
				if (this.rectangleBordersShowed == RectangleBordersShowedEnum.All || outline == false)
				{
					path.AppendRectangle (box);
				}
				else
				{
					if ((this.rectangleBordersShowed & RectangleBordersShowedEnum.Left) != 0)
					{
						path.MoveTo (box.TopLeft);
						path.LineTo (box.BottomLeft);
					}

					if ((this.rectangleBordersShowed & RectangleBordersShowedEnum.Right) != 0)
					{
						path.MoveTo (box.BottomRight);
						path.LineTo (box.TopRight);
					}

					if ((this.rectangleBordersShowed & RectangleBordersShowedEnum.Up) != 0)
					{
						path.MoveTo (box.TopRight);
						path.LineTo (box.TopLeft);
					}

					if ((this.rectangleBordersShowed & RectangleBordersShowedEnum.Down) != 0)
					{
						path.MoveTo (box.BottomLeft);
						path.LineTo (box.BottomRight);
					}
				}
			}

			return path;
		}

		/// <summary>
		/// Calcule la géométrie pour la flèche. Les points p1, p2 et p3 sont dans le sens CCW.
		/// </summary>
		/// <param name="box">Boîte sans la flèche.</param>
		/// <param name="p1">Départ de la flèche.</param>
		/// <param name="p2">Pointe de la flèche.</param>
		/// <param name="p3">Arrivée de la flèche.</param>
		/// <value>The arrow rectangle.</value>
		private void ComputeArrowGeometry(out Rectangle box, out Point p1, out Point p2, out Point p3, Direction arrowLocation)
		{
			Rectangle bounds = this.Client.Bounds;
			bounds.Deflate (0.5);

			double width;

			switch (arrowLocation)
			{
				default:
				case Direction.Left:
					box = new Rectangle (bounds.Left+TileContainer.arrowBreadth, bounds.Bottom, bounds.Width-TileContainer.arrowBreadth, bounds.Height);
					width = System.Math.Min (TileContainer.arrowWidth, bounds.Height/2);
					p2 = Point.Scale (bounds.TopLeft, bounds.BottomLeft, 0.5);
					p1 = new Point (box.Left, p2.Y+width);
					p3 = new Point (box.Left, p2.Y-width);
					break;

				case Direction.Right:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width-TileContainer.arrowBreadth, bounds.Height);
					width = System.Math.Min (TileContainer.arrowWidth, bounds.Height/2);
					p2 = Point.Scale (bounds.TopRight, bounds.BottomRight, 0.5);
					p1 = new Point (box.Right, p2.Y-width);
					p3 = new Point (box.Right, p2.Y+width);
					break;

				case Direction.Up:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-TileContainer.arrowBreadth);
					width = System.Math.Min (TileContainer.arrowWidth, bounds.Width/2);
					p2 = Point.Scale (bounds.TopLeft, bounds.TopRight, 0.5);
					p1 = new Point (p2.X+width, box.Top);
					p3 = new Point (p2.X-width, box.Top);
					break;

				case Direction.Down:
					box = new Rectangle (bounds.Left, bounds.Bottom+TileContainer.arrowBreadth, bounds.Width, bounds.Height-TileContainer.arrowBreadth);
					width = System.Math.Min (TileContainer.arrowWidth, bounds.Width/2);
					p2 = Point.Scale (bounds.BottomLeft, bounds.BottomRight, 0.5);
					p1 = new Point (p2.X-width, box.Bottom);
					p3 = new Point (p2.X+width, box.Bottom);
					break;
			}
		}


		private static readonly double arrowWidth = 24;
		private static readonly double arrowBreadth = 8;

		private Direction arrowLocation;
		private RectangleBordersShowedEnum rectangleBordersShowed;
		private bool enteredSensitivity;
	}
}
