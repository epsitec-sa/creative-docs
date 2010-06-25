//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public static class Extensions
	{
		public static bool IsEmpty(this AddressEntity entity)
		{
			if (entity.UnwrapNullEntity () == null)
			{
				return true;
			}

			return entity.Street.IsEmpty ()
				&& entity.PostBox.IsEmpty ()
				&& entity.Location.IsEmpty ();
		}

		public static bool IsEmpty(this StreetEntity entity)
		{
			if (entity.UnwrapNullEntity () == null)
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Complement)
				&& string.IsNullOrWhiteSpace (entity.StreetName);
		}
		
		public static bool IsEmpty(this PostBoxEntity entity)
		{
			if (entity.UnwrapNullEntity () == null)
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Number);
		}
		
		public static bool IsEmpty(this LocationEntity entity)
		{
			if (entity.UnwrapNullEntity () == null)
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.PostalCode)
				&& string.IsNullOrWhiteSpace (entity.Name)
				&& entity.Country.IsEmpty ()
				&& entity.Region.IsEmpty ();
		}

		public static bool IsEmpty(this CountryEntity entity)
		{
			if (entity.UnwrapNullEntity () == null)
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Code)
				&& string.IsNullOrWhiteSpace (entity.Name);
		}

		public static bool IsEmpty(this RegionEntity entity)
		{
			if (entity.UnwrapNullEntity () == null)
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Code)
				&& string.IsNullOrWhiteSpace (entity.Name)
				&& entity.Country.IsEmpty ();
		}

		public static bool IsEmpty(this PersonTitleEntity entity)
		{
			if (entity.UnwrapNullEntity () == null)
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Name)
				&& string.IsNullOrWhiteSpace (entity.ShortName);
		}

		public static bool IsNull(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () == null;
		}

		public static bool IsActive(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () != null;
		}

		public static bool CompareWith(this AbstractEntity that, AbstractEntity other)
		{
			return that.UnwrapNullEntity () == other.UnwrapNullEntity ();
		}
	}
}
