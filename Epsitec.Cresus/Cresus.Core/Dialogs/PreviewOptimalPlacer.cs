//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Place de façon optimale des rectangles d'aperçu (Widgets.PreviewEntity) dans une zone rectangulaire.
	/// Pour cela, on cherche à maximiser la surface des aperçus.
	/// </summary>
	class PreviewOptimalPlacer
	{
		public PreviewOptimalPlacer(List<Widgets.PreviewEntity> pagePreviews, Size pageSize)
		{
			this.pagePreviews = pagePreviews;
			this.pageSize     = pageSize;
		}

		public Size AvailableSize
		{
			get;
			set;
		}

		public int PageCount
		{
			get;
			set;
		}

		public void UpdateGeometry()
		{
			//	Positionne tous les Widgets.PreviewEntity, selon la taille disponible.
			if (this.pagePreviews.Count == 0)
			{
				return;
			}

			int pageCount = this.PageCount;
			double maxSurface = 0;
			int bestNx = -1;
			int bestNy = -1;
			Size pageSize;
			double spacing = 5;

			//	Pour toutes les combinaisons nx * ny, cherche celle avec laquelle la surface d'un
			//	aperçu est maximale.
			for (int ny = 1; ny <= pageCount; ny++)
			{
				for (int nx = 1; nx <= pageCount; nx++)
				{
					if (nx*ny < pageCount  ||  // insuffisant pour tout caser ?
						nx*ny > pageCount*2)   // beaucoup trop ?
					{
						continue;
					}

					pageSize = this.ComputePreviewSize (nx, ny, spacing);
					double surface = pageSize.Width * pageSize.Height;  // calcule la surface pour un preview

					if (maxSurface < surface)  // mieux ?
					{
						maxSurface = surface;
						bestNx = nx;
						bestNy = ny;
					}
				}
			}

			if (maxSurface == 0)  // garde-fou
			{
				return;
			}

			pageSize = this.ComputePreviewSize (bestNx, bestNy, spacing);

			int index = 0;
			double posY = this.AvailableSize.Height;

			for (int y=0; y<bestNy; y++)
			{
				double posX = 0;

				for (int x=0; x<bestNx; x++)
				{
					if (index >= this.PageCount)
					{
						break;
					}

					var preview = this.pagePreviews[index++];

					preview.SetManualBounds (new Rectangle (posX, posY-pageSize.Height, pageSize.Width, pageSize.Height));
					preview.Invalidate ();  // pour forcer le dessin

					posX += pageSize.Width + spacing;
				}

				posY -= pageSize.Height + spacing;
			}
		}

		private Size ComputePreviewSize(int nx, int ny, double spacing)
		{
			//	Retourne les dimensions d'un preview, sachant qu'on cherche à en caser nx * ny.
			double width  = System.Math.Floor ((this.AvailableSize.Width  + spacing) / nx) - spacing;
			double height = System.Math.Floor ((this.AvailableSize.Height + spacing) / ny) - spacing;

			return this.AdjustRatioPageSize (new Size (width, height));
		}

		public Size AdjustRatioPageSize(Size size)
		{
			//	Ajuste les dimensions pour une page en respectant les proportions.
			double width  = size.Width;
			double height = size.Height;

			double virtualWidth  = System.Math.Floor (height * this.pageSize.Width  / this.pageSize.Height);
			double virtualHeight = System.Math.Floor (width  * this.pageSize.Height / this.pageSize.Width);

			if (width < virtualWidth)
			{
				height = virtualHeight;
			}
			else
			{
				width = virtualWidth;
			}

			return new Size (width, height);
		}


		private readonly List<Widgets.PreviewEntity>	pagePreviews;
		private readonly Size							pageSize;
	}
}
