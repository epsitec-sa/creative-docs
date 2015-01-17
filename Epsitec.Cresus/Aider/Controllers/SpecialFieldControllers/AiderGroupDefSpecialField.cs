using Epsitec.Aider.Entities;

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers.SpecialFieldControllers;

using Epsitec.Cresus.Core.Entities;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Aider.Controllers.SpecialFieldControllers
{
	public sealed class AiderGroupDefSpecialField<TEntity> : SpecialFieldController<TEntity, AiderGroupDefEntity>
		where TEntity : AbstractEntity, new ()
	{
		public AiderGroupDefSpecialField(BusinessContext businessContext, TEntity entity, Expression<Func<TEntity, AiderGroupDefEntity>> getter)
			: base (businessContext, entity, getter)
		{
		}

		public override string GetWebFieldName()
		{
			return "epsitec.aidergroupspecialfield";
		}

		[SpecialFieldWebMethod]
		public object GetGroupTree(AiderGroupDefEntity groupDef)
		{
			var selection = groupDef.IsNull () ? new AiderGroupDefEntity () : groupDef;
			var rootGroups = AiderGroupDefEntity.FindRootGroupsDefinitions (this.BusinessContext);

			return this.GetGroupDefTree (null, rootGroups, selection);
		}

		private Dictionary<string, object> GetGroupDefTree(AiderGroupDefEntity group, IList<AiderGroupDefEntity> subgroups, AiderGroupDefEntity selection)
		{
			List<object> subgroupList;
			bool? hasSubgroups;

			if (subgroups != null && subgroups.Count > 0)
			{
				subgroupList = this.GetGroupDefList (subgroups, selection);
				hasSubgroups = subgroups.Count > 0;
			}
			else
			{
				// Here we could make a request to check whether the group has subgroups or not but
				// currently this is too costly so we don't to it.

				subgroupList = null;
				hasSubgroups = null;
			}

			return this.GetGroupDefData (group, subgroupList, hasSubgroups);
		}

		private List<object> GetGroupDefList(IList<AiderGroupDefEntity> groups, AiderGroupDefEntity selection)
		{
			var subgroupList = new List<object> ();

			foreach (var group in groups)
			{
				var data = selection != null && group == selection
					? this.GetGroupDefTree (group, group.Subgroups, selection)
					: this.GetGroupDefTree (group, null, null);

				subgroupList.Add (data);
			}

			return subgroupList;
		}

		[SpecialFieldWebMethod]
		public Dictionary<string, object> GetSubGroups(AiderGroupDefEntity group)
		{
			var subgroupList = this.GetGroupDefList (group.Subgroups, null);

			return this.GetGroupDefData (null, subgroupList, null);
		}

		private Dictionary<string, object> GetGroupDefData(AiderGroupDefEntity group, List<object> subgroupList, bool? hasSubgroups)
		{
			var result = new Dictionary<string, object> ();

			if (group != null)
			{
				result["group"] = group;
			}

			if (subgroupList != null)
			{
				result["groups"] = subgroupList;
			}

			if (hasSubgroups.HasValue)
			{
				result["hasSubgroups"] = hasSubgroups.Value;
			}

			return result;
		}
	}
}
