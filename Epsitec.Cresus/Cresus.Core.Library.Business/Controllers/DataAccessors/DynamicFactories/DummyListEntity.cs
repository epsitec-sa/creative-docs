//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories.Helpers
{
	/// <summary>
	/// The <c>DummyListEntity</c> generic class provides an <c>Items</c> property which
	/// is used together with the <see cref="SummaryDummyListEntityViewController&lt;T&gt;"/>,
	/// in order to display a list of all available items in the UI, without having to write
	/// a summary view controller for every entity type.
	/// </summary>
	/// <typeparam name="T">The type of the items</typeparam>
	public class DummyListEntity<T> : GenericEntity
			where T : AbstractEntity
	{
		public DummyListEntity()
			: base (Druid.Empty)
		{

		}

		public IList<T> Items
		{
			get
			{
				return this.GetFieldCollection<T> ("dummy");
			}
		}
	}
}
