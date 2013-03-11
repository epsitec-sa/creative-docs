//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupDefEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Classification, "~, ", this.Name);
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
			foreach (var childDefinition in this.Subgroups)
			{
				this.InstantiateSubgroup (businessContext, group, childDefinition);
			}

			if (this.Function.IsNotNull ())
			{
				foreach (var childDefinition in this.Function.Subgroups)
				{
					this.InstantiateSubgroup (businessContext, group, childDefinition);
				}
			}
		}

		private void InstantiateSubgroup(BusinessContext businessContext, AiderGroupEntity group, AiderGroupDefEntity childDefinition)
		{
			var childGroup = group.CreateSubgroup (businessContext, childDefinition.Name);
			childGroup.GroupDef = childDefinition;

			childDefinition.InstantiateSubgroups (businessContext, childGroup);
		}

		public bool IsParish()
		{
			return this.Level == 1 && this.Classification == GroupClassification.Parish;
		}

		public bool IsNoParish()
		{
			return this.Level == 0 && this.Classification == GroupClassification.NoParish;
		}
	}
}
