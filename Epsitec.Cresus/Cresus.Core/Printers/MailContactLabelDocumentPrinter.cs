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

	public class MailContactLabelDocumentPrinter : AbstractDocumentPrinter
	{
		public MailContactLabelDocumentPrinter(CoreData coreData, AbstractEntityPrinter entityPrinter, MailContactEntity entity)
			: base (coreData, entityPrinter, entity)
		{
		}

		public override string JobName
		{
			get
			{
				if (this.Entity.LegalPerson.IsNotNull ())
				{
					return TextFormatter.FormatText ("Client", this.Entity.LegalPerson.Name).ToSimpleText ();
				}

				if (this.Entity.NaturalPerson.IsNotNull ())
				{
					return TextFormatter.FormatText ("Client", this.Entity.NaturalPerson.Lastname).ToSimpleText ();
				}

				return "Client inconnu";
			}
		}


		public override Size MinimalPageSize
		{
			get
			{
				return new Size (60, 25);  // à voir à l'usage
			}
		}

		public override Size MaximalPageSize
		{
			get
			{
				return new Size (100, 100);  // carré de 10x10 cm (à voir à l'usage)
			}
		}

		public override Size PreferredPageSize
		{
			get
			{
				return new Size (62, 29);  // petite étiquette pour Brother QL-560
			}
		}


		protected override Margins PageMargins
		{
			get
			{
				return new Margins (3);
			}
		}


		/// <summary>
		/// Spécifie l'unité d'impression, afin de déterminer la taille des pages à produire.
		/// </summary>
		public override void SetPrinterUnit(PrinterUnit printerUnit)
		{
			Size size;

			if (printerUnit == null)
			{
				size = this.PreferredPageSize;
			}
			else
			{
				size = printerUnit.PhysicalPaperSize;
			}

			if (size.Width < size.Height)  // format portrait ?
			{
				// On veut toujours être en paysage.
				size = new Size (size.Height, size.Width);
			}

			if (!Common.InsidePageSize (size, this.MinimalPageSize, this.MaximalPageSize))
			{
				size = this.PreferredPageSize;
			}

			this.requiredPageSize = size;
		}

		public override void BuildSections(List<DocumentOption> forcingOptionsToClear = null, List<DocumentOption> forcingOptionsToSet = null)
		{
			base.BuildSections (forcingOptionsToClear, forcingOptionsToSet);

			this.documentContainer.Clear ();

			if (this.HasPrinterUnitDefined (PrinterUnitFunction.ForLabelPage))
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.Label);
				this.BuildSummary ();
				this.documentContainer.Ending (firstPage);
			}
		}

		public override void PrintForegroundCurrentPage(IPaintPort port)
		{
			base.PrintForegroundCurrentPage (port);

			this.documentContainer.PaintBackground (port, this.CurrentPage, this.PreviewMode);
			this.documentContainer.PaintForeground (port, this.CurrentPage, this.PreviewMode);
		}


		private void BuildSummary()
		{
			//	Ajoute le résumé dans le document.
			var band = new TextBand ();
			band.Text = this.Entity.GetSummary ();
			band.FontSize = 3.0;

			this.documentContainer.AddFromTop (band, 0.0);
		}



		private MailContactEntity Entity
		{
			get
			{
				return this.entity as MailContactEntity;
			}
		}
	}
}
