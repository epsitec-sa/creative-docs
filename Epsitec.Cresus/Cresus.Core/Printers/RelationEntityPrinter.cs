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

	public class RelationEntityPrinter : AbstractEntityPrinter<RelationEntity>
	{
		public RelationEntityPrinter(RelationEntity entity)
			: base (entity)
		{
		}

		public override string JobName
		{
			get
			{
				return UIBuilder.FormatText ("Client", this.entity.Id).ToSimpleText ();
			}
		}

		public override Size PageSize
		{
			get
			{
				return new Size (pageWidth, pageHeight);  // A4 vertical
			}
		}

		public override void Print(IPaintPort port, Rectangle bounds)
		{
			string text = "?";

			if (entity.Person is NaturalPersonEntity)
			{
				var x = this.entity.Person as NaturalPersonEntity;
				text = UIBuilder.FormatText ("N°", this.entity.Id, "<br/>", x.Title.Name, "<br/>", x.Firstname, x.Lastname, "(", x.Gender.Name, ")", "<br/>", x.BirthDate).ToString ();
			}

			if (entity.Person is LegalPersonEntity)
			{
				var x = this.entity.Person as LegalPersonEntity;
				text = UIBuilder.FormatText (x.Name).ToSimpleText ();
			}

			//?Font font = Font.GetFont ("Arial", "Regular");
			Font font = Font.GetFont ("Times New Roman", "Regular");
			AbstractEntityPrinter.PaintText (port, text, new Rectangle (this.PageMargins.Left, this.PageSize.Height-this.PageMargins.Top-150, 150, 150), font, fontSize);

#if false
			var t = new ObjectTextBox ();
			t.Text = "Ceci est un texte plus long que les autres, pour tester...";
			t.Bounds = new Rectangle (10, 50, 35.5, 150);
			t.Paint (port);

			double h = t.RequiredHeight;
			Rectangle bb = new Rectangle (t.Bounds.Left, t.Bounds.Top-h, t.Bounds.Width, h);
			port.LineWidth = 0.1;
			port.PaintOutline (Path.FromRectangle (bb));
#endif

#if true
			var table = new ObjectTable ();
			table.ColumnsCount = 4;
			table.RowsCount = 3;

			table.SetText (0, 0, "<b>Lundi</b>");
			table.SetText (1, 0, "<b>Mardi</b>");
			table.SetText (2, 0, "<b>Mercredi</b>");
			table.SetText (3, 0, "<b>Jeudi</b>");

			table.SetText (0, 1, "Gauche");
			table.SetText (1, 1, "");
			table.SetText (2, 1, "Ceci est un texte plus long que les autres, pour tester...");
			table.SetText (3, 1, "Droite");

			table.SetText (0, 2, "Rouge");
			table.SetText (1, 2, "Ceci est un <u>texte bidon</u> mais <b>assez long</b>, pour permettre de <font size=\"6\">tester</font> le découpage en plusieurs pavés distincts, qui seront dessinés sur plusieurs pages.");
			table.SetText (2, 2, "Vert");
			table.SetText (3, 2, "Bleu");

			//?table.SetRelativeColumWidth (1, 1.5);

			table.Bounds = new Rectangle(10, 100, 150, 150);
			table.Paint (port);
#endif

#if true
			string t = "Ceci est un <u>texte bidon</u> mais <b>assez long</b>, pour permettre de <font size=\"6\">tester</font> le découpage en plusieurs pavés distincts, qui seront dessinés sur plusieurs pages.<br/><br/><i>Et voilà la <font color=\"#ff0000\">suite et la fin</font> de ce chef d'œuvre littéraire sur une toute nouvelle ligne.</i>";
			Point pos;
			int firstLine;

			pos = new Point (10, 100);
			firstLine = 0;
			while (true)
			{
				Rectangle b = new Rectangle (pos.X, pos.Y, 50, 25);
				firstLine = AbstractEntityPrinter.PaintText (port, t, firstLine, b, font, fontSize);

				port.LineWidth = 0.1;
				port.PaintOutline (Path.FromRectangle (b));

				if (firstLine == -1)
				{
					break;
				}

				pos.X += 50+1;
			}

			pos = new Point (10, 100-25-2);
			firstLine = 0;
			while (true)
			{
				Rectangle b = new Rectangle (pos.X, pos.Y, 30, 25);
				firstLine = AbstractEntityPrinter.PaintText (port, t, firstLine, b, font, fontSize);

				port.LineWidth = 0.1;
				port.PaintOutline (Path.FromRectangle (b));

				if (firstLine == -1)
				{
					break;
				}

				pos.X += 30+1;
			}

			pos = new Point (10, 100-25-2-25-2);
			firstLine = 0;
			while (true)
			{
				Rectangle b = new Rectangle (pos.X, pos.Y, 20, 25);
				firstLine = AbstractEntityPrinter.PaintText (port, t, firstLine, b, font, fontSize);

				port.LineWidth = 0.1;
				port.PaintOutline (Path.FromRectangle (b));

				if (firstLine == -1)
				{
					break;
				}

				pos.X += 20+1;
			}

			pos = new Point (10, 100-25-2-25-2-25-2);
			firstLine = 0;
			while (true)
			{
				Rectangle b = new Rectangle (pos.X, pos.Y, 50, 25);
				firstLine = AbstractEntityPrinter.PaintText (port, t, firstLine, b, font, fontSize, ContentAlignment.TopLeft, TextJustifMode.AllButLast, TextBreakMode.Hyphenate);

				port.LineWidth = 0.1;
				port.PaintOutline (Path.FromRectangle (b));

				if (firstLine == -1)
				{
					break;
				}

				pos.X += 50+1;
			}
#endif
		}

		private void PaintMailContacts(IPaintPort port, Rectangle bounds)
		{
			foreach (var contact in this.entity.Person.Contacts)
			{
				if (contact is MailContactEntity)
				{
				}
			}
		}


		private static readonly double pageWidth  = 210;
		private static readonly double pageHeight = 297;  // A4 vertical

		private static readonly double fontSize = 4;
	}
}
