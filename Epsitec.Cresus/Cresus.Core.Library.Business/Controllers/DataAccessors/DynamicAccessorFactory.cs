//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>DynamicAccessorFactory</c> class provides a <see cref="CollectionAccessor"/>.
	/// See the generic version <see cref="DynamicAccessorFactory{T1,T2,T3}"/>
	/// </summary>
	internal abstract class DynamicAccessorFactory
	{
		protected DynamicAccessorFactory(CollectionAccessor accessor)
		{
			this.accessor = accessor;
		}

		public CollectionAccessor CollectionAccessor
		{
			get
			{
				return this.accessor;
			}
		}

		private readonly CollectionAccessor accessor;
	}
}
