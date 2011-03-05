//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public static class AbstractEntityExtensions
	{
		public static bool IsNull(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () == null;
		}

		public static bool IsNotNull(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () != null;
		}

		public static DataContext GetDataContext(this AbstractEntity entity)
		{
			return DataContext.GetDataContext (entity);
		}

		/// <summary>
		/// Compares two entities and returns <c>true</c> if they refer to the same database key
		/// or if they are the same memory instance.
		/// </summary>
		/// <param name="that">The reference entity.</param>
		/// <param name="other">The other entity.</param>
		/// <returns><c>true</c> if both entities refer to the same database key; otherwise, <c>false</c>.</returns>
		public static bool DbKeyEquals(this AbstractEntity that, AbstractEntity other)
		{
			if (that.RefEquals (other))
			{
				return true;
			}
			else
			{
				//	TODO: fix this
				throw new System.NotImplementedException ();
#if false
				return CoreProgram.Application.Data.DataContextPool.AreEqualDatabaseInstances (that, other);
#endif
			}
		}

		public static bool RefEquals(this AbstractEntity that, AbstractEntity other)
		{
			return that.UnwrapNullEntity () == other.UnwrapNullEntity ();
		}
		
		public static bool RefDiffers(this AbstractEntity that, AbstractEntity other)
		{
			return that.UnwrapNullEntity () != other.UnwrapNullEntity ();
		}
	}
}
