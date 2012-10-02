//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DataSetAccessor&lt;T&gt;</c> class is the specialized version of the base
	/// <see cref="DataSetAccessor"/> class.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	public class DataSetAccessor<T> : DataSetAccessor
		where T : AbstractEntity, new ()
	{
		public DataSetAccessor(CoreData data, DataSetMetadata metadata)
			: base (data, typeof (T), metadata)
		{
		}
		
		protected override AbstractEntity GetExample()
		{
			return new T ();
		}
	}
}
