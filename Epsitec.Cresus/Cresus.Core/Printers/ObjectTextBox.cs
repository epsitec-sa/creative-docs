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

		public bool DebugPaintFrame
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
			//	initialHeight et finalHeight doivent être plus petit ou égal à middleheight.
			System.Diagnostics.Debug.Assert (initialHeight <= middleheight);
			System.Diagnostics.Debug.Assert (finalHeight   <= middleheight);

			this.JustifInitialize (width);

			//	Découpe le texte en tranches verticales, la première ayant une hauteur de initialHeight et les
			//	suivantes de middleheight.
			int line = 0;
			bool first = true;
			bool ending;
			do
			{
				double heightAvailable = first ? initialHeight : middleheight;
				ending = this.JustifOnePage (ref line, heightAvailable);

				first = false;
			}
			while (!ending);

			//	Si la dernière tranche occupe plus de place que finalHeight, on crée une dernière page vide.
			if (finalHeight < middleheight &&
				this.pagesInfo.Count > 1 &&
				this.pagesInfo[this.pagesInfo.Count-1].Height > finalHeight)
			{
				this.pagesInfo.Add (new PageInfo (line, 0, 0));
			}
		}

		public void JustifInitialize(double width)
		{
			this.width = width;
			this.pagesInfo.Clear ();

			//	Crée un pavé à la bonne largeur mais de hauteur infinie, pour pouvoir calculer les hauteurs
			//	de toutes les lignes.
			var textLayout = this.CreateTextLayout ();

			int totalLineCount = textLayout.TotalLineCount;
			this.heights = new double[totalLineCount];
			for (int i = 0; i < totalLineCount; i++)
			{
				this.heights[i] = textLayout.GetLineHeight (i);
			}
		}

		public bool JustifOnePage(ref int line, double maxHeight)
		{
			//	Essaie de mettre un maximum de lignes sur une page donnée.
			//	Retourne true s'il y a assez de place pour tout mettre (donc jusqu'à la fin).
			double height = 0;

			for (int i = line; i < this.heights.Length; i++)
			{
				height += this.heights[i];

				if (height > maxHeight)
				{
					int lineCount = i-line;
					height -= this.heights[i];
					this.pagesInfo.Add (new PageInfo (line, lineCount, height));

					line += lineCount;
					return false;  // il reste encore des données
				}

				if (i == this.heights.Length-1)
				{
					int lineCount = i-line+1;
					this.pagesInfo.Add (new PageInfo (line, lineCount, height));

					line += lineCount;
					return true;  // tout est casé
				}
			}

			this.pagesInfo.Add (new PageInfo (line, 0, 0));
			return true;  // tout est casé
		}


		public int LastFirstLine
		{
			get
			{
				if (this.pagesInfo.Count > 0)
				{
					return this.pagesInfo[this.pagesInfo.Count-1].FirstLine;
				}

				return 0;
			}
		}

		public int LastLineCount
		{
			get
			{
				if (this.pagesInfo.Count > 0)
				{
					return this.pagesInfo[this.pagesInfo.Count-1].LineCount;
				}

				return 0;
			}
		}

		public double LastHeight
		{
			get
			{
				if (this.pagesInfo.Count > 0)
				{
					return this.pagesInfo[this.pagesInfo.Count-1].Height;
				}

				return 0;
			}
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

			if (textLayout.TotalRectangle.IsEmpty && !string.IsNullOrEmpty(textLayout.Text))
			{
				port.LineWidth = 0.1;
				port.Color = Color.FromBrightness (0);
				port.PaintOutline (Path.FromLine (clipRect.BottomLeft, clipRect.TopRight));
				port.PaintOutline (Path.FromLine (clipRect.TopLeft, clipRect.BottomRight));
			}
			else
			{
				textLayout.Paint (bounds.BottomLeft, port, clipRect, Color.Empty, GlyphPaintStyle.Normal);
			}

			if (this.DebugPaintFrame)
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


		private double				width;
		private List<PageInfo>		pagesInfo;
		private double[]			heights;
	}
}
