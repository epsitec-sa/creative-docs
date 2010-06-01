//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Controllers;

namespace Epsitec.Cresus.Core.Accessors
{
	public abstract class AbstractEntityAccessor : AbstractAccessor
	{
		protected AbstractEntityAccessor(object parentEntities, bool grouped)
			: base (parentEntities, grouped)
		{
		}

		public abstract AbstractEntity AbstractEntity
		{
			get;
		}

		public abstract AbstractEntity Create();

		public abstract void Remove();
	}

	public abstract class AbstractEntityAccessor<T> : AbstractEntityAccessor where T : AbstractEntity
	{
		protected AbstractEntityAccessor(object parentEntities, T entity, bool grouped)
			: base (parentEntities, grouped)
		{
			System.Diagnostics.Debug.Assert (entity != null);

			this.entity = entity;

			EntityNullReferenceVirtualizer.PatchNullReferences (entity);
		}

		public T Entity
		{
			get
			{
				return this.entity;
			}
		}

		public override AbstractEntity AbstractEntity
		{
			get
			{
				return this.Entity;
			}
		}

		public override AbstractEntity Create()
		{
			return null;
		}

		public override void Remove()
		{
		}



		private readonly T entity;
	}
}
