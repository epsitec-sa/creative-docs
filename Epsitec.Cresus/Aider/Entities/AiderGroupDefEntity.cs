//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupDefEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return TextFormatter.FormatText (this.Name);
		}

		public AiderGroupEntity Instantiate(BusinessContext businessContext)
		{
			var group = AiderGroupEntity.Create (businessContext, this, this.Name, 0, this.PathTemplate);

			this.InstantiateSubgroups (businessContext, group);

			return group;
		}

		public AiderGroupEntity InstantiateRegion(BusinessContext businessContext, string name, int code)
		{
			var path = AiderGroupIds.GetRegionId (code);
			var group = AiderGroupEntity.Create (businessContext, this, name, 0, path);

			this.InstantiateSubgroups (businessContext, group);

			return group;
		}

		public AiderGroupEntity InstantiateParish(BusinessContext businessContext, AiderGroupEntity region, string name, int code)
		{
			var pathPart = AiderGroupIds.GetParishId (code);
			var group = AiderGroupEntity.Create (businessContext, region, this, name, pathPart);

			this.InstantiateSubgroups (businessContext, group);

			return group;
		}

		private void InstantiateSubgroups(BusinessContext businessContext, AiderGroupEntity group)
		{
			// We discard the parish subgroup because it is a special template subgroup that we want
			// to instantiate manually later on.
			var subgroups = this.Subgroups
				.Where (g => !g.IsParish ())
				.ToList ();

			AiderGroupDefEntity.InstantiateSubgroups (businessContext, group, subgroups);

			if (this.Function.IsNotNull ())
			{
				AiderGroupDefEntity.InstantiateSubgroups (businessContext, group, this.Function.Subgroups);
			}
		}

		private static void InstantiateSubgroups(BusinessContext businessContext, AiderGroupEntity group, IEnumerable<AiderGroupDefEntity> childDefinitions)
		{
			foreach (var childDefinition in childDefinitions)
			{
				var childGroup = group.CreateSubgroup (businessContext, childDefinition);

				childDefinition.InstantiateSubgroups (businessContext, childGroup);
			}
		}

		public static AiderGroupDefEntity CreateDefinitionSubGroup(BusinessContext businessContext, AiderGroupDefEntity parent, string name, GroupClassification groupClass, bool subgroupsAllowed, bool membersAllowed, bool isMutable)
		{
			var aiderGroupDef = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

			aiderGroupDef.Name = name;
			aiderGroupDef.Number = ""; //?
			aiderGroupDef.Level = parent.Level + 1;
			aiderGroupDef.SubgroupsAllowed = subgroupsAllowed;
			aiderGroupDef.MembersAllowed = membersAllowed;
			
			var number = AiderGroupIds.FindNextSubGroupDefNumber (parent.Subgroups.Select (g => g.PathTemplate));
			aiderGroupDef.PathTemplate = AiderGroupIds.CreateDefinitionSubgroupPath (parent.PathTemplate, number);

			aiderGroupDef.Classification = groupClass;
			aiderGroupDef.Mutability = isMutable ? Mutability.Customizable : Mutability.None;

			//uplink
			parent.Subgroups.Add (aiderGroupDef);

			return aiderGroupDef;
		}

		public static AiderGroupDefEntity CreateDefinitionRootGroup(BusinessContext businessContext, string name, GroupClassification groupClass, bool isMutable)
		{
			var aiderGroupDef = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

			aiderGroupDef.Name = name;
			aiderGroupDef.Number = ""; //?
			aiderGroupDef.Level = AiderGroupIds.TopLevel;
			aiderGroupDef.SubgroupsAllowed = true;
			aiderGroupDef.MembersAllowed = false;
			var rootGroupsDefs = AiderGroupDefEntity.FindRootGroupsDefinitions(businessContext);
			var number = AiderGroupIds.FindNextSubGroupDefNumber (rootGroupsDefs.Select (g => g.PathTemplate));
			aiderGroupDef.PathTemplate = AiderGroupIds.CreateTopLevelPathTemplate (number);

			aiderGroupDef.Classification = groupClass;
			aiderGroupDef.Mutability = isMutable ? Mutability.Customizable : Mutability.None;

			return aiderGroupDef;
		}

		public bool IsRegion()
		{
			return this.Level == AiderGroupIds.RegionLevel
				&& this.Classification == GroupClassification.Region;
		}

		public bool IsParish()
		{
			return this.Level == AiderGroupIds.ParishLevel
				&& this.Classification == GroupClassification.Parish;
		}

		public bool IsNoParish()
		{
			return this.Level == AiderGroupIds.NoParishLevel
				&& this.Classification == GroupClassification.NoParish;
		}

		public bool IsGlobal()
		{
			return this.Level == AiderGroupIds.TopLevel
				&& this.Classification == GroupClassification.None;
		}

		public static IList<AiderGroupDefEntity> FindRootGroupsDefinitions(BusinessContext businessContext)
		{
			var dataContext = businessContext.DataContext;

			var example = new AiderGroupDefEntity ()
			{
				Level = AiderGroupIds.TopLevel,
			};

			var request = Request.Create (example);
			request.AddSortClause (ValueField.Create (example, x => x.Name));

			return dataContext.GetByRequest (request);
		}
	}
}
