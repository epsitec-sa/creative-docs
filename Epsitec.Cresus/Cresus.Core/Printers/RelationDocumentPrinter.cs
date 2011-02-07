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

	public class RelationDocumentPrinter : AbstractDocumentPrinter
	{
		public RelationDocumentPrinter(CoreData coreData, AbstractEntityPrinter entityPrinter, RelationEntity entity)
			: base (coreData, entityPrinter, entity)
		{
		}

		public override string JobName
		{
			get
			{
				return TextFormatter.FormatText ("Client", this.Entity.IdA).ToSimpleText ();
			}
		}


		public override Size MinimalPageSize
		{
			get
			{
				return new Size (210, 297);  // A4
			}
		}

		public override Size MaximalPageSize
		{
			get
			{
				return new Size (210, 297);  // A4
			}
		}

		public override Size PreferredPageSize
		{
			get
			{
				return new Size (210, 297);  // A4
			}
		}


		public override void BuildSections(List<DocumentOption> forcingOptionsToClear = null, List<DocumentOption> forcingOptionsToSet = null)
		{
			base.BuildSections (forcingOptionsToClear, forcingOptionsToSet);

			this.documentContainer.Clear ();

			if (this.SelectedDocumentType == Business.DocumentType.Summary)
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

				this.BuildTitle ();
				this.BuildSummary ();

				if (this.HasDocumentOption (DocumentOption.RelationMail))
				{
					this.BuildContacts (this.BuildMailContacts);
				}

				if (this.HasDocumentOption (DocumentOption.RelationTelecom))
				{
					this.BuildContacts (this.BuildTelecomContacts);
				}

				if (this.HasDocumentOption (DocumentOption.RelationUri))
				{
					this.BuildContacts (this.BuildUriContacts);
				}

				this.documentContainer.Ending (firstPage);
			}
		}

		public override void PrintForegroundCurrentPage(IPaintPort port)
		{
			base.PrintForegroundCurrentPage (port);

			if (this.SelectedDocumentType == Business.DocumentType.Summary)
			{
				this.documentContainer.PaintBackground (port, this.CurrentPage, this.PreviewMode);
				this.documentContainer.PaintForeground (port, this.CurrentPage, this.PreviewMode);
			}

			if (this.SelectedDocumentType == Business.DocumentType.Debug1)
			{
				this.PaintTest1 (port);
			}

			if (this.SelectedDocumentType == Business.DocumentType.Debug2)
			{
				this.PaintTest2 (port);
			}
		}


		private void BuildTitle()
		{
			//	Ajoute le titre dans le document.
			string text = "?";

			if (this.Entity.Person is NaturalPersonEntity)
			{
				var x = this.Entity.Person as NaturalPersonEntity;
				text = TextFormatter.FormatText ("N°", this.Entity.IdA, "-", x.Firstname, x.Lastname).ToString ();
			}

			if (this.Entity.Person is LegalPersonEntity)
			{
				var x = this.Entity.Person as LegalPersonEntity;
				text = TextFormatter.FormatText ("N°", this.Entity.IdA, "-", x.Name).ToString ();
			}

			var band = new TextBand ();
			band.Text = TextFormatter.FormatText ("<b>", text, "</b>");
			band.FontSize = 6.0;

			this.documentContainer.AddFromTop (band, 5.0);
		}

		private void BuildSummary()
		{
			//	Ajoute le résumé dans le document.
			FormattedText text = "?";

			if (this.Entity.Person is NaturalPersonEntity)
			{
				var x = this.Entity.Person as NaturalPersonEntity;
				text = TextFormatter.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "\n", x.Gender.Name, "\n", x.BirthDate);
			}

			if (this.Entity.Person is LegalPersonEntity)
			{
				var x = this.Entity.Person as LegalPersonEntity;
				text = TextFormatter.FormatText (x.Name);
			}

			var band = new TextBand ();
			band.Text = text;
			band.FontSize = 4.0;

			this.documentContainer.AddFromTop (band, 5.0);
		}


		private void BuildContacts(System.Func<bool> builder)
		{
			//	Ajoute un contact dans le document.
			builder ();
		}

		private bool BuildMailContacts()
		{
			//	Ajoute un contact dans le document.
			int count = 0;
			foreach (var contact in this.Entity.Person.Contacts)
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
			title.Text = Misc.Bold ("Adresses");
			title.FontSize = 4.5;
			this.documentContainer.AddFromTop (title, 1.0);

			var table = new TableBand ();
			table.ColumnsCount = 5;
			table.RowsCount = 1+count;

			table.SetRelativeColumWidth (0, 1.5);
			table.SetRelativeColumWidth (1, 2.0);
			table.SetRelativeColumWidth (2, 0.3);
			table.SetRelativeColumWidth (3, 1.0);
			table.SetRelativeColumWidth (4, 0.8);

			int index = 0;
			table.SetText (0, index, Misc.Bold ("Rôles"));
			table.SetText (1, index, Misc.Bold ("Adresse"));
			table.SetText (2, index, Misc.Bold ("NPA"));
			table.SetText (3, index, Misc.Bold ("Ville"));
			table.SetText (4, index, Misc.Bold ("Pays"));
			index++;

			foreach (var contact in this.Entity.Person.Contacts)
			{
				if (contact is MailContactEntity)
				{
					var x = contact as MailContactEntity;

					table.SetText (0, index, TextFormatter.FormatText (string.Join (", ", x.ContactGroups.Select (role => role.Name))));
					table.SetText (1, index, TextFormatter.FormatText (x.LegalPerson.Name, "\n", x.LegalPerson.Complement, "\n", x.Complement, "\n", x.Address.Street.StreetName, "\n", x.Address.Street.Complement, "\n", x.Address.PostBox.Number, "\n", x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name));
					table.SetText (2, index, x.Address.Location.PostalCode);
					table.SetText (3, index, x.Address.Location.Name);
					table.SetText (4, index, x.Address.Location.Country.Name);
					index++;
				}
			}

			this.documentContainer.AddFromTop (table, 5.0);
			return true;
		}

		private bool BuildTelecomContacts()
		{
			//	Ajoute un contact dans le document.
			int count = 0;
			foreach (var contact in this.Entity.Person.Contacts)
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
			title.Text = Misc.Bold ("Téléphones");
			title.FontSize = 4.5;
			this.documentContainer.AddFromTop (title, 1.0);

			var table = new TableBand ();
			table.ColumnsCount = 3;
			table.RowsCount = 1+count;

			table.SetRelativeColumWidth (0, 1.5);
			table.SetRelativeColumWidth (1, 1.5);
			table.SetRelativeColumWidth (2, 2.0+0.3+1.0+0.8-1.5);

			int index = 0;
			table.SetText (0, index, Misc.Bold ("Rôles"));
			table.SetText (1, index, Misc.Bold ("Type"));
			table.SetText (2, index, Misc.Bold ("Numéro"));
			index++;

			foreach (var contact in this.Entity.Person.Contacts)
			{
				if (contact is TelecomContactEntity)
				{
					var x = contact as TelecomContactEntity;

					table.SetText (0, index, TextFormatter.FormatText (string.Join (", ", x.ContactGroups.Select (role => role.Name))));
					table.SetText (1, index, TextFormatter.FormatText (x.TelecomType.Name));
					table.SetText (2, index, TextFormatter.FormatText (x.Number));
					index++;
				}
			}

			this.documentContainer.AddFromTop (table, 5.0);
			return true;
		}

		private bool BuildUriContacts()
		{
			//	Ajoute un contact dans le document.
			int count = 0;
			foreach (var contact in this.Entity.Person.Contacts)
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
			title.Text = Misc.Bold ("Emails");
			title.FontSize = 4.5;
			this.documentContainer.AddFromTop (title, 1.0);

			var table = new TableBand ();
			table.ColumnsCount = 2;
			table.RowsCount = 1+count;

			table.SetRelativeColumWidth (0, 1.5);
			table.SetRelativeColumWidth (1, 2.0+0.3+1.0+0.8);

			int index = 0;
			table.SetText (0, index, Misc.Bold ("Rôles"));
			table.SetText (1, index, Misc.Bold ("Adresses électroniques"));
			index++;

			foreach (var contact in this.Entity.Person.Contacts)
			{
				if (contact is UriContactEntity)
				{
					var x = contact as UriContactEntity;

					table.SetText (0, index, TextFormatter.FormatText (string.Join (", ", x.ContactGroups.Select (role => role.Name))));
					table.SetText (1, index, TextFormatter.FormatText (x.Uri));
					index++;
				}
			}

			this.documentContainer.AddFromTop (table, 5.0);
			return true;
		}


		private void PaintTest1(IPaintPort port)
		{
			Font font = Font.GetFont ("Times New Roman", "Regular");

			FormattedText t = "Ceci est un <u>texte bidon</u> mais <b>assez long</b>, pour permettre de <font size=\"6\">tester</font> le découpage en plusieurs pavés distincts, qui seront dessinés sur plusieurs pages.<br/><br/><i>Et voilà la <font color=\"#ff0000\">suite et la fin</font> de ce chef d'œuvre littéraire sur une toute nouvelle ligne.</i>";

			var textBand = new TextBand ();
			textBand.Font = font;
			textBand.FontSize = fontSize;
			textBand.Text = t;
			textBand.DebugPaintFrame = true;

			double top = 280;
			double initialHeight = 15;
			double middleHeight = 25+this.DebugParam1;
			double finalHeight = 16;

			for (int width = 50+this.DebugParam2; width >= 20; width-=10)
			{
				textBand.BuildSections (width, initialHeight, middleHeight, finalHeight);

				for (int i = 0; i < textBand.SectionCount; i++)
				{
					textBand.PaintForeground (port, PreviewMode.Print, i, new Point (10+(width+1)*i, top));
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
		}

		private void PaintTest2(IPaintPort port)
		{
			Font font = Font.GetFont ("Times New Roman", "Regular");

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

				table.PaintForeground (port, PreviewMode.Print, i, new Point (10, y));

				port.Color = Color.FromName (ok ? "Black" : "Red");
				port.PaintSurface (Path.FromRectangle (8, y-height, 1, height));
			}
		}


		private RelationEntity Entity
		{
			get
			{
				return this.entity as RelationEntity;
			}
		}


		private static readonly double fontSize = 4;
	}
}
