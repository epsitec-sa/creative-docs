//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class IndexDisplayerWidget : Widget
	{
		/// <summary>
		/// Widget très spécialisé qui s'affiche dans la première colonne du tableau CellTable, pour
		/// mettre en évidence les groupes de lignes à plusieurs niveaux.
		/// </summary>
		public IndexDisplayerWidget()
		{
			this.colors = new List<Color> ();
		}

		public IndexDisplayerWidget(Widget embedder)
			: this ()
		{
			this.SetEmbedder(embedder);
		}


		public List<Color> Colors
		{
			get
			{
				return this.colors;
			}
		}

		public int TopGroupIndex
		{
			set;
			internal get;
		}

		public int CurrentGroupIndex
		{
			set;
			internal get;
		}

		public int BottomGroupIndex
		{
			set;
			internal get;
		}

		public bool DrawTopSeparator
		{
			set;
			get;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			int bandWidth = (int) rect.Width / LinesEngine.maxGroupingDepth;
			int count = System.Math.Min (colors.Count, LinesEngine.maxGroupingDepth);

			for (int i = 0; i < count; i++)
			{
				int x = bandWidth*i;
				Rectangle band = new Rectangle (rect.Left+x, rect.Bottom, rect.Width-x, rect.Height);

				//	Dessine la surface colorée.
				graphics.AddFilledRectangle (band);
				graphics.RenderSolid (this.colors[i]);

				//	Dessine le trait vertical de séparation.
				graphics.AddLine (band.Left+0.5, band.Bottom, band.Left+0.5, band.Top);
				graphics.RenderSolid (adorner.ColorBorder);
			}

			if (this.DrawTopSeparator)
			{
				//	Dessine le trait horizontal supérieur.
				int shift = IndexDisplayerWidget.GetShift (this.TopGroupIndex, this.CurrentGroupIndex);
				double x = bandWidth*shift + 0.5;

				graphics.AddLine (rect.Left+x, rect.Top-0.5, rect.Right, rect.Top-0.5);
				graphics.RenderSolid (adorner.ColorBorder);
			}
		}

		private static int GetShift(int groupIndex1, int groupIndex2)
		{
			int i = 0;

			while (true)
			{
				int r1 = LinesEngine.LevelExtract (groupIndex1, i);
				int r2 = LinesEngine.LevelExtract (groupIndex2, i);

				if (r1 != 0 && r1 == r2)
				{
					i++;
				}
				else
				{
					break;
				}
			}

			return i;
		}


		private readonly List<Color>			colors;
	}
}
