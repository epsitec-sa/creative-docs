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

		public int FirstLine
		{
			get;
			set;
		}


		public override double RequiredHeight
		{
			get
			{
				var textLayout = this.CreateTextLayout ();

				double height = 0;

				int lineCount = textLayout.TotalLineCount;
				for (int i = 0; i < lineCount; i++)
				{
					height += textLayout.GetLineHeight (i);
				}

				return height;
			}
		}


		public override void Paint(IPaintPort port)
		{
			//	Dessine un texte dans un pavé à partir d'une ligne donnée. Retourne le numéro de la première ligne
			//	pour le pavé suivant, ou -1 s'il n'y en a pas.
			if (this.FirstLine == -1)
			{
				return;
			}

			//	Crée un pavé à la bonne largeur mais de hauteur infinie, pour pouvoir calculer les hauteurs
			//	de toutes les lignes.
			var textLayout = this.CreateTextLayout ();

			int lineCount = textLayout.TotalLineCount;

			if (this.FirstLine >= lineCount)
			{
				this.FirstLine = -1;
				return;
			}

			double[] heights = new double[lineCount];

			for (int i = 0; i < lineCount; i++)
			{
				heights[i] = textLayout.GetLineHeight (i);
			}

			//	Calcule la distance verticale correspondant aux lignes à ne pas afficher.
			double verticalOffset = 0;
			for (int i = 0; i < this.FirstLine; i++)
			{
				verticalOffset += heights[i];
			}

			Rectangle clipRect = this.Bounds;  // clipping sur le rectangle demandé
			Rectangle bounds = this.Bounds;
			bounds.Top += verticalOffset;  // remonte le début, qui sera clippé

			//	Adapte le pavé avec les données réelles et dessine-le.
			textLayout.LayoutSize = bounds.Size;
			textLayout.Paint (bounds.BottomLeft, port, clipRect, Color.Empty, GlyphPaintStyle.Normal);

			//	Calcul l'index de la première ligne du pavé suivant.
			this.FirstLine = textLayout.VisibleLineCount;

			if (this.FirstLine >= lineCount)
			{
				this.FirstLine = -1;
				return;
			}
		}

		private TextLayout CreateTextLayout()
		{
			var textLayout = new TextLayout ()
			{
				Alignment             = this.Alignment,
				JustifMode            = this.Justif,
				BreakMode             = this.BreakMode,
				DefaultFont           = this.Font,
				DefaultFontSize       = this.FontSize,
				LayoutSize            = new Size (this.Bounds.Width, double.MaxValue),
				DefaultUnderlineWidth = 0.1,
				DefaultWaveWidth      = 0.75,
				Text                  = this.Text,
			};

			return textLayout;
		}
	}
}
