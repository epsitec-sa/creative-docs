//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderMailingCategoryEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText
			(
				this.Name
			);
		}


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public void RefreshCache()
		{
			this.UpdateGroupPathCache ();
			
		}

		public static AiderMailingCategoryEntity Create(BusinessContext context, string name, AiderGroupEntity group)
		{
			var mailingCategory = context.CreateAndRegisterEntity<AiderMailingCategoryEntity> ();

			mailingCategory.Name  = name;
			mailingCategory.Group = group;

			mailingCategory.UpdateGroupPathCache ();

			return mailingCategory;
		}


		public static void Delete(BusinessContext businessContext, AiderMailingCategoryEntity mailingCategory)
		{
			businessContext.DeleteEntity (mailingCategory);			
		}

		public static IEnumerable<AiderMailingCategoryEntity> GetCantonCategories(BusinessContext context, string groupPath)
		{
			return AiderMailingCategoryEntity.GetMailingCategories (context, AiderGroupIds.GlobalPrefix)
				.Where (x => x.Group.IsGlobalOrGlobalSubgroup ());
		}

		public static IEnumerable<AiderMailingCategoryEntity> GetRegionCategories(BusinessContext context, string groupPath)
		{
			return AiderMailingCategoryEntity.GetMailingCategories (context, groupPath.Substring (0, 5))
				.Where (x => x.Group.IsParishOrParishSubgroup () == false);
		}

		public static IEnumerable<AiderMailingCategoryEntity> GetParishCategories(BusinessContext context, string groupPath)
		{
			return AiderMailingCategoryEntity.GetMailingCategories (context, groupPath);
		}

		
		private void UpdateGroupPathCache()
		{
			this.GroupPathCache = this.Group.IsNull () ? "" : this.Group.Path;
		}
		
		private static IEnumerable<AiderMailingCategoryEntity> GetMailingCategories(BusinessContext context, string groupPath)
		{
			var dataContext = context.DataContext;

			var example = new AiderMailingCategoryEntity ();
			var request = Request.Create (example);

			request.AddCondition (dataContext, example, x => SqlMethods.Like (x.GroupPathCache, groupPath + "%"));

			var categories = dataContext.GetByRequest (request).ToList ();

			if ((categories.Count == 0) &&
				(groupPath.Length > 1))
			{
				using (var localContext = new BusinessContext (context.Data, false))
				{
					var group = AiderGroupEntity.FindGroups (localContext, groupPath).FirstOrDefault ();

					if (group.IsNotNull ())
					{
						var name = AiderMailingCategoryEntity.GetGroupName (group);
						AiderMailingCategoryEntity.Create (localContext, name, group);
						localContext.SaveChanges (LockingPolicy.ReleaseLock);
					}
				}

				categories = dataContext.GetByRequest (request).ToList ();
			}

			return categories;
		}

		private static string GetGroupName(AiderGroupEntity group)
		{
			if (group.GroupDef.IsParish ())
			{
				return group.Parent.Name + ", " + group.Name;
			}
			if (group.GroupDef.IsRegion ())
			{
				return group.Name;
			}

			return group.Name;
		}
	}
}
