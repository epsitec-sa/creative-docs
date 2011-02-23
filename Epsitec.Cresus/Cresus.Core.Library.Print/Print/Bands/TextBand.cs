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

namespace Epsitec.Cresus.Core.Print.Bands
{
	public class TextBand : AbstractBand
	{
		public TextBand() : base()
		{
			this.Alignment = ContentAlignment.TopLeft;
			this.Justif    = TextJustifMode.None;
			this.BreakMode = TextBreakMode.Hyphenate;

			this.TableCellMargins    = Margins.Zero;
			this.TableCellBorder     = CellBorder.Empty;
			this.TableCellBackground = Color.Empty;

			this.sectionsInfo = new List<SectionInfo> ();
			this.textLayout = new TextLayout ();
		}


		public FormattedText Text
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

		public Margins TableCellMargins
		{
			//	Cette propriété n'est pas exploitée directement pas TextBand, mais par TableBand !
			//	Si elle n'a jamais été initialisée et qu'elle vaut Margins.Zero, cela signifie qu'il
			//	faut utiliser la valeur générale du tableau (TableBand.CellMargins).
			get;
			set;
		}

		public CellBorder TableCellBorder
		{
			//	Cette propriété n'est pas exploitée directement pas TextBand, mais par TableBand !
			//	Si elle n'a jamais été initialisée et qu'elle vaut CellBorder.Empty, cela signifie qu'il
			//	faut utiliser la valeur générale du tableau (TableBand.CellBorder).
			get;
			set;
		}

		public Color TableCellBackground
		{
			//	Cette propriété n'est pas exploitée directement pas TextBand, mais par TableBand !
			//	Si elle n'a jamais été initialisée et qu'elle vaut Color.Empty, cela signifie qu'il
			//	faut utiliser la valeur générale du tableau.
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
			this.UpdateTextLayout ();

			double height = 0;

			int lineCount = this.textLayout.TotalLineCount;
			for (int i = 0; i < lineCount; i++)
			{
				height += this.textLayout.GetLineHeight (i);
			}

			return height;
		}

		public double RequiredWidth()
		{
			//	Retourne la largeur requise si le texte est mis sur une seule ligne.
			this.UpdateTextLayout ();

			return this.textLayout.SingleLineSize.Width;
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

				if (heightAvailable < 0)
				{
					break;
				}

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

			//	Met à jour un pavé à la bonne largeur mais de hauteur infinie, pour pouvoir calculer les hauteurs
			//	de toutes les lignes.
			this.UpdateTextLayout ();

			int totalLineCount = this.textLayout.TotalLineCount;
			this.heights = new double[totalLineCount];
			for (int i = 0; i < totalLineCount; i++)
			{
				this.heights[i] = this.textLayout.GetLineHeight (i);
			}
		}

		public bool JustifOneSection(ref int line, double maxHeight)
		{
			//	Essaie de mettre un maximum de lignes sur une section donnée.
			//	Retourne true s'il y a assez de place pour tout mettre (donc jusqu'à la fin).
			if (maxHeight <= 0)
			{
				return false;  // il reste encore des données
			}

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
		/// <returns>Retourne false si le contenu est trop grand et n'a pas pu être dessiné correctement</returns>
		public override bool PaintForeground(IPaintPort port, PreviewMode previewMode, int section, Point topLeft)
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

			//	Met à jour le TextLayout avec les données réelles et dessine-le.
			this.UpdateTextLayout ();
			this.textLayout.LayoutSize = bounds.Size;

			if (this.textLayout.TotalRectangle.IsEmpty && !string.IsNullOrEmpty (this.textLayout.Text))
			{
#if false
				//	Dessine une grande croix 'x'.
				port.LineWidth = 0.1;
				port.Color = Color.FromBrightness (0);
				port.PaintOutline (Path.FromLine (clipRect.BottomLeft, clipRect.TopRight));
				port.PaintOutline (Path.FromLine (clipRect.TopLeft, clipRect.BottomRight));
#else
				//	S'il n'y a pas la place, dessine le texte avec une police plus petite, sur un fond orange.
				port.Color = Color.FromName ("Orange");
				port.PaintSurface (Path.FromRectangle (clipRect));

				for (int i = 0; i < 10; i++)
				{
					this.textLayout.DefaultFontSize *= 0.8;

					if (!this.textLayout.TotalRectangle.IsEmpty)
					{
						this.textLayout.Paint (bounds.BottomLeft, port, clipRect, Color.Empty, GlyphPaintStyle.Normal);
						break;
					}
				}
#endif

				ok = false;  // il y a eu un problème !
			}
			else
			{
				this.textLayout.Paint (bounds.BottomLeft, port, clipRect, Color.Empty, GlyphPaintStyle.Normal);
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

	
		private void UpdateTextLayout()
		{
			//	Met à jour le TextLayout à la bonne largeur mais avec une hauteur infinie.
			this.textLayout.Alignment             = this.Alignment;
			this.textLayout.JustifMode            = this.Justif;
			this.textLayout.BreakMode             = this.BreakMode;
			this.textLayout.DefaultFont           = this.Font;
			this.textLayout.DefaultFontSize       = this.FontSize;
			this.textLayout.LayoutSize            = new Size (this.width, double.MaxValue);
			this.textLayout.DefaultUnderlineWidth = AbstractBand.defaultUnderlineWidth;
			this.textLayout.DefaultWaveWidth      = AbstractBand.defaultWaveWidth;
			this.textLayout.Text                  = this.Text.ToString ();
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
		private TextLayout			textLayout;
	}
}
