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
