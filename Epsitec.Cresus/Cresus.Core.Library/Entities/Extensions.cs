//	Copyright © 2010-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

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
			if (text.IsNullOrWhiteSpace ())
			{
				return EntityStatus.Empty;
			}
			else
			{
				return EntityStatus.Valid;
			}
		}

		public static EntityStatus GetEntityStatus(this Date? date)
		{
			if (date.HasValue)
			{
				return EntityStatus.Valid;
			}
			else
			{
				return EntityStatus.Empty;
			}
		}

		public static EntityStatus GetEntityStatus(this System.DateTime date)
		{
			if (date.Ticks != 0)
			{
				return EntityStatus.Valid;
			}
			else
			{
				return EntityStatus.Empty;
			}
		}

		public static EntityStatus GetEntityStatus(this System.DateTime? date)
		{
			if ((date.HasValue) &&
				(date.Value.Ticks != 0))
			{
				return EntityStatus.Valid;
			}
			else
			{
				return EntityStatus.Empty;
			}
		}

		public static EntityStatus GetEntityStatus(this int? number)
		{
			if (number.HasValue)
			{
				return EntityStatus.Valid;
			}
			else
			{
				return EntityStatus.Empty;
			}
		}

		public static EntityStatus GetEntityStatus(this Druid? druid)
		{
			if ((druid.HasValue) &&
				(druid.Value.IsEmpty == false))
			{
				return EntityStatus.Valid;
			}
			else
			{
				return EntityStatus.Empty;
			}
		}

		public static EntityStatus GetEntityStatus(this Druid druid)
		{
			if (druid.IsEmpty == false)
			{
				return EntityStatus.Valid;
			}
			else
			{
				return EntityStatus.Empty;
			}
		}

		public static EntityStatus GetEntityStatus(this decimal? number)
		{
			if (number.HasValue)
			{
				return EntityStatus.Valid;
			}
			else
			{
				return EntityStatus.Empty;
			}
		}

		public static EntityStatus TreatAsOptional(this EntityStatus status)
		{
			if (status.HasFlag (EntityStatus.Empty))
			{
				return status | EntityStatus.Valid;
			}
			else
			{
				return status;
			}
		}
	}
}
