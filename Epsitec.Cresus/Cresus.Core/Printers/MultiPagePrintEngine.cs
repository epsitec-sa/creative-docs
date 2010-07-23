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
		public MultiPagePrintEngine(Printers.AbstractEntityPrinter entityPrinter, Transform transform)
		{
			this.entityPrinter = entityPrinter;
			this.transform = transform;
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
			this.entityPrinter.CurrentPage = 0;
		}

		public PrintEngineStatus PrintPage(PrintPort port)
		{
			port.Transform = this.transform;
			this.entityPrinter.PrintCurrentPage (port);

			this.entityPrinter.CurrentPage++;

			if (this.entityPrinter.CurrentPage < this.entityPrinter.PageCount)
			{
				return PrintEngineStatus.MorePages;
			}

			return PrintEngineStatus.FinishJob;
		}
		#endregion

		private readonly Printers.AbstractEntityPrinter entityPrinter;
		private readonly Transform transform;
	}
}
