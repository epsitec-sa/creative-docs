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

			return entity.Complement.IsNullOrWhiteSpace &&
				   entity.StreetName.IsNullOrWhiteSpace;
		}
		
		public static bool IsEmpty(this PostBoxEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return entity.Number.IsNullOrWhiteSpace;
		}
		
		public static bool IsEmpty(this LocationEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			//	We consider a location to be empty if it has neither postal code, nor
			//	location name; a location with just a coutry or region is still empty.

			return entity.PostalCode.IsNullOrWhiteSpace &&
				   entity.Name.IsNullOrWhiteSpace;
		}

		public static bool IsEmpty(this CountryEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Code) &&
				   entity.Name.IsNullOrWhiteSpace;
		}

		public static bool IsEmpty(this RegionEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return string.IsNullOrWhiteSpace (entity.Code) &&
				   entity.Name.IsNullOrWhiteSpace &&
				   entity.Country.IsEmpty ();
		}

		public static bool IsEmpty(this PersonTitleEntity entity)
		{
			if (entity.IsNull ())
			{
				return true;
			}

			return entity.Name.IsNullOrWhiteSpace &&
				   entity.ShortName.IsNullOrWhiteSpace;
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

			return entity.Name.IsNullOrWhiteSpace &&
				   entity.ShortName.IsNullOrWhiteSpace;
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

			return string.IsNullOrWhiteSpace (entity.IdA) &&
				   entity.ShortDescription.IsNullOrWhiteSpace &&
				   entity.LongDescription.IsNullOrWhiteSpace;

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


		/// <summary>
		/// Méthode d'extension pour l'entité ArticleDefinitionEntity, qui ajoute une méthode AllArticleGroups()
		/// contenant toutes les définitions de groupe (et pas seulement celles utilisées par l'article).
		/// Il n'est hélas pas possible de définir une propriété d'extension, mais seulement une méthode.
		/// D'où l'écriture AllArticleGroups() avec les parenthèses.
		/// Donc:
		/// article.ArticleGroups       -> liste des ArticleGroupEntity de cet article
		/// article.AllArticleGroups () -> liste de tous les ArticleGroupEntity connus
		/// </summary>
		public static IList<ArticleGroupEntity> AllArticleGroups(this ArticleDefinitionEntity entity)
		{
			List<ArticleGroupEntity> list = CoreProgram.Application.Data.GetArticleGroups ().ToList ();
			list.Sort (Controllers.SelectionController<ArticleGroupEntity>.CompareItems);

			return list;
		}

		public static IList<ContactRoleEntity> AllRoles(this AbstractContactEntity entity)
		{
			List<ContactRoleEntity> list = CoreProgram.Application.Data.GetRoles ().ToList ();
			list.Sort (Controllers.SelectionController<ContactRoleEntity>.CompareItems);

			return list;
		}
	}
}
