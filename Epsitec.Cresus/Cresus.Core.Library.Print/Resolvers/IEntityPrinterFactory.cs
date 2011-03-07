//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Print.EntityPrinters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	public interface IEntityPrinterFactory
	{
		bool CanPrint(AbstractEntity entity, OptionsDictionary options);
		AbstractPrinter CreatePrinter(IBusinessContext businessContext, IEnumerable<AbstractEntity> entities, OptionsDictionary options, PrintingUnitsDictionary printingUnits);
	}
}
