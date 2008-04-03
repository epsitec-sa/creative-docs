//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	public abstract class InternalEntityBase : IStructuredTypeProvider
	{
		protected InternalEntityBase()
		{
			this.context = EntityContext.Current;
		}

		/// <summary>
		/// Gets the context associated with this entity.
		/// </summary>
		/// <returns>The <see cref="EntityContext"/> instance.</returns>
		public EntityContext GetEntityContext()
		{
			return this.context;
		}

		/// <summary>
		/// Gets the id of the <see cref="StructuredType"/> which describes
		/// this entity.
		/// </summary>
		/// <returns>The id of the <see cref="StructuredType"/>.</returns>
		public abstract Druid GetEntityStructuredTypeId();

		#region IStructuredTypeProvider Members

		IStructuredType IStructuredTypeProvider.GetStructuredType()
		{
			return this.GetStructuredType ();
		}

		#endregion

		protected virtual IStructuredType GetStructuredType()
		{
			return this.context.GetStructuredType (this.GetEntityStructuredTypeId ());
		}

		private readonly EntityContext context;
	}
}
