//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Print.Bands;
using Epsitec.Cresus.Core.Print.Containers;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Epsitec.Cresus.Core.Print.EntityPrinters;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.EntityPrinters
{
	public class MailContactPrinter : AbstractPrinter
	{
		private MailContactPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
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
				double leftMargin   = this.GetOptionValue (DocumentOption.LeftMargin,   3);
				double rightMargin  = this.GetOptionValue (DocumentOption.RightMargin,  3);
				double topMargin    = this.GetOptionValue (DocumentOption.TopMargin,    3);
				double bottomMargin = this.GetOptionValue (DocumentOption.BottomMargin, 3);

				return new Margins (leftMargin, rightMargin, topMargin, bottomMargin);
			}
		}


		public override void BuildSections()
		{
			base.BuildSections ();

			this.documentContainer.Clear ();

			if (this.HasPrintingUnitDefined (PageType.Label))
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
			band.FontSize = this.FontSize;

			this.documentContainer.AddFromTop (band, 0.0);
		}


		private MailContactEntity Entity
		{
			get
			{
				return this.entity as MailContactEntity;
			}
		}
		
		private class Factory : IEntityPrinterFactory
		{
			#region IEntityPrinterFactory Members

			public bool CanPrint(AbstractEntity entity, PrintingOptionDictionary options)
			{
				return entity is MailContactEntity;
			}

			public IEnumerable<System.Type> GetSupportedEntityTypes()
			{
				yield return typeof (MailContactEntity);
			}

			public AbstractPrinter CreatePrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			{
				return new MailContactPrinter (businessContext, entity, options, printingUnits);
			}

			#endregion
		}
	}
}
