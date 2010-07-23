﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class TextBand : AbstractBand
	{
		public TextBand() : base()
		{
			this.Alignment = ContentAlignment.TopLeft;
			this.Justif    = TextJustifMode.None;
			this.BreakMode = TextBreakMode.Hyphenate;

			this.TableCellBorderWidth = double.NaN;
			this.TableCellBackground = Color.Empty;

			this.sectionsInfo = new List<SectionInfo> ();
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

		public double TableCellBorderWidth
		{
			//	Cette propriété n'est pas exploitée directement pas TextBand, mais par TableBand !
			get;
			set;
		}

		public Color TableCellBackground
		{
			//	Cette propriété n'est pas exploitée directement pas TextBand, mais par TableBand !
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


		/// <summary>
		/// Effectue la justification verticale pour découper le texte en sections.
		/// </summary>
		/// <param name="width">Largeur pour toutes les sections</param>
		/// <param name="initialHeight">Hauteur de la première section</param>
		/// <param name="middleheight">Hauteur des sections suivantes</param>
		/// <param name="finalHeight">Hauteur de la dernière section</param>
		/// <returns>Retourne false s'il n'a pas été possible de mettre tout le contenu</returns>
		public override bool BuildSections(double width, double initialHeight, double middleheight, double finalHeight)
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
				ending = this.JustifOneSection (ref line, heightAvailable);

				first = false;
			}
			while (!ending);

			//	Si la dernière tranche occupe plus de place que finalHeight, on crée une dernière section vide.
			if (finalHeight < middleheight &&
				this.sectionsInfo.Count > 1 &&
				this.sectionsInfo[this.sectionsInfo.Count-1].Height > finalHeight)
			{
				this.sectionsInfo.Add (new SectionInfo (line, 0, 0));
			}

			return true;
		}

		public void JustifInitialize(double width)
		{
			this.width = width;
			this.sectionsInfo.Clear ();

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

		public bool JustifOneSection(ref int line, double maxHeight)
		{
			//	Essaie de mettre un maximum de lignes sur une section donnée.
			//	Retourne true s'il y a assez de place pour tout mettre (donc jusqu'à la fin).
			double height = 0;

			for (int i = line; i < this.heights.Length; i++)
			{
				height += this.heights[i];

				if (height > maxHeight)
				{
					int lineCount = i-line;
					height -= this.heights[i];
					this.sectionsInfo.Add (new SectionInfo (line, lineCount, height));

					line += lineCount;
					return false;  // il reste encore des données
				}

				if (i == this.heights.Length-1)
				{
					int lineCount = i-line+1;
					this.sectionsInfo.Add (new SectionInfo (line, lineCount, height));

					line += lineCount;
					return true;  // tout est casé
				}
			}

			this.sectionsInfo.Add (new SectionInfo (line, 0, 0));
			return true;  // tout est casé
		}

		public void JustifRemoveLastSection()
		{
			//	Annule la dernière section justifiée.
			if (this.sectionsInfo.Count != 0)
			{
				this.sectionsInfo.RemoveAt (this.sectionsInfo.Count-1);
			}
		}


		public int LastFirstLine
		{
			get
			{
				if (this.sectionsInfo.Count > 0)
				{
					return this.sectionsInfo[this.sectionsInfo.Count-1].FirstLine;
				}

				return 0;
			}
		}

		public int LastLineCount
		{
			get
			{
				if (this.sectionsInfo.Count > 0)
				{
					return this.sectionsInfo[this.sectionsInfo.Count-1].LineCount;
				}

				return 0;
			}
		}

		public double LastHeight
		{
			get
			{
				if (this.sectionsInfo.Count > 0)
				{
					return this.sectionsInfo[this.sectionsInfo.Count-1].Height;
				}

				return 0;
			}
		}


		public override int SectionCount
		{
			get
			{
				return this.sectionsInfo.Count;
			}
		}

		/// <summary>
		/// Retourne la hauteur que l'objet occupe dans une section.
		/// </summary>
		/// <param name="section"></param>
		/// <returns></returns>
		public override double GetSectionHeight(int section)
		{
			if (section >= 0 && section < this.sectionsInfo.Count)
			{
				return this.sectionsInfo[section].Height;
			}

			return 0;
		}

		/// <summary>
		/// Dessine une section de l'objet à une position donnée.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="section">Rang de la section à dessiner</param>
		/// <param name="topLeft">Coin supérieur gauche</param>
		/// <returns>Retourne false si le contenu est trop grand et n'a pas pu être dessiné</returns>
		public override bool Paint(IPaintPort port, bool isPreview, int section, Point topLeft)
		{
			if (section < 0 || section >= this.sectionsInfo.Count)
			{
				return true;
			}

			var ok = true;
			var sectionInfo = this.sectionsInfo[section];

			Rectangle clipRect = new Rectangle (topLeft.X, topLeft.Y-sectionInfo.Height, this.width, sectionInfo.Height);

			//	Calcule la distance verticale correspondant aux lignes à ne pas afficher.
			double verticalOffset = 0;
			for (int i = section-1; i >= 0; i--)
			{
				verticalOffset += this.sectionsInfo[i].Height;
			}

			Rectangle bounds = clipRect;
			bounds.Top += verticalOffset;  // remonte le début, qui sera clippé

			//	Crée le TextLayout avec les données réelles et dessine-le.
			var textLayout = this.CreateTextLayout ();
			textLayout.LayoutSize = bounds.Size;

			if (textLayout.TotalRectangle.IsEmpty && !string.IsNullOrEmpty(textLayout.Text))
			{
				//	Dessine une grande croix 'x'.
				port.LineWidth = 0.1;
				port.Color = Color.FromBrightness (0);
				port.PaintOutline (Path.FromLine (clipRect.BottomLeft, clipRect.TopRight));
				port.PaintOutline (Path.FromLine (clipRect.TopLeft, clipRect.BottomRight));

				ok = false;
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

			return ok;
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


		private class SectionInfo
		{
			public SectionInfo(int firstLine, int lineCount, double height)
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
		private List<SectionInfo>	sectionsInfo;
		private double[]			heights;
	}
}
