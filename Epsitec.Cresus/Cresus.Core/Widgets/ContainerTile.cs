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
	public class ContainerTile : BackgroundTile
	{
		public ContainerTile()
		{
		}

		public ContainerTile(Widget embedder)
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
				return ContainerTile.arrowBreadth;
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

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Path outlinePath = this.GetFramePath (0.0 ,true);
			Path surfacePath = this.GetFramePath (0.5, false);
			Path enteredPath = this.HasMouseHilite && !this.HasRevertedArrow ? this.GetFramePath (2.0, true) : null;

			this.PaintPath (graphics, enteredPath, outlinePath, surfacePath);
		}

		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.HasRevertedArrow)
			{
				Path path = this.GetRevertedFramePath (0.0);
				Path enteredPath = this.HasMouseHilite ? this.GetRevertedFramePath (2.0) : null;

				this.PaintPath (graphics, enteredPath, path, path);
			}
		}

		private void PaintPath(Graphics graphics, Path enteredPath, Path outlinePath, Path surfacePath)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			//	Dessine toujours le fond.
			graphics.Rasterizer.AddSurface (surfacePath);
			graphics.RenderSolid (this.BackgroundColor);

			//	Dessine le hilite sous forme d'une jolie bordure orange, en accord avec l'adorner utilisé (mais pas les autres).
			if (enteredPath != null)
			{
				graphics.Rasterizer.AddSurface (enteredPath);
				graphics.RenderSolid (this.BackgroundSurfaceHilitedColor);

				graphics.Rasterizer.AddOutline (enteredPath, 3);
				graphics.RenderSolid (this.BackgroundOutlineHilitedColor);
			}

			//	Dessine le cadre.
			if (enteredPath != null || this.IsSelected)
			{
				graphics.Rasterizer.AddOutline (outlinePath);
				graphics.RenderSolid (adorner.ColorBorder);
			}
		}

		private Path GetFramePath(double deflate, bool outline)
		{
			Rectangle bounds = this.Client.Bounds;
			bounds.Deflate (deflate);

			if (this.HasArrow)
			{
				return ContainerTile.GetArrowPath (bounds, this.arrowLocation);
			}
			else
			{
				Path path = new Path ();

				Rectangle box;
				Point pick;
				ContainerTile.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick);

				path.AppendRectangle (box);

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

			return ContainerTile.GetArrowPath (bounds, arrowLocation);
		}

		private static Path GetArrowPath(Rectangle bounds, Direction arrowLocation)
		{
			Path path = new Path ();

			Rectangle box;
			Point pick;
			ContainerTile.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick);

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
					box = new Rectangle (bounds.Left+ContainerTile.arrowBreadth, bounds.Bottom, bounds.Width-ContainerTile.arrowBreadth, bounds.Height);
					pick = Point.Scale (bounds.TopLeft, bounds.BottomLeft, 0.5);
					break;

				case Direction.Right:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width-ContainerTile.arrowBreadth, bounds.Height);
					pick = Point.Scale (bounds.TopRight, bounds.BottomRight, 0.5);
					break;

				case Direction.Up:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-ContainerTile.arrowBreadth);
					pick = Point.Scale (bounds.TopLeft, bounds.TopRight, 0.5);
					break;

				case Direction.Down:
					box = new Rectangle (bounds.Left, bounds.Bottom+ContainerTile.arrowBreadth, bounds.Width, bounds.Height-ContainerTile.arrowBreadth);
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
		private bool enteredSensitivity;
	}
}
