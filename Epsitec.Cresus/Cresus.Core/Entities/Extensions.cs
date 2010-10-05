//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public static class Extensions
	{
		public static EntityStatus GetEntityStatus(this string text)
		{
			if (string.IsNullOrWhiteSpace (text))
			{
				return EntityStatus.Empty;
			}
			else
			{
				return EntityStatus.Valid;
			}
		}

		public static EntityStatus GetEntityStatus(this FormattedText text)
		{
			if (text.IsNullOrWhiteSpace)
			{
				return EntityStatus.Empty;
			}
			else
			{
				return EntityStatus.Valid;
			}
		}

		public static EntityStatus TreatAsOptional(this EntityStatus status)
		{
			if ((status & EntityStatus.Empty) != 0)
			{
				status |= EntityStatus.Valid;
			}

			return status;
		}


		public static bool IsNull(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () == null;
		}

		public static bool IsNotNull(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () != null;
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
				return DataContextPool.Instance.AreEqualDatabaseInstances (that, other);
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
