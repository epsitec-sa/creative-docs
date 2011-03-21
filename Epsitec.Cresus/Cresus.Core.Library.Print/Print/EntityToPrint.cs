//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Documents;

using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print
{
	public class EntityToPrint
	{
		public EntityToPrint(AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits, string title)
		{
			this.Entity        = entity;
			this.Options       = options;
			this.PrintingUnits = printingUnits;
			this.Title         = title;
		}

		public AbstractEntity Entity
		{
			get;
			private set;
		}

		public PrintingOptionDictionary Options
		{
			get;
			private set;
		}

		public PrintingUnitDictionary PrintingUnits
		{
			get;
			private set;
		}

		public string Title
		{
			get;
			private set;
		}
	}
}
