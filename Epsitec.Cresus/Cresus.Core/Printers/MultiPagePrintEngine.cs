//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class MultiPagePrintEngine : IPrintEngine
	{
		public MultiPagePrintEngine(Printers.AbstractEntityPrinter entityPrinter, Transform transform, int firstPage, int pageCount)
		{
			this.entityPrinter = entityPrinter;
			this.transform     = transform;
			this.firstPage     = firstPage;
			this.lastPage      = firstPage + pageCount;
		}

		#region IPrintEngine Members
		public void PrepareNewPage(PageSettings settings)
		{
			settings.Margins = new Margins (0);
		}

		public void FinishingPrintJob()
		{
		}

		public void StartingPrintJob()
		{
			this.entityPrinter.IsPreview = false;
			this.entityPrinter.CurrentPage = this.firstPage;
		}

		public PrintEngineStatus PrintPage(PrintPort port)
		{
			port.Transform = this.transform;
			this.entityPrinter.PrintCurrentPage (port);

			this.entityPrinter.CurrentPage++;

			if (this.entityPrinter.CurrentPage < this.lastPage)
			{
				return PrintEngineStatus.MorePages;
			}

			return PrintEngineStatus.FinishJob;
		}
		#endregion

		private readonly Printers.AbstractEntityPrinter entityPrinter;
		private readonly Transform transform;
		private readonly int firstPage;
		private readonly int lastPage;
	}
}
