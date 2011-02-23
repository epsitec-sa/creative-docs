#if false
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
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Print.Bands;
using Epsitec.Cresus.Core.Print.Containers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print.EntityPrinters
{

	public class RelationPrinter : AbstractPrinter
	{
		public RelationPrinter(CoreData coreData, IEnumerable<AbstractEntity> entities, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
			: base (coreData, entities, options, printingUnits)
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


		public override void BuildSections()
		{
			base.BuildSections ();

			this.documentContainer.Clear ();

			int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

			this.BuildTitle ();
			this.BuildSummary ();

			if (this.HasOption (DocumentOption.RelationMail))
			{
				this.BuildContacts (this.BuildMailContacts);
			}

			if (this.HasOption (DocumentOption.RelationTelecom))
			{
				this.BuildContacts (this.BuildTelecomContacts);
			}

			if (this.HasOption (DocumentOption.RelationUri))
			{
				this.BuildContacts (this.BuildUriContacts);
			}

			this.documentContainer.Ending (firstPage);
		}

		public override void PrintForegroundCurrentPage(IPaintPort port)
		{
			base.PrintForegroundCurrentPage (port);

			this.documentContainer.PaintBackground (port, this.CurrentPage, this.PreviewMode);
			this.documentContainer.PaintForeground (port, this.CurrentPage, this.PreviewMode);
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


		private RelationEntity Entity
		{
			get
			{
				return this.entities.FirstOrDefault () as RelationEntity;
			}
		}


		private static readonly double fontSize = 4;
	}
}
#endif
