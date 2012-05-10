//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DataSetAccessor&lt;T&gt;</c> class is the specialized version of the base
	/// <see cref="DataSetAccessor"/> class.
	/// on the user's need.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	public class DataSetAccessor<T> : DataSetAccessor
		where T : AbstractEntity, new ()
	{
		public DataSetAccessor(CoreData data)
			: base (data, typeof (T))
		{
		}
		
		protected override AbstractEntity GetExample()
		{
			return new T ();
		}
	}
}
