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

	public class NaturalPersonLabelDocumentPrinter : AbstractDocumentPrinter
	{
		public NaturalPersonLabelDocumentPrinter(AbstractEntityPrinter entityPrinter, NaturalPersonEntity entity)
			: base (entityPrinter, entity)
		{
		}

		public override string JobName
		{
			get
			{
				return TextFormatter.FormatText ("Client", this.Entity.Lastname).ToSimpleText ();
			}
		}

		public override Size PageSize
		{
			get
			{
				return new Size (90, 62);  // étiquette
			}
		}

		public override void BuildSections(List<DocumentOption> forcingOptionsToClear = null, List<DocumentOption> forcingOptionsToSet = null)
		{
			base.BuildSections (forcingOptionsToClear, forcingOptionsToSet);

			this.documentContainer.Clear ();
			int firstPage = this.documentContainer.PrepareEmptyPage (PageType.Label);
			this.BuildSummary ();
			this.documentContainer.Ending (firstPage);
		}

		public override void PrintForegroundCurrentPage(IPaintPort port)
		{
			base.PrintForegroundCurrentPage (port);

			this.documentContainer.PaintBackground (port, this.CurrentPage, this.IsPreview);
			this.documentContainer.PaintForeground (port, this.CurrentPage, this.IsPreview);
		}


		private void BuildSummary()
		{
			//	Ajoute le résumé dans le document.
			FormattedText text = "?";

			text = TextFormatter.FormatText (this.Entity.Title.Name, "\n", this.Entity.Firstname, this.Entity.Lastname, "\n", this.Entity.Gender.Name, "\n", this.Entity.BirthDate);

			var band = new TextBand ();
			band.Text = text;
			band.FontSize = 4.0;

			this.documentContainer.AddFromTop (band, 5.0);
		}



		private NaturalPersonEntity Entity
		{
			get
			{
				return this.entity as NaturalPersonEntity;
			}
		}


		private static readonly double fontSize = 4;
	}
}
