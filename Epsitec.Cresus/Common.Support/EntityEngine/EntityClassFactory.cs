//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	using Assembly=System.Reflection.Assembly;

	/// <summary>
	/// The <c>EntityClassResolver</c> class is used to allocate entity instances
	/// based on entity ids. The mapping between entity id and entity class
	/// must be marked with the <see cref="EntityAttribute"/> attribute, at
	/// the <c>assembly</c> level.
	/// </summary>
	public class EntityClassFactory : PlugIns.PlugInFactory<AbstractEntity, EntityClassAttribute, Druid>
	{
		/// <summary>
		/// Creates an empty entity instance.
		/// </summary>
		/// <param name="id">The entity id.</param>
		/// <returns>The new entity instance or <c>null</c> if the id
		/// cannot be resolved.</returns>
		public static AbstractEntity CreateEmptyEntity(Druid id)
		{
			return EntityClassFactory.CreateInstance (id);
		}

		public static Druid GetEntityId(System.Type type)
		{
			return EntityClassFactory.FindId (type);
		}
	}
}
