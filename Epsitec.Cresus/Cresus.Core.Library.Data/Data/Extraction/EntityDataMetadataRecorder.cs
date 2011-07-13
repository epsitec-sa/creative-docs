//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	public abstract class EntityDataMetadataRecorder
	{
		public EntityDataMetadata GetMetadata()
		{
			return new EntityDataMetadata (this.Columns);
		}

		public abstract IEnumerable<EntityDataColumn> Columns
		{
			get;
		}
	}
}
