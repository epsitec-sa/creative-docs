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
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Print;

namespace Epsitec.Cresus.Core.Business.EntityPrinters
{
	public sealed class RelationPrinter : AbstractPrinter
	{
		private RelationPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
		}

		private static IEnumerable<DocumentOption> RequiredDocumentOptions
		{
			get
			{
				yield return DocumentOption.Orientation;
				yield return DocumentOption.Specimen;
				yield return DocumentOption.FontSize;
				yield return DocumentOption.Language;

				yield return DocumentOption.LeftMargin;
				yield return DocumentOption.RightMargin;
				yield return DocumentOption.TopMargin;
				yield return DocumentOption.BottomMargin;

				yield return DocumentOption.LayoutFrame;

				yield return DocumentOption.RelationMail;
				yield return DocumentOption.RelationTelecom;
				yield return DocumentOption.RelationUri;
			}
		}

		private static IEnumerable<PageType> RequiredPageTypes
		{
			get
			{
				yield return PageType.Single;
				yield return PageType.First;
				yield return PageType.Following;
			}
		}


		public override string JobName
		{
			get
			{
				return TextFormatter.FormatText ("Contact").ToSimpleText ();
			}
		}


		public override Size PreferredPageSize
		{
			get
			{
				return new Size (210, 297);  // A4
			}
		}

		protected override Margins GetPageMargins()
		{
			double leftMargin   = this.GetOptionValue (DocumentOption.LeftMargin);
			double rightMargin  = this.GetOptionValue (DocumentOption.RightMargin);
			double topMargin    = this.GetOptionValue (DocumentOption.TopMargin);
			double bottomMargin = this.GetOptionValue (DocumentOption.BottomMargin);

			return new Margins (leftMargin, rightMargin, topMargin, bottomMargin);
		}


		public override FormattedText BuildSections()
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

			return null;  // ok
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
			FormattedText text = "?";

			if (this.Entity.Person is NaturalPersonEntity)
			{
				var x = this.Entity.Person as NaturalPersonEntity;
				text = TextFormatter.FormatText (/*"N°", this.Entity.IdA, "-", */x.Firstname, x.Lastname).ToString ();
			}

			if (this.Entity.Person is LegalPersonEntity)
			{
				var x = this.Entity.Person as LegalPersonEntity;
				text = TextFormatter.FormatText (/*"N°", this.Entity.IdA, "-", */x.Name).ToString ();
			}

			var band = new TextBand ();
			band.TwoLetterISOLanguageName = this.TwoLetterISOLanguageName;
			band.Text = text.ApplyBold ();
			band.FontSize = this.FontSize*2.0;

