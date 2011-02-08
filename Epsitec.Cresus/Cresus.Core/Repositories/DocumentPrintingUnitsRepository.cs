﻿//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Repositories
{
	public class DocumentPrintingUnitsRepository : Repository<DocumentPrintingUnitsEntity>
	{
		public DocumentPrintingUnitsRepository(CoreData data, DataContext context = null)
			: base (data, context)
		{
		}
	}
}
