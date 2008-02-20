//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>GenericEntity</c> class provides a default wrapper for entities
	/// which cannot be resolved by the <see cref="EntityResolver"/> class.
	/// </summary>
	public class GenericEntity : AbstractEntity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericEntity"/> class.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		public GenericEntity(Druid entityId)
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
