//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Place de façon optimale des rectangles d'aperçu (Widgets.EntityPreviewer) dans une zone rectangulaire.
	/// Pour cela, on cherche à maximiser la surface des aperçus.
	/// </summary>
	public class OptimalPreviewPlacer
	{
		public OptimalPreviewPlacer(Rectangle availableSurface, Size pageSize, Size additionnalSize, double margin, int minimalHope)
		{
			//	pageSize détermine la proportion largeur/hauteur des widgets à placer.
			//	additionnalSize détermine une taille fixe et indépendante du zoom pour les widgets à placer.
			this.availableSurface = availableSurface;
			this.pageSize         = pageSize;
			this.additionnalSize  = additionnalSize;
			this.margin           = margin;
			this.minimalHope      = minimalHope;

			this.isDirty = true;
		}

		public int Total
		{
			//	Retourne le nombre de widgets que l'on peut placer.
			//	Ce nombre peut être supérieur à minimalHope.
			get
			{
				this.ComputeGeometry ();
				return this.totalColumns * this.totalRows;
			}
		}

		public Size Size
		{
			//	Retourne la taille d'un widget.
			get
			{
				this.ComputeGeometry ();
				return this.size;
			}
		}

		public Size GetZoomedSize(Size pageSize, double zoom)
		{
			//	Retourne la taille à utiliser pour afficher une seule page zoomée.
			double dx = this.availableSurface.Width  * zoom;
			double dy = this.availableSurface.Height * zoom;

			if (pageSize.Width/pageSize.Height < dx/dy)
			{
				dx = dy*pageSize.Width/pageSize.Height;
			}
			else
			{
				dy = dx*pageSize.Height/pageSize.Width;
			}

			return new Size (dx, dy);
		}

		public void UpdateGeometry<T>(List<T> widgets)
			where T: Widget
		{
			//	Positionne une liste de widgets selon la géométrie calculée. Le premier sera en haut à gauche,
			//	et les suivants comme les caractères d'un texte.
			this.ComputeGeometry ();

			int i = 0;

			for (int ny = 0; ny < this.totalRows; ny++)
			{
				for (int nx = 0; nx < this.totalColumns; nx++)
				{
					if (i >= widgets.Count)
					{
						break;
					}

					var widget = widgets[i++];

					double x = this.availableSurface.Left + (this.size.Width+this.margin)*nx;
					double y = this.availableSurface.Top - (this.size.Height+this.margin)*ny - this.size.Height;
					var bounds = new Rectangle (x, y, this.size.Width, this.size.Height);

					widget.SetManualBounds (bounds);
					widget.Invalidate ();  // pour forcer le dessin
				}
			}
		}


		private void ComputeGeometry()
		{
			if (!this.isDirty)
			{
				return;
			}

			double maxSurface = 0;
			this.size         = Size.Zero;
			this.totalColumns = 0;
			this.totalRows    = 0;

			for (int ny = 1; ny <= this.minimalHope; ny++)
			{
				for (int nx = 1; nx <= this.minimalHope; nx++)
				{
					if (nx*ny < this.minimalHope  ||  // insuffisant pour tout caser ?
						nx*ny > this.minimalHope*2)   // beaucoup trop ?
					{
						continue;
					}

					var size = this.GetSize (nx, ny);
					if (size.IsEmpty)
					{
						continue;
					}

					double surface = size.Width * size.Height;  // calcule la surface pour un preview

					if (maxSurface < surface)  // mieux ?
					{
						maxSurface        = surface;
						this.size         = size;
						this.totalColumns = nx;
						this.totalRows    = ny;
					}
				}
			}

			//	Si on arrive placer plus de widgets que demandé dans minimalHope, tant mieux, on le fait.
			this.totalColumns = (int) ((this.availableSurface.Width +this.margin) / (this.size.Width +this.margin));
			this.totalRows    = (int) ((this.availableSurface.Height+this.margin) / (this.size.Height+this.margin));

			this.size = this.GetSize (this.totalColumns, this.totalRows);

			this.isDirty = false;
		}

		private Size GetSize(int nx, int ny)
		{
			//	Retourne la taille à utiliser si on voulait placer nx*ny widgets.
			double sx = System.Math.Floor ((this.availableSurface.Width  - this.additionnalSize.Width *nx - this.margin*(nx-1)) / nx);
			double sy = System.Math.Floor ((this.availableSurface.Height - this.additionnalSize.Height*ny - this.margin*(ny-1)) / ny);

			if (sx <= 0 || sy <= 0)
			{
				return Size.Zero;
			}

			if (sx/sy < this.pageSize.Width/this.pageSize.Height)
			{
				sy = System.Math.Floor (sx*this.pageSize.Height/this.pageSize.Width);
			}
			else
			{
				sx = System.Math.Floor (sy*this.pageSize.Width/this.pageSize.Height);
			}

			return new Size (sx+this.additionnalSize.Width, sy+this.additionnalSize.Height);
		}


		private readonly Rectangle		availableSurface;
		private readonly Size			pageSize;
		private readonly Size			additionnalSize;
		private readonly double			margin;
		private readonly int			minimalHope;

		private bool					isDirty;
		private int						totalColumns;
		private int						totalRows;
		private Size					size;
	}
}
