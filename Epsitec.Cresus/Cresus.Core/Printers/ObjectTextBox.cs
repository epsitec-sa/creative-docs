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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class ObjectTextBox : AbstractObject
	{
		public ObjectTextBox() : base()
		{
			this.Alignment = ContentAlignment.TopLeft;
			this.Justif    = TextJustifMode.None;
			this.BreakMode = TextBreakMode.Hyphenate;

			this.pagesInfo = new List<PageInfo> ();
		}


		public string Text
		{
			get;
			set;
		}

		public ContentAlignment Alignment
		{
			get;
			set;
		}

		public TextJustifMode Justif
		{
			get;
			set;
		}

		public TextBreakMode BreakMode
		{
			get;
			set;
		}

		public bool PaintFrame
		{
			get;
			set;
		}


		public override double RequiredHeight(double width)
		{
			this.width = width;
			var textLayout = this.CreateTextLayout ();

			double height = 0;

			int lineCount = textLayout.TotalLineCount;
			for (int i = 0; i < lineCount; i++)
			{
				height += textLayout.GetLineHeight (i);
			}

			return height;
		}


		public override void InitializePages(double width, double initialHeight, double middleheight, double finalHeight)
		{
			this.width = width;
			this.pagesInfo.Clear ();

			//	Crée un pavé à la bonne largeur mais de hauteur infinie, pour pouvoir calculer les hauteurs
			//	de toutes les lignes.
			var textLayout = this.CreateTextLayout ();

			int totalLineCount = textLayout.TotalLineCount;
			double[] heights = new double[totalLineCount];
			for (int i = 0; i < totalLineCount; i++)
			{
				heights[i] = textLayout.GetLineHeight (i);
			}

			//	Essaie de tout mettre sur la première page.
			int lineCount;
			double height;
			if (ObjectTextBox.PageCompute (heights, 0, initialHeight, out lineCount, out height))
			{
				this.pagesInfo.Add (new PageInfo(0, lineCount, height));
				return;
			}

			//	Essaie de tout mettre sur la première et la dernière page, sans pages intermédiaires.
			int lineCount2;
			double height2;
			if (ObjectTextBox.PageCompute (heights, lineCount, finalHeight, out lineCount2, out height2))
			{
				this.pagesInfo.Add (new PageInfo (0, lineCount, height));
				this.pagesInfo.Add (new PageInfo (lineCount, lineCount2, height2));
				return;
			}

			//	Met tout avec des pages intermédiaires.
			this.pagesInfo.Add (new PageInfo (0, lineCount, height));

			for (int middlePageCount = 1; middlePageCount < 100; middlePageCount++)
			{
				int index = lineCount;

				//	Essaie avec middlePageCount pages intermédiaires.
				for (int i = 0; i < middlePageCount; i++)
				{
					ObjectTextBox.PageCompute (heights, index, middleheight, out lineCount2, out height2);
					index += lineCount2;
				}

				//	Essaie avec la dernière page.
				if (ObjectTextBox.PageCompute (heights, index, finalHeight, out lineCount2, out height2))
				{
					index = lineCount;

					//	Met les pages intermédiaires
					for (int i = 0; i < middlePageCount; i++)
					{
						ObjectTextBox.PageCompute (heights, index, middleheight, out lineCount2, out height2);
						this.pagesInfo.Add (new PageInfo (index, lineCount2, height2));
						index += lineCount2;
					}

					//	Met la dernière page.
					ObjectTextBox.PageCompute (heights, index, finalHeight, out lineCount2, out height2);
					this.pagesInfo.Add (new PageInfo (index, lineCount2, height2));
					return;
				}
			}

			//	On ne devrait jamais arriver ici !
		}

		private static bool PageCompute(double[] heights, int startIndex, double maxHeight, out int lineCount, out double height)
		{
			//	Essaie de mettre un maximum de lignes sur une page donnée.
			//	Retourne true s'il y a assez de place pour tout mettre (donc jusquà la fin).
			height = 0;

			for (int i = startIndex; i < heights.Length; i++)
			{
				height += heights[i];

				if (height > maxHeight)
				{
					lineCount = i-startIndex;
					height -= heights[i];
					return false;  // il reste encore des données
				}

				if (i == heights.Length-1)
				{
					lineCount = i-startIndex+1;
					return true;  // tout est casé
				}
			}

			lineCount = 0;
			height = 0;
			return true;  // tout est casé
		}

		public override int PageCount
		{
			get
			{
				return this.pagesInfo.Count;
			}
		}

		public override void Paint(IPaintPort port, int page, Point topLeft)
		{
			if (page < 0 || page >= this.pagesInfo.Count)
			{
				return;
			}

			var pageInfo = this.pagesInfo[page];

			Rectangle clipRect = new Rectangle (topLeft.X, topLeft.Y-pageInfo.Height, this.width, pageInfo.Height);

			//	Calcule la distance verticale correspondant aux lignes à ne pas afficher.
			double verticalOffset = 0;
			for (int i = page-1; i >= 0; i--)
			{
				verticalOffset += this.pagesInfo[i].Height;
			}

			Rectangle bounds = clipRect;
			bounds.Top += verticalOffset;  // remonte le début, qui sera clippé

			//	Crée le TextLayout avec les données réelles et dessine-le.
			var textLayout = this.CreateTextLayout ();
			textLayout.LayoutSize = bounds.Size;
			textLayout.Paint (bounds.BottomLeft, port, clipRect, Color.Empty, GlyphPaintStyle.Normal);

			if (this.PaintFrame)
			{
				if (clipRect.Height == 0)
				{
					clipRect.Bottom -= 1.0;  // pour voir qq chose
				}

				port.LineWidth = 0.1;
				port.Color = Color.FromBrightness (0);
				port.PaintOutline (Path.FromRectangle (clipRect));
			}
		}

	
		private TextLayout CreateTextLayout()
		{
			//	Crée un TextLayout à la bonne largeur mais de hauteur infinie.
			var textLayout = new TextLayout ()
			{
				Alignment             = this.Alignment,
				JustifMode            = this.Justif,
				BreakMode             = this.BreakMode,
				DefaultFont           = this.Font,
				DefaultFontSize       = this.FontSize,
				LayoutSize            = new Size (this.width, double.MaxValue),
				DefaultUnderlineWidth = 0.1,
				DefaultWaveWidth      = 0.75,
				Text                  = this.Text,
			};

			return textLayout;
		}


		private class PageInfo
		{
			public PageInfo(int firstLine, int lineCount, double height)
			{
				this.FirstLine = firstLine;
				this.LineCount = lineCount;
				this.Height    = height;
			}

			public int		FirstLine;
			public int		LineCount;
			public double	Height;
		}


		private double width;
		private List<PageInfo> pagesInfo;
	}
}
