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

		public List<int> TopGroupIndexList
		{
			set;
			internal get;
		}

		public List<int> CurrentGroupIndexList
		{
			set;
			internal get;
		}

		public List<int> BottomGroupIndexList
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

			int bandWidth = (int) rect.Width / IndexDisplayerWidget.maxDeep;
			int count = System.Math.Min (colors.Count, IndexDisplayerWidget.maxDeep);

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
				int shift = IndexDisplayerWidget.GetShift (this.TopGroupIndexList, this.CurrentGroupIndexList);
				double x = bandWidth*shift + 0.5;

				graphics.AddLine (rect.Left+x, rect.Top-0.5, rect.Right, rect.Top-0.5);
				graphics.RenderSolid (adorner.ColorBorder);
			}
		}

		private static int GetShift(List<int> topList, List<int> currentList)
		{
			int i = 0;

			while (true)
			{
				if (topList     != null && i < topList.Count     &&
					currentList != null && i < currentList.Count &&
					topList[i] == currentList[i])
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


		private static readonly int maxDeep = 4;

		private readonly List<Color>			colors;
	}
}
