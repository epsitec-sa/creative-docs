//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	public sealed class EntityDataColumnConverter
	{
		public EntityDataColumnConverter(EntityDataType dataType, System.Func<AbstractEntity, string> f1, System.Func<AbstractEntity, long> f2)
		{
			this.dataType = dataType;
			this.f1 = f1;
			this.f2 = f2;
		}

		public EntityDataType DataType
		{
			get
			{
				return this.dataType;
			}
		}

		public bool IsNumeric
		{
			get
			{
				return this.f2 != null;
			}
		}

		public string GetText(AbstractEntity entity)
		{
			return this.f1 (entity);
		}

		public long GetNumber(AbstractEntity entity)
		{
			return this.f2 (entity);
		}

		private readonly EntityDataType dataType;
		System.Func<AbstractEntity, string> f1;
		System.Func<AbstractEntity, long> f2;
	}
}
