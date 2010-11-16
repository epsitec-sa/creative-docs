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
	public class OptimalPreviewPlacer2
	{
		public OptimalPreviewPlacer2(Rectangle availableSurface, Size pageSize, double margin, int maxPageCount)
		{
			this.availableSurface = availableSurface;
			this.pageSize         = pageSize;
			this.margin           = margin;
			this.maxPageCount     = maxPageCount;

			this.isDirty = true;
		}

		public int Total
		{
			//	Retourne le nombre de widgets que l'on peut placer.
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

		public void UpdateGeometry<T>(List<T> widgets)
			where T: Widget
		{
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
			this.size = Size.Zero;
			this.totalColumns = 0;
			this.totalRows = 0;

			for (int ny = 1; ny <= this.maxPageCount; ny++)
			{
				for (int nx = 1; nx <= this.maxPageCount; nx++)
				{
					if (nx*ny < this.maxPageCount  ||  // insuffisant pour tout caser ?
						nx*ny > this.maxPageCount*2)   // beaucoup trop ?
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
						maxSurface = surface;
						this.size = size;
						this.totalColumns = nx;
						this.totalRows = ny;
					}
				}
			}

			this.totalColumns = (int) ((this.availableSurface.Width+this.margin) / (this.size.Width+this.margin));
			this.totalRows    = (int) ((this.availableSurface.Height+this.margin) / (this.size.Height+this.margin));

			this.isDirty = false;
		}

		private Size GetSize(int nx, int ny)
		{
			double sx = System.Math.Floor (this.availableSurface.Width/nx  - this.margin*(nx-1));
			double sy = System.Math.Floor (this.availableSurface.Height/ny - this.margin*(ny-1));

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

			return new Size (sx, sy);
		}


		private readonly Rectangle		availableSurface;
		private readonly Size			pageSize;
		private readonly double			margin;
		private readonly int			maxPageCount;

		private bool					isDirty;
		private int						totalColumns;
		private int						totalRows;
		private Size					size;
	}
}
