//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Helpers
{
	internal class EntityDataMapping
	{
		public EntityDataMapping(AbstractEntity entity)
		{
			this.entity = entity;
			this.rowKey = new DbKey ();
		}

		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}

		public DbKey RowKey
		{
			get
			{
				return this.rowKey;
			}
		}

		private readonly AbstractEntity entity;
		private DbKey rowKey;
	}
}
