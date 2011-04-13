//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	/// <summary>
	/// The <c>AbstractEntityExtensions</c> class provides extension methods for the <see cref="AbstractEntity"/>
	/// class.
	/// </summary>
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
			return DataContextPool.GetDataContext (entity);
		}

		/// <summary>
		/// Compares two entities and returns <c>true</c> if they refer to the same database key
		/// or if they are the same memory instance.
		/// </summary>
		/// <param name="entityA">The first entity.</param>
		/// <param name="entityB">The second entity.</param>
		/// <returns><c>true</c> if both entities refer to the same database key; otherwise, <c>false</c>.</returns>
		public static bool DbKeyEquals(this AbstractEntity entityA, AbstractEntity entityB)
		{
			if (entityA.RefEquals (entityB))
			{
				return true;
			}
			
			if ((entityA == null) ||
				(entityB == null))
			{
				return false;
			}

			DataContext contextA = entityA.GetDataContext ();
			DataContext contextB = entityB.GetDataContext ();

			if ((contextA == null) ||
				(contextB == null))
			{
				return false;
			}

			var keyA = contextA.GetNormalizedEntityKey (entityA);
			var keyB = contextB.GetNormalizedEntityKey (entityB);

			if ((keyA.HasValue) &&
				(keyB.HasValue))
			{
				return keyA.Value == keyB.Value;
			}

			return false;
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
