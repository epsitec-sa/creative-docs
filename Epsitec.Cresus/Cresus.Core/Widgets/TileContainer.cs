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

		public bool ArrowEnabled
		{
			get;
			set;
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
			Path outlinePath = this.GetFramePath (0 ,true);
			Path surfacePath = this.GetFramePath (0, false);
			Path enteredPath = this.HasMouseHilite && !this.HasRevertedArrow ? this.GetFramePath (2, true) : null;

			double surfaceAlpha;

			if (this.HasRevertedArrow)
			{
				surfaceAlpha = 0.1;
			}
			else
			{
				surfaceAlpha = this.SurfaceAlpha;
			}

			this.PaintPath (graphics, enteredPath, outlinePath, surfacePath, surfaceAlpha);
		}

		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.HasRevertedArrow)
			{
				Path path = this.GetRevertedFramePath (0);
				Path enteredPath = this.HasMouseHilite ? this.GetRevertedFramePath (2) : null;

				this.PaintPath (graphics, enteredPath, path, path, 0.2);
			}
		}

		private void PaintPath(Graphics graphics, Path enteredPath, Path outlinePath, Path surfacePath, double surfaceAlpha)
		{
			//	Dessine toujous le fond.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Color backColor = this.BackColor;

			if (backColor.IsVisible == false)
			{
				backColor = adorner.ColorTextBackground;
			}

			graphics.Rasterizer.AddSurface (surfacePath);
			graphics.RenderSolid (backColor);

			//	En mode 'survolé' ou 'sélectionné', hilite le fond.
			if (this.HasMouseHilite || this.IsSelected || this.IsEditing)
			{
				backColor = new Color (surfaceAlpha, adorner.ColorCaption.R, adorner.ColorCaption.G, adorner.ColorCaption.B);

				graphics.Rasterizer.AddSurface (surfacePath);
				graphics.RenderSolid (backColor);
			}

			//	Dessine le hilite sous forme d'une jolie bordure orange, en accord avec l'adorner utilisé (mais pas les autres).
			// TODO: Adapter aux autres adorners
			if (enteredPath != null)
			{
				graphics.Rasterizer.AddOutline (enteredPath, 3);
				graphics.RenderSolid (Color.FromHexa ("ffc83c"));  // orange
			}

			//	Dessine le cadre.
			graphics.Rasterizer.AddOutline (outlinePath);
			graphics.RenderSolid (adorner.ColorBorder);
		}

		private double SurfaceAlpha
		{
			get
			{
				double alpha = 0.05;

				if (this.IsSelected)
				{
					alpha *= 2;
				}

				if (this.IsEditing)
				{
					alpha *= 2;
				}

				return alpha;
			}
		}

		private Path GetFramePath(double deflate, bool outline)
		{
			Rectangle bounds = this.Client.Bounds;
			bounds.Deflate (deflate);

			if (this.HasArrow)
			{
				return TileContainer.GetArrowPath (bounds, this.arrowLocation);
			}
			else
			{
				Path path = new Path ();

				Rectangle box;
				Point pick;
				TileContainer.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick);

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

				return path;
			}
		}

		private Path GetRevertedFramePath(double deflate)
		{
			Rectangle bounds = this.Client.Bounds;
			Direction arrowLocation = Direction.None;
			double revertedarrowBody;

			switch (this.arrowLocation)
			{
				case Direction.Left:
					arrowLocation = Direction.Right;
					revertedarrowBody = System.Math.Floor (bounds.Width*0.25);
					bounds = new Rectangle (bounds.Left, bounds.Bottom, revertedarrowBody, bounds.Height);
					break;

				case Direction.Right:
					arrowLocation = Direction.Left;
					revertedarrowBody = System.Math.Floor (bounds.Width*0.25);
					bounds = new Rectangle (bounds.Right-revertedarrowBody, bounds.Bottom, revertedarrowBody, bounds.Height);
					break;

				case Direction.Up:
					arrowLocation = Direction.Down;
					revertedarrowBody = System.Math.Floor (bounds.Height*0.25);
					bounds = new Rectangle (bounds.Left, bounds.Top-revertedarrowBody, bounds.Width, revertedarrowBody);
					break;

				case Direction.Down:
					arrowLocation = Direction.Up;
					revertedarrowBody = System.Math.Floor (bounds.Height*0.25);
					bounds = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, revertedarrowBody);
					break;
			}

			bounds.Deflate (deflate);

			return TileContainer.GetArrowPath (bounds, arrowLocation);
		}

		private static Path GetArrowPath(Rectangle bounds, Direction arrowLocation)
		{
			Path path = new Path ();

			Rectangle box;
			Point pick;
			TileContainer.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick);

			switch (arrowLocation)
			{
				case Direction.Left:
					path.MoveTo (pick);
					path.LineTo (box.BottomLeft);
					path.LineTo (box.BottomRight);
					path.LineTo (box.TopRight);
					path.LineTo (box.TopLeft);
					path.Close ();
					break;

				case Direction.Right:
					path.MoveTo (pick);
					path.LineTo (box.TopRight);
					path.LineTo (box.TopLeft);
					path.LineTo (box.BottomLeft);
					path.LineTo (box.BottomRight);
					path.Close ();
					break;

				case Direction.Up:
					path.MoveTo (pick);
					path.LineTo (box.TopLeft);
					path.LineTo (box.BottomLeft);
					path.LineTo (box.BottomRight);
					path.LineTo (box.TopRight);
					path.Close ();
					break;

				case Direction.Down:
					path.MoveTo (pick);
					path.LineTo (box.BottomRight);
					path.LineTo (box.TopRight);
					path.LineTo (box.TopLeft);
					path.LineTo (box.BottomLeft);
					path.Close ();
					break;
			}

			return path;
		}

		private static void ComputeArrowGeometry(Rectangle bounds, Direction arrowLocation, out Rectangle box, out Point pick)
		{
			bounds.Deflate (0.5);

			switch (arrowLocation)
			{
				default:
				case Direction.Left:
					box = new Rectangle (bounds.Left+TileContainer.arrowBreadth, bounds.Bottom, bounds.Width-TileContainer.arrowBreadth, bounds.Height);
					pick = Point.Scale (bounds.TopLeft, bounds.BottomLeft, 0.5);
					break;

				case Direction.Right:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width-TileContainer.arrowBreadth, bounds.Height);
					pick = Point.Scale (bounds.TopRight, bounds.BottomRight, 0.5);
					break;

				case Direction.Up:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-TileContainer.arrowBreadth);
					pick = Point.Scale (bounds.TopLeft, bounds.TopRight, 0.5);
					break;

				case Direction.Down:
					box = new Rectangle (bounds.Left, bounds.Bottom+TileContainer.arrowBreadth, bounds.Width, bounds.Height-TileContainer.arrowBreadth);
					pick = Point.Scale (bounds.BottomLeft, bounds.BottomRight, 0.5);
					break;
			}
		}


		private bool HasArrow
		{
			get
			{
				return this.ArrowEnabled && (this.IsSelected || this.IsEntered) && !this.HasRevertedArrow;
			}
		}

		private bool HasRevertedArrow
		{
			get
			{
				return this.ArrowEnabled && this.IsSelected && this.HasMouseHilite;
			}
		}

		private bool HasMouseHilite
		{
			get
			{
				return this.IsEntered && this.enteredSensitivity;
			}
		}


		private static readonly double arrowBreadth = 8;

		private Direction arrowLocation;
		private RectangleBordersShowedEnum rectangleBordersShowed;
		private bool enteredSensitivity;
	}
}
