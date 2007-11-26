//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityExcerpt</c> class stores an excerpt of an entity, possibly
	/// including some of its relations.
	/// </summary>
	public class EntityExcerpt : AbstractEntity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityExcerpt"/> class.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		public EntityExcerpt(Druid entityId)
		{
			this.entityId = entityId;
		}


		/// <summary>
		/// Gets the id of the <see cref="StructuredType"/> which describes
		/// this entity.
		/// </summary>
		/// <returns>
		/// The id of the <see cref="StructuredType"/>.
		/// </returns>
		public override Druid GetEntityStructuredTypeId()
		{
			return this.entityId;
		}


		private readonly Druid entityId;
	}
}
