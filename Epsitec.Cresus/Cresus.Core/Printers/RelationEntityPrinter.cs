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
			double top =  this.PageSize.Height-this.PageMargins.Top;
			top = this.PaintSummary  (port,                            top) - 10.0;
			top = this.PaintContacts (port, this.PaintMailContacts,    top) -  5.0;
			top = this.PaintContacts (port, this.PaintTelecomContacts, top) -  5.0;
			top = this.PaintContacts (port, this.PaintUriContacts,     top) -  5.0;

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

#if false
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

#if false
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


		private double PaintSummary(IPaintPort port, double top)
		{
			//	Dessine le résumé et retourne la hauteur utilisée.
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

			double width = this.PageSize.Width - this.PageMargins.Left - this.PageMargins.Right;
			Rectangle bounds = new Rectangle (this.PageMargins.Left, this.PageMargins.Bottom, width, top-this.PageMargins.Bottom);

			var textBox = new ObjectTextBox ();
			textBox.Text = text;
			textBox.FontSize = 4.0;
			textBox.Bounds = bounds;
			textBox.Paint (port);

			return top - textBox.RequiredHeight;
		}


		private double PaintContacts(IPaintPort port, System.Func<IPaintPort, Rectangle, double> painter, double top)
		{
			//	Dessine un contact et retourne la position 'top' suivante.
			double width = this.PageSize.Width - this.PageMargins.Left - this.PageMargins.Right;
			Rectangle bounds = new Rectangle (this.PageMargins.Left, this.PageMargins.Bottom, width, top-this.PageMargins.Bottom);

			double h = painter (port, bounds);

			return top - h;
		}

		private double PaintMailContacts(IPaintPort port, Rectangle bounds)
		{
			//	Dessine un contact et retourne la hauteur utilisée.
			int count = 0;
			foreach (var contact in this.entity.Person.Contacts)
			{
				if (contact is MailContactEntity)
				{
					count++;
				}
			}

			if (count == 0)
			{
				return 0;
			}

			var title = new ObjectTextBox ();
			title.Text = "<b>Adresses</b>";
			title.FontSize = 4.5;
			title.Bounds = bounds;
			title.Paint (port);

			double titleHeight = title.RequiredHeight;

			var table = new ObjectTable ();
			table.ColumnsCount = 5;
			table.RowsCount = 1+count;

			table.SetRelativeColumWidth (0, 1.5);
			table.SetRelativeColumWidth (1, 2.0);
			table.SetRelativeColumWidth (2, 0.3);
			table.SetRelativeColumWidth (3, 1.0);
			table.SetRelativeColumWidth (4, 0.8);

			int index = 0;
			table.SetText (0, index, "<b>Rôles</b>");
			table.SetText (1, index, "<b>Adresse</b>");
			table.SetText (2, index, "<b>NPA</b>");
			table.SetText (3, index, "<b>Ville</b>");
			table.SetText (4, index, "<b>Pays</b>");
			index++;

			foreach (var contact in this.entity.Person.Contacts)
			{
				if (contact is MailContactEntity)
				{
					var x = contact as MailContactEntity;

					table.SetText (0, index, UIBuilder.FormatText (string.Join (", ", x.Roles.Select (role => role.Name))).ToString ());
					table.SetText (1, index, UIBuilder.FormatText (x.LegalPerson.Name, "\n", x.LegalPerson.Complement, "\n", x.Complement, "\n", x.Address.Street.StreetName, "\n", x.Address.Street.Complement, "\n", x.Address.PostBox.Number, "\n", x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name).ToString ());
					table.SetText (2, index, x.Address.Location.PostalCode);
					table.SetText (3, index, x.Address.Location.Name);
					table.SetText (4, index, x.Address.Location.Country.Name);
					index++;
				}
			}

			table.Bounds = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-titleHeight);
			table.Paint (port);

			return titleHeight + table.RequiredHeight;
		}

		private double PaintTelecomContacts(IPaintPort port, Rectangle bounds)
		{
			//	Dessine un contact et retourne la hauteur utilisée.
			int count = 0;
			foreach (var contact in this.entity.Person.Contacts)
			{
				if (contact is TelecomContactEntity)
				{
					count++;
				}
			}

			if (count == 0)
			{
				return 0;
			}

			var title = new ObjectTextBox ();
			title.Text = "<b>Téléphones</b>";
			title.FontSize = 4.5;
			title.Bounds = bounds;
			title.Paint (port);

			double titleHeight = title.RequiredHeight;

			var table = new ObjectTable ();
			table.ColumnsCount = 3;
			table.RowsCount = 1+count;

			table.SetRelativeColumWidth (0, 1.5);
			table.SetRelativeColumWidth (1, 1.5);
			table.SetRelativeColumWidth (2, 2.0+0.3+1.0+0.8-1.5);

			int index = 0;
			table.SetText (0, index, "<b>Rôles</b>");
			table.SetText (1, index, "<b>Type</b>");
			table.SetText (2, index, "<b>Numéro</b>");
			index++;

			foreach (var contact in this.entity.Person.Contacts)
			{
				if (contact is TelecomContactEntity)
				{
					var x = contact as TelecomContactEntity;

					table.SetText (0, index, UIBuilder.FormatText (string.Join (", ", x.Roles.Select (role => role.Name))).ToString ());
					table.SetText (1, index, UIBuilder.FormatText (x.TelecomType.Name).ToString ());
					table.SetText (2, index, UIBuilder.FormatText (x.Number).ToString ());
					index++;
				}
			}

			table.Bounds = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-titleHeight);
			table.Paint (port);

			return titleHeight + table.RequiredHeight;
		}

		private double PaintUriContacts(IPaintPort port, Rectangle bounds)
		{
			//	Dessine un contact et retourne la hauteur utilisée.
			int count = 0;
			foreach (var contact in this.entity.Person.Contacts)
			{
				if (contact is UriContactEntity)
				{
					count++;
				}
			}

			if (count == 0)
			{
				return 0;
			}

			var title = new ObjectTextBox ();
			title.Text = "<b>Emails</b>";
			title.FontSize = 4.5;
			title.Bounds = bounds;
			title.Paint (port);

			double titleHeight = title.RequiredHeight;

			var table = new ObjectTable ();
			table.ColumnsCount = 2;
			table.RowsCount = 1+count;

			table.SetRelativeColumWidth (0, 1.5);
			table.SetRelativeColumWidth (1, 2.0+0.3+1.0+0.8);

			int index = 0;
			table.SetText (0, index, "<b>Rôles</b>");
			table.SetText (1, index, "<b>Adresses électroniques</b>");
			index++;

			foreach (var contact in this.entity.Person.Contacts)
			{
				if (contact is UriContactEntity)
				{
					var x = contact as UriContactEntity;

					table.SetText (0, index, UIBuilder.FormatText (string.Join (", ", x.Roles.Select (role => role.Name))).ToString ());
					table.SetText (1, index, UIBuilder.FormatText (x.Uri).ToString ());
					index++;
				}
			}

			table.Bounds = new Rectangle (bounds.Left, bounds.Bottom, bounds.Width, bounds.Height-titleHeight);
			table.Paint (port);

			return titleHeight + table.RequiredHeight;
		}


		private static readonly double pageWidth  = 210;
		private static readonly double pageHeight = 297;  // A4 vertical

		private static readonly double fontSize = 4;
	}
}
