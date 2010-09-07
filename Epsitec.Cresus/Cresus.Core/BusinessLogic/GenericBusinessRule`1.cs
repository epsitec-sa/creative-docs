//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public abstract class GenericBusinessRule<T> : GenericBusinessRule
		where T : AbstractEntity
	{
		protected GenericBusinessRule()
		{
		}

		public sealed override System.Type EntityType
		{
			get
			{
				return typeof (T);
			}
		}

		public sealed override void Apply(AbstractEntity entity)
		{
			System.Diagnostics.Debug.Assert (entity is T);

			this.Apply (entity as T);
		}

		protected abstract void Apply(T entity);
	}
}
