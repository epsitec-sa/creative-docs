//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataMetadataRecorder</c> class is used to record column definitions. See
	/// the generic type for more details.
	/// </summary>
	public abstract class EntityDataMetadataRecorder
	{
		/// <summary>
		/// Gets the metadata information, which is available only after all columns have
		/// been added to the recorder.
		/// </summary>
		/// <returns></returns>
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
