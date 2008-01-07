//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public class DataQueryColumn
	{
		public DataQueryColumn(EntityFieldPath fieldPath)
		{
			this.fieldPath = fieldPath;
		}
		
		public DataQueryColumn(EntityFieldPath entityFieldPath, DataQuerySortOrder sortOrder)
		{
			this.fieldPath = entityFieldPath;
			this.sortOrder = sortOrder;
		}

		
		public EntityFieldPath FieldPath
		{
			get
			{
				return this.fieldPath;
			}
		}

		public DataQuerySortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}


		private readonly EntityFieldPath fieldPath;
		private readonly DataQuerySortOrder sortOrder;
	}
}
