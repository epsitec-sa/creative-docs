//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class PrinterToUse
	{
		public PrinterToUse(PageTypeEnum pageType, string description)
		{
			this.PageType = pageType;
			this.Description = description;
		}

		public PageTypeEnum PageType
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string LogicalPrinterName
		{
			get;
			set;
		}
	}
}