			this.documentContainer.AddFromTop (band, 5.0);
		}

		private void BuildSummary()
		{
			//	Ajoute le résumé dans le document.
			FormattedText text = "?";

			if (this.Entity.Person is NaturalPersonEntity)
			{
				var x = this.Entity.Person as NaturalPersonEntity;
				text = TextFormatter.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "\n", x.Gender.Name, "\n", x.DateOfBirth);
			}

			if (this.Entity.Person is LegalPersonEntity)
			{
				var x = this.Entity.Person as LegalPersonEntity;
				text = TextFormatter.FormatText (x.Name);
			}

			var band = new TextBand ();
			band.TwoLetterISOLanguageName = this.TwoLetterISOLanguageName;
			band.Text = text;
			band.FontSize = this.FontSize*1.2;

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
			title.TwoLetterISOLanguageName = this.TwoLetterISOLanguageName;
			title.Text = TextFormatter.FormatText ("Adresses").ApplyBold ();
			title.FontSize = this.FontSize*1.5;
			this.documentContainer.AddFromTop (title, 1.0);

			var table = new TableBand ();
			table.TwoLetterISOLanguageName = this.TwoLetterISOLanguageName;
			table.ColumnsCount = 5;
			table.RowsCount = 1+count;
			table.CellBorder = this.GetCellBorder ();
			table.CellMargins = new Margins (this.CellMargin);
			table.SetCellBorder (0, this.GetCellBorder (bottomBold: true));

			table.SetRelativeColumWidth (0, 1.5);
			table.SetRelativeColumWidth (1, 2.0);
			table.SetRelativeColumWidth (2, 0.4);
			table.SetRelativeColumWidth (3, 1.0);
			table.SetRelativeColumWidth (4, 0.8);

			int index = 0;
			table.SetText (0, index, TextFormatter.FormatText ("Rôles"  ).ApplyBold (), this.FontSize);
			table.SetText (1, index, TextFormatter.FormatText ("Adresse").ApplyBold (), this.FontSize);
			table.SetText (2, index, TextFormatter.FormatText ("NPA"    ).ApplyBold (), this.FontSize);
			table.SetText (3, index, TextFormatter.FormatText ("Ville"  ).ApplyBold (), this.FontSize);
			table.SetText (4, index, TextFormatter.FormatText ("Pays"   ).ApplyBold (), this.FontSize);
			index++;

			foreach (var x in this.Entity.Person.Contacts.OfType<MailContactEntity> ())
			{
				table.SetText (0, index, TextFormatter.FormatText (string.Join (", ", x.ContactGroups.Select (role => role.Name))), this.FontSize);
				table.SetText (1, index, TextFormatter.FormatText (x.PersonAddress, "\n", x.Complement, "\n", x.StreetName, "\n", x.PostBoxNumber, "\n", x.Location.Country.CountryCode, "~-", x.Location.PostalCode, x.Location.Name), this.FontSize);
				table.SetText (2, index, x.Location.PostalCode, this.FontSize);
				table.SetText (3, index, x.Location.Name, this.FontSize);
				table.SetText (4, index, x.Location.Country.Name, this.FontSize);
				index++;
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
			title.TwoLetterISOLanguageName = this.TwoLetterISOLanguageName;
			title.Text = TextFormatter.FormatText ("Téléphones").ApplyBold ();
			title.FontSize = this.FontSize*1.5;
			this.documentContainer.AddFromTop (title, 1.0);

			var table = new TableBand ();
			table.TwoLetterISOLanguageName = this.TwoLetterISOLanguageName;
			table.ColumnsCount = 3;
			table.RowsCount = 1+count;
			table.CellBorder = this.GetCellBorder ();
			table.CellMargins = new Margins (this.CellMargin);
			table.SetCellBorder (0, this.GetCellBorder (bottomBold: true));

			table.SetRelativeColumWidth (0, 1.5);
			table.SetRelativeColumWidth (1, 1.5);
			table.SetRelativeColumWidth (2, 2.0+0.3+1.0+0.8-1.5);

			int index = 0;
			table.SetText (0, index, TextFormatter.FormatText ("Rôles" ).ApplyBold (), this.FontSize);
			table.SetText (1, index, TextFormatter.FormatText ("Type"  ).ApplyBold (), this.FontSize);
			table.SetText (2, index, TextFormatter.FormatText ("Numéro").ApplyBold (), this.FontSize);
			index++;

			foreach (var contact in this.Entity.Person.Contacts)
			{
				if (contact is TelecomContactEntity)
				{
					var x = contact as TelecomContactEntity;

					table.SetText (0, index, TextFormatter.FormatText (string.Join (", ", x.ContactGroups.Select (role => role.Name))), this.FontSize);
					table.SetText (1, index, TextFormatter.FormatText (x.TelecomType.Name), this.FontSize);
					table.SetText (2, index, TextFormatter.FormatText (x.Number), this.FontSize);
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
			title.TwoLetterISOLanguageName = this.TwoLetterISOLanguageName;
			title.Text = TextFormatter.FormatText ("Emails").ApplyBold ();
			title.FontSize = this.FontSize*1.5;
			this.documentContainer.AddFromTop (title, 1.0);

			var table = new TableBand ();
			table.TwoLetterISOLanguageName = this.TwoLetterISOLanguageName;
			table.ColumnsCount = 2;
			table.RowsCount = 1+count;
			table.CellBorder = this.GetCellBorder ();
			table.CellMargins = new Margins (this.CellMargin);
			table.SetCellBorder (0, this.GetCellBorder (bottomBold: true));

			table.SetRelativeColumWidth (0, 1.5);
			table.SetRelativeColumWidth (1, 2.0+0.3+1.0+0.8);

			int index = 0;
			table.SetText (0, index, TextFormatter.FormatText ("Rôles"                 ).ApplyBold (), this.FontSize);
			table.SetText (1, index, TextFormatter.FormatText ("Adresses électroniques").ApplyBold (), this.FontSize);
			index++;

			foreach (var contact in this.Entity.Person.Contacts)
			{
				if (contact is UriContactEntity)
				{
					var x = contact as UriContactEntity;

					table.SetText (0, index, TextFormatter.FormatText (string.Join (", ", x.ContactGroups.Select (role => role.Name))), this.FontSize);
					table.SetText (1, index, TextFormatter.FormatText (x.Uri), this.FontSize);
					index++;
				}
			}

			this.documentContainer.AddFromTop (table, 5.0);
			return true;
		}


		private double CellMargin
		{
			get
			{
				return this.IsWithFrame ? 1 : 2;
			}
		}

		private CellBorder GetCellBorder(bool bottomBold = false, bool topBold = false, bool bottomLess = false, bool topLess = false)
		{
			//	Retourne les bordures à utiliser pour une ligne entière.
			double leftWidth   = 0;
			double rightWidth  = 0;
			double bottomWidth = 0;
			double topWidth    = 0;

			//	Initialise pour le style choisi.
			if (this.IsWithFrame)
			{
				leftWidth   = CellBorder.NormalWidth;
				rightWidth  = CellBorder.NormalWidth;
				bottomWidth = CellBorder.NormalWidth;
				topWidth    = CellBorder.NormalWidth;
			}
			else if (this.IsWithLine)
			{
				bottomWidth = CellBorder.NormalWidth;
			}

			//	Ajoute ou enlève, selon les exceptions.
			if (bottomBold)
			{
				bottomWidth = CellBorder.BoldWidth;
			}

			if (topBold)
			{
				topWidth = CellBorder.BoldWidth;
			}

			if (bottomLess)
			{
				bottomWidth = 0;
			}

			if (topLess)  // :-O
			{
				topWidth = 0;
			}

			return new CellBorder (leftWidth, rightWidth, bottomWidth, topWidth);
		}

		private bool IsWithLine
		{
			get
			{
				return this.HasOption (DocumentOption.LayoutFrame, "WithLine");
			}
		}

		private bool IsWithFrame
		{
			get
			{
				return this.HasOption (DocumentOption.LayoutFrame, "WithFrame");
			}
		}


		private RelationEntity Entity
		{
			get
			{
				return this.entity as RelationEntity;
			}
		}
		
		
		private class Factory : IEntityPrinterFactory
		{
			#region IEntityPrinterFactory Members

			public bool CanPrint(AbstractEntity entity, PrintingOptionDictionary options)
			{
				return entity is RelationEntity || entity is CustomerEntity;
			}

			public IEnumerable<System.Type> GetSupportedEntityTypes()
			{
				yield return typeof (RelationEntity);
				yield return typeof (CustomerEntity);
			}

			public DocumentType GetDocumentType(AbstractEntity entity)
			{
				if ((entity is RelationEntity) ||
					(entity is CustomerEntity))
				{
					return DocumentType.RelationSummary;
				}

				return DocumentType.Unknown;
			}

			public IEntityPrinter CreatePrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			{
				var customer = entity as CustomerEntity;

				if (customer != null)
				{
					return new RelationPrinter (businessContext, customer.MainRelation, options, printingUnits);
				}

				return new RelationPrinter (businessContext, entity, options, printingUnits);
			}

			public IEnumerable<DocumentOption> GetRequiredDocumentOptions(DocumentType documentType)
			{
				switch (documentType)
				{
					case DocumentType.RelationSummary:
						return RelationPrinter.RequiredDocumentOptions;
				}

				return null;
			}

			public IEnumerable<PageType> GetRequiredPageTypes(DocumentType documentType)
			{
				switch (documentType)
				{
					case DocumentType.RelationSummary:
						return RelationPrinter.RequiredPageTypes;
				}

				return null;
			}

			#endregion
		}

	}
}
