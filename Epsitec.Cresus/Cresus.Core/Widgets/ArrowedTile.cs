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
	public enum PaintingArrowMode
	{
		None,
		Normal,
		Revert,
	}


	/// <summary>
	/// Conteneur de base pour toutes les tuiles, qui s'occupe de dessiner l'éventuelle flèche.
	/// </summary>
	public class ArrowedTile : FrameBox
	{
		public ArrowedTile()
		{
		}

		public ArrowedTile(Widget embedder)
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
				return ArrowedTile.arrowBreadth;
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


		/// <summary>
		/// Détermine le côté sur lequel s'affiche la flèche. Si la flèche n'est pas dessinée, le côté
		/// correspondant aura un vide.
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

		public PaintingArrowMode PaintingArrowMode
		{
			get
			{
				return this.paintingArrowMode;
			}
			set
			{
				if (this.paintingArrowMode != value)
				{
					this.paintingArrowMode = value;

					this.Invalidate ();
				}
			}
		}


		public Color SurfaceColor
		{
			get
			{
				return this.surfaceColor;
			}
			set
			{
				if (this.surfaceColor != value)
				{
					this.surfaceColor = value;

					this.Invalidate ();
				}
			}
		}

		public Color OutlineColor
		{
			get
			{
				return this.outlineColor;
			}
			set
			{
				if (this.outlineColor != value)
				{
					this.outlineColor = value;

					this.Invalidate ();
				}
			}
		}

		public Color ThicknessColor
		{
			get
			{
				return this.thicknessColor;
			}
			set
			{
				if (this.thicknessColor != value)
				{
					this.thicknessColor = value;

					this.Invalidate ();
				}
			}
		}


		public Color RevertSurfaceColor
		{
			get
			{
				return this.revertSurfaceColor;
			}
			set
			{
				if (this.revertSurfaceColor != value)
				{
					this.revertSurfaceColor = value;

					this.Invalidate ();
				}
			}
		}

		public Color RevertOutlineColor
		{
			get
			{
				return this.revertOutlineColor;
			}
			set
			{
				if (this.revertOutlineColor != value)
				{
					this.revertOutlineColor = value;

					this.Invalidate ();
				}
			}
		}

		public Color RevertThicknessColor
		{
			get
			{
				return this.revertThicknessColor;
			}
			set
			{
				if (this.revertThicknessColor != value)
				{
					this.revertThicknessColor = value;

					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.PaintArrow (graphics, clipRect, this.paintingArrowMode, this.thicknessColor, this.outlineColor, this.surfaceColor);
		}

		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.paintingArrowMode == Widgets.PaintingArrowMode.Revert)
			{
				this.PaintRevertArrow (graphics, clipRect, this.paintingArrowMode, this.revertThicknessColor, this.revertOutlineColor, this.revertSurfaceColor);
			}
		}


		protected void PaintArrow(Graphics graphics, Rectangle clipRect, PaintingArrowMode mode, Color thicknessColor, Color outlineColor, Color surfaceColor)
		{
			if (surfaceColor.IsValid)
			{
				graphics.Rasterizer.AddSurface (this.GetPath (0.5, mode));
				graphics.RenderSolid (surfaceColor);
			}

			if (thicknessColor.IsValid)
			{
				graphics.Rasterizer.AddOutline (this.GetPath (2.0, mode), 3);
				graphics.RenderSolid (thicknessColor);
			}

			if (outlineColor.IsValid)
			{
				graphics.Rasterizer.AddOutline (this.GetPath (0.0, mode));
				graphics.RenderSolid (outlineColor);
			}
		}

		protected void PaintRevertArrow(Graphics graphics, Rectangle clipRect, PaintingArrowMode mode, Color thicknessColor, Color outlineColor, Color surfaceColor)
		{
			if (surfaceColor.IsValid)
			{
				graphics.Rasterizer.AddSurface (this.GetRevertPath (0.5));
				graphics.RenderSolid (surfaceColor);
			}

			if (thicknessColor.IsValid)
			{
				graphics.Rasterizer.AddOutline (this.GetRevertPath (2.0), 3);
				graphics.RenderSolid (thicknessColor);
			}

			if (outlineColor.IsValid)
			{
				graphics.Rasterizer.AddOutline (this.GetRevertPath (0.0));
				graphics.RenderSolid (outlineColor);
			}
		}


		private Path GetPath(double deflate, PaintingArrowMode mode)
		{
			Rectangle bounds = this.Client.Bounds;
			bounds.Deflate (deflate);

			if (mode == Widgets.PaintingArrowMode.None)
			{
				Rectangle box;
				Point pick;
				ArrowedTile.ComputeArrowGeometry (bounds, this.arrowLocation, out box, out pick);

				Path path = new Path ();
				path.AppendRectangle (box);
				return path;
			}
			else
			{
				return ArrowedTile.GetArrowPath (bounds, this.arrowLocation);
			}
		}

		private Path GetRevertPath(double deflate)
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

			return ArrowedTile.GetArrowPath (bounds, arrowLocation);
		}

		private static Path GetArrowPath(Rectangle bounds, Direction arrowLocation)
		{
			Path path = new Path ();

			Rectangle box;
			Point pick;
			ArrowedTile.ComputeArrowGeometry (bounds, arrowLocation, out box, out pick);

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
					box = new Rectangle (bounds.Left+ArrowedTile.arrowBreadth, bounds.Bottom, bounds.Width-ArrowedTile.arrowBreadth, bounds.Height);
					pick = Point.Scale (bounds.TopLeft, bounds.BottomLeft, 0.5);
					break;

				case Direction.Right:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width-ArrowedTile.arrowBreadth, bounds.Height);
					pick = Point.Scale (bounds.TopRight, bounds.BottomRight, 0.5);
					break;

				case Direction.Up:
					box = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-ArrowedTile.arrowBreadth);
					pick = Point.Scale (bounds.TopLeft, bounds.TopRight, 0.5);
					break;

				case Direction.Down:
					box = new Rectangle (bounds.Left, bounds.Bottom+ArrowedTile.arrowBreadth, bounds.Width, bounds.Height-ArrowedTile.arrowBreadth);
					pick = Point.Scale (bounds.BottomLeft, bounds.BottomRight, 0.5);
					break;
			}
		}


		public static Color BorderColor
		{
			get
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				return adorner.ColorBorder;
			}
		}

		public static Color BackgroundSummaryColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffffff");
			}
		}

		public static Color BackgroundEditingColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("eef6ff");
			}
		}

		public static Color BackgroundSelectedGroupingColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("d8e8fe");
			}
		}

		public static Color BackgroundSelectedContainerColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("c6defe");
			}
		}

		public static Color BackgroundSurfaceHilitedColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffeec2");  // orange
			}
		}

		public static Color BackgroundOutlineHilitedColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffc83c");  // orange
			}
		}


		private static readonly double arrowBreadth = 8;

		private Direction arrowLocation;
		private PaintingArrowMode paintingArrowMode;

		private Color surfaceColor;
		private Color outlineColor;
		private Color thicknessColor;

		private Color revertSurfaceColor;
		private Color revertOutlineColor;
		private Color revertThicknessColor;
	}
}
