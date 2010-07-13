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
				return UIBuilder.FormatText ("Client", this.entity.IdA).ToSimpleText ();
			}
		}

		public override Size PageSize
		{
			get
			{
				return new Size (pageWidth, pageHeight);  // A4 vertical
			}
		}

		public override void BuildSections()
		{
			this.BuildTitle    (this.documentContainer);
			this.BuildSummary  (this.documentContainer);

			this.BuildContacts (this.documentContainer, this.BuildMailContacts);
			this.BuildContacts (this.documentContainer, this.BuildTelecomContacts);
			this.BuildContacts (this.documentContainer, this.BuildUriContacts);
		}

		public override void PrintCurrentPage(IPaintPort port, Rectangle bounds)
		{
#if true
			this.documentContainer.Paint (port, this.CurrentPage);
#else
			this.PaintTest (port);
#endif
		}


		private void BuildTitle(DocumentContainer document)
		{
			//	Dessine le titre et retourne la hauteur utilisée.
			string text = "?";

			if (this.entity.Person is NaturalPersonEntity)
			{
				var x = this.entity.Person as NaturalPersonEntity;
				text = UIBuilder.FormatText ("N°", this.entity.IdA, "-", x.Firstname, x.Lastname).ToString ();
			}

			if (this.entity.Person is LegalPersonEntity)
			{
				var x = this.entity.Person as LegalPersonEntity;
				text = UIBuilder.FormatText ("N°", this.entity.IdA, "-", x.Name).ToString ();
			}

			var textBox = new TextBand ();
			textBox.Text = string.Concat("<b>", text, "</b>");
			textBox.FontSize = 6.0;

			document.AddFromTop (textBox, 5.0);
		}

		private void BuildSummary(DocumentContainer document)
		{
			//	Dessine le résumé et retourne la hauteur utilisée.
			string text = "?";

			if (this.entity.Person is NaturalPersonEntity)
			{
				var x = this.entity.Person as NaturalPersonEntity;
				text = UIBuilder.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "\n", x.Gender.Name, "\n", x.BirthDate).ToString ();
			}

			if (this.entity.Person is LegalPersonEntity)
			{
				var x = this.entity.Person as LegalPersonEntity;
				text = UIBuilder.FormatText (x.Name).ToString ();
			}

			var textBox = new TextBand ();
			textBox.Text = text;
			textBox.FontSize = 4.0;

			document.AddFromTop (textBox, 5.0);
		}


		private void BuildContacts(DocumentContainer document, System.Func<DocumentContainer, bool> builder)
		{
			//	Dessine un contact et retourne la position 'top' suivante.
			for (int i = 0; i < 10; i++)  // TODO: debug, à enlever
			{
				builder (document);
			}
		}

		private bool BuildMailContacts(DocumentContainer document)
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
				return true;
			}

			var title = new TextBand ();
			title.Text = "<b>Adresses</b>";
			title.FontSize = 4.5;
			document.AddFromTop (title, 1.0);

			var table = new TableBand ();
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

			document.AddFromTop (table, 5.0);
			return true;
		}

		private bool BuildTelecomContacts(DocumentContainer document)
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
				return true;
			}

			var title = new TextBand ();
			title.Text = "<b>Téléphones</b>";
			title.FontSize = 4.5;
			document.AddFromTop (title, 1.0);

			var table = new TableBand ();
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

			document.AddFromTop (table, 5.0);
			return true;
		}

		private bool BuildUriContacts(DocumentContainer document)
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
				return true;
			}

			var title = new TextBand ();
			title.Text = "<b>Emails</b>";
			title.FontSize = 4.5;
			document.AddFromTop (title, 1.0);

			var table = new TableBand ();
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

			document.AddFromTop (table, 5.0);
			return true;
		}


		private void PaintTest(IPaintPort port)
		{
			Font font = Font.GetFont ("Times New Roman", "Regular");

#if true
			var table = new TableBand ();
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
			table.SetText (2, 2, "<font size=\"2\">L'histoire en Grèce antique conserve certains de ces aspects en développant parallèlement des préoccupations littéraires et scientifiques comme en témoignent les oeuvres d'Hérodote, de Thucydide et de Polybe. Hérodote (-484 ou -482, -425) est un savant grec qui parcourt durant sa vie l'Égypte actuelle et le Moyen-Orient, allant jusqu'à Babylone. Dans ses Enquêtes, il veut faire oeuvre de mémorialiste et raconte des événements récents, les guerres médiques, « <i>afin que le temps n'abolisse pas les travaux des hommes</i> ». Il se place donc dans une perspective historique qui fait qu'on a pu le qualifier de « <i>père de l'histoire</i> ».</font>");
			table.SetText (3, 2, "MotTropLongPourLaCellule");

			table.SetRelativeColumWidth (1, 1.5);
			table.DebugPaintFrame = true;

			double height = 20+this.DebugParam1;
			bool ok = table.BuildSections (90+this.DebugParam2, height, height, height);

			for (int i = 0; i < table.SectionCount; i++)
			{
				double y = 280-(height+2)*i;

				table.Paint (port, i, new Point (10, y));

				port.Color = Color.FromName (ok ? "Black" : "Red");
				port.PaintSurface (Path.FromRectangle (8, y-height, 1, height));
			}
#endif

#if false
			string t = "Ceci est un <u>texte bidon</u> mais <b>assez long</b>, pour permettre de <font size=\"6\">tester</font> le découpage en plusieurs pavés distincts, qui seront dessinés sur plusieurs pages.<br/><br/><i>Et voilà la <font color=\"#ff0000\">suite et la fin</font> de ce chef d'œuvre littéraire sur une toute nouvelle ligne.</i>";

			var textBox = new ObjectTextBox ();
			textBox.Font = font;
			textBox.FontSize = fontSize;
			textBox.Text = t;
			textBox.DebugPaintFrame = true;

			double top = 120;
			double initialHeight = 15;
			double middleHeight = 25;
			double finalHeight = 16;

			for (int width = 50; width >= 20; width-=10)
			{
				textBox.InitializePages (width, initialHeight, middleHeight, finalHeight);

				for (int i = 0; i < textBox.PageCount; i++)
				{
					textBox.Paint (port, i, new Point (10+(width+1)*i, top));
				}

				port.LineWidth = 0.1;
				port.Color = Color.FromName ("Blue");
				port.PaintOutline (Path.FromRectangle (new Rectangle (10, top-middleHeight, 190, middleHeight)));
				port.Color = Color.FromName ("Green");
				port.PaintOutline (Path.FromLine (10, top-initialHeight, 200, top-initialHeight));
				port.Color = Color.FromName ("Red");
				port.PaintOutline (Path.FromLine (10, top-finalHeight, 200, top-finalHeight));

				top -= middleHeight+2;
			}
#endif
		}


		private static readonly double pageWidth  = 210;
		private static readonly double pageHeight = 297;  // A4 vertical

		private static readonly double fontSize = 4;
	}
}
