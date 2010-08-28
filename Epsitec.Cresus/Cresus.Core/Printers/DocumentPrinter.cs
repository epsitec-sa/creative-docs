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
	public class DocumentPrinter
	{
		public DocumentPrinter(PrinterType printerType, string description, string group)
		{
			this.PrinterType = printerType;
			this.Description = description;
			this.Group       = group;
		}

		public PrinterType PrinterType
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string Group
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
