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
			if (entity.IsNull ())
			{
				return true;
			}

			return entity.Street.IsEmpty ()
				&& entity.PostBox.IsEmpty ()
				&& entity.Location.IsEmpty ();
		}

		public static bool IsEmpty(this StreetEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Complement)
				&& string.IsNullOrWhiteSpace (entity.StreetName);
		}
		
		public static bool IsEmpty(this PostBoxEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Number);
		}
		
		public static bool IsEmpty(this LocationEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			//	We consider a location to be empty if it has neither postal code, nor
			//	location name; a location with just a coutry or region is still empty.

			return string.IsNullOrWhiteSpace (entity.PostalCode)
				&& string.IsNullOrWhiteSpace (entity.Name);
		}

		public static bool IsEmpty(this CountryEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Code)
				&& string.IsNullOrWhiteSpace (entity.Name);
		}

		public static bool IsEmpty(this RegionEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Code)
				&& string.IsNullOrWhiteSpace (entity.Name)
				&& entity.Country.IsEmpty ();
		}

		public static bool IsEmpty(this PersonTitleEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Name)
				&& string.IsNullOrWhiteSpace (entity.ShortName);
		}

		public static bool IsEmpty(this NaturalPersonEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return entity.Firstname.IsNullOrWhiteSpace &&
				   entity.Lastname.IsNullOrWhiteSpace;
		}

		public static bool IsEmpty(this LegalPersonEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Name)
				&& string.IsNullOrWhiteSpace (entity.ShortName);
		}

		public static bool IsEmpty(this AbstractPersonEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			var legalPerson   = entity as LegalPersonEntity;
			var naturalPerson = entity as NaturalPersonEntity;

			return legalPerson.IsEmpty ()
				&& naturalPerson.IsEmpty ();
		}

		public static bool IsEmpty(this RelationEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.IdA)
				&& entity.FirstContactDate.HasValue == false
				&& entity.Affairs.Count == 0
				&& entity.Comments.Count == 0
				&& entity.Person.IsEmpty ()
				&& entity.SalesRepresentative.IsEmpty ();
		}

		public static bool IsEmpty(this ArticleDefinitionEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.IdA)
				&& string.IsNullOrWhiteSpace (entity.ShortDescription)
				&& string.IsNullOrWhiteSpace (entity.LongDescription);

			//	TODO: compléter...
		}



		public static bool IsNull(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () == null;
		}

		public static bool IsActive(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () != null;
		}


		public static bool RefEquals(this AbstractEntity that, AbstractEntity other)
		{
			return that.UnwrapNullEntity () == other.UnwrapNullEntity ();
		}
	}
}
