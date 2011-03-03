//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>DynamicAccessorFactory</c> generic class creates <see cref="CollectionAccessor"/>
	/// instances based on the type arguments.
	/// </summary>
	/// <typeparam name="T1">The type of the data source.</typeparam>
	/// <typeparam name="T2">The type of the data items.</typeparam>
	/// <typeparam name="T3">The type of the data items represented by the collection template (same type as <c>T2</c> or derived from <c>T2</c>).</typeparam>
	internal sealed class DynamicAccessorFactory<T1, T2, T3> : DynamicAccessorFactory
		where T1 : AbstractEntity, new ()
		where T2 : AbstractEntity, new ()
		where T3 : T2, new ()
	{
		public DynamicAccessorFactory(EntityViewController<T1> controller, System.Func<T1, IList<T2>> collectionResolver, CollectionTemplate<T3> template)
			: base (CollectionAccessor.Create (controller.EntityGetter, collectionResolver, template))
		{
		}
	}
}
