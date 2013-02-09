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
	public sealed class AiderGroupSpecialField<TEntity> : SpecialFieldController<TEntity, AiderGroupEntity>
		where TEntity : AbstractEntity, new ()
	{
		public AiderGroupSpecialField(BusinessContext businessContext, TEntity entity, Expression<Func<TEntity, AiderGroupEntity>> getter)
			: base (businessContext, entity, getter)
		{
		}

		public override string GetWebFieldName()
		{
			return "epsitec.aidergroupspecialfield";
		}

		[SpecialFieldWebMethod]
		public object GetGroupTree(AiderGroupEntity group)
		{
			var selection = group.IsNull ()
				? new List<AiderGroupEntity> ()
				: group.GetGroupChain (this.BusinessContext).ToList ();

			if (selection.Count > 0)
			{
				selection.RemoveAt (selection.Count - 1);
			}

			var subgroups = AiderGroupEntity.FindRootGroups (this.BusinessContext);

			return this.GetGroupTree (null, subgroups, selection, 0);
		}

		private Dictionary<string, object> GetGroupTree(AiderGroupEntity group, IList<AiderGroupEntity> subgroups, IList<AiderGroupEntity> selection, int index)
		{
			List<object> subgroupList;
			bool? hasSubgroups;

			if (subgroups != null && subgroups.Count > 0)
			{
				subgroupList = this.GetGroupList (subgroups, selection, index);
				hasSubgroups = subgroups.Count > 0;
			}
			else
			{
				// Here we could make a request to check whether the group has subgroups or not but
				// currently this is too costly so we don't to it.

				subgroupList = null;
				hasSubgroups = null;
			}

			return this.GetGroupData (group, subgroupList, hasSubgroups);
		}

		private List<object> GetGroupList(IList<AiderGroupEntity> groups, IList<AiderGroupEntity> selection, int index)
		{
			var subgroupList = new List<object> ();
			var selectedGroup = selection != null && index < selection.Count
				? selection[index]
				: null;

			foreach (var group in groups)
			{
				var data = selectedGroup != null && group == selectedGroup
					? this.GetGroupTree (group, group.Subgroups, selection, index + 1)
					: this.GetGroupTree (group, null, null, 0);

				subgroupList.Add (data);
			}

			return subgroupList;
		}

		[SpecialFieldWebMethod]
		public Dictionary<string, object> GetSubGroups(AiderGroupEntity group)
		{
			var subgroupList = this.GetGroupList (group.Subgroups, null, 0);

			return this.GetGroupData (null, subgroupList, null);
		}

		private Dictionary<string, object> GetGroupData(AiderGroupEntity group, List<object> subgroupList, bool? hasSubgroups)
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
